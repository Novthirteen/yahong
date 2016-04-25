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
    public class CheckupRemindBaseMgr : SessionBase, ICheckupRemindBaseMgr
    {
        public ICheckupRemindDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateCheckupRemind(CheckupRemind entity)
        {
            entityDao.CreateCheckupRemind(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual CheckupRemind LoadCheckupRemind(Int32 id)
        {
            return entityDao.LoadCheckupRemind(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<CheckupRemind> GetAllCheckupRemind()
        {
            return entityDao.GetAllCheckupRemind();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateCheckupRemind(CheckupRemind entity)
        {
            entityDao.UpdateCheckupRemind(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckupRemind(Int32 id)
        {
            entityDao.DeleteCheckupRemind(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckupRemind(CheckupRemind entity)
        {
            entityDao.DeleteCheckupRemind(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckupRemind(IList<Int32> pkList)
        {
            entityDao.DeleteCheckupRemind(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckupRemind(IList<CheckupRemind> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteCheckupRemind(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
