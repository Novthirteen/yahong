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
using NPOI.SS.UserModel;
namespace com.Sconit.Service.Batch.Job
{
    [Transactional]
    public class OverSORepJob : RepJob
    {
        [Transaction(TransactionMode.Unspecified)]
        public override void Execute(JobRunContext context)
        {
            string key = "销售异常报表";

            string separator = BusinessConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;1. 逾期销售单：超期未关闭销售单");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;2. 超期未确认回单");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;3. 收货差异(日)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;4. 收货差异(月)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;5. 未开票账龄");
            int soExpiredDays = int.Parse(entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[] { "SOExpiredDays" })[0].Value);

            DateTime endDate = DateTime.Now.AddDays(-1 * soExpiredDays);
            DateTime startDate = endDate.AddDays(-1);
            //todo 2012-10-31有数据
            //endDate = new DateTime(2012, 10, 31);
            //startDate = endDate.AddDays(-30);
            SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_OVERSOREP, desc.ToString(), startDate, endDate);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            bool result1 = ProcessSO(workbook, startDate, endDate);

            bool result2 = ProcessSOASN(workbook, startDate, endDate);

            bool result3 = ProcessSOGap(workbook, startDate, endDate);

            bool result4 = false;
            //月最后一天
            if (endDate.HasValue && endDate.Value.ToString("yyyy-MM-dd") == endDate.Value.AddDays(1 - endDate.Value.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"))
            {
                result4 = ProcessSOBillAging(workbook);
            }
            return result1 || result2 || result3 || result4;
        }

    }
}



#region Extend Class


namespace com.Sconit.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class OverSORepJob : com.Sconit.Service.Batch.Job.OverSORepJob
    {
        public OverSORepJob()
        {
        }
    }
}

#endregion
