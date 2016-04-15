using System;
using com.Sconit.Entity.MRP;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP
{
    public interface IMrpPlanTransactionMgr : IMrpPlanTransactionBaseMgr
    {
        #region Customized Methods

        IList<MrpPlanTransaction> GetMrpPlanTransactions(string flowCode, string locCode, string itemCode, DateTime effectiveDate, DateTime? winDate, DateTime? startDate);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Service.Ext.MRP
{
    public partial interface IMrpPlanTransactionMgrE : com.Sconit.Service.MRP.IMrpPlanTransactionMgr
    {
    }
}

#endregion Extend Interface