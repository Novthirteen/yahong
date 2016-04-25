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
    public class TaskMstrBaseMgr : SessionBase, ITaskMstrBaseMgr
    {
        public ITaskMstrDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTaskMstr(TaskMstr entity)
        {
            entityDao.CreateTaskMstr(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TaskMstr LoadTaskMstr(String code)
        {
            return entityDao.LoadTaskMstr(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskMstr> GetAllTaskMstr()
        {
            return entityDao.GetAllTaskMstr();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTaskMstr(TaskMstr entity)
        {
            entityDao.UpdateTaskMstr(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskMstr(String code)
        {
            entityDao.DeleteTaskMstr(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskMstr(TaskMstr entity)
        {
            entityDao.DeleteTaskMstr(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskMstr(IList<String> pkList)
        {
            entityDao.DeleteTaskMstr(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskMstr(IList<TaskMstr> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTaskMstr(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
