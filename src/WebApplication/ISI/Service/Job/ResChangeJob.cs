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

namespace com.Sconit.ISI.Service.Batch.Job
{
    [Transactional]
    public class ResChangeJob : RepJob
    {
        public IResMatrixMgrE resMatrixMgrE { get; set; }
        public ILanguageMgrE languageMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public ICriteriaMgrE criteriaMgr { get; set; }

        [Transaction(TransactionMode.Unspecified)]
        public override void Execute(JobRunContext context)
        {
            //resMatrixMgr.SendResChangeLog();

            SendResChangeLog();
        }


        public void SendResChangeLog()
        {
            var startDate = DateTime.Now.Date.AddDays(-6);
            var endDate = DateTime.Now.Date.AddDays(1);

            DetachedCriteria criteria = DetachedCriteria.For(typeof(ResMatrixLog));
            criteria.Add(Expression.Ge("CreateDate", startDate));
            criteria.Add(Expression.Lt("CreateDate", endDate));
            criteria.AddOrder(NHibernate.Expression.Order.Asc("UserCode"));
            var resMatrixLogList = criteriaMgr.FindAll<ResMatrixLog>(criteria) ?? new List<ResMatrixLog>();


            #region ���ʼ�
            string key = "ְ���������ܣ�";
            string separator = ISIConstants.EMAIL_SEPRATOR;
            StringBuilder desc = new StringBuilder();
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;ְ��������");
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;��ʼʱ�䣺");
            desc.Append(startDate.ToShortDateString());
            desc.Append(separator);
            desc.Append("&nbsp;&nbsp;&nbsp;&nbsp;����ʱ�䣺");
            desc.Append(endDate.ToShortDateString());
            #endregion

            #region ȫ����־

            SendEmail(key, ISIConstants.PERMISSION_PAGE_VALUE_RESMATRIXCHANGELOG, desc.ToString(), startDate, endDate);

            #endregion

            #region ���û���־
            if (resMatrixLogList.Count > 0)
            {
                var groupResMatrixLogList = resMatrixLogList.GroupBy(p => p.UserCode);
                foreach (var groupResMatrixLog in groupResMatrixLogList)
                {
                    var listBody = new StringBuilder();
                    GetListBody(groupResMatrixLog.ToList(), listBody);
                    SendUserMail(startDate, endDate, listBody, groupResMatrixLog.Key);
                }
            }
            #endregion
        }

        [Transaction(TransactionMode.Unspecified)]
        protected override bool FileData(string key, IWorkbook workbook, DateTime? startDate, DateTime? endDate)
        {

            bool result = false;
            DetachedCriteria criteria = DetachedCriteria.For(typeof(ResMatrixLog));
            criteria.Add(Expression.Ge("CreateDate", startDate));
            criteria.Add(Expression.Lt("CreateDate", endDate));
            criteria.AddOrder(NHibernate.Expression.Order.Asc("UserCode"));
            var resMatrixLogList = criteriaMgr.FindAll<ResMatrixLog>(criteria) ?? new List<ResMatrixLog>();
            result = ProcessResChangeLog(workbook, resMatrixLogList);

            return result;

        }


