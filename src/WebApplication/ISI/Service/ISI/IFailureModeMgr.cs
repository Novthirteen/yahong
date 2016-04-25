using System;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IFailureModeMgr : IFailureModeBaseMgr
    {
        #region Customized Methods

        IList<FailureMode> GetAllFailureMode(string taskSubType);

        bool IsRef(string code);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IFailureModeMgrE : com.Sconit.ISI.Service.IFailureModeMgr
    {
    }
}

#endregion Extend Interface