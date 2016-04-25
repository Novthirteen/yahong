using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Service.Ext;
using System.Linq;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class AccountMgr : AccountBaseMgr, IAccountMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }
        public IBudgetMgrE budgetMgrE { get; set; }
        public IBudgetDetMgrE budgetDetMgrE { get; set; }
        #region Customized Methods
        [Transaction(TransactionMode.Unspecified)]
        public IList<Account> GetAccount1()
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Account));
            criteria.Add(Expression.Eq("Type", ISIConstants.CODE_MASTER_WFS_ACCOUNT_LEVEL1));
            return this.criteriaMgrE.FindAll<Account>(criteria);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<Account> GetAccount1(string costCenterHead)
        {
            return this.GetAccount1(costCenterHead, string.Empty);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<Account> GetAccount1(string costCenterHead, string costCenterDetail)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Account));
            criteria.Add(Expression.Eq("Type", ISIConstants.CODE_MASTER_WFS_ACCOUNT_LEVEL1));
            var accmountList = this.criteriaMgrE.FindAll<Account>(criteria);
            if (accmountList != null && accmountList.Count > 0)
            {
                string costCenter = !string.IsNullOrEmpty(costCenterDetail) ? costCenterDetail : (!string.IsNullOrEmpty(costCenterHead) ? costCenterHead : string.Empty);
                if (!string.IsNullOrEmpty(costCenter))
                {
                    var budgetDetList = budgetDetMgrE.GetBudgetDetByTaskSubType(costCenter);
                    if (budgetDetList != null && budgetDetList.Count > 0)
                    {
                        var account1CodeList = budgetDetList.Select(b => b.Account1).Distinct().ToList();
                        ProcessAccount(accmountList, account1CodeList);
                    }
                }
            }
            return accmountList;
        }

        private void ProcessAccount(IList<Account> accmountList, List<string> account1CodeList)
        {
            for (int i = account1CodeList.Count - 1; i >= 0; i--)
            {
                var cList = accmountList.Where(a => a.Code == account1CodeList[i]).ToList();
                if (cList != null && cList.Count > 0)
                {
                    accmountList.Remove(cList[0]);
                    cList[0].Name = cList[0].Desc1 + " - ‘§À„Budget";
                    accmountList.Insert(0, cList[0]);
                }
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Account> GetAccount2()
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Account));
            criteria.Add(Expression.Eq("Type", ISIConstants.CODE_MASTER_WFS_ACCOUNT_LEVEL2));
            return this.criteriaMgrE.FindAll<Account>(criteria);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<Account> GetAccount2(string costCenterHead, string account1)
        {
            return this.GetAccount2(costCenterHead, string.Empty, account1);
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<Account> GetAccount2(string costCenterHead, string costCenterDetail, string account1)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Account));
            criteria.Add(Expression.Eq("Type", ISIConstants.CODE_MASTER_WFS_ACCOUNT_LEVEL2));
            var accmountList = this.criteriaMgrE.FindAll<Account>(criteria);
            if (accmountList != null && accmountList.Count > 0)
            {
                string costCenter = !string.IsNullOrEmpty(costCenterDetail) ? costCenterDetail : (!string.IsNullOrEmpty(costCenterHead) ? costCenterHead : string.Empty);
                if (!string.IsNullOrEmpty(costCenter))
                {
                    var budgetDetList = budgetDetMgrE.GetBudgetDet(costCenter, account1);
                    if (budgetDetList != null && budgetDetList.Count > 0)
                    {
                        var account2CodeList = budgetDetList.Select(b => b.Account2).Distinct().ToList();
                        ProcessAccount(accmountList, account2CodeList);
                    }
                }
            }
            return accmountList;
        }
        [Transaction(TransactionMode.Unspecified)]
        public bool IsRef(string code)
        {
            //»ŒŒÒ
            DetachedCriteria criteria = DetachedCriteria.For(typeof(BudgetDet)).SetProjection(Projections.CountDistinct("Id"));
            criteria.Add(Expression.Or(Expression.Eq("Account1", code), Expression.Eq("Account2", code)));
            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }

            criteria = DetachedCriteria.For(typeof(Cost)).SetProjection(Projections.CountDistinct("Id"));
            criteria.Add(Expression.Or(Expression.Eq("Account1", code), Expression.Eq("Account2", code)));
            count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }

            return false;
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class AccountMgrE : com.Sconit.ISI.Service.Impl.AccountMgr, IAccountMgrE
    {
    }
}

#endregion Extend Class