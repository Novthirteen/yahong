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
    public class RepBillSOGroupMgr : RepTemplate1
    {
        public override string reportTemplateFolder { get; set; }

        public IBillMgrE billMgrE { get; set; }
        public IReceiptMgrE receiptMgrE { get; set; }
        public IInProcessLocationMgrE inprocessLocationMgrE { get; set; }

        public RepBillSOGroupMgr()
        {
            //明细部分的行数
            this.pageDetailRowCount = 45;
            //列数   1起始
            this.columnCount = 9;
            //报表头的行数  1起始
            this.headRowCount = 4;
            //报表尾的行数  1起始
            this.bottomRowCount = 3;

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
                    //零件号
                    this.SetRowCell(pageIndex, rowIndex, 0, billDetail.Item.Code);
                    //客户零件号
                    this.SetRowCell(pageIndex, rowIndex, 1, billDetail.ReferenceItemCode);//todo
                    //零件名称	
                    this.SetRowCell(pageIndex, rowIndex, 2, billDetail.Item.Description);
                    //出库数量	
                    this.SetRowCell(pageIndex, rowIndex, 3, billDetail.BilledQty.ToString("#,##0.########"));
                    //单位	
                    this.SetRowCell(pageIndex, rowIndex, 4, billDetail.Uom.Code);
                    //单价	
                    this.SetRowCell(pageIndex, rowIndex, 5, billDetail.UnitPrice.ToString("#,##0.########"));
                    //货币
                    this.SetRowCell(pageIndex, rowIndex, 6, billDetail.Currency.Code);
                    //合计	
                    this.SetRowCell(pageIndex, rowIndex, 7, billDetail.GroupAmount.ToString("#,##0.########"));
                    //是否暂估
                    this.SetRowCell(pageIndex, rowIndex, 8, billDetail.IsProvisionalEstimate ? "√" : string.Empty);

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
        private void FillHead(Bill bill)
        {
            //创建时间：
            this.SetRowCell(1, 4, bill.CreateDate.ToString("yyyy-MM-dd HH:mm"));

            //最后修改时间：
            this.SetRowCell(1, 7, bill.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));

            this.SetRowCell(2, 1, bill.BillAddress.Party.Name);
            this.SetRowCell(2, 5, bill.BillNo);

            this.SetRowCell(49, 1, bill.CreateUser.CodeName);
            this.SetRowCell(49, 7, bill.TotalBillDetailAmount.ToString("#,##0.##"));
            this.SetRowCell(50, 7, bill.Discount.HasValue ? bill.Discount.Value.ToString("#,##0.##") : "0");
            this.SetRowCell(51, 7, bill.TotalBillAmount.ToString("#,##0.##"));
        }

        /**
           * 需要拷贝的数据与合并单元格操作
           * 
           * Param pageIndex 页号
           */
        public override void CopyPageValues(int pageIndex)
        {
            //对账员：
            this.CopyCell(pageIndex, 49, 0, "A50");
            this.CopyCell(pageIndex, 49, 1, "B50");
            //主管：
            this.CopyCell(pageIndex, 51, 0, "A52");
            this.CopyCell(pageIndex, 51, 1, "B52");

            //合计:
            this.CopyCell(pageIndex, 49, 6, "G50");
            this.CopyCell(pageIndex, 49, 7, "H50");
            //折扣:
            this.CopyCell(pageIndex, 50, 6, "G51");
            this.CopyCell(pageIndex, 50, 7, "H51");
            //发票:
            this.CopyCell(pageIndex, 51, 6, "G52");
            this.CopyCell(pageIndex, 51, 7, "H52");

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
    public partial class RepBillSOGroupMgrE : com.Sconit.Service.Report.Impl.RepBillSOGroupMgr, IReportBaseMgrE
    {

    }
}

#endregion
