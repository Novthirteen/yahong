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
    public class OverWORepJob : RepJob
    {
        
        [Transaction(TransactionMode.Unspecified)]
        public override void Execute(JobRunContext context)
        {
            string key = "�����쳣����";

            string separator = BusinessConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;���1. δ���ڹرյ�������");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;1. ���ڵ���������������ϸ����");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;2. ���ڵ��������嵥");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;���2. ȡ����������");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;1. ȡ������������������ϸ����");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;2. ȡ�����������嵥");
            int woExpiredDays = int.Parse(entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[] { "WOExpiredDays" })[0].Value);

            DateTime now = DateTime.Now;
            DateTime endDate = now.AddDays(-1 * woExpiredDays);

            SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_OVERWOREP, desc.ToString(), now, endDate);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {
            bool result1 = ProcessWO(workbook, startDate, endDate);

            //bool result2 = ProcessWOGap(key, workbook,startDate,endDate);

            return result1;
        }
    }
}



#region Extend Class


namespace com.Sconit.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class OverWORepJob : com.Sconit.Service.Batch.Job.OverWORepJob
    {
        public OverWORepJob()
        {
        }
    }
}

#endregion
