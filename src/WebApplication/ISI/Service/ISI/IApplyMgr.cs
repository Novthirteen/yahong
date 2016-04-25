using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IApplyMgr : IApplyBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IApplyMgrE : com.Sconit.ISI.Service.IApplyMgr
    {
    }
}

#endregion Extend Interface