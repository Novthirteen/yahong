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
    public class TaskApplyBaseMgr : SessionBase, ITaskApplyBaseMgr
    {
        public ITaskApplyDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTaskApply(TaskApply entity)
        {
            entityDao.CreateTaskApply(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TaskApply LoadTaskApply(Int32 id)
        {
            return entityDao.LoadTaskApply(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskApply> GetAllTaskApply()
        {
            return entityDao.GetAllTaskApply();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTaskApply(TaskApply entity)
        {
            entityDao.UpdateTaskApply(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskApply(Int32 id)
        {
            entityDao.DeleteTaskApply(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskApply(TaskApply entity)
        {
            entityDao.DeleteTaskApply(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskApply(IList<Int32> pkList)
        {
            entityDao.DeleteTaskApply(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskApply(IList<TaskApply> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTaskApply(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
