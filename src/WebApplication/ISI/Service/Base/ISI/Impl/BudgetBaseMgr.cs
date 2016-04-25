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
    public class BudgetBaseMgr : SessionBase, IBudgetBaseMgr
    {
        public IBudgetDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateBudget(Budget entity)
        {
            entityDao.CreateBudget(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual Budget LoadBudget(String code)
        {
            return entityDao.LoadBudget(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Budget> GetAllBudget()
        {
            return entityDao.GetAllBudget(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Budget> GetAllBudget(bool includeInactive)
        {
            return entityDao.GetAllBudget(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateBudget(Budget entity)
        {
            entityDao.UpdateBudget(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteBudget(String code)
        {
            entityDao.DeleteBudget(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteBudget(Budget entity)
        {
            entityDao.DeleteBudget(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteBudget(IList<String> pkList)
        {
            entityDao.DeleteBudget(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteBudget(IList<Budget> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteBudget(entityList);
        }   
        
        [Transaction(TransactionMode.Unspecified)]
        public virtual Budget LoadBudget(String taskSubType, Boolean isActive, Int32 year)
        {
            return entityDao.LoadBudget(taskSubType, isActive, year);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteBudget(String taskSubType, Boolean isActive, Int32 year)
        {
            entityDao.DeleteBudget(taskSubType, isActive, year);
        }   
        #endregion Method Created By CodeSmith
    }
}
