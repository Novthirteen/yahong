using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class ResWokShopMgr : ResWokShopBaseMgr, IResWokShopMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class ResWokShopMgrE : com.Sconit.ISI.Service.Impl.ResWokShopMgr, IResWokShopMgrE
    {
    }
}

#endregion Extend Class