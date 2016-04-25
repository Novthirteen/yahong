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
    public class FailureModeBaseMgr : SessionBase, IFailureModeBaseMgr
    {
        public IFailureModeDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateFailureMode(FailureMode entity)
        {
            entityDao.CreateFailureMode(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual FailureMode LoadFailureMode(String code)
        {
            return entityDao.LoadFailureMode(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FailureMode> GetAllFailureMode()
        {
            return entityDao.GetAllFailureMode(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<FailureMode> GetAllFailureMode(bool includeInactive)
        {
            return entityDao.GetAllFailureMode(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateFailureMode(FailureMode entity)
        {
            entityDao.UpdateFailureMode(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFailureMode(String code)
        {
            entityDao.DeleteFailureMode(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFailureMode(FailureMode entity)
        {
            entityDao.DeleteFailureMode(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFailureMode(IList<String> pkList)
        {
            entityDao.DeleteFailureMode(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteFailureMode(IList<FailureMode> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteFailureMode(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