        public override void SendEmail(string key, string permissionCode, string desc, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                string mailList = uesrMgrE.FindEmailByPermission(new string[] { permissionCode });

                IList<EntityPreference> entityPreferenceList = entityPreferenceMgrE.GetEntityPreferenceOrderBySeq(new string[]{BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME,
                                                    ISIConstants.ENTITY_PREFERENCE_WEBADDRESS});
                
                log.Info(key + ",�����б�" + mailList);
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
                    content.Append("����");
                    content.Append(separator);
                    content.Append(desc);
                    content.Append(separator);
                    //content.Append(separator);
                    //content.Append("&nbsp;&nbsp;�������");

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

        private void GetListBody(IList<ResMatrixLog> resMatrixLogList, StringBuilder listBody)
        {
            var updateResMatrixLogList = resMatrixLogList.Where(p => p.Action == "Update").ToList();
            if (updateResMatrixLogList.Count() > 0)
            {
                listBody.Append("<br /><span style='font-size:14px;'>�����ְ��</span><br />");
                GetTable(updateResMatrixLogList, listBody);
            }

            var newResMatrixLogList = resMatrixLogList.Where(p => p.Action == "New").ToList();
            if (newResMatrixLogList.Count() > 0)
            {
                listBody.Append("<br /><span style='font-size:14px;'>������ְ��</span><br />");
                GetTable(newResMatrixLogList, listBody);
            }

            var deleteResMatrixLogList = resMatrixLogList.Where(p => p.Action == "New").ToList();
            if (deleteResMatrixLogList.Count() > 0)
            {
                listBody.Append("<br /><span style='font-size:14px;'>ɾ����ְ��</span><br />");
                GetTable(deleteResMatrixLogList, listBody);
            }
        }

        private void SendUserMail(DateTime startDate, DateTime endDate, StringBuilder listBody, string userCode)
        {
            var user = this.userMgrE.LoadUser(userCode);

            if (user.IsActive && !string.IsNullOrEmpty(user.Email))
            {
                string subject = "���η���ְ��仯��־";
                StringBuilder mailBody = new StringBuilder();
                string companyName = entityPreferenceMgrE.LoadEntityPreference(BusinessConstants.ENTITY_PREFERENCE_CODE_COMPANYNAME).Value;
                ISIUtil.AppendTestText(companyName, mailBody, ISIConstants.EMAIL_SEPRATOR);

                string gender = "����/Ůʿ";
                if (user.Gender == "M")
                {
                    gender = "����";
                }
                else if (user.Gender == "M")
                {
                    gender = "Ůʿ";
                }

                mailBody.Append(string.Format("<span style='font-size:13px;'>�𾴵�{0}{1}:</span><br /><br />", user.Name, gender));
                mailBody.Append("&nbsp;&nbsp;<span style='font-size:13px;'>���� " + subject + "��</span><br />");
                mailBody.Append("&nbsp;&nbsp;<span style='font-size:13px;'>������ʱ�䷶Χ: " + startDate.ToString("yyyy-MM-dd") + " �� " + endDate.ToString("yyyy-MM-dd") + "</span><br /><br /><br />");

                string webAddress = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_WEBADDRESS).Value;

                mailBody.Append("<span style='font-size:14px;'>��ϸ���</span><br />");

                mailBody.Append(listBody.ToString());

                mailBody.Append("<br /><br /><br /><br />");
                mailBody.Append("��֪Ϥ!<br /><br />");
                mailBody.Append("<span style='font-size:13px;'>��Ҫ:���ʼ�ֻ����ʾ,���������� <a href='http://" + webAddress + "'>ISIϵͳ</a> Ϊ׼.</span><br />");
                mailBody.Append("<span style='font-size:13px;'>лл����!</span><br /><br />");
                mailBody.Append("<span style='font-size:13px;'>" + companyName + "</span><br/>");
                mailBody.Append("<span style='font-size:13px;'><a href='http://" + webAddress + "'>http://" + webAddress + "</a></span>");
                //todo
                this.smtpMgrE.AsyncSend(subject, mailBody.ToString(), user.Email, string.Empty, MailPriority.Normal);
            }
        }

