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
using com.Sconit.Facility.Entity;
using com.Sconit.Service.Report;

namespace com.Sconit.Facility.Service.Report.Impl
{
    [Transactional]
    public class RepFacilityStocktakeMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }

        public IMiscOrderMgrE miscOrderMgrE { get; set; }
        public ICodeMasterMgrE codeMasterMgrE { get; set; }

        public RepFacilityStocktakeMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 36;
            //列数   1起始
            this.columnCount = 11;
            //报表头的行数  1起始
            this.headRowCount = 6;
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

                FacilityStockMaster facilityStockMaster = (FacilityStockMaster)(list[0]);
                IList<FacilityStockDetail> facilityStockDetails = (IList<FacilityStockDetail>)(list[1]);

                if (facilityStockMaster == null
                  || facilityStockDetails == null || facilityStockDetails.Count == 0)
                {
                    return false;
                }

                this.barCodeFontName = this.GetBarcodeFontName(2, 9);

                this.CopyPage(facilityStockDetails.Count);

                this.FillHead(facilityStockMaster);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                int i = 1;
                foreach (FacilityStockDetail facilityStockDetail in facilityStockDetails)
                {
                    // No.	
                    this.SetRowCell(pageIndex, rowIndex, 0, "" + i++);


                    //FCID
                    this.SetRowCell(pageIndex, rowIndex, 1, facilityStockDetail.FacilityMaster.FCID);


                    //AssetNo
                    this.SetRowCell(pageIndex, rowIndex, 2, facilityStockDetail.FacilityMaster.AssetNo);

                    this.SetRowCell(pageIndex, rowIndex, 3, facilityStockDetail.FacilityMaster.Name);

                    this.SetRowCell(pageIndex, rowIndex, 4, facilityStockDetail.FacilityMaster.Specification);

                    this.SetRowCell(pageIndex, rowIndex, 5, facilityStockDetail.FacilityMaster.CurrChargePersonName);

                    this.SetRowCell(pageIndex, rowIndex, 6, facilityStockDetail.FacilityMaster.ChargeSite);

                    this.SetRowCell(pageIndex, rowIndex, 7, facilityStockDetail.InvQty.ToString("0.########"));

                   // this.SetRowCell(pageIndex, rowIndex, 8, string.Empty);

                   
                    ////单位Uom
                    //this.SetRowCell(pageIndex, rowIndex, 3, miscOrderDetail.Item.Uom.Code);

                    ////条码
                    //if (miscOrderDetail.HuId != null && miscOrderDetail.HuId.Trim().Length > 0)
                    //    this.SetRowCell(pageIndex, rowIndex, 4, miscOrderDetail.HuId);
                    ////批次
                    //if (miscOrderDetail.LotNo != null && miscOrderDetail.LotNo.Trim().Length > 0)
                    //    this.SetRowCell(pageIndex, rowIndex, 5, miscOrderDetail.LotNo);
                    ////数量
                    //this.SetRowCell(pageIndex, rowIndex, 6, miscOrderDetail.Qty.ToString("0.########"));

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
        private void FillHead(FacilityStockMaster facilityStockMaster)
        {
            //订单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(facilityStockMaster.StNo, this.barCodeFontName);
            this.SetRowCell(2, 9, orderCode);
            //Order No.:
            this.SetRowCell(2, 5, facilityStockMaster.StNo);

            this.SetRowCell(2, 2, facilityStockMaster.EffDate.ToString("yyyy-MM-dd"));

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

        //public override IList<object> GetDataList(string code)
        //{
        //    IList<object> list = new List<object>();
        //    MiscOrder miscOrder = miscOrderMgrE.ReLoadMiscOrder(code);
        //    if (miscOrder != null && miscOrder.MiscOrderDetails != null && miscOrder.MiscOrderDetails.Count > 0)
        //    {
        //        list.Add(miscOrder);
        //        list.Add(miscOrder.MiscOrderDetails);
        //    }
        //    return list;
        //}
    }
}

#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Report.Impl
{
    [Transactional]
    public partial class RepFacilityStocktakeMgrE : com.Sconit.Facility.Service.Report.Impl.RepFacilityStocktakeMgr, IReportBaseMgrE
    {


    }
}

#endregion
