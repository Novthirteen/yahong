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
    public class RepVisteonASNMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }

        public IOrderHeadMgrE orderHeadMgrE { get; set; }

        public IInProcessLocationMgrE inProcessLocationMgrE { get; set; }

        public RepVisteonASNMgr()
        {
            //明细部分的行数 
            this.pageDetailRowCount = 15;
            //列数   1起始
            this.columnCount = 10;
            //报表头的行数  1起始
            this.headRowCount = 4;
            //报表尾的行数  1起始
            this.bottomRowCount = 1;
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
                if (list == null || list.Count < 2) return false;

                InProcessLocation inProcessLocation = (InProcessLocation)list[0];
                IList<InProcessLocationDetail> inProcessLocationDetailList = (IList<InProcessLocationDetail>)list[1];

                if (inProcessLocation == null
                    || inProcessLocationDetailList == null || inProcessLocationDetailList.Count == 0)
                {
                    return false;
                }

                this.barCodeFontName = this.GetBarcodeFontName(0, 8);

                this.CopyPage(inProcessLocationDetailList.Count);

                this.FillHead(inProcessLocation);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;

                //ASN号:
                //List<Transformer> transformerList = Utility.TransformerHelper.ConvertInProcessLocationDetailsToTransformers(inProcessLocationDetailList);

                foreach (var ipDetail in inProcessLocationDetailList)
                {
                    OrderHead orderHead = ipDetail.OrderLocationTransaction.OrderDetail.OrderHead;
                    //序号	物流号	部品番号	部品名称	条码	单包装	送货数量	数量条码	购买订单号	订单号条码
                    //[序号]
                    this.SetRowCell(pageIndex, rowIndex, 0, rowIndex + 1);
                    //[物流号]
                    this.SetRowCell(pageIndex, rowIndex, 1, ipDetail.OrderLocationTransaction.Item.Code);
                    //[部品番号???]
                    this.SetRowCell(pageIndex, rowIndex, 2, ipDetail.OrderLocationTransaction.Item.Desc2);
                    //[部品名称]"描述Description"	
                    this.SetRowCell(pageIndex, rowIndex, 3, ipDetail.OrderLocationTransaction.Item.Desc1);
                    //[条码???]
                    string desc2 = Utility.BarcodeHelper.GetBarcodeStr(ipDetail.OrderLocationTransaction.Item.Desc2, this.barCodeFontName);
                    this.SetRowCell(pageIndex, rowIndex, 4, desc2);
                    //[单包装]"单包装UC"	
                    this.SetRowCell(pageIndex, rowIndex, 5, ipDetail.UnitCount.ToString("0.##"));
                    //[送货数量]发货 Delivery
                    this.SetRowCell(pageIndex, rowIndex, 6, ipDetail.Qty.ToString("0.##"));
                    //[数量条码]
                    string qty = Utility.BarcodeHelper.GetBarcodeStr(ipDetail.Qty.ToString("0.##"), this.barCodeFontName);
                    this.SetRowCell(pageIndex, rowIndex, 7, qty);
                    //[购买订单号]
                    this.SetRowCell(pageIndex, rowIndex, 8, ipDetail.ReferenceItemCode);
                    //[订单号条码]
                    string referenceItemCode = Utility.BarcodeHelper.GetBarcodeStr(ipDetail.ReferenceItemCode, this.barCodeFontName);
                    this.SetRowCell(pageIndex, rowIndex, 9, referenceItemCode);

                    if (this.isPageBottom(rowIndex, rowTotal))//页的最后一行
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

                if (inProcessLocation.IsPrinted == null || inProcessLocation.IsPrinted == false)
                {
                    inProcessLocation.IsPrinted = true;
                    inProcessLocationMgrE.UpdateInProcessLocation(inProcessLocation);
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
         * Param pageIndex 页号
         * Param orderHead 订单头对象
         * Param orderDetails 订单明细对象
         */
        protected void FillHead(InProcessLocation inProcessLocation)
        {
            string asnCode = Utility.BarcodeHelper.GetBarcodeStr(inProcessLocation.IpNo, this.barCodeFontName);
            //BarCode:
            this.SetRowCell(0, 8, asnCode);
            //ASN NO:
            this.SetRowCell(2, 8, inProcessLocation.IpNo);
            //制单时间 Window Time:???
            this.SetRowCell(0, 3, inProcessLocation.InProcessLocationDetails[0].OrderLocationTransaction.OrderDetail.OrderHead.WindowTime.ToString("yyyy-MM-dd HH:mm"));
            //制单人
            this.SetRowCell(0, 5, inProcessLocation.CreateUser.CodeName);
            //客户联系人	
            this.SetRowCell(1, 3, inProcessLocation.ShipTo == null ? string.Empty : inProcessLocation.ShipTo.ContactPersonName);
            //客户电话 Telephone:		
            this.SetRowCell(1, 5, inProcessLocation.ShipTo == null ? string.Empty : inProcessLocation.ShipTo.TelephoneNumber);
            //客户地址	
            this.SetRowCell(2, 3, inProcessLocation.PartyTo == null ? string.Empty : inProcessLocation.ShipTo.Address);
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //实际到货时间:
            this.CopyCell(pageIndex, 19, 1, "A20");
            //发单人签字:
            this.CopyCell(pageIndex, 19, 3, "D20");
            //检验签字:
            this.CopyCell(pageIndex, 19, 5, "F20");
            //客户签字:
            this.CopyCell(pageIndex, 19, 8, "I20");
            ////* 我已阅读延锋杰华的安全告知！
            //this.CopyCell(pageIndex, 51, 0, "A52");
        }

        public override IList<object> GetDataList(string code)
        {
            IList<object> list = new List<object>();
            InProcessLocation inProcessLocation = inProcessLocationMgrE.LoadInProcessLocation(code, true);
            if (inProcessLocation != null)
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
    public partial class RepVisteonASNMgrE : com.Sconit.Service.Report.Impl.RepVisteonASNMgr, IReportBaseMgrE
    {

    }


}



#endregion
