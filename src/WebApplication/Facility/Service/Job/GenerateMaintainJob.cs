

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Service.Batch;
using com.Sconit.Facility.Service.Ext;

namespace com.Sconit.Facility.Service.Batch.Job
{
    [Transactional]
    public class GenerateMaintainJob : IJob
    {
        public IFacilityMasterMgrE facilityMasterMgrE { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            facilityMasterMgrE.GenerateISITasks();
        }
    }
}

#region Extend Class

namespace com.Sconit.Facility.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class GenerateMaintainJob : com.Sconit.Facility.Service.Batch.Job.GenerateMaintainJob
    {
        public GenerateMaintainJob()
        {
        }
    }
}

#endregion
