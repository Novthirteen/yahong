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
    public class TaskStatusViewBaseMgr : SessionBase, ITaskStatusViewBaseMgr
    {
        public ITaskStatusViewDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTaskStatusView(TaskStatusView entity)
        {
            entityDao.CreateTaskStatusView(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TaskStatusView LoadTaskStatusView(String taskCode)
        {
            return entityDao.LoadTaskStatusView(taskCode);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskStatusView> GetAllTaskStatusView()
        {
            return entityDao.GetAllTaskStatusView();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTaskStatusView(TaskStatusView entity)
        {
            entityDao.UpdateTaskStatusView(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskStatusView(String taskCode)
        {
            entityDao.DeleteTaskStatusView(taskCode);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskStatusView(TaskStatusView entity)
        {
            entityDao.DeleteTaskStatusView(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskStatusView(IList<String> pkList)
        {
            entityDao.DeleteTaskStatusView(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskStatusView(IList<TaskStatusView> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTaskStatusView(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
