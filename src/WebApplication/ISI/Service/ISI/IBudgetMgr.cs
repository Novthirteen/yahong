using com.Sconit.ISI.Entity;
using System;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IBudgetMgr : IBudgetBaseMgr
    {
        #region Customized Methods
        Budget LoadBudget(string taskSubType, string year);
        Budget LoadBudget(string code, string taskSubType, string year);
        //TODO: Add other methods here.

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IBudgetMgrE : com.Sconit.ISI.Service.IBudgetMgr
    {
    }
}

#endregion Extend Interface