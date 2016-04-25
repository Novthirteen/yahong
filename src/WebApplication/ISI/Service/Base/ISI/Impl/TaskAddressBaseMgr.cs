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
    public class TaskAddressBaseMgr : SessionBase, ITaskAddressBaseMgr
    {
        public ITaskAddressDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTaskAddress(TaskAddress entity)
        {
            entityDao.CreateTaskAddress(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TaskAddress LoadTaskAddress(String code)
        {
            return entityDao.LoadTaskAddress(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskAddress> GetAllTaskAddress()
        {
            return entityDao.GetAllTaskAddress();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTaskAddress(TaskAddress entity)
        {
            entityDao.UpdateTaskAddress(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskAddress(String code)
        {
            entityDao.DeleteTaskAddress(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskAddress(TaskAddress entity)
        {
            entityDao.DeleteTaskAddress(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskAddress(IList<String> pkList)
        {
            entityDao.DeleteTaskAddress(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskAddress(IList<TaskAddress> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTaskAddress(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
