using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IUserSubscriptionBaseDao
    {
        #region Method Created By CodeSmith

        void CreateUserSubscription(UserSubscription entity);

        UserSubscription LoadUserSubscription(Int32 id);
  
        IList<UserSubscription> GetAllUserSubscription();
  
        void UpdateUserSubscription(UserSubscription entity);
        
        void DeleteUserSubscription(Int32 id);
    
        void DeleteUserSubscription(UserSubscription entity);
    
        void DeleteUserSubscription(IList<Int32> pkList);
    
        void DeleteUserSubscription(IList<UserSubscription> entityList);    
        #endregion Method Created By CodeSmith
    }
}
