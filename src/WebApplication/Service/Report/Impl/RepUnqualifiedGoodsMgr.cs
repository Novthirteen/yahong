using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MasterData;
using Castle.Services.Transaction;
using com.Sconit.Utility;
using com.Sconit.Entity;
using com.Sconit.Service.MasterData;
using com.Sconit.Entity.View;
using com.Sconit.Service.Ext.MasterData;

namespace com.Sconit.Service.Report.Impl
{

    /**
     * 
     * 不合格品处理单
     * 
     */
    [Transactional]
    public class RepUnqualifiedGoodsMgr : RepTemplate1
    {

        public override string reportTemplateFolder { get; set; }

        public ICodeMasterMgrE codeMasterMgrE { get; set; }
        public IInspectResultMgrE inspectResultMgrE { get; set; }

        public RepUnqualifiedGoodsMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 15;
            //列数  1起始
            this.columnCount = 7;
            //报表头的行数  1起始
            this.headRowCount = 9;
            //报表尾的行数  1起始
            this.bottomRowCount = 18;
        }

        /**
         * 填充报表
         * 
         * Param list [0]InspectOrder
         * Param list [0]IList<InspectOrderDetail>           
         */
        [Transaction(TransactionMode.Requires)]
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                if (list == null || list.Count < 2) return false;

                InspectOrder inspectOrder = (InspectOrder)(list[0]);
                IList<InspectResult> inspectResultList = (IList<InspectResult>)(list[1]);

                if (inspectOrder == null
                    || inspectResultList == null || inspectResultList.Count == 0)
                {
                    return false;
                }

                IList<UnqualifiedGoodsView> unqualifiedGoodsList = new List<UnqualifiedGoodsView>();
                foreach (InspectResult inspectResult in inspectResultList)
                {
                    if (!inspectResult.IsPrinted)
                    {
                        inspectResult.PrintNo = inspectOrder.InspectNo;
                        inspectResult.IsPrinted = true;
                    }
                    inspectResult.LastModifyDate = DateTime.Now;
                    inspectResult.LastModifyUser = inspectOrder.CreateUser;
                    inspectResult.PrintCount += 1;
                    inspectResultMgrE.UpdateInspectResult(inspectResult);

                    UnqualifiedGoodsView ufg = new UnqualifiedGoodsView();
                    ufg.Item = inspectResult.InspectOrderDetail.LocationLotDetail.Item;
                    ufg.RejectedQty = inspectResult.RejectedQty.HasValue ? inspectResult.RejectedQty.Value : 0;
                    ufg.Disposition = inspectResult.InspectOrderDetail.Disposition;
                    ufg.FinishGoods = inspectResult.InspectOrderDetail.FinishGoods;
                    ufg.LocationFrom = inspectResult.InspectOrderDetail.LocationFrom;
                    bool isExist = false;

                    foreach (UnqualifiedGoodsView unq in unqualifiedGoodsList)
                    {
                        if (unq.Item.Code == ufg.Item.Code && unq.LocationFrom.Code == ufg.LocationFrom.Code
                            && unq.DefectClassification == ufg.DefectClassification && unq.Disposition == ufg.Disposition
                            && ((unq.FinishGoods == null && unq.FinishGoods == null) || (unq.FinishGoods != null && unq.FinishGoods != null &&
                            unq.FinishGoods.Code == ufg.FinishGoods.Code)))
                        {
                            isExist = true;
                        }
                        if (isExist)
                        {
                            unq.RejectedQty += ufg.RejectedQty;
                            break;
                        }
                    }

                    if (!isExist)
                    {
                        unqualifiedGoodsList.Add(ufg);
                    }
                }


                this.CopyPage(inspectResultList.Count);

