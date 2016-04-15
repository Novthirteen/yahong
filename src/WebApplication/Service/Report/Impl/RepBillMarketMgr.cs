using com.Sconit.Service.Ext.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Distribution;
using Castle.Services.Transaction;
using com.Sconit.Utility;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Service.Ext.Distribution;

namespace com.Sconit.Service.Report.Impl
{
    [Transactional]
    public class RepBillMarketMgr : RepTemplate1
    {
        /// <summary>
        /// 销售账单
        /// </summary>
        public override string reportTemplateFolder { get; set; }

        public IBillMgrE billMgrE { get; set; }
        public IReceiptMgrE receiptMgrE { get; set; }
        public IInProcessLocationMgrE inprocessLocationMgrE { get; set; }

        public RepBillMarketMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 32;
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
         * Param list [0]bill
         * Param list [0]IList<BillDetail>           
         */
        protected override bool FillValuesImpl(String templateFileName, IList<object> list)
        {
            try
            {
                if (list == null || list.Count < 2) return false;

                Bill bill = (Bill)(list[0]);
                IList<BillDetail> billDetails = (IList<BillDetail>)(list[1]);


                if (bill == null
                    || billDetails == null || billDetails.Count == 0)
                {
                    return false;
                }

                this.CopyPage(billDetails.Count);

                this.FillHead(bill);

                int pageIndex = 1;
                int rowIndex = 0;
                int rowTotal = 0;
                foreach (BillDetail billDetail in billDetails)
                {
                    //客户回单号	
                    this.SetRowCell(pageIndex, rowIndex, 0, billDetail.ActingBill.ExternalReceiptNo);
                    //销售单号	
                    this.SetRowCell(pageIndex, rowIndex, 1, billDetail.ActingBill.OrderNo);
                    //if (billDetail.ActingBill.ReceiptNo != null && billDetail.ActingBill.ReceiptNo.Length > 0)
                    //{
                    //    Receipt receipt = receiptMgrE.LoadReceipt(billDetail.ActingBill.ReceiptNo);
                    //    //出门证号
                    //    this.SetRowCell(pageIndex, rowIndex, 2, receipt.ReferenceIpNo);
                    //}

                    //出门证号
                    this.SetRowCell(pageIndex, rowIndex, 2, billDetail.IpNo);

                    //零件号
                    this.SetRowCell(pageIndex, rowIndex, 3, billDetail.ActingBill.Item.Code);
                    //客户零件号
                    this.SetRowCell(pageIndex, rowIndex, 4, billDetail.ActingBill.ReferenceItemCode);//todo
                    //零件名称	
                    this.SetRowCell(pageIndex, rowIndex, 5, billDetail.ActingBill.Item.Description);
                    //出库数量	
                    this.SetRowCell(pageIndex, rowIndex, 6, billDetail.BilledQty.ToString("#,##0.########"));
                    //单位	
                    this.SetRowCell(pageIndex, rowIndex, 7, billDetail.ActingBill.Uom.Code);
                    //单价	
                    this.SetRowCell(pageIndex, rowIndex, 8, billDetail.UnitPrice.ToString("#,##0.########"));
                    //货币
                    this.SetRowCell(pageIndex, rowIndex, 9, billDetail.Currency.Code);
                    //合计	
                    this.SetRowCell(pageIndex, rowIndex, 10, billDetail.Amount.ToString("#,##0.########"));
                    //出库日期(发货日期)
                    InProcessLocation ipLocation = inprocessLocationMgrE.LoadInProcessLocation(billDetail.IpNo);
                    this.SetRowCell(pageIndex, rowIndex, 11, ipLocation.CreateDate.ToString("yyyy-MM-dd"));
                    //是否暂估
                    this.SetRowCell(pageIndex, rowIndex, 12, billDetail.ActingBill.IsProvisionalEstimate ? "√" : string.Empty);
                    if (billDetail.BilledQty == 0)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 12, "补开");
                        if (billDetail.ActingBill.BillQty > 0)
                        {
                            decimal price = billDetail.Amount / billDetail.ActingBill.BillQty;
                            this.SetRowCell(pageIndex, rowIndex, 8, price.ToString("#,##0.########"));
                        }
                    }
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

                //this.SetRowCell(pageIndex, 37, 1, bill.CreateUser.Name);
                //this.SetRowCell(pageIndex, 37, 4, bill.TotalBillDetailAmount.ToString("#,##0.########"));
                //this.SetRowCell(pageIndex, 37, 6, bill.Discount.HasValue ? bill.Discount.Value.ToString("#,##0.########") : "0");
                //this.SetRowCell(pageIndex, 37, 10, bill.TotalBillAmount.ToString("#,##0.########"));
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
        private void FillHead(Bill bill)
        {
            //创建时间：
            this.SetRowCell(2, 2, bill.CreateDate.ToString("yyyy-MM-dd HH:mm"));

            //最后修改时间：
            this.SetRowCell(2, 6, bill.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));

            this.SetRowCell(3, 2, bill.BillAddress.Party.Name);
            this.SetRowCell(3, 8, bill.BillNo);

            this.SetRowCell(37, 1, bill.CreateUser.Name);
            this.SetRowCell(37, 4, bill.TotalBillDetailAmount.ToString("#,##0.##"));
            this.SetRowCell(37, 6, bill.Discount.HasValue ? bill.Discount.Value.ToString("#,##0.##") : "0");
            this.SetRowCell(37, 10, bill.TotalBillAmount.ToString("#,##0.##"));
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //对账员：
            //this.CopyCell(pageIndex, 37, 1, "B38");
            //主管：
            //this.CopyCell(pageIndex, 37, 7, "H38");

            //对账员：
            this.CopyCell(pageIndex, 37, 0, "A38");
            this.CopyCell(pageIndex, 37, 1, "B38");
            //合计:
            this.CopyCell(pageIndex, 37, 3, "D38");
            this.CopyCell(pageIndex, 37, 4, "E38");
            //折扣:
            this.CopyCell(pageIndex, 37, 5, "F38");
            this.CopyCell(pageIndex, 37, 6, "G38");
            //发票:
            this.CopyCell(pageIndex, 37, 9, "J38");
            this.CopyCell(pageIndex, 37, 10, "K38");
        }

        public override IList<object> GetDataList(string code)
        {
            IList<object> list = new List<object>();
            Bill bill = billMgrE.LoadBill(code, true);
            if (bill != null)
            {
                list.Add(bill);
                list.Add(bill.BillDetails);
            }
            return list;
        }
    }
}




#region Extend Class

namespace com.Sconit.Service.Ext.Report.Impl
{
    [Transactional]
    public partial class RepBillMarketMgrE : com.Sconit.Service.Report.Impl.RepBillMarketMgr, IReportBaseMgrE
    {

    }
}

#endregion
