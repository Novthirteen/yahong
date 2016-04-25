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
    public class UserSubscriptionBaseMgr : SessionBase, IUserSubscriptionBaseMgr
    {
        public IUserSubscriptionDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateUserSubscription(UserSubscription entity)
        {
            entityDao.CreateUserSubscription(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual UserSubscription LoadUserSubscription(Int32 id)
        {
            return entityDao.LoadUserSubscription(id);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<UserSubscription> GetAllUserSubscription()
        {
            return entityDao.GetAllUserSubscription();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateUserSubscription(UserSubscription entity)
        {
            entityDao.UpdateUserSubscription(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteUserSubscription(Int32 id)
        {
            entityDao.DeleteUserSubscription(id);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteUserSubscription(UserSubscription entity)
        {
            entityDao.DeleteUserSubscription(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteUserSubscription(IList<Int32> pkList)
        {
            entityDao.DeleteUserSubscription(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteUserSubscription(IList<UserSubscription> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteUserSubscription(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
