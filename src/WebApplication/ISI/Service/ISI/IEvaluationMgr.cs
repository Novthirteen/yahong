using com.Sconit.ISI.Entity;
using System;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface IEvaluationMgr : IEvaluationBaseMgr
    {
        #region Customized Methods
        IList<Evaluation> GetEvaluation(DateTime date, bool isInclude);
        IList<Evaluation> GetEvaluation(DateTime date);
        void UpdateEvaluation(IList<Evaluation> evaluationList);
        IList<Evaluation> GetEvaluation(string userCode);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface IEvaluationMgrE : com.Sconit.ISI.Service.IEvaluationMgr
    {
    }
}

#endregion Extend Interface