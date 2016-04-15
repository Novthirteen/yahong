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
    public class OverPORepJob : RepJob
    {

        [Transaction(TransactionMode.Unspecified)]
        public override void Execute(JobRunContext context)
        {
            string key = "�ɹ��쳣����";

            string separator = BusinessConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;1. ���ڲɹ���������δ�رղɹ���");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;2. �˻�(��)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;3. �˻�(��)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;4. ���ڲɹ��ͻ���������δȷ�ϲɹ��ͻ���");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;5. �ջ�����(��)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;6. �ջ�����(��)");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;7. δ��Ʊ����");
            int poExpiredDays = int.Parse(entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[] { "POExpiredDays" })[0].Value);

            DateTime endDate = DateTime.Now.AddDays(-1 * poExpiredDays);
            DateTime startDate = endDate.AddDays(-1);
            //todo 2012-10-31������
            //endDate = new DateTime(2012, 10, 31);
            //startDate = endDate.AddDays(-30);
            SendEmail(key, BusinessConstants.PERMISSION_PAGE_VALUE_OVERPOREP, desc.ToString(), startDate, endDate);
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {

            bool result1 = false;
            result1 = ProcessPO(workbook, startDate, endDate);

            bool result2 = false;
            result2 = ProcessPOASN(workbook, startDate, endDate);

            bool result3 = false;
            result3 = ProcessPOGap(workbook, startDate, endDate);

            bool result4 = false;

            //�����һ��
            if (endDate.HasValue &&
                    endDate.Value.ToString("yyyy-MM-dd") == endDate.Value.AddDays(1 - endDate.Value.Day).AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd"))
            {
                result4 = ProcessPOBillAging(workbook);
            }

            return result1 || result2 || result3 || result4;

        }
    }

}

#region Extend Class


namespace com.Sconit.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class OverPORepJob : com.Sconit.Service.Batch.Job.OverPORepJob
    {
        public OverPORepJob()
        {
        }
    }
}

#endregion
