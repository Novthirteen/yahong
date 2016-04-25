using System;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityStockMgr : IFacilityStockBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityStockMgrE : com.Sconit.Facility.Service.IFacilityStockMgr
    {
    }
}

#endregion Extend Interface