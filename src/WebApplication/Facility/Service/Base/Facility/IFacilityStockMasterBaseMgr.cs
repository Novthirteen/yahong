using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityStockMasterBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateFacilityStockMaster(FacilityStockMaster entity);

        FacilityStockMaster LoadFacilityStockMaster(String orderNo);

        IList<FacilityStockMaster> GetAllFacilityStockMaster();
    
        void UpdateFacilityStockMaster(FacilityStockMaster entity);

        void DeleteFacilityStockMaster(String orderNo);
    
        void DeleteFacilityStockMaster(FacilityStockMaster entity);
    
        void DeleteFacilityStockMaster(IList<String> pkList);
    
        void DeleteFacilityStockMaster(IList<FacilityStockMaster> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
