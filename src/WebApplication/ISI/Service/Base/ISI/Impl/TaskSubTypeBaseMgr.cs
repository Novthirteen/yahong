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
    public class TaskSubTypeBaseMgr : SessionBase, ITaskSubTypeBaseMgr
    {
        public ITaskSubTypeDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTaskSubType(TaskSubType entity)
        {
            entityDao.CreateTaskSubType(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TaskSubType LoadTaskSubType(String code)
        {
            return entityDao.LoadTaskSubType(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskSubType> GetAllTaskSubType()
        {
            return entityDao.GetAllTaskSubType(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskSubType> GetAllTaskSubType(bool includeInactive)
        {
            return entityDao.GetAllTaskSubType(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTaskSubType(TaskSubType entity)
        {
            entityDao.UpdateTaskSubType(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskSubType(String code)
        {
            entityDao.DeleteTaskSubType(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskSubType(TaskSubType entity)
        {
            entityDao.DeleteTaskSubType(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskSubType(IList<String> pkList)
        {
            entityDao.DeleteTaskSubType(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskSubType(IList<TaskSubType> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTaskSubType(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
