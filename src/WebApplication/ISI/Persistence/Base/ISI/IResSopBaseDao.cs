using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IResSopBaseDao
    {
        #region Method Created By CodeSmith

        void CreateResSop(ResSop entity);

        ResSop LoadResSop(int id);
  
        IList<ResSop> GetAllResSop();
  
        void UpdateResSop(ResSop entity);
        
        void DeleteResSop(int workShop);
    
        void DeleteResSop(ResSop entity);
    
        void DeleteResSop(IList<int> pkList);
    
        void DeleteResSop(IList<ResSop> entityList);    
        #endregion Method Created By CodeSmith
    }
}
