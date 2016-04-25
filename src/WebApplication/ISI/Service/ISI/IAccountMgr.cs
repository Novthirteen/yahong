using com.Sconit.ISI.Entity;
using System;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IAccountMgr : IAccountBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.
        bool IsRef(string code);

        IList<Account> GetAccount1();
        IList<Account> GetAccount1(string costCenterHead);
        IList<Account> GetAccount1(string costCenterHead, string costCenterDetail);
        IList<Account> GetAccount2();
        IList<Account> GetAccount2(string costCenterHead, string account1);
        IList<Account> GetAccount2(string costCenterHead, string costCenterDetail, string account1);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IAccountMgrE : com.Sconit.ISI.Service.IAccountMgr
    {
    }
}

#endregion Extend Interface