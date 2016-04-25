using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using com.Sconit.Facility.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityStockBaseMgr
    {
        #region Method Created By CodeSmith

        void CreateFacilityStock(FacilityStock entity);

        FacilityStock LoadFacilityStock(Int32 id);

        IList<FacilityStock> GetAllFacilityStock();
    
        void UpdateFacilityStock(FacilityStock entity);

        void DeleteFacilityStock(Int32 id);
    
        void DeleteFacilityStock(FacilityStock entity);
    
        void DeleteFacilityStock(IList<Int32> pkList);
    
        void DeleteFacilityStock(IList<FacilityStock> entityList);    
    
        #endregion Method Created By CodeSmith
    }
}
