using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityItemBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateFacilityItem(FacilityItem entity);

        FacilityItem LoadFacilityItem(Int32 id);

        IList<FacilityItem> GetAllFacilityItem();
    
        IList<FacilityItem> GetAllFacilityItem(bool includeInactive);
      
        void UpdateFacilityItem(FacilityItem entity);

        void DeleteFacilityItem(Int32 id);
    
        void DeleteFacilityItem(FacilityItem entity);

        void DeleteFacilityItem(IList<Int32> pkList);
    
        void DeleteFacilityItem(IList<FacilityItem> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
