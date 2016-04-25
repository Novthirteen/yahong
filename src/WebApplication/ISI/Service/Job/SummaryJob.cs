
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
    public class SummaryJob : IJob
    {
        public IEvaluationMgrE evaluationMgrE { get; set; }
        public ISummaryMgrE summaryMgrE { get; set; }


        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        [Transaction(TransactionMode.Unspecified)]
        public void Execute(JobRunContext context)
        {
            DateTime now = DateTime.Now;
            int day = now.Day;
            DateTime date = DateTime.Parse(DateTime.Now.AddMonths(-1).ToString("yyyy-MM-01"));

            int startDays = context.JobDataMap.GetIntValue("StartDays");
            int endDays = context.JobDataMap.GetIntValue("EndDays");
            //������Щ��û�ύ
            int remindDays = context.JobDataMap.GetIntValue("RemindDays");
            int approveDays = context.JobDataMap.GetIntValue("ApproveDays");

            if (startDays <= day && day <= endDays)
            {
                try
                {
                    summaryMgrE.SummaryRemind1(date, startDays, endDays, approveDays);
                }
                catch (Exception e)
                {
                    log.Error(e.Message, e);
                }
            }

            var evaluationList = evaluationMgrE.GetEvaluation(date, false);
            //���Ѹ�������Щ�˻�û�ύ
            if (day == remindDays)
            {
                summaryMgrE.SummaryRemind3(evaluationList);
            }

            if (day == approveDays)
            {
                try
                {
                    //����������
                    summaryMgrE.SummaryRemind2(evaluationList);
                }
                catch (Exception e)
                {
                    log.Error(e.Message, e);
                }

                try
                {
                    //�����ļ���������
                    summaryMgrE.SummaryRemind4(date, evaluationList, null);
                }
                catch (Exception e)
                {
                    log.Error(e.Message, e);
                }
            }
        }

    }
}



#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class SummaryJob : com.Sconit.ISI.Service.Batch.Job.SummaryJob
    {
        public SummaryJob()
        {
        }
    }
}

#endregion
