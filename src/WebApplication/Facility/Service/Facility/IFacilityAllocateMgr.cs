using System;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service
{
    public interface IFacilityAllocateMgr : IFacilityAllocateBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Facility.Service.Ext
{
    public partial interface IFacilityAllocateMgrE : com.Sconit.Facility.Service.IFacilityAllocateMgr
    {
    }
}

#endregion Extend Interface