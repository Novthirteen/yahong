using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class SummaryDetMgr : SummaryDetBaseMgr, ISummaryDetMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        //TODO: Add other methods here.
        [Transaction(TransactionMode.Unspecified)]
        public IList<SummaryDet> GetSummaryDet(string summaryCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(SummaryDet));
            criteria.Add(Expression.Eq("SummaryCode", summaryCode));
            criteria.AddOrder(Order.Asc("Id"));
            var summaryDetList = this.criteriaMgrE.FindAll<SummaryDet>(criteria);
            return summaryDetList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<SummaryDet> GetSummaryDet(string summaryCode, string userCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(SummaryDet));
            criteria.Add(Expression.Eq("SummaryCode", summaryCode));

            DetachedCriteria subCriteria = DetachedCriteria.For<Summary>();
            subCriteria.Add(Expression.Eq("Code", summaryCode));
            subCriteria.Add(Expression.Eq("UserCode", userCode));
            subCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("Code")));

            criteria.Add(Subqueries.PropertyIn("SummaryCode", subCriteria));

            var summaryDetList = this.criteriaMgrE.FindAll<SummaryDet>(criteria);
            return summaryDetList;
        }


        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class SummaryDetMgrE : com.Sconit.ISI.Service.Impl.SummaryDetMgr, ISummaryDetMgrE
    {
    }
}

#endregion Extend Class