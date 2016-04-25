using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityCategoryBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateFacilityCategory(FacilityCategory entity);

        FacilityCategory LoadFacilityCategory(String code);

        IList<FacilityCategory> GetAllFacilityCategory();
    
        void UpdateFacilityCategory(FacilityCategory entity);

        void DeleteFacilityCategory(String code);
    
        void DeleteFacilityCategory(FacilityCategory entity);
    
        void DeleteFacilityCategory(IList<String> pkList);
    
        void DeleteFacilityCategory(IList<FacilityCategory> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
