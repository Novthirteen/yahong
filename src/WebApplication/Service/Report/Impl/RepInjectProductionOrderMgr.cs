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
    ///作用：亚虹注塑生产单
    ///作者：liqiuyun
    ///编写日期：2011-02-11
    ///
    [Transactional]
    public class RepInjectProductionOrderMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }
        public IOrderHeadMgrE orderHeadMgr { get; set; }
        public IOrderLocationTransactionMgrE orderLocationTransactionMgr { get; set; }
        public IFlowMgrE flowMgr { get; set; }

        public RepInjectProductionOrderMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 23;
            //列数   1起始
            this.columnCount = 13;
            //报表头的行数  1起始
            this.headRowCount = 5;
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

                //this.barCodeFontName = this.GetBarcodeFontName(1, 11);
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
                    //this.SetRowCell(pageIndex, rowIndex, 2, orderDetail.Uom.Code);
                    //"包装UC"	
                    //this.SetRowCell(pageIndex, rowIndex, 3, orderDetail.UnitCount.ToString("0.##"));

                    IList<OrderLocationTransaction> orderLocationTransactions
                        = orderLocationTransactionMgr.GetOrderLocationTransaction(orderDetail, BusinessConstants.IO_TYPE_OUT);

                    var q = orderLocationTransactions.Where(o => o.NeedPrint);

                    if (q.Count() > 0)
                    {
                        OrderLocationTransaction orderLocationTransaction = q.FirstOrDefault();
                        //"原料"	
                        this.SetRowCell(pageIndex, rowIndex, 3, orderLocationTransaction.Item.Code);
                        //"原料描述"	
                        this.SetRowCell(pageIndex, rowIndex, 4, orderLocationTransaction.Item.Description);
                        //"原料数"
                        this.SetRowCell(pageIndex, rowIndex, 5, orderLocationTransaction.OrderedQty.ToString("0.####"));

                    }
                    //"设备"
                    this.SetRowCell(pageIndex, rowIndex, 6, orderDetail.Remark);
                    //"成品数 Qty"	
                    this.SetRowCell(pageIndex, rowIndex, 7, orderDetail.OrderedQty.ToString("0.####"));

                    //"合格数"	
                    this.SetRowCell(pageIndex, rowIndex, 8, orderDetail.ReceivedQty.HasValue ? orderDetail.ReceivedQty.Value.ToString("0.####") : string.Empty);
                    //"不合格数"	
                    this.SetRowCell(pageIndex, rowIndex, 9, orderDetail.RejectedQty.HasValue ? orderDetail.RejectedQty.Value.ToString("0.####") : string.Empty);

                    //包材
                    this.SetRowCell(pageIndex, rowIndex, 10, orderDetail.TextField1);
                    //批号
                    this.SetRowCell(pageIndex, rowIndex, 11, orderDetail.TextField2);

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
            this.barCodeFontName = this.GetBarcodeFontName(0, 11);
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(orderHead.OrderNo, this.barCodeFontName);
            this.SetRowCell(0, 11, orderCode);

            //"生产班组：Shift："
            this.SetRowCell(0, 2, orderHead.Shift == null ? string.Empty : orderHead.Shift.Code + "[" + orderHead.Shift.ShiftName + "]");
            // "生产线：Prodline："
            Flow flow = this.flowMgr.LoadFlow(orderHead.Flow);
            this.SetRowCell(0, 6, flow.Code + "[" + flow.Description + "]");
            //开始时间：
            this.SetRowCell(1, 2, orderHead.StartTime.ToString("yyyy-MM-dd hh:mm"));
            //结束时间：
            this.SetRowCell(1, 4, orderHead.WindowTime.ToString("yyyy-MM-dd hh:mm"));
            //发单人：
            this.SetRowCell(1, 6, orderHead.CreateUser.CodeName);
            //注意事项
            this.SetRowCell(2, 1, orderHead.Memo);

            string orderSubType = string.Empty;
            if ("Urgent" == orderHead.Priority)
            {
                orderSubType = "紧急";
            }
            else
            {
                orderSubType = "正常";
            }
            //返工
            if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RWO)
            {
                orderSubType = orderSubType + "返工单";
            }

            this.SetRowCell(0, 4, orderSubType);
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
            this.CopyCell(pageIndex, 28, 0, "A29");
            //完工确认	
            this.CopyCell(pageIndex, 28, 4, "E29");
            this.CopyCell(pageIndex, 28, 11, "L29");
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
    public partial class RepInjectProductionOrderMgrE : com.Sconit.Service.Report.Impl.RepInjectProductionOrderMgr, IReportBaseMgrE
    {


    }

}

#endregion
