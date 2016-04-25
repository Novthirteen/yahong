using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IResUserMgr : IResUserBaseMgr
    {
        #region Customized Methods

        void BatchUpdateEndDate(string userCode, DateTime endDate, string modifyUserCode);
        void CloneUser(string oldUser, string newUser, string createUser);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IResUserMgrE : com.Sconit.ISI.Service.IResUserMgr
    {
    }
}

#endregion Extend Interface