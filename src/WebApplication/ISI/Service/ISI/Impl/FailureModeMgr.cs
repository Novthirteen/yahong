using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class FailureModeMgr : FailureModeBaseMgr, IFailureModeMgr
    {
        private static IList<FailureMode> cachedAllFailureMode;

        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        [Transaction(TransactionMode.Unspecified)]
        public IList<FailureMode> GetAllFailureMode(string taskSubType)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FailureMode));
            criteria.Add(Expression.Eq("IsActive", true));
            if (!string.IsNullOrEmpty(taskSubType))
            {
                criteria.Add(Expression.Eq("TaskSubType.Code", taskSubType));
            }

            IList<FailureMode> cachedAllFailureMode = this.criteriaMgrE.FindAll<FailureMode>(criteria);

            return cachedAllFailureMode;
        }

        [Transaction(TransactionMode.Unspecified)]
        public bool IsRef(string code)
        {
            //ÈÎÎñ
            DetachedCriteria criteria = DetachedCriteria.For(typeof(TaskMstr)).SetProjection(Projections.CountDistinct("Code"));
            criteria.Add(Expression.Eq("FailureMode.Code", code));
            IList<int> count = this.criteriaMgrE.FindAll<int>(criteria);
            if (count != null && count.Count > 0 && count[0] > 0)
            {
                return true;
            }
            return false;
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class FailureModeMgrE : com.Sconit.ISI.Service.Impl.FailureModeMgr, IFailureModeMgrE
    {
    }
}

#endregion Extend Class