using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Service.Batch;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity;
using System.Net.Mail;
using com.Sconit.ISI.Service.Util;
using com.Sconit.Utility;
using com.Sconit.Entity.MasterData;
using NPOI.SS.UserModel;
using com.Sconit.ISI.Batch;
using com.Sconit.Service.SPC;
using com.Sconit.Entity.SPC;

namespace com.Sconit.ISI.Batch.Job
{
    [Transactional]
    public class SPCJob : RepJob
    {
        public ISPCMgr spcMgrE { get; set; }
        public ILanguageMgrE languageMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public ICriteriaMgrE criteriaMgr { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public override void Execute(JobRunContext context)
        {
            SendSPCChangeLog();
        }


        public void SendSPCChangeLog()
        {
            #region 每天发昨天的
            DateTime dateTimeNow = DateTime.Now.Date;
            //DateTime dateTimeNow = new DateTime(2014, 12, 3);
            var startDate = dateTimeNow.AddDays(-1);
            var endDate = dateTimeNow.Date;

            string key = "SPC异常报表（天）";
            string separator = ISIConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;SPC异常报表");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;开始时间：");
            desc.Append(startDate.ToShortDateString());
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;结束时间：");
            desc.Append(endDate.ToShortDateString());
            SendEmail(key, ISIConstants.PERMISSION_PAGE_VALUE_SPCLOG, desc.ToString(), startDate, endDate);
            #endregion

            #region 每周发上周的
            if (dateTimeNow.DayOfWeek == DayOfWeek.Monday)
            {
                endDate = dateTimeNow.AddDays(-1);
                startDate = endDate.AddDays(-7);

                key = "SPC异常报表（周）";
                separator = ISIConstants.EMAIL_SEPRATOR;
                desc = new StringBuilder();
                desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;SPC异常报表");
                desc.Append(separator);
                desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;开始时间：");
                desc.Append(startDate.ToShortDateString());
                desc.Append(separator);
                desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;结束时间：");
                desc.Append(endDate.ToShortDateString());
                SendEmail(key, ISIConstants.PERMISSION_PAGE_VALUE_SPCLOG, desc.ToString(), startDate, endDate);
            }
            #endregion

        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {

            bool result = false;

            DetachedCriteria traceCriteria = DetachedCriteria.For(typeof(XRTrace));
            traceCriteria.Add(Expression.Ge("CreateDate", startDate));
            traceCriteria.Add(Expression.Lt("CreateDate", endDate));
            var xrTraceList = criteriaMgr.FindAll<XRTrace>(traceCriteria) ?? new List<XRTrace>();



            if (xrTraceList != null && xrTraceList.Count > 0)
            {
                DetachedCriteria masterCriteria = DetachedCriteria.For(typeof(XRMaster));
                masterCriteria.Add(Expression.In("Code", xrTraceList.Select(p => p.SpcCode).Distinct().ToArray()));
                var xrMasterList = criteriaMgr.FindAll<XRMaster>(masterCriteria) ?? new List<XRMaster>();

                DetachedCriteria criteria = DetachedCriteria.For(typeof(XRDetail));
                criteria.Add(Expression.In("Id", xrTraceList.Select(p => p.SpcDetId).Distinct().ToArray()));
                criteria.AddOrder(NHibernate.Expression.Order.Asc("SpcCode"));
                var xrDetailList = criteriaMgr.FindAll<XRDetail>(criteria) ?? new List<XRDetail>();

                result = ProcessSPCLog(workbook, xrDetailList, xrMasterList, xrTraceList);
            }
            return result;

        }


        public override void SendEmail(string key, string permissionCode, string desc, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string mailList = uesrMgrE.FindEmailByPermission(new string[] { permissionCode });

                IList<EntityPreference> entityPreferenceList = entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[]{BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME,
                                                    ISIConstants.ENTITY_PREFERENCE_WEBADDRESS,BusinessConstants.ENTITY_PREFERENCE_CODE_DEBUGMODE,BusinessConstants.ENTITY_PREFERENCE_CODE_DEBUGMODEEMAIL});
                var debugMode = entityPreferenceList.Where(e => e.Code == BusinessConstants.ENTITY_PREFERENCE_CODE_DEBUGMODE).SingleOrDefault();

                if (debugMode != null && debugMode.Value.ToUpper() == "TRUE")
                {

                    mailList = "wangxiang@yfgm.com.cn";
                }
                log.Info(key + ",发送列表：" + mailList);
                if (string.IsNullOrEmpty(mailList)) return;

                DateTime now = DateTime.Now;
                string companyName = entityPreferenceList.Where(e => e.Code == BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).SingleOrDefault().Value;
                string webAddress = entityPreferenceList.Where(e => e.Code == ISIConstants.ENTITY_PREFERENCE_WEBADDRESS).SingleOrDefault().Value;

                string file = GenerateReport(key, permissionCode, entityPreferenceList, startDate, endDate);

                if (!string.IsNullOrEmpty(file))
                {
                    IList<string> files = new List<string>();
                    files.Add(file);

                    StringBuilder content = new StringBuilder();
                    content.Append("<p style='font-size:15px;'>");
                    string separator = ISIConstants.EMAIL_SEPRATOR;

                    ISIUtil.AppendTestText(companyName, content, separator);

                    content.Append(separator);
                    content.Append("您好");
                    content.Append(separator);
                    content.Append(desc);
                    content.Append(separator);
                    //content.Append(separator);
                    //content.Append("&nbsp;&nbsp;请见附件");

                    content.Append(separator);
                    content.Append(companyName);
                    content.Append(separator);
                    content.Append("<a href='http://" + webAddress + "'>http://" + webAddress + "</a>");
                    content.Append(separator);
                    content.Append("</p>");

                    smtpMgrE.AsyncSend2(companyName + "-" + key, content.ToString(), mailList, string.Empty, MailPriority.Normal, files);
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        private bool ProcessSPCLog(IWorkbook workbook, IList<XRDetail> xrDetailList, IList<XRMaster> xrMasterList, IList<XRTrace> xrTraceList)
        {
            try
            {
                #region 初始化Sheet

                ISheet sheet = workbook.CreateSheet("SPC异常");

                for (int i = 0; i <= 12; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }

                sheet.SetColumnWidth(0, 10 * 256);//序号
                sheet.SetColumnWidth(1, 20 * 256);//SPC单号
                sheet.SetColumnWidth(2, 10 * 256);//物料
                sheet.SetColumnWidth(3, 40 * 256);//物料描述
                sheet.SetColumnWidth(4, 10 * 256);//测试设备号
                sheet.SetColumnWidth(5, 10 * 256);//测试点
                sheet.SetColumnWidth(6, 10 * 256);//标范
                sheet.SetColumnWidth(7, 10 * 256);//上公差
                sheet.SetColumnWidth(8, 10 * 256);//下公差
                sheet.SetColumnWidth(9, 10 * 256);//均值
                sheet.SetColumnWidth(10, 10 * 256);//极差
                sheet.SetColumnWidth(11, 50 * 256);//异常描述
                sheet.SetColumnWidth(12, 200 * 256);//跟踪

                int rownum = 1;
                int column = 0;

                XlsHelper.SetRowCell(sheet, rownum, column++, "序号", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "SPC单号", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "物料", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "物料描述", headStyle);

                XlsHelper.SetRowCell(sheet, rownum, column++, "测试设备号", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "测试点", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "标范", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "上公差", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "下公差", headStyle);

                XlsHelper.SetRowCell(sheet, rownum, column++, "均值", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "极差", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "异常描述", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "跟踪", headStyle);
                #endregion

                XlsHelper.SetRowCell(sheet, 0, 0, "SPC异常：", headStyle2);
                XlsHelper.SetRowCell(sheet, 0, 1, xrDetailList.Count() + " 条", headStyle2);

                #region 输出数据
                rownum = 2;
                for (int i = 0; i < xrDetailList.Count(); i++)
                {
                    column = 0;
                    var xrDetail = xrDetailList[i];
                    var xrMaster = xrMasterList.Where(p => p.Code == xrDetail.SpcCode).FirstOrDefault();
                    var xrTraces = xrTraceList.Where(p => p.SpcDetId == xrDetail.Id).ToList();

                    XlsHelper.SetRowCell(sheet, rownum, column++, (i + 1).ToString());
                    XlsHelper.SetRowCell(sheet, rownum, column++, xrMaster.Code);
                    XlsHelper.SetRowCell(sheet, rownum, column++, xrMaster.Item);
                    XlsHelper.SetRowCell(sheet, rownum, column++, xrMaster.ItemDescription);

                    XlsHelper.SetRowCell(sheet, rownum, column++, xrMaster.ToolNumber);
                    XlsHelper.SetRowCell(sheet, rownum, column++, xrMaster.Cavities);
                    XlsHelper.SetRowCell(sheet, rownum, column++, xrMaster.Spec);
                    XlsHelper.SetRowCell(sheet, rownum, column++, xrMaster.Plus.ToString("F2"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, xrMaster.Minus.ToString("F2"));

                    XlsHelper.SetRowCell(sheet, rownum, column++, xrDetail.AveragePoint.Value.ToString("F2"));
                    XlsHelper.SetRowCell(sheet, rownum, column++, xrDetail.RangePoint.Value.ToString("F2"));

                    #region 描述

                    string description = string.Format("{0}:  {1}",
                 xrDetail.LastModifyDate.ToString("yyyy-MM-dd HH:mm"), xrDetail.Info);
                    XlsHelper.SetRowCell(sheet, rownum, column++, description);
                    #endregion


                    #region 跟踪
                    string traceDescription = string.Empty;
                    foreach (var xrTrace in xrTraces)
                    {
                        var user = userMgrE.LoadUser(xrTrace.CreateUser);
                        traceDescription += string.Format("{0}({1}):{2} \n",
                             user.Name, xrTrace.CreateDate, xrTrace.Feedback);
                    }
                    XlsHelper.SetRowCell(sheet, rownum, column++, traceDescription);
                    #endregion
                    rownum++;
                }

                sheet.ForceFormulaRecalculation = true;
                sheet.CreateFreezePane(1, 0, 1, 0);

                #endregion

                return true;
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
            return false;
        }
    }
}




#region Extend Class

namespace com.Sconit.ISI.Batch.Ext.Job
{
    [Transactional]
    public partial class SPCJob : com.Sconit.ISI.Batch.Job.SPCJob
    {
        public SPCJob()
        {
        }
    }
}

#endregion
