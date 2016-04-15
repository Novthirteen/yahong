using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence.Cost;
using com.Sconit.Entity.Cost;
using com.Sconit.Entity.Exception;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Service.Ext.Cost;
using com.Sconit.Entity;
using System.Linq;

//TODO: Add other using statements here.

namespace com.Sconit.Service.Cost.Impl
{
    [Transactional]
    public class CostTransactionMgr : CostTransactionBaseMgr, ICostTransactionMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IFinanceCalendarMgrE financeCalendarMgr { get; set; }
        public ICostDetailMgrE costDetailMgr { get; set; }
        public ICostCenterMgrE costCenterMgr { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgr { get; set; }
        public IItemMgrE itemMgr { get; set; }
        public ICurrencyMgrE currencyMgr { get; set; }


        [Transaction(TransactionMode.Requires)]
        public void RecordChangeCostBalance(Dictionary<string, decimal> itemValue, User user, DateTime effectiveDate, string costCenterCode)
        {
            foreach (var item in itemValue)
            {
                RecordChangeCostBalance(item.Key, costCenterCode, item.Value, null, user, effectiveDate);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void RecordChangeCostBalance(string item, string costCenterCode, decimal amount, string orderNo, User user, DateTime effectiveDate)
        {
            FinanceCalendar financeCalendar = this.financeCalendarMgr.GetLastestOpenFinanceCalendar();
            int lastFinanceYear = financeCalendar.FinanceMonth != 1 ? financeCalendar.FinanceYear : financeCalendar.FinanceYear - 1;
            int lastFinanceMonth = financeCalendar.FinanceMonth != 1 ? financeCalendar.FinanceMonth - 1 : 12;
            CostCenter cc = costCenterMgr.CheckAndLoadCostCenter(costCenterCode);

            IList<CostDetail> costDetailList = costDetailMgr.GetCostDetail(item, cc.CostGroup.Code, lastFinanceYear, lastFinanceMonth);

            if (costDetailList != null && costDetailList.Count > 0)
            {
                decimal remainAmount = amount;
                decimal detailAmount = 0;
                int decimalLength = Convert.ToInt32(entityPreferenceMgr.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_AMOUNT_DECIMAL_LENGTH).Value);
                decimal denominator = costDetailList.Sum(c => c.Cost);

                foreach (CostDetail costDetail in costDetailList)
                {
                    if (costDetailList.IndexOf(costDetail) != costDetailList.Count - 1)
                    {
                        detailAmount = Math.Round(amount * costDetail.Cost / denominator, decimalLength, MidpointRounding.AwayFromZero);
                        remainAmount -= detailAmount;
                    }
                    else
                    {
                        detailAmount = remainAmount;
                    }

                    CostTransaction costTransaction = new CostTransaction();
                    costTransaction.Item = item;
                    costTransaction.ItemCategory = itemMgr.CheckAndLoadItem(item).ItemCategory.Code;
                    costTransaction.OrderNo = orderNo;
                    costTransaction.CostGroup = cc.CostGroup;
                    costTransaction.CostCenter = cc;
                    costTransaction.CostElement = costDetail.CostElement;
                    costTransaction.Currency = currencyMgr.CheckAndLoadCurrency(entityPreferenceMgr.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_BASE_CURRENCY).Value).Code;
                    costTransaction.BaseCurrency = costTransaction.Currency;
                    costTransaction.ExchangeRate = 1;
                    costTransaction.Qty = 0;
                    costTransaction.StandardAmount = 0;
                    costTransaction.ActualAmount = detailAmount;
                    costTransaction.EffectiveDate = effectiveDate;
                    costTransaction.CreateUser = user.Code;
                    costTransaction.CreateDate = DateTime.Now;
                    costTransaction.AdjType = BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_ADJ;

                    this.CreateCostTransaction(costTransaction);
                }
            }
        }

        public IList<CostTransaction> GetCostTransaction(IList<string> itemList, IList<string> costgroupList, IList<string> itemcategoryList, FinanceCalendar financeCalendar)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(CostTransaction));
            if (itemList != null && itemList.Count > 0)
            {
                if (itemList.Count == 1)
                {
                    criteria.Add(Expression.Eq("Item", itemList[0]));
                }
                else
                {
                    criteria.Add(Expression.InG<string>("Item", itemList));
                }
            }
            if (costgroupList != null && costgroupList.Count > 0)
            {
                if (costgroupList.Count == 1)
                {
                    criteria.Add(Expression.Eq("CostGroup.Code", costgroupList[0]));
                }
                else
                {
                    criteria.Add(Expression.InG<string>("CostGroup.Code", costgroupList));
                }
            }
            if (itemcategoryList != null && itemcategoryList.Count > 0)
            {
                if (itemcategoryList.Count == 1)
                {
                    criteria.Add(Expression.Eq("ItemCategory", itemcategoryList[0]));
                }
                else
                {
                    criteria.Add(Expression.InG<string>("ItemCategory", itemcategoryList));
                }
            }
            criteria.Add(Expression.Ge("EffectiveDate", financeCalendar.StartDate));
            criteria.Add(Expression.Lt("EffectiveDate", financeCalendar.EndDate));

            return criteriaMgrE.FindAll<CostTransaction>(criteria);

        }
    }
}

#region Extend Class
namespace com.Sconit.Service.Ext.Cost.Impl
{
    [Transactional]
    public partial class CostTransactionMgrE : com.Sconit.Service.Cost.Impl.CostTransactionMgr, ICostTransactionMgrE
    {

    }
}
#endregion