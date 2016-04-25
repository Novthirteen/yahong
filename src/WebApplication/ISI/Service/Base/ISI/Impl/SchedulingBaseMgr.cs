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
    public class SchedulingBaseMgr : SessionBase, ISchedulingBaseMgr
    {
        public ISchedulingDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateScheduling(Scheduling entity)
        {
            entityDao.CreateScheduling(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual Scheduling LoadScheduling(Int32 id)
        {
            return entityDao.LoadScheduling(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Scheduling> GetAllScheduling()
        {
            return entityDao.GetAllScheduling();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateScheduling(Scheduling entity)
        {
            entityDao.UpdateScheduling(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteScheduling(Int32 id)
        {
            entityDao.DeleteScheduling(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteScheduling(Scheduling entity)
        {
            entityDao.DeleteScheduling(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteScheduling(IList<Int32> pkList)
        {
            entityDao.DeleteScheduling(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteScheduling(IList<Scheduling> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteScheduling(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
