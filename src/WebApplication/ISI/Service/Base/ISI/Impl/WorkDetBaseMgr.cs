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
    public class WorkDetBaseMgr : SessionBase, IWorkDetBaseMgr
    {
        public IWorkDetDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateWorkDet(WorkDet entity)
        {
            entityDao.CreateWorkDet(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual WorkDet LoadWorkDet(Int32 iD)
        {
            return entityDao.LoadWorkDet(iD);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<WorkDet> GetAllWorkDet()
        {
            return entityDao.GetAllWorkDet();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateWorkDet(WorkDet entity)
        {
            entityDao.UpdateWorkDet(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteWorkDet(Int32 iD)
        {
            entityDao.DeleteWorkDet(iD);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteWorkDet(WorkDet entity)
        {
            entityDao.DeleteWorkDet(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteWorkDet(IList<Int32> pkList)
        {
            entityDao.DeleteWorkDet(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteWorkDet(IList<WorkDet> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteWorkDet(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
