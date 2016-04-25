using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IProcessInstanceMgr : IProcessInstanceBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IProcessInstanceMgrE : com.Sconit.ISI.Service.IProcessInstanceMgr
    {
    }
}

#endregion Extend Interface