
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
    public class UserSubJob : IJob
    {
        public IUserSubscriptionMgrE userSubscriptionMgrE { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            userSubscriptionMgrE.Check();
        }
    }
}



#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class UserSubJob : com.Sconit.ISI.Service.Batch.Job.UserSubJob
    {
        public UserSubJob()
        {
        }
    }
}

#endregion
