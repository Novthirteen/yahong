using com.Sconit.ISI.Entity;
using System;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ISummaryDetMgr : ISummaryDetBaseMgr
    {
        #region Customized Methods
        IList<SummaryDet> GetSummaryDet(string summaryCode);
        IList<SummaryDet> GetSummaryDet(string summaryCode, string userCode);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ISummaryDetMgrE : com.Sconit.ISI.Service.ISummaryDetMgr
    {
    }
}

#endregion Extend Interface