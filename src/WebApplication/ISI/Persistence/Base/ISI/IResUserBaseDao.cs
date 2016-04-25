using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IResUserBaseDao
    {
        #region Method Created By CodeSmith

        void CreateResUser(ResUser entity);

        ResUser LoadResUser(Int32 id);
  
        IList<ResUser> GetAllResUser();
  
        void UpdateResUser(ResUser entity);
        
        void DeleteResUser(Int32 id);
    
        void DeleteResUser(ResUser entity);
    
        void DeleteResUser(IList<Int32> pkList);
    
        void DeleteResUser(IList<ResUser> entityList);    
        #endregion Method Created By CodeSmith
    }
}
