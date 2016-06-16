using System;
using com.Sconit.Facility.Entity;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityCategoryMgr : IFacilityCategoryBaseMgr
    {
        #region Customized Methods

         IList<FacilityCategory> GetAllMouldCategory();

         IList<FacilityCategory> GetAllEquipmentCategory();

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityCategoryMgrE : com.Sconit.Facility.Service.IFacilityCategoryMgr
    {
    }
}

#endregion Extend Interface