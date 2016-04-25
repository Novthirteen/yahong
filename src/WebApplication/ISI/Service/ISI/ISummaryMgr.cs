using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Entity;
using System;
using System.Collections.Generic;
using System.Text;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ISummaryMgr : ISummaryBaseMgr
    {
        #region Customized Methods

        void SummaryRemind4(DateTime date, IList<Evaluation> evaluationList, User user);
        IList<Summary> GetSummaryByDate(DateTime date);
        IList<Summary> GetSummaryByDate(string userCode, DateTime date);
        void SummaryRemind3(IList<Evaluation> evaluationList);
        void OutputNoEvaluation(IList<Evaluation> evaluationList, StringBuilder body);
        void SummaryRemind1(DateTime date, int startDays, int endDays, int approveDays);
        void SummaryRemind2(IList<Evaluation> evaluationList);
        void UpdateSummary(Summary summary, IList<SummaryDet> summaryDetList, User user);
        IList<Evaluation> GetNoSummary(string date, User user, bool isHasPermission);
        IList<Summary> GetSummary(string userCode, string date);
        void CancelSummary(string summaryCode, User user);
        Summary LoadSummary(string userCode, string date);

        Summary TransferEvaluation2Summary(User user);
        IList<SummaryDet> GetSummaryDet(DateTime date);
        IList<SummaryDet> GetSummaryDet(string userCode, string date);

        void CreateSummary(Summary summary, IList<SummaryDet> summaryDetList, User user);
        void SubmitSummary(Summary summary, IList<SummaryDet> summaryDetList, User user);
        void CloseSummary(string summaryCode, User user);
        void ApproveSummary(string code, User user);
        void ApproveSummary(Summary summary, IList<SummaryDet> summaryDetList, User user);

        string ApproveSummary2(Summary summary, IList<SummaryDet> summaryDetList, User user);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ISummaryMgrE : com.Sconit.ISI.Service.ISummaryMgr
    {
    }
}

#endregion Extend Interface