using com.Sconit.Service.Ext.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MasterData;
using Castle.Services.Transaction;
using NPOI.HSSF.UserModel;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity;


namespace com.Sconit.Service.Report.Impl
{

    ///
    ///作用：亚虹组装生产单
    ///作者：liqiuyun
    ///编写日期：2011-02-11
    ///
    [Transactional]
    public class RepAssemblyProductionOrderMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }
        public IOrderHeadMgrE orderHeadMgr { get; set; }
        public IOrderLocationTransactionMgrE orderLocationTransactionMgr { get; set; }
        public IFlowMgrE flowMgr { get; set; }

        public RepAssemblyProductionOrderMgr()
        {

            //明细部分的行数
            this.pageDetailRowCount = 35;
            //列数   1起始
            this.columnCount = 9;
            //报表头的行数  1起始
            this.headRowCount = 6;
            //报表尾的行数  1起始
            this.bottomRowCount = 1;

        }

        /**
         * 填充报表
         * 
         * Param list [0]订单头对象
         *            [1]订单明细对象
         *            [2]订单库位事物对象
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                if (list == null || list.Count < 2) return false;

                OrderHead orderHead = (OrderHead)list[0];
                IList<OrderDetail> orderDetails = (IList<OrderDetail>)list[1];

                if (orderHead == null
                    || orderDetails == null || orderDetails.Count == 0)
                {
                    return false;
                }

                this.barCodeFontName = this.GetBarcodeFontName(0, 6);
                this.CopyPage(orderDetails.Count);

                this.FillHead(orderHead);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                orderDetails = orderDetails.OrderBy(r => r.Sequence).ThenBy(r => r.Item.Code).ToList();

                #region 产品信息  Product Information
                foreach (OrderDetail orderDetail in orderDetails)
                {
                    //"成品物料号 FG Item Code"	
                    this.SetRowCell(pageIndex, rowIndex, 0, orderDetail.Item.Code);
                    //"描述Description"	
                    this.SetRowCell(pageIndex, rowIndex, 1, orderDetail.Item.Description);
                    //"单位Unit"	
                    this.SetRowCell(pageIndex, rowIndex, 2, orderDetail.Uom.Code);
                    //"包装UC"	
                    this.SetRowCell(pageIndex, rowIndex, 3, orderDetail.UnitCount.ToString("0.##"));
                    //"计划数Dmd Qty"	
                    this.SetRowCell(pageIndex, rowIndex, 4, orderDetail.OrderedQty.ToString("0.##"));
                    //"合格数Conf Qty"	
                    this.SetRowCell(pageIndex, rowIndex, 5, orderDetail.ReceivedQty.HasValue ? orderDetail.ReceivedQty.Value.ToString("0.##") : string.Empty);
                    //"不合格数NC Qty"	
                    this.SetRowCell(pageIndex, rowIndex, 6, orderDetail.RejectedQty.HasValue ? orderDetail.RejectedQty.Value.ToString("0.##") : string.Empty);
                    //"废品数Scrap Qty"	
                    this.SetRowCell(pageIndex, rowIndex, 7, orderDetail.ScrapQty.HasValue ? orderDetail.ScrapQty.Value.ToString("0.##") : string.Empty);
                    //"收货人Receiver"	

                    this.SetRowCell(pageIndex, rowIndex, 8, orderDetail.Remark);

                    // "收货日期Rct Date"
                    //this.SetRowCell(pageIndex, rowIndex, 9, string.Empty);

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
                #endregion

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
         * Param pageIndex 页号
         * Param orderHead 订单头对象
         * Param orderDetails 订单明细对象
         */
        private void FillHead(OrderHead orderHead)
        {
            #region 报表头
            //工单号码Order code
            this.barCodeFontName = this.GetBarcodeFontName(0, 6);
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(orderHead.OrderNo, this.barCodeFontName);
            this.SetRowCell(0, 6, orderCode);

            // "生产线：Prodline："
            Flow flow = this.flowMgr.LoadFlow(orderHead.Flow);
            this.SetRowCell(1, 1, flow.Code + "[" + flow.Description + "]");
            //"生产班组：Shift："
            this.SetRowCell(1, 3, orderHead.Shift == null ? string.Empty : orderHead.Shift.Code + "[" + orderHead.Shift.ShiftName + "]");
            //发单人：
            this.SetRowCell(1, 7, orderHead.CreateUser.CodeName);
            //开始时间：
            this.SetRowCell(2, 1, orderHead.StartTime.ToString("yyyy-MM-dd HH:mm"));
            //结束时间：
            this.SetRowCell(2, 3, orderHead.WindowTime.ToString("yyyy-MM-dd HH:mm"));
            //交货地点：
            this.SetRowCell(2, 7, orderHead.ShipTo == null ? string.Empty : orderHead.ShipTo.Address);
            //注意事项
            this.SetRowCell(3, 1, orderHead.Memo);

            //if ("Urgent" == orderHead.Priority)
            //{
            //    this.SetRowCell(0, 3, "紧急");
            //}
            //else
            //{
            //    this.SetRowCell(0, 3, "正常");
            //}
            //返工
            if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RWO)
            {
                this.SetRowCell(0, 3, "返工单");
            }
            #endregion
        }

        /**
         * 需要拷贝的数据与合并单元格操作
         * 
         * Param pageIndex 页号
         */
        public override void CopyPageValues(int pageIndex)
        {
            //质量:
            this.CopyCell(pageIndex, 41, 0, "A42");
            //完工确认	
            this.CopyCell(pageIndex, 41, 5, "F42");
        }


        public override IList<object> GetDataList(string code)
        {
            IList<object> list = new List<object>();
            OrderHead orderHead = orderHeadMgr.LoadOrderHead(code, true);
            if (orderHead != null)
            {
                list.Add(orderHead);
                list.Add(orderHead.OrderDetails);
                IList<OrderLocationTransaction> orderLocationTransactions = orderLocationTransactionMgr.GetOrderLocationTransaction(orderHead.OrderNo);
                list.Add(orderLocationTransactions);
            }
            return list;
        }
    }
}


#region Extend Class

namespace com.Sconit.Service.Ext.Report.Impl
{
    /**
     * 
     * 原材料条码
     * 
     */
    [Transactional]
    public partial class RepAssemblyProductionOrderMgrE : com.Sconit.Service.Report.Impl.RepAssemblyProductionOrderMgr, IReportBaseMgrE
    {


    }

}

#endregion
