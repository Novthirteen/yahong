using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IHelpLotDetailMgr : IHelpLotDetailBaseMgr
    {
        #region Customized Methods

        IList<HelpLotDetail> GetComment(string url, string targetId, string targetText, string userCode);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IHelpLotDetailMgrE : com.Sconit.ISI.Service.IHelpLotDetailMgr
    {
    }
}

#endregion Extend Interface