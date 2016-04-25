using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.ISI.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.ISI.Persistence
{
    public interface IBudgetBaseDao
    {
        #region Method Created By CodeSmith

        void CreateBudget(Budget entity);

        Budget LoadBudget(String code);
  
        IList<Budget> GetAllBudget();
  
        IList<Budget> GetAllBudget(bool includeInactive);
  
        void UpdateBudget(Budget entity);
        
        void DeleteBudget(String code);
    
        void DeleteBudget(Budget entity);
    
        void DeleteBudget(IList<String> pkList);
    
        void DeleteBudget(IList<Budget> entityList);    
        
        Budget LoadBudget(String taskSubType, Boolean isActive, Int32 year);
    
        void DeleteBudget(String taskSubType, Boolean isActive, Int32 year);
        #endregion Method Created By CodeSmith
    }
}
