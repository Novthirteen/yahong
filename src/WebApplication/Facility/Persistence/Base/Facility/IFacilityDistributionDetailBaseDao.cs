using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.Facility.Persistence
{
    public interface IFacilityDistributionDetailBaseDao
    {
        #region Method Created By CodeSmith

        void CreateFacilityDistributionDetail(FacilityDistributionDetail entity);

        FacilityDistributionDetail LoadFacilityDistributionDetail(Int32 id);
  
        IList<FacilityDistributionDetail> GetAllFacilityDistributionDetail();
  
        void UpdateFacilityDistributionDetail(FacilityDistributionDetail entity);
        
        void DeleteFacilityDistributionDetail(Int32 id);
    
        void DeleteFacilityDistributionDetail(FacilityDistributionDetail entity);

        void DeleteFacilityDistributionDetail(IList<Int32> pkList);    
        #endregion Method Created By CodeSmith
    }
}
