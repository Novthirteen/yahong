using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;
//TODO: Add other using statements here.

namespace com.Sconit.Facility.Persistence
{
    public interface IFacilityStockDetailBaseDao
    {
        #region Method Created By CodeSmith

        void CreateFacilityStockDetail(FacilityStockDetail entity);

        FacilityStockDetail LoadFacilityStockDetail(Int32 id);
  
        IList<FacilityStockDetail> GetAllFacilityStockDetail();
  
        void UpdateFacilityStockDetail(FacilityStockDetail entity);
        
        void DeleteFacilityStockDetail(Int32 id);
    
        void DeleteFacilityStockDetail(FacilityStockDetail entity);
    
        void DeleteFacilityStockDetail(IList<Int32> pkList);
    
        void DeleteFacilityStockDetail(IList<FacilityStockDetail> entityList);    
        #endregion Method Created By CodeSmith
    }
}
