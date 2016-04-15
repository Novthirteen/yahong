using com.Sconit.Service.Ext.Report;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.Distribution;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using com.Sconit.Service;
using com.Sconit.Service.Ext.MasterData;
using Castle.Services.Transaction;
using com.Sconit.Service.Distribution;
using com.Sconit.Service.Ext.Distribution;
using com.Sconit.Service.MasterData.Impl;
using com.Sconit.Service.MasterData;


namespace com.Sconit.Service.Report.Impl
{
    [Transactional]
    public class RepDeliveryNoteMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }

        public IOrderHeadMgrE orderHeadMgrE { get; set; }

        public IInProcessLocationMgrE inProcessLocationMgrE { get; set; }

        public RepDeliveryNoteMgr()
        {
            //明细部分的行数 
            this.pageDetailRowCount = 35;
            //列数   1起始
            this.columnCount = 10;
            //报表头的行数  1起始
            this.headRowCount = 15;
            //报表尾的行数  1起始
            this.bottomRowCount = 2;
        }

        /**
         * 填充报表
         * 
         * Param list [0]InProcessLocation
         *            [1]inProcessLocationDetailList
         */
        [Transaction(TransactionMode.Requires)]
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                if(list == null || list.Count < 2) return false;

                InProcessLocation inProcessLocation = (InProcessLocation)list[0];
                IList<InProcessLocationDetail> inProcessLocationDetailList = (IList<InProcessLocationDetail>)list[1];

                if(inProcessLocation == null
                    || inProcessLocationDetailList == null || inProcessLocationDetailList.Count == 0)
                {
                    return false;
                }

                this.barCodeFontName = this.GetBarcodeFontName(2, 6);

                this.CopyPage(inProcessLocationDetailList.Count);

                this.FillHead(inProcessLocation);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;

                //ASN号:

                List<Transformer> transformerList = Utility.TransformerHelper.ConvertInProcessLocationDetailsToTransformers(inProcessLocationDetailList);

                foreach(Transformer transformer in transformerList)
                {
                    OrderHead orderHead = orderHeadMgrE.LoadOrderHead(transformer.OrderNo);

                    //订单号 (销售发运显示客户订单号,其它为订单号)
                    if(orderHead.Type == BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION
                        && orderHead.ExternalOrderNo != null && orderHead.ExternalOrderNo.Trim() != string.Empty)
                        this.SetRowCell(pageIndex, rowIndex, 0, orderHead.ExternalOrderNo);
                    else
                        this.SetRowCell(pageIndex, rowIndex, 0, orderHead.OrderNo);

                    //序号.
                    //新南港客户化,销售发运单显示批号
                    if(templateFileName == "DeliveryNote2.xls")
                        this.SetRowCell(pageIndex, rowIndex, 1, transformer.LotNo);
                    else
                        this.SetRowCell(pageIndex, rowIndex, 1, transformer.Sequence.ToString());

                    //"零件号Item Code"	
                    this.SetRowCell(pageIndex, rowIndex, 2, transformer.ItemCode);
                    //"参考号Ref No."	
                    this.SetRowCell(pageIndex, rowIndex, 3, transformer.ReferenceItemCode);

                    //"描述Description"	
                    this.SetRowCell(pageIndex, rowIndex, 4, transformer.ItemDescription);
                    //"单位Unit"	
                    this.SetRowCell(pageIndex, rowIndex, 5, transformer.UomCode);
                    //"单包装UC"	
                    this.SetRowCell(pageIndex, rowIndex, 6, transformer.UnitCount.ToString("0.##"));
                    //发货 Delivery	包装
                    //int UCs = (int)Math.Ceiling(transformer.Qty / transformer.UnitCount);
                    //this.SetRowCell(pageIndex, rowIndex, 7, UCs.ToString());

                    //发货 Delivery	发货数
                    this.SetRowCell(pageIndex, rowIndex, 7, transformer.Qty.ToString("0.##"));

                    //实收 Received	包装
                    //this.SetRowCell(pageIndex, rowIndex, 8, "");
                    //实收 Received	零件数
                    //this.SetRowCell(pageIndex, rowIndex, 9, "");


                    if(this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                    {
                        //实际到货时间:
                        //this.SetRowCell(pageIndex, rowIndex, , "");

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

                if(inProcessLocation.IsPrinted == null || inProcessLocation.IsPrinted == false)
                {
                    inProcessLocation.IsPrinted = true;
                    inProcessLocationMgrE.UpdateInProcessLocation(inProcessLocation);
                }
            }
            catch(Exception)
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
        protected void FillHead(InProcessLocation inProcessLocation)
        {
            string asnCode = Utility.BarcodeHelper.GetBarcodeStr(inProcessLocation.IpNo, this.barCodeFontName);
            //ASN号:
            this.SetRowCell(2, 6, asnCode);
            //this.AddPicture
            //ASN No.:
            this.SetRowCell(3, 6, inProcessLocation.IpNo);

            //窗口时间 Window Time:
            this.SetRowCell(4, 6, inProcessLocation.CreateDate.ToString("yyyy-MM-dd HH:mm"));

            //供应商代码 Supplier Code:
            this.SetRowCell(6, 3, inProcessLocation.PartyFrom == null ? string.Empty : inProcessLocation.PartyFrom.Code);
            //供应商名称 Supplier Name:		
            this.SetRowCell(7, 3, inProcessLocation.PartyFrom == null ? string.Empty : inProcessLocation.PartyFrom.Name);
            //供应商地址 Supplier Address:		
            this.SetRowCell(8, 3, inProcessLocation.ShipFrom == null ? string.Empty : inProcessLocation.ShipFrom.Address);
            //供应商联系人 Contact:		
            this.SetRowCell(9, 3, inProcessLocation.ShipFrom == null ? string.Empty : inProcessLocation.ShipFrom.ContactPersonName);
            //供应商联系人 Telephone:		
            this.SetRowCell(10, 3, inProcessLocation.ShipFrom == null ? string.Empty : inProcessLocation.ShipFrom.TelephoneNumber);
            //供应商电话 Fax:		
            this.SetRowCell(11, 3, inProcessLocation.ShipFrom == null ? string.Empty : inProcessLocation.ShipFrom.Fax);


            //客户代码 Customer Code:
            this.SetRowCell(6, 6, inProcessLocation.PartyTo == null ? string.Empty : inProcessLocation.PartyTo.Code);
            //客户名称 Customer Name:		
            this.SetRowCell(7, 6, inProcessLocation.PartyTo == null ? string.Empty : inProcessLocation.PartyTo.Name);
            //收货地址 Address:		
            this.SetRowCell(8, 6, inProcessLocation.ShipTo == null ? string.Empty : inProcessLocation.ShipTo.Address);
            //到货时间 Window Time:		
            this.SetRowCell(9, 6, inProcessLocation.InProcessLocationDetails[0].OrderLocationTransaction.OrderDetail.OrderHead.WindowTime.ToString("yyyy-MM-dd HH:mm"));
            //物流协调员 Follow Up:		
            this.SetRowCell(10, 6, inProcessLocation.ShipTo == null ? string.Empty : inProcessLocation.ShipTo.ContactPersonName);
            //客户电话 Telephone:		
            this.SetRowCell(11, 6, inProcessLocation.ShipTo == null ? string.Empty : inProcessLocation.ShipTo.TelephoneNumber);
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //实际到货时间:
            this.CopyCell(pageIndex, 50, 1, "B51");
            //发单人签字:
            this.CopyCell(pageIndex, 50, 3, "D51");
            //检验签字:
            this.CopyCell(pageIndex, 50, 4, "E51");
            //客户签字:
            this.CopyCell(pageIndex, 50, 7, "H51");
            //* 我已阅读延锋杰华的安全告知！
            this.CopyCell(pageIndex, 51, 0, "A52");
        }

        public override IList<object> GetDataList(string code)
        {
            IList<object> list = new List<object>();
            InProcessLocation inProcessLocation = inProcessLocationMgrE.LoadInProcessLocation(code, true);
            if(inProcessLocation != null)
            {
                list.Add(inProcessLocation);
            }
            return list;
        }
    }
}


#region Extend Class




namespace com.Sconit.Service.Ext.Report.Impl
{
    [Transactional]
    public partial class RepDeliveryNoteMgrE : com.Sconit.Service.Report.Impl.RepDeliveryNoteMgr, IReportBaseMgrE
    {

    }


}



#endregion