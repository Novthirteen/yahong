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
    public class ResMatrixLogMgr : ResMatrixLogBaseMgr, IResMatrixLogMgr
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
    public partial class ResMatrixLogMgrE : com.Sconit.ISI.Service.Impl.ResMatrixLogMgr, IResMatrixLogMgrE
    {
    }
}

#endregion Extend Class