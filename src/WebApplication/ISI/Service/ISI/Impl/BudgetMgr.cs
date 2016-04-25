using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Service.Ext;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class BudgetMgr : BudgetBaseMgr, IBudgetMgr
    {
        public IBudgetDetMgrE budgetDetMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }
        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public override void DeleteBudget(Budget entity)
        {
            var budgetDetList = budgetDetMgrE.GetBudgetDet(entity.Code);
            budgetDetMgrE.DeleteBudgetDet(budgetDetList);
            entityDao.DeleteBudget(entity);
        }

        //TODO: Add other methods here.
        [Transaction(TransactionMode.Unspecified)]
        public Budget LoadBudget(string taskSubType, string year)
        {
            return this.LoadBudget(string.Empty, taskSubType, year);
        }

        [Transaction(TransactionMode.Unspecified)]
        public Budget LoadBudget(string code, string taskSubType, string year)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Budget));
            if (!string.IsNullOrEmpty(code))
            {
                criteria.Add(Expression.Not(Expression.Eq("Code", code)));
            }
            if (!string.IsNullOrEmpty(year))
            {
                criteria.Add(Expression.Eq("Year", int.Parse(year)));
            }
            else
            {
                criteria.Add(Expression.IsNull("Year"));
            }
            criteria.Add(Expression.Eq("IsActive", true));
            criteria.Add(Expression.Eq("TaskSubType", taskSubType));

            var budgetList = this.criteriaMgrE.FindAll<Budget>(criteria);
            if (budgetList != null && budgetList.Count > 0)
            {
                return budgetList[0];
            }
            return null;
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class BudgetMgrE : com.Sconit.ISI.Service.Impl.BudgetMgr, IBudgetMgrE
    {
    }
}

#endregion Extend Class