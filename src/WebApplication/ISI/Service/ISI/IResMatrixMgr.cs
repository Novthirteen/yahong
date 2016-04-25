using System;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IResMatrixMgr : IResMatrixBaseMgr
    {
        #region Customized Methods

        void CreateTask(User user, DateTime dateTime);

        void CreateTask(User user);

        void SendResChangeLog();

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IResMatrixMgrE : com.Sconit.ISI.Service.IResMatrixMgr
    {
    }
}

#endregion Extend Interface