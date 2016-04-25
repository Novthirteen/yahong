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
    public class ResSopBaseMgr : SessionBase, IResSopBaseMgr
    {
        public IResSopDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateResSop(ResSop entity)
        {
            entityDao.CreateResSop(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ResSop LoadResSop(int id)
        {
            return entityDao.LoadResSop(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ResSop> GetAllResSop()
        {
            return entityDao.GetAllResSop();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateResSop(ResSop entity)
        {
            entityDao.UpdateResSop(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResSop(int id)
        {
            entityDao.DeleteResSop(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResSop(ResSop entity)
        {
            entityDao.DeleteResSop(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResSop(IList<int> pkList)
        {
            entityDao.DeleteResSop(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResSop(IList<ResSop> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteResSop(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
