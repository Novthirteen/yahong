using System;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.MasterData;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service
{
    public interface ICheckupMgr : ICheckupBaseMgr
    {
        #region Customized Methods

        Checkup CreateCheckup(string type, string checkupProjectCode, DateTime checkupDate, string[] userCodes, string content, decimal? amount, bool isAutoRelease, string auditInstructions, User user);

        void SubmitCheckup(int id, decimal? amount, string content, User user);

        void SubmitCheckup(Summary summary, User user, DateTime effdate);

        void ApproveCheckup(int id, decimal? amount, string auditInstructions, User user);

        void CancelCheckup(int id, User user);

        void CloseCheckup(int id, User user);

        void CloseCheckup(IList<Checkup> checkupList, User user);

        void ApproveCheckup(IList<Checkup> checkupList, User user);

        void CheckupRemind();

        void CheckupApproveRemind();

        void CloseRemind(User user);

        void Publish(User user);

        #endregion Customized Methods
    }
}


#region Extend Interface

namespace com.Sconit.ISI.Service.Ext
{
    public partial interface ICheckupMgrE : com.Sconit.ISI.Service.ICheckupMgr
    {
    }
}

#endregion Extend Interface