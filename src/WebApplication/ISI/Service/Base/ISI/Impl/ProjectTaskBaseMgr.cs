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
    public class ProjectTaskBaseMgr : SessionBase, IProjectTaskBaseMgr
    {
        public IProjectTaskDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateProjectTask(ProjectTask entity)
        {
            entityDao.CreateProjectTask(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ProjectTask LoadProjectTask(Int32 id)
        {
            return entityDao.LoadProjectTask(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ProjectTask> GetAllProjectTask()
        {
            return entityDao.GetAllProjectTask(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ProjectTask> GetAllProjectTask(bool includeInactive)
        {
            return entityDao.GetAllProjectTask(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateProjectTask(ProjectTask entity)
        {
            entityDao.UpdateProjectTask(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProjectTask(Int32 id)
        {
            entityDao.DeleteProjectTask(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProjectTask(ProjectTask entity)
        {
            entityDao.DeleteProjectTask(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProjectTask(IList<Int32> pkList)
        {
            entityDao.DeleteProjectTask(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteProjectTask(IList<ProjectTask> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteProjectTask(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
