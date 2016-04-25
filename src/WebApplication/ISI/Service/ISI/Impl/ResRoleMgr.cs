using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Persistence;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class ResRoleMgr : ResRoleBaseMgr, IResRoleMgr
    {
        #region Customized Methods


        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class ResRoleMgrE : com.Sconit.ISI.Service.Impl.ResRoleMgr, IResRoleMgrE
    {
    }
}

#endregion Extend Class