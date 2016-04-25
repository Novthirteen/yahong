using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IResRoleMgr : IResRoleBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IResRoleMgrE : com.Sconit.ISI.Service.IResRoleMgr
    {
    }
}

#endregion Extend Interface