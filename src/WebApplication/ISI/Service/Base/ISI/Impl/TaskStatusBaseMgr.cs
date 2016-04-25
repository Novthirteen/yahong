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
    public class TaskStatusBaseMgr : SessionBase, ITaskStatusBaseMgr
    {
        public ITaskStatusDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTaskStatus(TaskStatus entity)
        {
            entityDao.CreateTaskStatus(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TaskStatus LoadTaskStatus(Int32 id)
        {
            return entityDao.LoadTaskStatus(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskStatus> GetAllTaskStatus()
        {
            return entityDao.GetAllTaskStatus();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTaskStatus(TaskStatus entity)
        {
            entityDao.UpdateTaskStatus(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskStatus(Int32 id)
        {
            entityDao.DeleteTaskStatus(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskStatus(TaskStatus entity)
        {
            entityDao.DeleteTaskStatus(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskStatus(IList<Int32> pkList)
        {
            entityDao.DeleteTaskStatus(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskStatus(IList<TaskStatus> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTaskStatus(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
