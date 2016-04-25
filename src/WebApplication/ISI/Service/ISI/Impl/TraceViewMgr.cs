using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Persistence;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using NHibernate;
using com.Sconit.Service.Ext.Criteria;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class TraceViewMgr : TraceViewBaseMgr, ITraceViewMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }
        #region Customized Methods

        [Transaction(TransactionMode.Unspecified)]
        public IList<Comment> GetComment(string taskCode, int firstRow, int maxRows)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TraceView));
            criteria.Add(Expression.Eq("TaskCode", taskCode));
            ICriteria c = criteria.GetExecutableCriteria(this.daoBase.GetSession());
            ProjectionList list = Projections.ProjectionList().Create();
            list.Add(Projections.Distinct(Projections.Property("Id")));
            list.Add(Projections.GroupProperty("Id"), "Id");
            list.Add(Projections.GroupProperty("Desc"), "Value");
            list.Add(Projections.GroupProperty("TaskCode"), "TaskCode");
            list.Add(Projections.GroupProperty("CreateDate"), "CreateDate");
            list.Add(Projections.GroupProperty("CreateUserNm"), "CreateUser");
            list.Add(Projections.GroupProperty("Type"), "Type");

            criteria.SetProjection(list);
            criteria.AddOrder(Order.Desc("CreateDate"));
            criteria.AddOrder(Order.Desc("Id"));
            c.SetFirstResult(firstRow);
            c.SetMaxResults(maxRows);
            c.SetResultTransformer(new NHibernate.Transform.AliasToBeanResultTransformer(typeof(Comment)));

            return c.List<Comment>();
        }

        [Transaction(TransactionMode.Unspecified)]
        public int GetCommentCount(string taskCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TraceView));
            criteria.Add(Expression.Eq("TaskCode", taskCode));

            criteria.SetProjection(Projections.ProjectionList().Add(Projections.Count("Id")));

            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return count[0];
            }
            else
            {
                return 0;
            }
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class TraceViewMgrE : com.Sconit.ISI.Service.Impl.TraceViewMgr, ITraceViewMgrE
    {
    }
}

#endregion Extend Class