using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IAccountBaseDao
    {
        #region Method Created By CodeSmith

        void CreateAccount(Account entity);

        Account LoadAccount(String code);
  
        IList<Account> GetAllAccount();
  
        void UpdateAccount(Account entity);
        
        void DeleteAccount(String code);
    
        void DeleteAccount(Account entity);
    
        void DeleteAccount(IList<String> pkList);
    
        void DeleteAccount(IList<Account> entityList);    
        #endregion Method Created By CodeSmith
    }
}
