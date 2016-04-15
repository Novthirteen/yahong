using System;
using com.Sconit.Entity.MRP;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP
{
    public interface IMrpRunLogMgr : IMrpRunLogBaseMgr
    {
        #region Customized Methods

        MrpRunLog GetLastestMrpRunLog();

        MrpRunLog GetLastestMrpRunLog(DateTime effectiveDate);

        IList<DateTime> GetLastestMrpRunLogDateTime(int count);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Service.Ext.MRP
{
    public partial interface IMrpRunLogMgrE : com.Sconit.Service.MRP.IMrpRunLogMgr
    {
    }
}

#endregion Extend Interface