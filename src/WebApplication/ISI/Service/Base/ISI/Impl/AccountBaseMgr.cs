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
    public class AccountBaseMgr : SessionBase, IAccountBaseMgr
    {
        public IAccountDao entityDao { get; set; }
        
        #region Method Created By CodeSmith

        [Transaction(TransactionMode.Requires)]
        public virtual void CreateAccount(Account entity)
        {
            entityDao.CreateAccount(entity);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual Account LoadAccount(String code)
        {
            return entityDao.LoadAccount(code);
        }

        [Transaction(TransactionMode.Unspecified)]
        public virtual IList<Account> GetAllAccount()
        {
            return entityDao.GetAllAccount();
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void UpdateAccount(Account entity)
        {
            entityDao.UpdateAccount(entity);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteAccount(String code)
        {
            entityDao.DeleteAccount(code);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteAccount(Account entity)
        {
            entityDao.DeleteAccount(entity);
        }
    
        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteAccount(IList<String> pkList)
        {
            entityDao.DeleteAccount(pkList);
        }

        [Transaction(TransactionMode.Requires)]
        public virtual void DeleteAccount(IList<Account> entityList)
        {
            if ((entityList == null) || (entityList.Count == 0))
            {
                return;
            }
            
            entityDao.DeleteAccount(entityList);
        }   
        #endregion Method Created By CodeSmith
    }
}
