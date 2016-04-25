using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskApplyViewMgr : ITaskApplyViewBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskApplyViewMgrE : com.Sconit.ISI.Service.ITaskApplyViewMgr
    {
    }
}

#endregion Extend Interface