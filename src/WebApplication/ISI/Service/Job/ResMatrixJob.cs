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
    public class ResMatrixJob : IJob
    {
        public IResMatrixMgrE resMatrixMgr { get; set; }
        public IUserMgrE userMgr { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            resMatrixMgr.CreateTask(this.userMgr.GetMonitorUser());
        }
    }
}




#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class ResMatrixJob : com.Sconit.ISI.Service.Batch.Job.ResMatrixJob
    {
        public ResMatrixJob()
        {
        }
    }
}

#endregion
