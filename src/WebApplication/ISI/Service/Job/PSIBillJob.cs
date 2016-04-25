
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Service.Batch;
using com.Sconit.ISI.Entity;

using System.IO;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.ISI.Service.Util;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Ionic.Zip;

namespace com.Sconit.ISI.Service.Batch.Job
{
    [Transactional]
    public class PSIBillJob : IJob
    {
        public IMouldMgrE mouldMgrE { get; set; }
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            try
            {
                mouldMgrE.RemindBill();
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }

        }

    }
}



#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class PSIBillJob : com.Sconit.ISI.Service.Batch.Job.PSIBillJob
    {
        public PSIBillJob()
        {
        }
    }
}

#endregion
