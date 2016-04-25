using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using System.Linq;
using com.Sconit.ISI.Entity.Util;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class BudgetDetMgr : BudgetDetBaseMgr, IBudgetDetMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        //TODO: Add other methods here.

        [Transaction(TransactionMode.Unspecified)]
        public IList<BudgetDet> GetBudgetDet(string budgetCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(BudgetDet));
            criteria.Add(Expression.Eq("BudgetCode", budgetCode));
            return this.criteriaMgrE.FindAll<BudgetDet>(criteria);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<BudgetDet> GetAccount1(string costCenterHead)
        {
            var budgetDetList = GetAccount1(costCenterHead, string.Empty);
            return budgetDetList;
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<BudgetDet> GetAccount1(string costCenterHead, string costCenterDetail)
        {
            var taskSubType = !string.IsNullOrEmpty(costCenterDetail) ? costCenterDetail : costCenterHead;

            IList<BudgetDet> budgetDetList = GetBudgetDet(taskSubType, DateTime.Now.Year, string.Empty);
            if (budgetDetList == null || budgetDetList.Count == 0) return budgetDetList;

            budgetDetList = budgetDetList.Select(b => new BudgetDet() { Account1 = b.Account1, Account1Desc = b.Account1Desc }).Distinct().ToList();
            return budgetDetList;
        }


        [Transaction(TransactionMode.Unspecified)]
        public IList<BudgetDet> GetAccount2(string costCenterHead, string account1)
        {
            var budgetDetList = this.GetAccount2(costCenterHead, string.Empty, account1);
            return budgetDetList;
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<BudgetDet> GetAccount2(string costCenterHead, string costCenterDetail, string account1)
        {
            string costCenter = !string.IsNullOrEmpty(costCenterDetail) ? costCenterDetail : (!string.IsNullOrEmpty(costCenterHead) ? costCenterHead : string.Empty);
            if (!string.IsNullOrEmpty(costCenter))
            {
                var budgetDetList = this.GetBudgetDet(costCenter, account1);
                if (budgetDetList == null || budgetDetList.Count == 0) return budgetDetList;
                budgetDetList = budgetDetList.Select(b => new BudgetDet() { Account2 = b.Account2, Account2Desc = b.Account2Desc }).Distinct().ToList();
                return budgetDetList;
            }
            return null;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BudgetDet> GetBudgetDetByTaskSubType(string taskSubType)
        {
            return GetBudgetDet(taskSubType, DateTime.Now.Year, string.Empty);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BudgetDet> GetBudgetDet(string taskSubType, string account1)
        {
            return GetBudgetDet(taskSubType, DateTime.Now.Year, account1);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<BudgetDet> GetBudgetDet(string taskSubType, int year, string account1)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(BudgetDet));
            if (!string.IsNullOrEmpty(account1))
            {
                criteria.Add(Expression.Eq("Account1", account1));
            }
            if (!string.IsNullOrEmpty(taskSubType))
            {
                DetachedCriteria subCriteria = DetachedCriteria.For(typeof(Budget));
                subCriteria.Add(Expression.Eq("IsActive", true));
                subCriteria.Add(Expression.Eq("TaskSubType", taskSubType));
                subCriteria.Add(Expression.Or(Expression.Eq("Year", year), Expression.IsNull("Year")));
                subCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("Code")));
                criteria.Add(Subqueries.PropertyIn("BudgetCode", subCriteria));
            }
            return this.criteriaMgrE.FindAll<BudgetDet>(criteria);
        }


        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class BudgetDetMgrE : com.Sconit.ISI.Service.Impl.BudgetDetMgr, IBudgetDetMgrE
    {
    }
}

#endregion Extend Class