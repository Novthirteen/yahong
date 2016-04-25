
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
    public class OpenRemindJob : IJob
    {
        public ITaskMgrE taskMgrE { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            taskMgrE.OpenRemind();
        }
    }
}



#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class OpenRemindJob : com.Sconit.ISI.Service.Batch.Job.OpenRemindJob
    {
        public OpenRemindJob()
        {
        }
    }
}

#endregion
