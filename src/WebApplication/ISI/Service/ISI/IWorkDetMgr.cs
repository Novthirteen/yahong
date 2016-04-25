using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IWorkDetMgr : IWorkDetBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.
        void DeleteWorkDet(string taskCode);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IWorkDetMgrE : com.Sconit.ISI.Service.IWorkDetMgr
    {
    }
}

#endregion Extend Interface