        private void GetTable(IList<ResMatrixLog> resMatrixLogList, StringBuilder listBody)
        {
            listBody.Append("<table cellspacing='0' cellpadding='4' rules='all' border='1' style='width:100%;border-collapse:collapse;font-size:12px;'>");
            listBody.Append("<tr nowrap style='color:#FFFFFF;background-color:#000060;font-weight:bold;line-height:150%;'>");
            listBody.Append("<th nowrap scope='col'>��ҵ��</th>");
            listBody.Append("<th nowrap scope='col'>��λ</th>");
            listBody.Append("<th nowrap scope='col'>��λ</th>");
            listBody.Append("<th nowrap scope='col'>���ȼ�</th>");
            listBody.Append("<th nowrap scope='col'>��Ա</th>");
            listBody.Append("<th nowrap scope='col'>��ʼʱ��</th>");
            listBody.Append("<th nowrap scope='col'>����ʱ��</th>");
            listBody.Append("<th nowrap scope='col'>ְ��</th>");
            listBody.Append("<th nowrap scope='col'>��־</th>");
            listBody.Append("</tr>");
            foreach (var resMatrixLog in resMatrixLogList)
            {
                listBody.Append("<tr><td>");
                listBody.Append(resMatrixLog.WorkShopCodeName);
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.OperateCodeName);
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.RoleCodeName);
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.Priority);
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.UserCodeName);
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.StartDate.ToString("yyyy-MM-dd HH:mm"));
                listBody.Append("</td><td>");
                listBody.Append(resMatrixLog.EndDate.ToString("yyyy-MM-dd HH:mm"));
                listBody.Append("</td><td style='word-break:break-all;word-wrap:break-word;white-space:pre-wrap;'>");

                listBody.Append(DiffMatchPatchHelper.DiffPrettyHtml(resMatrixLog.OldResponsibility, resMatrixLog.Responsibility));
                listBody.Append("</td><td style='word-break:break-all;word-wrap:break-word;white-space:pre-wrap;'>");
                listBody.Append(resMatrixLog.Logs);
                listBody.Append("</td></tr>");
            }
            listBody.Append("</table>");
        }

        private bool ProcessResChangeLog(IWorkbook workbook, IList<ResMatrixLog> resMatrixLogList)
        {
            try
            {
                #region ��ʼ��Sheet

                ISheet sheet = workbook.CreateSheet("ְ����");

                for (int i = 0; i <= 7; i++)
                {
                    sheet.AutoSizeColumn(i);
                    sheet.SetDefaultColumnStyle(i, cellStyle);
                }

                sheet.SetColumnWidth(0, 10 * 256);//���
                sheet.SetColumnWidth(1, 20 * 256);//��Ա
                sheet.SetColumnWidth(2, 20 * 256);//��ҵ��
                sheet.SetColumnWidth(3, 10 * 256);//��λ
                sheet.SetColumnWidth(4, 80 * 256);//ְ��
                sheet.SetColumnWidth(5, 100 * 256);//�����־
                sheet.SetColumnWidth(6, 20 * 256);//����ʱ��
                sheet.SetColumnWidth(6, 20 * 256);//�仯

                int rownum = 1;
                int column = 0;

                XlsHelper.SetRowCell(sheet, rownum, column++, "���", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "��Ա", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "��ҵ��", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "��λ", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "ְ��", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "�����־", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "����ʱ��", headStyle);
                XlsHelper.SetRowCell(sheet, rownum, column++, "�仯", headStyle);
                #endregion

                XlsHelper.SetRowCell(sheet, 0, 0, "ְ������", headStyle2);
                XlsHelper.SetRowCell(sheet, 0, 1, resMatrixLogList.Count() + " ��", headStyle2);

                #region �������
                rownum = 2;
                for (int i = 0; i < resMatrixLogList.Count(); i++)
                {
                    column = 0;
                    var resMatrix = resMatrixLogList[i];


                    XlsHelper.SetRowCell(sheet, rownum, column++, (i + 1).ToString());
                    XlsHelper.SetRowCell(sheet, rownum, column++, resMatrix.UserCode + "[" + resMatrix.UserName + "]");
                    XlsHelper.SetRowCell(sheet, rownum, column++, resMatrix.WorkShop + "[" + resMatrix.WorkShopName + "]");
                    XlsHelper.SetRowCell(sheet, rownum, column++, resMatrix.Operate + "[" + resMatrix.OperateDesc + "]");

                    XlsHelper.SetRowCell(sheet, rownum, column++, resMatrix.Responsibility);
                    XlsHelper.SetRowCell(sheet, rownum, column++, resMatrix.Logs);

                    XlsHelper.SetRowCell(sheet, rownum, column++, resMatrix.EndDate.ToShortDateString());
                    XlsHelper.SetRowCell(sheet, rownum, column++, resMatrix.Action);
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

namespace com.Sconit.ISI.Service.Ext.Batch.Job
{
    [Transactional]
    public partial class ResChangeJob : com.Sconit.ISI.Service.Batch.Job.ResChangeJob
    {
        public ResChangeJob()
        {
        }
    }
}

#endregion
