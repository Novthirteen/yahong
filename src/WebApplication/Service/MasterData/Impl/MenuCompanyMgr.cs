using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class MenuCompanyMgr : MenuCompanyBaseMgr, IMenuCompanyMgr
    {
        

        #region Customized Methods

        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class MenuCompanyMgrE : com.Sconit.Service.MasterData.Impl.MenuCompanyMgr, IMenuCompanyMgrE
    {
    }
}

#endregion Extend Class