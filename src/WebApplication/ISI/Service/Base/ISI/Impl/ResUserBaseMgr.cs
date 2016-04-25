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
    public class ResUserBaseMgr : SessionBase, IResUserBaseMgr
    {
        public IResUserDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateResUser(ResUser entity)
        {
            entityDao.CreateResUser(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual ResUser LoadResUser(Int32 id)
        {
            return entityDao.LoadResUser(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<ResUser> GetAllResUser()
        {
            return entityDao.GetAllResUser();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateResUser(ResUser entity)
        {
            entityDao.UpdateResUser(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResUser(Int32 id)
        {
            entityDao.DeleteResUser(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResUser(ResUser entity)
        {
            entityDao.DeleteResUser(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResUser(IList<Int32> pkList)
        {
            entityDao.DeleteResUser(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteResUser(IList<ResUser> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteResUser(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
