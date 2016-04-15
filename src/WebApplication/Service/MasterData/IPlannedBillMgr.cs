using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Procurement;
using System.Collections.Generic;
using System;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData
{
    public interface IPlannedBillMgr : IPlannedBillBaseMgr
    {
        #region Customized Methods

        IList<PlannedBill> GetUnSettledPlannedBill(OrderHead orderHead);

        IList<PlannedBill> GetUnSettledPlannedBill(string orderNo);

        //PlannedBill CreatePlannedBill(ReceiptDetail receiptDetail, User user);

        IList<PlannedBill> CreatePlannedBill(ReceiptDetail receiptDetail, User user);

        void RecalculatePrice(User user, string moduleType, string partyCode, string flowCode, DateTime? startDate, DateTime? endDate, string itemCode);

        #endregion Customized Methods
    }
}



#region Extend Interface






namespace com.Sconit.Service.Ext.MasterData
{
    public partial interface IPlannedBillMgrE : com.Sconit.Service.MasterData.IPlannedBillMgr
    {

    }
}

#endregion

#region Extend Interface






namespace com.Sconit.Service.Ext.MasterData
{
    public partial interface IPlannedBillMgrE : com.Sconit.Service.MasterData.IPlannedBillMgr
    {

    }
}

#endregion
