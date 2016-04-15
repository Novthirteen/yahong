using System;
using com.Sconit.Entity.Procurement;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.Service.Procurement
{
    public interface IOrderTracerMgr : IOrderTracerBaseMgr
    {
        #region Customized Methods

        IList<OrderTracer> GetOrderTracer(DateTime? startTime, DateTime? endTime, string TracerType);

        IList<OrderTracer> GetOrderTracer(List<int> ids, List<string> tracerTypes);

        void DeleteOrderTracerByOrderDetailId(IList<int> olt);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.Service.Ext.Procurement
{
    public partial interface IOrderTracerMgrE : com.Sconit.Service.Procurement.IOrderTracerMgr
    {
    }
}

#endregion Extend Interface