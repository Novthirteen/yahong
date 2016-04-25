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
    public class ProcessApplyBaseMgr : SessionBase, IProcessApplyBaseMgr
    {
        public IProcessApplyDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateProcessApply(ProcessApply entity)
        {
            entityDao.CreateProcessApply(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ProcessApply LoadProcessApply(Int32 id)
        {
            return entityDao.LoadProcessApply(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ProcessApply> GetAllProcessApply()
        {
            return entityDao.GetAllProcessApply();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateProcessApply(ProcessApply entity)
        {
            entityDao.UpdateProcessApply(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessApply(Int32 id)
        {
            entityDao.DeleteProcessApply(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessApply(ProcessApply entity)
        {
            entityDao.DeleteProcessApply(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessApply(IList<Int32> pkList)
        {
            entityDao.DeleteProcessApply(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProcessApply(IList<ProcessApply> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteProcessApply(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
