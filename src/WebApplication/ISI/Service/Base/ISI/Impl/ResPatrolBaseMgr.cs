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
    public class ResPatrolBaseMgr : SessionBase, IResPatrolBaseMgr
    {
        public IResPatrolDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateResPatrol(ResPatrol entity)
        {
            entityDao.CreateResPatrol(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ResPatrol LoadResPatrol(Int32 id)
        {
            return entityDao.LoadResPatrol(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ResPatrol> GetAllResPatrol()
        {
            return entityDao.GetAllResPatrol();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateResPatrol(ResPatrol entity)
        {
            entityDao.UpdateResPatrol(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResPatrol(Int32 id)
        {
            entityDao.DeleteResPatrol(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResPatrol(ResPatrol entity)
        {
            entityDao.DeleteResPatrol(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResPatrol(IList<Int32> pkList)
        {
            entityDao.DeleteResPatrol(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResPatrol(IList<ResPatrol> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteResPatrol(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
