using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.Facility.Persistence
{
    public interface IMaintainPlanBaseDao
    {
        #region Method Created By CodeSmith

        void CreateMaintainPlan(MaintainPlan entity);

        MaintainPlan LoadMaintainPlan(String code);
  
        IList<MaintainPlan> GetAllMaintainPlan();
  
        void UpdateMaintainPlan(MaintainPlan entity);
        
        void DeleteMaintainPlan(String code);
    
        void DeleteMaintainPlan(MaintainPlan entity);
    
        void DeleteMaintainPlan(IList<String> pkList);
    
        void DeleteMaintainPlan(IList<MaintainPlan> entityList);    
        #endregion Method Created By CodeSmith
    }
}
