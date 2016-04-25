using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Facility.Persistence;

//TODO: Add other using statements here.

namespace com.Sconit.Facility.Service.Impl
{
    [Transactional]
    public class FacilityAllocateMgr : FacilityAllocateBaseMgr, IFacilityAllocateMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Impl
{
    [Transactional]
    public partial class FacilityAllocateMgrE : com.Sconit.Facility.Service.Impl.FacilityAllocateMgr, IFacilityAllocateMgrE
    {
    }
}

#endregion Extend Class