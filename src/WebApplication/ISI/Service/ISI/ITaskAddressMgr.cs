using System;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ITaskAddressMgr : ITaskAddressBaseMgr
    {
        #region Customized Methods

        TaskAddress CheckAndLoadTaskAddress(string code);

        IList<TaskAddress> GetCacheAllTaskAddress();

        IList<TaskAddress> GetTaskAddressNotCode(string code);

        IList<TaskAddress> GetTaskAddressByParent(string code);

        bool IsRef(string code);

        void UpdateTaskAddress(TaskAddress taskAddress, User user);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ITaskAddressMgrE : com.Sconit.ISI.Service.ITaskAddressMgr
    {
    }
}

#endregion Extend Interface