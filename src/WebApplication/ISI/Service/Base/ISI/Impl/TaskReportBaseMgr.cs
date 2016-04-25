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
    public class TaskReportBaseMgr : SessionBase, ITaskReportBaseMgr
    {
        public ITaskReportDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTaskReport(TaskReport entity)
        {
            entityDao.CreateTaskReport(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TaskReport LoadTaskReport(Int32 id)
        {
            return entityDao.LoadTaskReport(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskReport> GetAllTaskReport()
        {
            return entityDao.GetAllTaskReport(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskReport> GetAllTaskReport(bool includeInactive)
        {
            return entityDao.GetAllTaskReport(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTaskReport(TaskReport entity)
        {
            entityDao.UpdateTaskReport(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskReport(Int32 id)
        {
            entityDao.DeleteTaskReport(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskReport(TaskReport entity)
        {
            entityDao.DeleteTaskReport(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskReport(IList<Int32> pkList)
        {
            entityDao.DeleteTaskReport(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskReport(IList<TaskReport> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTaskReport(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
