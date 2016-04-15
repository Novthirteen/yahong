using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence.MRP;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.Entity.MRP;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MRP.Impl
{
    [Transactional]
    public class MrpLocationLotDetailMgr : MrpLocationLotDetailBaseMgr, IMrpLocationLotDetailMgr
    {
        public ICriteriaMgrE criteriaMgr { get; set; }
        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public MrpLocationLotDetail LoadMrpLocationLotDetail(string locCode, string itemCode, DateTime effectiveDate)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(MrpLocationLotDetail));
            if (itemCode != null && itemCode.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq("Item", itemCode));
            }
            if (locCode != null && locCode.Trim() != string.Empty)
            {
                criteria.Add(Expression.Eq("Location", locCode));
            }
            criteria.Add(Expression.Eq("EffectiveDate", effectiveDate.Date));

            IList<MrpLocationLotDetail> list = criteriaMgr.FindAll<MrpLocationLotDetail>(criteria);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return null;
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.Service.Ext.MRP.Impl
{
    [Transactional]
    public partial class MrpLocationLotDetailMgrE : com.Sconit.Service.MRP.Impl.MrpLocationLotDetailMgr, IMrpLocationLotDetailMgrE
    {
    }
}

#endregion Extend Class