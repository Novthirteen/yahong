using System;
using com.Sconit.Entity.MRP;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP
{
    public interface IMrpLocationLotDetailMgr : IMrpLocationLotDetailBaseMgr
    {
        #region Customized Methods

        MrpLocationLotDetail LoadMrpLocationLotDetail(string locCode, string itemCode, DateTime effectiveDate);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Service.Ext.MRP
{
    public partial interface IMrpLocationLotDetailMgrE : com.Sconit.Service.MRP.IMrpLocationLotDetailMgr
    {
    }
}

#endregion Extend Interface