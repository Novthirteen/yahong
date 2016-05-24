using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Entity;
using System;
using System.Collections.Generic;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ISummaryMgr : ISummaryBaseMgr
    {
        #region Customized Methods
        void UpdateSummary(Summary summary, IList<SummaryDet> summaryDetList, User user);
        IList<Evaluation> GetNoSummary(string date, User user);
        IList<Summary> GetSummary(string userCode, string date);
        void CancelSummary(string summaryCode, User user);
        Summary LoadSummary(string userCode, string date);

        Summary TransferEvaluation2Summary(User user);

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