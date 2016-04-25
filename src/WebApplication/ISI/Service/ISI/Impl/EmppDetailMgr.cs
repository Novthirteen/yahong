using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class EmppDetailMgr : EmppDetailBaseMgr, IEmppDetailMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        [Transaction(TransactionMode.Unspecified)]
        public IList<EmppDetail> GetEmppDetail(string MsgID)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(EmppDetail));
            criteria.Add(Expression.Eq("MsgID", MsgID));

            return criteriaMgrE.FindAll<EmppDetail>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<EmppDetail> GetEmppDetail(string MsgID, int SeqID)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(EmppDetail));
            criteria.Add(Expression.Eq("MsgID", MsgID));
            criteria.Add(Expression.Eq("SeqID", SeqID));

            return criteriaMgrE.FindAll<EmppDetail>(criteria);
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<EmppDetail> GetEmppDetailByDestID(string DestID)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(EmppDetail));
            criteria.Add(Expression.Eq("DestID", DestID));

            return criteriaMgrE.FindAll<EmppDetail>(criteria);
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class EmppDetailMgrE : com.Sconit.ISI.Service.Impl.EmppDetailMgr, IEmppDetailMgrE
    {
    }
}

#endregion Extend Class