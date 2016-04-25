using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface ICostBaseDao
    {
        #region Method Created By CodeSmith

        void CreateCost(Cost entity);

        Cost LoadCost(Int32 id);
  
        IList<Cost> GetAllCost();
  
        void UpdateCost(Cost entity);
        
        void DeleteCost(Int32 id);
    
        void DeleteCost(Cost entity);
    
        void DeleteCost(IList<Int32> pkList);
    
        void DeleteCost(IList<Cost> entityList);    
        #endregion Method Created By CodeSmith
    }
}
