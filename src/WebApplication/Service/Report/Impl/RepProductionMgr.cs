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
    ///作用：亚虹组装生产单 一料一单
    ///作者：liqiuyun
    ///编写日期：2011-06-15
    ///
    [Transactional]
    public class RepProductionMgr : RepTemplate1
    {

        public override string reportTemplateFolder { get; set; }
        public IOrderHeadMgrE orderHeadMgr { get; set; }
        public IOrderLocationTransactionMgrE orderLocationTransactionMgr { get; set; }
        public IFlowMgrE flowMgr { get; set; }

        public RepProductionMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 36;
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
                this.CopyPage(orderDetails.Count * this.pageDetailRowCount);

                this.FillHead(orderHead);

                int pageIndex = 1;


                orderDetails = orderDetails.OrderBy(r => r.Sequence).ThenBy(r => r.Item.Code).ToList();

                #region 产品信息  Product Information
                foreach (OrderDetail orderDetail in orderDetails)
                {

                    int rowIndex = 0;

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


                    var query = from o in orderDetail.OrderLocationTransactions
                                where o.IOType == BusinessConstants.IO_TYPE_OUT
                                group o by new { o.Item, o.Uom, o.UnitQty } into g
                                select new OrderLocationTransaction
                                {
                                    Item = g.Key.Item,
                                    Uom = g.Key.Uom,
                                    UnitQty = g.Key.UnitQty,
                                    OrderedQty = g.Sum(d => d.OrderedQty),
                                };

                    rowIndex = 4;
                    int transRowIndes = rowIndex;
                    foreach (var item in query)
                    {

                        this.SetMergedRegion(pageIndex, transRowIndes + this.headRowCount, 6, transRowIndes + this.headRowCount, 8);

                        //"成品物料号 FG Item Code"	
                        this.SetRowCell(pageIndex, transRowIndes, 0, item.Item.Code);
                        //"描述Description"	
                        this.SetRowCell(pageIndex, transRowIndes, 1, item.Item.Description);
                        //"单位Unit"	
                        this.SetRowCell(pageIndex, transRowIndes, 2, item.Uom.Code);
                        //"包装UC"	
                        this.SetRowCell(pageIndex, transRowIndes, 3, item.UnitQty.ToString("0.####"));
                        //"计划数Dmd Qty"	
                        this.SetRowCell(pageIndex, transRowIndes, 4, item.OrderedQty.ToString("0.########"));
                        //""	
                        this.SetRowCell(pageIndex, transRowIndes, 5, item.AccumulateQty.HasValue ? item.AccumulateQty.Value.ToString("0.########") : string.Empty);
                        transRowIndes++;
                        if (transRowIndes > 32)
                        {
                            this.SetRowCell(pageIndex, transRowIndes, 6, "原材料明细不能大于32条");
                            break;
                        }
                    }
                    pageIndex++;
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
            this.SetRowCell(2, 1, orderHead.StartTime.ToString("yyyy-MM-dd hh:mm"));
            //结束时间：
            this.SetRowCell(2, 3, orderHead.WindowTime.ToString("yyyy-MM-dd hh:mm"));
            //交货地点：
            this.SetRowCell(2, 7, orderHead.ShipTo == null ? string.Empty : orderHead.ShipTo.Address);
            //注意事项            
            this.SetRowCell(3, 1, orderHead.Memo);


            if (orderHead.Priority == BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_URGENT)
            {
                this.SetRowCell(0, 2, "紧急");
            }
            else
            {
                this.SetRowCell(0, 2, "正常");
            }
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

            this.SetMergedRegion(pageIndex, 9, 6, 9, 8);


            //物料  Materials	
            this.CopyCell(pageIndex, 8, 0, "A9");
            //物料号	
            this.CopyCell(pageIndex, 9, 0, "A10");
            //物料描述	
            this.CopyCell(pageIndex, 9, 1, "B10");
            //单位	
            this.CopyCell(pageIndex, 9, 2, "C10");
            //单用量	
            this.CopyCell(pageIndex, 9, 3, "D10");
            //计划数	
            this.CopyCell(pageIndex, 9, 4, "E10");
            //实消耗
            this.CopyCell(pageIndex, 9, 5, "F10");
            //其他		
            this.CopyCell(pageIndex, 9, 6, "G10");


            //质量:
            this.CopyCell(pageIndex, 42, 0, "A43");
            //完工确认	
            this.CopyCell(pageIndex, 42, 5, "F43");
        }


        public override IList<object> GetDataList(string code)
        {
            IList<object> list = new List<object>();
            OrderHead orderHead = orderHeadMgr.LoadOrderHead(code, true, false, true);
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
    public partial class RepProductionMgrE : com.Sconit.Service.Report.Impl.RepProductionMgr, IReportBaseMgrE
    {


    }

}

#endregion
