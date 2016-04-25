using com.Sconit.ISI.Entity;
using System;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IBudgetDetMgr : IBudgetDetBaseMgr
    {
        #region Customized Methods

        //TODO: Add other methods here.

        IList<BudgetDet> GetBudgetDet(string taskSubType, string account1);
        IList<BudgetDet> GetBudgetDetByTaskSubType(string taskSubType);

        IList<BudgetDet> GetBudgetDet(string taskSubType, int year, string account1);
        IList<BudgetDet> GetBudgetDet(string budgetCode);


        IList<BudgetDet> GetAccount1(string costCenterHead);
        IList<BudgetDet> GetAccount1(string costCenterHead, string costCenterDetail);
        IList<BudgetDet> GetAccount2(string costCenterHead, string account1);
        IList<BudgetDet> GetAccount2(string costCenterHead, string costCenterDetail, string account1);
        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IBudgetDetMgrE : com.Sconit.ISI.Service.IBudgetDetMgr
    {
    }
}

#endregion Extend Interface