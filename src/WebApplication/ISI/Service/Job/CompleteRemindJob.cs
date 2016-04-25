
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
    public class CompleteRemindJob : IJob
    {
        public ITaskMgrE taskMgrE { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            taskMgrE.CompleteRemind();
        }
    }
}



#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class CompleteRemindJob : com.Sconit.ISI.Service.Batch.Job.CompleteRemindJob
    {
        public CompleteRemindJob()
        {
        }
    }
}

#endregion
