
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
    public class TaskReportCheckJob : IJob
    {
        public ITaskReportMgrE taskReportMgrE { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            taskReportMgrE.Check();
        }
    }
}



#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class TaskReportCheckJob : com.Sconit.ISI.Service.Batch.Job.TaskReportCheckJob
    {
        public TaskReportCheckJob()
        {
        }
    }
}

#endregion
