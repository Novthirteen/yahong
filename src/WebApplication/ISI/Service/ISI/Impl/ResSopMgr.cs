using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using System.Linq;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class ResSopMgr : ResSopBaseMgr, IResSopMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }
        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public ResSop LoadResSop(string workShop, int operate)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ResSop));
            criteria.Add(Expression.Eq("WorkShop", workShop));
            criteria.Add(Expression.Eq("Operate", operate));
            return criteriaMgrE.FindAll<ResSop>(criteria).FirstOrDefault();
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteResSop(int id)
        {
            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(AttachmentDetail));
            selectCriteria.Add(Expression.Eq("ModuleType", typeof(ResSop).FullName));
            selectCriteria.Add(Expression.Eq("TaskCode", id.ToString()));
            var list = criteriaMgrE.FindAll<AttachmentDetail>(selectCriteria) ?? new List<AttachmentDetail>();
            foreach (var attatchment in list)
            {
                criteriaMgrE.Delete(attatchment);
            }
            base.DeleteResSop(id);
        }

        [Transaction(TransactionMode.Requires)]
        public override void DeleteResSop(ResSop entity)
        {
            DeleteResSop(entity.Id);
        }

        [Transaction(TransactionMode.Requires)]
        public List<ResSop> GetResSopList(string workShop)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ResSop));
            criteria.Add(Expression.Eq("WorkShop", workShop));
            return criteriaMgrE.FindAll<ResSop>(criteria).ToList();
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class ResSopMgrE : com.Sconit.ISI.Service.Impl.ResSopMgr, IResSopMgrE
    {
    }
}

#endregion Extend Class