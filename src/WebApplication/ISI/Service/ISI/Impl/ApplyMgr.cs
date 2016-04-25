using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;


//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class ApplyMgr : ApplyBaseMgr, IApplyMgr
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
    public partial class ApplyMgrE : com.Sconit.ISI.Service.Impl.ApplyMgr, IApplyMgrE
    {
    }
}

#endregion Extend Class