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
    public class TaskDetailBaseMgr : SessionBase, ITaskDetailBaseMgr
    {
        public ITaskDetailDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTaskDetail(TaskDetail entity)
        {
            entityDao.CreateTaskDetail(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TaskDetail LoadTaskDetail(Int32 id)
        {
            return entityDao.LoadTaskDetail(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskDetail> GetAllTaskDetail()
        {
            return entityDao.GetAllTaskDetail(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskDetail> GetAllTaskDetail(bool includeInactive)
        {
            return entityDao.GetAllTaskDetail(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTaskDetail(TaskDetail entity)
        {
            entityDao.UpdateTaskDetail(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskDetail(Int32 id)
        {
            entityDao.DeleteTaskDetail(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskDetail(TaskDetail entity)
        {
            entityDao.DeleteTaskDetail(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskDetail(IList<Int32> pkList)
        {
            entityDao.DeleteTaskDetail(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskDetail(IList<TaskDetail> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTaskDetail(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
