using com.Sconit.Service.Ext.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.Sconit.Entity.MasterData;
using Castle.Services.Transaction;
using com.Sconit.Utility;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Service.MasterData;

namespace com.Sconit.Service.Report.Impl
{
    /// <summary>
    /// 采购账单
    /// </summary>
    [Transactional]
    public class RepBillMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }
        public IReceiptMgrE receiptMgrE { get; set; }
        public IBillMgrE billMgrE { get; set; }
        public RepBillMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 26;
            //列数   1起始
            this.columnCount = 12;
            //报表头的行数  1起始
            this.headRowCount = 10;
            //报表尾的行数  1起始
            this.bottomRowCount = 1;
        }

        /**
         * 填充报表
         * 
         * Param list [0]OrderHead
         * Param list [0]IList<OrderDetail>           
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
                decimal totalPrice = 0;
                foreach (BillDetail billDetail in billDetails)
                {

                    //采购单	
                    this.SetRowCell(pageIndex, rowIndex, 0, billDetail.ActingBill.OrderNo);
                    //送货单号
                    this.SetRowCell(pageIndex, rowIndex, 1, billDetail.IpNo);
                    //收货单号	
                    this.SetRowCell(pageIndex, rowIndex, 2, billDetail.ActingBill.ReceiptNo);
                    //零件号	
                    this.SetRowCell(pageIndex, rowIndex, 3, billDetail.ActingBill.Item.Code);
                    //零件名称	
                    this.SetRowCell(pageIndex, rowIndex, 4, billDetail.ActingBill.Item.Description);
                    //单位	
                    this.SetRowCell(pageIndex, rowIndex, 5, billDetail.ActingBill.Uom.Code);
                    //入库数量 
                    this.SetRowCell(pageIndex, rowIndex, 6, billDetail.BilledQty.ToString("#,##0.########"));
                    //采购单价	
                    this.SetRowCell(pageIndex, rowIndex, 7, billDetail.UnitPrice.ToString("#,##0.########"));
                    //货币
                    this.SetRowCell(pageIndex, rowIndex, 8, billDetail.Currency.Code);
                    //金额
                    this.SetRowCell(pageIndex, rowIndex, 9, billDetail.Amount.ToString("#,##0.########"));
                    //入库日期
                    this.SetRowCell(pageIndex, rowIndex, 10, billDetail.ActingBill.EffectiveDate.ToString("yyyy-MM-dd"));
                    //是否暂估
                    this.SetRowCell(pageIndex, rowIndex, 11, billDetail.ActingBill.IsProvisionalEstimate ? "暂估" : string.Empty);
                    if (billDetail.BilledQty == 0)
                    {
                        this.SetRowCell(pageIndex, rowIndex, 11, "补开");
                        if (billDetail.ActingBill.BillQty > 0)
                        {
                            decimal price = billDetail.Amount / billDetail.ActingBill.BillQty;
                            this.SetRowCell(pageIndex, rowIndex, 7, price.ToString("#,##0.########"));
                        }
                    }
                    if (!this.isPageBottom(rowIndex, rowTotal))//页的最后一行
                    {
                        rowIndex++;
                    }
                    else
                    {
                        pageIndex++;
                        rowIndex = 0;
                    }
                    rowTotal++;
                }

                this.sheet.DisplayGridlines = false;
                this.sheet.IsPrintGridlines = false;

                //this.SetRowCell(pageIndex, 36, 3, bill.TotalBillDetailAmount.ToString("#,##0.########"));
                //this.SetRowCell(pageIndex, 36, 5, bill.Discount.HasValue ? bill.Discount.Value.ToString("#,##0.########") : "0");
                //this.SetRowCell(pageIndex, 36, 9, bill.TotalBillAmount.ToString("#,##0.########"));
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
         * Param repack 头对象
         */
        private void FillHead(Bill bill)
        {
            //创建时间：
            this.SetRowCell(5, 3, bill.CreateDate.ToString("yyyy-MM-dd HH:mm"));

            //最后修改时间：
            this.SetRowCell(6, 3, bill.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));

            this.SetRowCell(8, 2, bill.BillAddress.Party.Name);
            this.SetRowCell(8, 5, bill.BillNo);
            this.SetRowCell(8, 10, bill.CreateUser.Name);

            this.SetRowCell(36, 3, bill.TotalBillDetailAmount.ToString("#,##0.##"));
            this.SetRowCell(36, 5, bill.Discount.HasValue ? bill.Discount.Value.ToString("#,##0.##") : "0");
            this.SetRowCell(36, 9, bill.TotalBillAmount.ToString("#,##0.##"));
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //供应商：
            this.CopyCell(pageIndex, 36, 0, "A37");
            //this.CopyCell(pageIndex, 36, 1, "B37");
            //合计:
            this.CopyCell(pageIndex, 36, 2, "C37");
            this.CopyCell(pageIndex, 36, 3, "D37");
            //折扣:
            this.CopyCell(pageIndex, 36, 4, "E37");
            this.CopyCell(pageIndex, 36, 5, "F37");
            //发票:
            this.CopyCell(pageIndex, 36, 8, "I37");
            this.CopyCell(pageIndex, 36, 9, "J37");
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
    public partial class RepBillMgrE : com.Sconit.Service.Report.Impl.RepBillMgr, IReportBaseMgrE
    {

    }
}

#endregion