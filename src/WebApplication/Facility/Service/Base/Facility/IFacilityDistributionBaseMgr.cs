using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityDistributionBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateFacilityDistribution(FacilityDistribution entity);

        FacilityDistribution LoadFacilityDistribution(Int32 id);

        IList<FacilityDistribution> GetAllFacilityDistribution();
    
        void UpdateFacilityDistribution(FacilityDistribution entity);

        void DeleteFacilityDistribution(Int32 id);
    
        void DeleteFacilityDistribution(FacilityDistribution entity);
    
        void DeleteFacilityDistribution(IList<Int32> pkList);
    
        void DeleteFacilityDistribution(IList<FacilityDistribution> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
