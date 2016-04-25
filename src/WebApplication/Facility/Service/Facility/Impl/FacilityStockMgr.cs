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
    public class FacilityStockMgr : FacilityStockBaseMgr, IFacilityStockMgr
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
    public partial class FacilityStockMgrE : com.Sconit.Facility.Service.Impl.FacilityStockMgr, IFacilityStockMgrE
    {
    }
}

#endregion Extend Class