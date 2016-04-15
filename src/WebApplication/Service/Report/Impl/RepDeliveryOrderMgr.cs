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

namespace com.Sconit.Service.Report.Impl
{
    [Transactional]
    public class RepDeliveryOrderMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }

        public IOrderHeadMgrE orderHeadMgrE { get; set; }

        public RepDeliveryOrderMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 35;
            //列数   1起始
            this.columnCount = 12;
            //报表头的行数  1起始
            this.headRowCount = 15;
            //报表尾的行数  1起始
            this.bottomRowCount = 2;
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

                OrderHead orderHead = (OrderHead)(list[0]);
                IList<OrderDetail> orderDetails = (IList<OrderDetail>)(list[1]);

                orderDetails = orderDetails.OrderBy(o => o.Sequence).ThenBy(o => o.Item.Code).ToList();

                if (orderHead == null
                    || orderDetails == null || orderDetails.Count == 0)
                {
                    return false;
                }


                //this.SetRowCellBarCode(0, 2, 8);
                this.barCodeFontName = this.GetBarcodeFontName(2, 8);
                this.CopyPage(orderDetails.Count);

                this.FillHead(orderHead);


                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                foreach (OrderDetail orderDetail in orderDetails)
                {
                    // No.	
                    this.SetRowCell(pageIndex, rowIndex, 0, "" + orderDetail.Sequence);

                    //零件号 Item Code
                    this.SetRowCell(pageIndex, rowIndex, 1, orderDetail.Item.Code);

                    //参考号 Ref No.
                    this.SetRowCell(pageIndex, rowIndex, 2, orderDetail.ReferenceItemCode);

                    //描述Description
                    this.SetRowCell(pageIndex, rowIndex, 3, orderDetail.Item.Description);

                    //单位Uom
                    this.SetRowCell(pageIndex, rowIndex, 4, orderDetail.Uom.Code);

                    //单包装UC
                    this.SetRowCell(pageIndex, rowIndex, 5, orderDetail.UnitCount.ToString("0.########"));

                    //需求 Request	包装
                    //int UCs = (int)Math.Ceiling(orderDetail.OrderedQty / orderDetail.UnitCount);
                    //this.SetRowCell(pageIndex, rowIndex, 6, UCs.ToString());

                    //需求 Request	零件数
                    this.SetRowCell(pageIndex, rowIndex, 6, orderDetail.OrderedQty.ToString("0.########"));

                    //Location from
                    this.SetRowCell(pageIndex, rowIndex, 7, orderDetail.DefaultLocationFrom == null ? string.Empty : orderDetail.DefaultLocationFrom.Code);

                    //发货数
                    this.SetRowCell(pageIndex, rowIndex, 8, orderDetail.ShippedQty.HasValue ? orderDetail.ShippedQty.Value.ToString("0.########") : string.Empty);

                    //实收 Received	零件数
                    this.SetRowCell(pageIndex, rowIndex, 9, orderDetail.ReceivedQty.HasValue ? orderDetail.ReceivedQty.Value.ToString("0.########") : string.Empty);

                    //批号/备注
                    this.SetRowCell(pageIndex, rowIndex, 10, orderDetail.Remark);

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

                if (orderHead.IsPrinted == null || orderHead.IsPrinted == false)
                {
                    orderHead.IsPrinted = true;
                    orderHeadMgrE.UpdateOrderHead(orderHead);
                }
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
        private void FillHead(OrderHead orderHead)
        {
            //订单号:
            string orderCode = Utility.BarcodeHelper.GetBarcodeStr(orderHead.OrderNo, this.barCodeFontName);
            this.SetRowCell(2, 8, orderCode);
            //Order No.:
            this.SetRowCell(3, 8, orderHead.OrderNo);

            if (orderHead.SubType == BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN)
            {
                this.SetRowCell(0, 3, "退货");
            }

            if (orderHead.Priority == BusinessConstants.CODE_MASTER_ORDER_PRIORITY_VALUE_NORMAL)
            {
                this.SetRowCell(4, 5, "");
            }
            else
            {
                this.SetRowCell(3, 5, "");
            }

            this.SetRowCell(3, 3, orderHead.ReferenceOrderNo);
            this.SetRowCell(4, 3, orderHead.ExternalOrderNo);

            //制单时间 Create Time:
            this.SetRowCell(4, 9, orderHead.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"));

            //客户代码 Customer Code:	
            this.SetRowCell(6, 3, orderHead.PartyTo != null ? orderHead.PartyTo.Code : String.Empty);
            //开始时间 Start Time:
            this.SetRowCell(6, 8, orderHead.StartTime.ToString("yyyy-MM-dd HH:mm:ss"));

            //客户名称 Supplier Name:		
            this.SetRowCell(7, 3, orderHead.PartyTo != null ? orderHead.PartyTo.Name : String.Empty);
            //窗口时间 Window Time:
            this.SetRowCell(7, 8, orderHead.WindowTime.ToString("yyyy-MM-dd HH:mm:ss"));

            //客户地址 Address:	
            this.SetRowCell(8, 3, orderHead.ShipTo != null ? orderHead.ShipTo.Address : String.Empty);

            //交货道口 Delivery Dock:
            this.SetRowCell(8, 8, orderHead.DockDescription != null ? orderHead.DockDescription : string.Empty);

            //客户联系人 Contact:	
            this.SetRowCell(9, 3, orderHead.ShipTo != null ? orderHead.ShipTo.ContactPersonName : String.Empty);
            //物流协调员 Follow Up:
            this.SetRowCell(9, 8, orderHead.ShipFrom != null ? orderHead.ShipFrom.ContactPersonName : String.Empty);

            //客户电话 Telephone:		
            this.SetRowCell(10, 3, orderHead.ShipTo != null ? orderHead.ShipTo.TelephoneNumber : String.Empty);
            //YFV电话 Telephone:
            this.SetRowCell(10, 8, orderHead.ShipFrom != null ? orderHead.ShipFrom.TelephoneNumber : String.Empty);

            //客户传真 Fax:	
            this.SetRowCell(11, 3, orderHead.ShipTo != null ? orderHead.ShipTo.Fax : String.Empty);
            //YFV传真 Fax:
            this.SetRowCell(11, 8, orderHead.ShipFrom != null ? orderHead.ShipFrom.Fax : String.Empty);

            //系统号 SysCode:
            //this.SetRowCell(++rowNum, 3, "");
            //版本号 Version:
            //this.SetRowCell(rowNum, 8, "");
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            this.CopyCell(pageIndex, 50, 1, "B51");
            this.CopyCell(pageIndex, 50, 5, "F51");
            this.CopyCell(pageIndex, 50, 9, "J51");
            this.CopyCell(pageIndex, 51, 0, "A52");
        }

        public override IList<object> GetDataList(string code)
        {
            IList<object> list = new List<object>();
            OrderHead orderHead = orderHeadMgrE.LoadOrderHead(code, true);
            if (orderHead != null)
            {
                list.Add(orderHead);
                list.Add(orderHead.OrderDetails);
            }
            return list;
        }
    }
}




#region Extend Class

namespace com.Sconit.Service.Ext.Report.Impl
{
    [Transactional]
    public partial class RepDeliveryOrderMgrE : com.Sconit.Service.Report.Impl.RepDeliveryOrderMgr, IReportBaseMgrE
    {


    }
}

#endregion
