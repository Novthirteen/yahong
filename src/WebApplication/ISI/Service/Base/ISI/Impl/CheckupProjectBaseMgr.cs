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
    public class CheckupProjectBaseMgr : SessionBase, ICheckupProjectBaseMgr
    {
        public ICheckupProjectDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateCheckupProject(CheckupProject entity)
        {
            entityDao.CreateCheckupProject(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual CheckupProject LoadCheckupProject(String code)
        {
            return entityDao.LoadCheckupProject(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<CheckupProject> GetAllCheckupProject()
        {
            return entityDao.GetAllCheckupProject(false);
        }
    
        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<CheckupProject> GetAllCheckupProject(bool includeInactive)
        {
            return entityDao.GetAllCheckupProject(includeInactive);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateCheckupProject(CheckupProject entity)
        {
            entityDao.UpdateCheckupProject(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckupProject(String code)
        {
            entityDao.DeleteCheckupProject(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckupProject(CheckupProject entity)
        {
            entityDao.DeleteCheckupProject(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckupProject(IList<String> pkList)
        {
            entityDao.DeleteCheckupProject(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteCheckupProject(IList<CheckupProject> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteCheckupProject(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
