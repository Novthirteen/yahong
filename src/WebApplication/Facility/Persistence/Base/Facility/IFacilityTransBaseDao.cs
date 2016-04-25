using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.Facility.Persistence
{
    public interface IFacilityTransBaseDao
    {
        #region Method Created By CodeSmith

        void CreateFacilityTrans(FacilityTrans entity);

        FacilityTrans LoadFacilityTrans(Int32 id);
  
        IList<FacilityTrans> GetAllFacilityTrans();
  
        void UpdateFacilityTrans(FacilityTrans entity);
        
        void DeleteFacilityTrans(Int32 id);
    
        void DeleteFacilityTrans(FacilityTrans entity);
    
        void DeleteFacilityTrans(IList<Int32> pkList);
    
        void DeleteFacilityTrans(IList<FacilityTrans> entityList);    
        #endregion Method Created By CodeSmith
    }
}
