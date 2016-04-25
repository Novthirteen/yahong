using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class HelpLotDetailMgr : HelpLotDetailBaseMgr, IHelpLotDetailMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        public IList<HelpLotDetail> GetComment(string url, string targetId, string targetText, string userCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(HelpLotDetail));
            criteria.Add(Expression.Eq("Url", url));
            criteria.Add(Expression.Eq("TargetId", targetId));
            criteria.Add(Expression.Or(Expression.Eq("IsDefault", true), Expression.Eq("CreateUser", userCode)));
            return this.criteriaMgrE.FindAll<HelpLotDetail>(criteria);
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class HelpLotDetailMgrE : com.Sconit.ISI.Service.Impl.HelpLotDetailMgr, IHelpLotDetailMgrE
    {
    }
}

#endregion Extend Class