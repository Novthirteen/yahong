using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskStatusViewMgr : ITaskStatusViewBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskStatusViewMgrE : com.Sconit.ISI.Service.ITaskStatusViewMgr
    {
    }
}

#endregion Extend Interface