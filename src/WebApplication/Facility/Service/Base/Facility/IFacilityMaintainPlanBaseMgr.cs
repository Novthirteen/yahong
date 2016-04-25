using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityMaintainPlanBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateFacilityMaintainPlan(FacilityMaintainPlan entity);

        FacilityMaintainPlan LoadFacilityMaintainPlan(Int32 id);

        IList<FacilityMaintainPlan> GetAllFacilityMaintainPlan();
    
        void UpdateFacilityMaintainPlan(FacilityMaintainPlan entity);

        void DeleteFacilityMaintainPlan(Int32 id);
    
        void DeleteFacilityMaintainPlan(FacilityMaintainPlan entity);
    
        void DeleteFacilityMaintainPlan(IList<Int32> pkList);
    
        void DeleteFacilityMaintainPlan(IList<FacilityMaintainPlan> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
