using System;
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData
{
    public interface IInspectResultMgr : IInspectResultBaseMgr
    {
        #region Customized Methods

         IList<InspectResult> GetInspectResults(string printNo);

        #endregion Customized Methods
    }
}

#region Extend Interface

namespace com.Sconit.Service.Ext.MasterData
{
    public partial interface IInspectResultMgrE : com.Sconit.Service.MasterData.IInspectResultMgr
    {

    }
}

#endregion