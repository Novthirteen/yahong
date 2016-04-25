using com.Sconit.Service.Ext.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MasterData;
using Castle.Services.Transaction;
using com.Sconit.Utility;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity;
using com.Sconit.Service.Report;
using com.Sconit.Facility.Entity;

namespace com.Sconit.Facility.Service.Report.Impl
{
    [Transactional]
    public class RepCheckListOrderMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }

        public IMiscOrderMgrE miscOrderMgrE { get; set; }
        public ICodeMasterMgrE codeMasterMgrE { get; set; }

        public RepCheckListOrderMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 35;
            //列数   1起始
            this.columnCount = 5;
            //报表头的行数  1起始
            this.headRowCount = 12;
            //报表尾的行数  1起始
            this.bottomRowCount = 0;
        }

        /**
         * 填充报表
         * 
         * Param list [0]OrderHead
         * Param list [0]IList<OrderDetail>           
         */
        [Transaction(TransactionMode.Requires)]
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                if (list == null || list.Count < 2) return false;

                CheckListOrderMaster checkListOrderMaster = (CheckListOrderMaster)(list[0]);
                IList<CheckListOrderDetail> checkListOrderDetails = (IList<CheckListOrderDetail>)(list[1]);

                if (checkListOrderMaster == null
                  || checkListOrderDetails == null || checkListOrderDetails.Count == 0)
                {
                    return false;
                }

                this.barCodeFontName = this.GetBarcodeFontName(2, 9);

                this.CopyPage(checkListOrderDetails.Count);

                this.FillHead(checkListOrderMaster);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                int i = 1;
                foreach (CheckListOrderDetail checkListOrderDetail in checkListOrderDetails)
                {
                    // No.	
                    this.SetRowCell(pageIndex, rowIndex, 0, checkListOrderDetail.Seq.ToString());

                    //CheckListDetailCode
                    this.SetRowCell(pageIndex, rowIndex, 1, checkListOrderDetail.CheckListDetailCode);

                    //Description
                    this.SetRowCell(pageIndex, rowIndex, 2, checkListOrderDetail.Description);

                    //IsNormal
                    this.SetRowCell(pageIndex, rowIndex, 3, checkListOrderDetail.IsNormal ? "正常" : "不正常");

                    //Remark
                    this.SetRowCell(pageIndex, rowIndex, 4, checkListOrderDetail.Remark);

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
         * Param repack 巡检单头
         */
        private void FillHead(CheckListOrderMaster checkListOrderMaster)
        {
            //Code:
            this.SetRowCell(4, 2, checkListOrderMaster.Code);
            //Region.:
            this.SetRowCell(4, 4, checkListOrderMaster.Region);
            //checkListCode
            this.SetRowCell(5, 2, checkListOrderMaster.CheckListCode);
            //checkListName
            this.SetRowCell(5, 4, checkListOrderMaster.CheckListName);
            //fcid
            this.SetRowCell(6, 2, checkListOrderMaster.FacilityID);
            //fcname
            this.SetRowCell(6, 4, checkListOrderMaster.FacilityName);
            //checkuser
            this.SetRowCell(7, 2, checkListOrderMaster.CheckUser);
            //checkdate
            this.SetRowCell(7, 4, checkListOrderMaster.CheckDate.ToLongDateString());
            //description
            this.SetRowCell(8, 2, checkListOrderMaster.Description);
            //remark
            this.SetRowCell(9, 2, checkListOrderMaster.Remark);
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //this.CopyCell(pageIndex, 48, 2, "C49");
            //this.CopyCell(pageIndex, 49, 0, "A50");
        }


    }
}

#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Report.Impl
{
    [Transactional]
    public partial class RepCheckListOrderMgrE : com.Sconit.Facility.Service.Report.Impl.RepCheckListOrderMgr, IReportBaseMgrE
    {


    }
}

#endregion