                this.FillHead(inspectOrder);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                foreach (UnqualifiedGoodsView unqualifiedGoods in unqualifiedGoodsList)
                {
                    //"零件号Part No."
                    this.SetRowCell(pageIndex, rowIndex, 0, unqualifiedGoods.Item.Code);
                    //零件名称     Part Name	
                    this.SetRowCell(pageIndex, rowIndex, 1, unqualifiedGoods.Item.Description);
                    //"数量     QTY."
                    this.SetRowCell(pageIndex, rowIndex, 2, unqualifiedGoods.RejectedQty.ToString("0.########"));
                    //起末库位	
                    this.SetRowCell(pageIndex, rowIndex, 3, unqualifiedGoods.LocationFrom.Code);
                    //处理方法  Disposition 
                    if (unqualifiedGoods.Disposition != null && unqualifiedGoods.Disposition != string.Empty)
                    {
                        CodeMaster codeMaster = codeMasterMgrE.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_INSPECT_DISPOSITION, unqualifiedGoods.Disposition);
                        if (codeMaster != null && codeMaster.Description != null && codeMaster.Description.Length > 0)
                        {
                            this.SetRowCell(pageIndex, rowIndex, 4, codeMaster.Description); //inspectOrderDetail.Disposition
                        }
                    }
                    //成品
                    this.SetRowCell(pageIndex, rowIndex, 5, unqualifiedGoods.FinishGoods == null ? string.Empty : unqualifiedGoods.FinishGoods.Code);

                    //"缺陷  Defect"		
                    this.SetRowCell(pageIndex, rowIndex, 6, string.Empty);

                    if (this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                    {
                        pageIndex++;
                        rowIndex = 0;
                    }
                    else
                    {
                        rowIndex++;
                    }
                    rowTotal++;
                }

                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /*
         * 填充报表头
         * 
         * Param repack 报验单头对象
         */
        private void FillHead(InspectOrder inspectOrder)
        {
            //序号
            this.SetRowCell(2, 6, inspectOrder.InspectNo);
            //部门/小组
            //this.SetRowCell(5, 1,  );
            //班次
            //this.SetRowCell(5, 3, inspectOrder );
            //填写人
            this.SetRowCell(5, 3, inspectOrder.CreateUser.Name);
            //日期
            this.SetRowCell(5, 6, inspectOrder.CreateDate.ToString("yyyy-MM-dd HH:mm"));
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            this.SetMergedRegion(pageIndex, 34, 5, 34, 6);

            this.CopyCell(pageIndex, 24, 0, "A25");
            //this.CopyCell(pageIndex, 24, 1, "B18");
            this.CopyCell(pageIndex, 26, 0, "A27");
            this.CopyCell(pageIndex, 27, 0, "A28");
            this.CopyCell(pageIndex, 29, 0, "A30");
            this.CopyCell(pageIndex, 29, 2, "C30");
            this.CopyCell(pageIndex, 29, 5, "F30");

            this.CopyCell(pageIndex, 30, 0, "A31");
            this.CopyCell(pageIndex, 30, 2, "C31");
            this.CopyCell(pageIndex, 30, 5, "F31");

            this.CopyCell(pageIndex, 31, 0, "A32");
            this.CopyCell(pageIndex, 32, 0, "A33");

            this.CopyCell(pageIndex, 34, 5, "F35");

            this.CopyCell(pageIndex, 35, 0, "A36");
            this.CopyCell(pageIndex, 36, 0, "A37");
            this.CopyCell(pageIndex, 37, 0, "A38");
            this.CopyCell(pageIndex, 38, 0, "A39");
            this.CopyCell(pageIndex, 39, 0, "A40");
            this.CopyCell(pageIndex, 40, 0, "A41");
            //this.CopyCell(pageIndex, 41, 0, "A42");

            //this.CopyCell(pageIndex, 35, 0, "A36");
            //this.CopyCell(pageIndex, 35, 2, "C36");
            //this.CopyCell(pageIndex, 35, 4, "E36");
            //this.CopyCell(pageIndex, 35, 6, "G36");

            //this.CopyCell(pageIndex, 36, 0, "A37");
            //this.CopyCell(pageIndex, 36, 2, "C37");
            //this.CopyCell(pageIndex, 36, 4, "E37");
            //this.CopyCell(pageIndex, 36, 6, "G37");
        }
    }
}

#region Extend Class

namespace com.Sconit.Service.Ext.Report.Impl
{
    /*
     * 废弃
     * 
     */
    [Transactional]
    public partial class RepUnqualifiedGoodsMgrE : com.Sconit.Service.Report.Impl.RepUnqualifiedGoodsMgr, IReportBaseMgrE
    {

    }
}

#endregion