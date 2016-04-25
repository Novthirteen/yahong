using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityMasterBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateFacilityMaster(FacilityMaster entity);

        FacilityMaster LoadFacilityMaster(String fCID);

        IList<FacilityMaster> GetAllFacilityMaster();

        void UpdateFacilityMaster(FacilityMaster entity);

        void DeleteFacilityMaster(String fCID);

        void DeleteFacilityMaster(FacilityMaster entity);

        void DeleteFacilityMaster(IList<String> pkList);

        void DeleteFacilityMaster(IList<FacilityMaster> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
