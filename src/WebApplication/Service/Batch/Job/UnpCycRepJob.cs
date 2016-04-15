using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.View;
using com.Sconit.Service.Ext;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Service.Ext.Hql;

using com.Sconit.Service.Ext.MasterData;

using com.Sconit.Utility;
using NHibernate.Expression;
using NHibernate.Transform;
using NPOI.HPSF;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;

namespace com.Sconit.Service.Batch.Job
{
    [Transactional]
    public class UnpCycRepJob : RepJob
    {

        [Transaction(TransactionMode.Unspecified)]
        public override void Execute(JobRunContext context)
        {
            string key = "库存调整报表";
            DateTime endDate = DateTime.Now;
            //todo
            DateTime startDate = endDate.AddDays(-1);
            //DateTime startDate = endDate.AddDays(-129);
            string separator = BusinessConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;1. 计划外出库");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;2. 计划外入库");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;3. 盘点");
            SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_UNPCYCREP, desc.ToString(), startDate, endDate);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            
            bool result1 = ProcessUnpCyc(workbook, startDate, endDate);

            return result1;
        }
    }
}



#region Extend Class


namespace com.Sconit.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class UnpCycRepJob : com.Sconit.Service.Batch.Job.UnpCycRepJob
    {
        public UnpCycRepJob()
        {
        }
    }
}

#endregion
