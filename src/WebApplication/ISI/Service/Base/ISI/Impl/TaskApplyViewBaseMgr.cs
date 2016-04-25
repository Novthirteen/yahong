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
    public class TaskApplyViewBaseMgr : SessionBase, ITaskApplyViewBaseMgr
    {
        public ITaskApplyViewDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateTaskApplyView(TaskApplyView entity)
        {
            entityDao.CreateTaskApplyView(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual TaskApplyView LoadTaskApplyView(Int32 id)
        {
            return entityDao.LoadTaskApplyView(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<TaskApplyView> GetAllTaskApplyView()
        {
            return entityDao.GetAllTaskApplyView();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateTaskApplyView(TaskApplyView entity)
        {
            entityDao.UpdateTaskApplyView(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskApplyView(Int32 id)
        {
            entityDao.DeleteTaskApplyView(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskApplyView(TaskApplyView entity)
        {
            entityDao.DeleteTaskApplyView(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskApplyView(IList<Int32> pkList)
        {
            entityDao.DeleteTaskApplyView(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteTaskApplyView(IList<TaskApplyView> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteTaskApplyView(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
