using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class WFDetailMgr : WFDetailBaseMgr, IWFDetailMgr
    {
        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public void CreateWFDetail(string taskCode, string status, DateTime now, User user)
        {
            this.CreateWFDetail(taskCode, status, null, null, string.Empty, now, user);
        }
        [Transaction(TransactionMode.Requires)]
        public void CreateWFDetail(string taskCode, string status, string content, DateTime now, User user)
        {
            this.CreateWFDetail(taskCode, status, null, null, content, now, user);
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateWFDetail(string taskCode, string status, int? level, int? preLevel, DateTime now, User user)
        {
            this.CreateWFDetail(taskCode, status, level, preLevel, string.Empty, now, user);
        }
        [Transaction(TransactionMode.Requires)]
        private void CreateWFDetail(string taskCode, string status, int? level, int? preLevel, string content, DateTime now, User user)
        {
            WFDetail wf = new WFDetail();
            wf.Content = content;
            wf.TaskCode = taskCode;
            wf.Status = status;
            wf.Level = level;
            wf.PreLevel = preLevel;
            wf.CreateDate = now;
            wf.CreateUser = user.Code;
            wf.CreateUserNm = user.Name;
            this.CreateWFDetail(wf);
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class WFDetailMgrE : com.Sconit.ISI.Service.Impl.WFDetailMgr, IWFDetailMgrE
    {
    }
}

#endregion Extend Class