using System;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityDistributionDetailMgr : IFacilityDistributionDetailBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityDistributionDetailMgrE : com.Sconit.Facility.Service.IFacilityDistributionDetailMgr
    {
    }
}

#endregion Extend Interface