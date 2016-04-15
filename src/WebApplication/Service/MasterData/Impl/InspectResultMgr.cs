using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence.MasterData;
using com.Sconit.Entity.MasterData;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;

//TODO: Add other using statements here.

namespace com.Sconit.Service.MasterData.Impl
{
    [Transactional]
    public class InspectResultMgr : InspectResultBaseMgr, IInspectResultMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }
        #region Customized Methods


        [Transaction(TransactionMode.Unspecified)]
        public IList<InspectResult> GetInspectResults(string printNo)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(InspectResult));
            criteria.Add(Expression.Eq("PrintNo", printNo));
            return criteriaMgrE.FindAll<InspectResult>(criteria);
        }




        #endregion Customized Methods
    }
}

#region Extend Interface


namespace com.Sconit.Service.Ext.MasterData.Impl
{
    [Transactional]
    public partial class InspectResultMgrE : com.Sconit.Service.MasterData.Impl.InspectResultMgr, IInspectResultMgrE
    {

    }


}
#endregion