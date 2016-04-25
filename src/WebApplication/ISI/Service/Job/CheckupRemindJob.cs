

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Service.Batch;

namespace com.Sconit.ISI.Service.Batch.Job
{
    [Transactional]
    public class CheckupRemindJob : IJob
    {
        public ICheckupMgrE checkupMgrE { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            checkupMgrE.CheckupRemind();
        }
    }
}



#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class CheckupRemindJob : com.Sconit.ISI.Service.Batch.Job.CheckupRemindJob
    {
        public CheckupRemindJob()
        {
        }
    }
}

#endregion
