using com.Sconit.Service.Ext.MasterData;


using System;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using com.Sconit.Persistence.MasterData;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class BillDetailMgr : BillDetailBaseMgr, IBillDetailMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }

        #region Customized Methods
        [Transaction(TransactionMode.Unspecified)]
        public IList<BillDetail> GetBillDetail(string billNo)
        {
            DetachedCriteria criteria = DetachedCriteria.For<BillDetail>();
            criteria.Add(Expression.Eq("Bill.BillNo", billNo));

            return this.criteriaMgrE.FindAll<BillDetail>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public BillDetail TransferAtingBill2BillDetail(ActingBill actingBill)
        {
            EntityPreference entityPreference = this.entityPreferenceMgrE.LoadEntityPreference(
                BusinessConstants.ENTITY_PREFERENCE_CODE_AMOUNT_DECIMAL_LENGTH);
            int amountDecimalLength = int.Parse(entityPreference.Value);
            BillDetail billDetail = new BillDetail();
            billDetail.ActingBill = actingBill;

            billDetail.Currency = actingBill.Currency;
            billDetail.IsIncludeTax = actingBill.IsIncludeTax;
            billDetail.TaxCode = actingBill.TaxCode;
            billDetail.UnitPrice = actingBill.UnitPrice;
            billDetail.ListPrice = actingBill.ListPrice;
            billDetail.BilledQty = actingBill.CurrentBillQty;
            billDetail.Amount = actingBill.CurrentBillAmount;
            billDetail.Discount = actingBill.CurrentDiscount;
            billDetail.HeadDiscount = actingBill.CurrentHeadDiscount;
            billDetail.LocationFrom = actingBill.LocationFrom;
            billDetail.IpNo = actingBill.IpNo;
            billDetail.ReferenceItemCode = actingBill.ReferenceItemCode;
            billDetail.FlowCode = actingBill.FlowCode;
            billDetail.CostCenter = actingBill.CostCenter;
            billDetail.CostGroup = actingBill.CostGroup;
            billDetail.IsProvisionalEstimate = actingBill.IsProvisionalEstimate;
            billDetail.RecTime = actingBill.RecTime;
            billDetail.InvIOTime = actingBill.InvIOTime;
            if (actingBill.CurrentBillQty != (actingBill.BillQty - actingBill.BilledQty))
            {
                //本次开票数量大于剩余数量
                if (actingBill.CurrentBillQty > (actingBill.BillQty - actingBill.BilledQty))
                {
                    throw new BusinessErrorException("ActingBill.Error.CurrentBillQtyGeRemainQty");
                }

            }

            if (actingBill.CurrentBillAmount != (actingBill.BillAmount - actingBill.BilledAmount))
            {
                //本次开票金额大于剩余金额
                //if (actingBill.CurrentBillAmount > (actingBill.BillAmount - actingBill.BilledAmount))
                //{
                //    throw new BusinessErrorException("ActingBill.Error.CurrentBillAmountGeRemainAmount");
                //}

            }

            return billDetail;
        }

        #endregion Customized Methods       
    }    
}


#region Extend Class


namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class BillDetailMgrE : com.Sconit.Service.MasterData.Impl.BillDetailMgr, IBillDetailMgrE
    {
       
    }    
}
#endregion
