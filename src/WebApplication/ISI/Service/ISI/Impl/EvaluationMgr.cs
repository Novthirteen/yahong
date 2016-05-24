using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Criteria;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class EvaluationMgr : EvaluationBaseMgr, IEvaluationMgr
    {
        public ICriteriaMgrE criteriaMgrE { get; set; }

        #region Customized Methods

        [Transaction(TransactionMode.Unspecified)]
        public IList<Evaluation> GetEvaluation(string userCode)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Evaluation));
            if (!string.IsNullOrEmpty(userCode))
            {
                criteria.Add(Expression.Eq("UserCode", userCode));
            }
            return this.criteriaMgrE.FindAll<Evaluation>(criteria);

        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateEvaluation(IList<Evaluation> evaluationList)
        {
            if (evaluationList != null && evaluationList.Count > 0)
            {
                foreach (var evaluation in evaluationList)
                {
                    if (evaluation.IsBlankDetail)
                    {
                        this.CreateEvaluation(evaluation);
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(evaluation.OldUserCode))
                        {
                            this.DeleteEvaluation(evaluation.OldUserCode);
                        }
                        this.CreateEvaluation(evaluation);
                    }
                }
            }
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class EvaluationMgrE : com.Sconit.ISI.Service.Impl.EvaluationMgr, IEvaluationMgrE
    {
    }
}

#endregion Extend Class