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
    public class TaskWeekStatisticsViewBaseMgr : SessionBase, ITaskWeekStatisticsViewBaseMgr
    {
        public ITaskWeekStatisticsViewDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTaskWeekStatisticsView(TaskWeekStatisticsView entity)
        {
            entityDao.CreateTaskWeekStatisticsView(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TaskWeekStatisticsView LoadTaskWeekStatisticsView(String code)
        {
            return entityDao.LoadTaskWeekStatisticsView(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskWeekStatisticsView> GetAllTaskWeekStatisticsView()
        {
            return entityDao.GetAllTaskWeekStatisticsView();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTaskWeekStatisticsView(TaskWeekStatisticsView entity)
        {
            entityDao.UpdateTaskWeekStatisticsView(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskWeekStatisticsView(String code)
        {
            entityDao.DeleteTaskWeekStatisticsView(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskWeekStatisticsView(TaskWeekStatisticsView entity)
        {
            entityDao.DeleteTaskWeekStatisticsView(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskWeekStatisticsView(IList<String> pkList)
        {
            entityDao.DeleteTaskWeekStatisticsView(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskWeekStatisticsView(IList<TaskWeekStatisticsView> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTaskWeekStatisticsView(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
