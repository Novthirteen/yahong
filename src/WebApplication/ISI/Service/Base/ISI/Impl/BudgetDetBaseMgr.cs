using System;
using System.Collections;
using System.Text;
using System.Collections.Generic;
using Castle.Services.Transaction;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class BudgetDetBaseMgr : SessionBase, IBudgetDetBaseMgr
    {
        public IBudgetDetDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateBudgetDet(BudgetDet entity)
        {
            entityDao.CreateBudgetDet(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual BudgetDet LoadBudgetDet(Int32 id)
        {
            return entityDao.LoadBudgetDet(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<BudgetDet> GetAllBudgetDet()
        {
            return entityDao.GetAllBudgetDet();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateBudgetDet(BudgetDet entity)
        {
            entityDao.UpdateBudgetDet(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteBudgetDet(Int32 id)
        {
            entityDao.DeleteBudgetDet(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteBudgetDet(BudgetDet entity)
        {
            entityDao.DeleteBudgetDet(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteBudgetDet(IList<Int32> pkList)
        {
            entityDao.DeleteBudgetDet(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteBudgetDet(IList<BudgetDet> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteBudgetDet(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
