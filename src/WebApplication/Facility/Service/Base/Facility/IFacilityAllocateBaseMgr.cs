using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityAllocateBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateFacilityAllocate(FacilityAllocate entity);

        FacilityAllocate LoadFacilityAllocate(Int32 id);

        IList<FacilityAllocate> GetAllFacilityAllocate();
    
        IList<FacilityAllocate> GetAllFacilityAllocate(bool includeInactive);
      
        void UpdateFacilityAllocate(FacilityAllocate entity);

        void DeleteFacilityAllocate(Int32 id);
    
        void DeleteFacilityAllocate(FacilityAllocate entity);
    
        void DeleteFacilityAllocate(IList<Int32> pkList);
    
        void DeleteFacilityAllocate(IList<FacilityAllocate> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
