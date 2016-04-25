using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ICheckupRemindMgr : ICheckupRemindBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ICheckupRemindMgrE : com.Sconit.ISI.Service.ICheckupRemindMgr
    {
    }
}

#endregion Extend Interface