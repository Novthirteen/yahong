using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Persistence;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.ISI.Service.Util;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class CheckupMgr : CheckupBaseMgr, ICheckupMgr
    {
        public IUserSubscriptionMgrE userSubscriptionMgrE { get; set; }
        public ISummaryDao summaryDao { get; set; }
        public IEntityPreferenceMgrE entityPreferenceMgrE { get; set; }
        public ITaskMgrE taskMgrE { get; set; }
        public IUserMgrE userMgrE { get; set; }
        public ICheckupProjectMgrE checkupProjectMgrE { get; set; }
        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        #region Customized Methods

        [Transaction(TransactionMode.Requires)]
        public Checkup CreateCheckup(string type, string checkupProjectCode, DateTime checkupDate, string[] userCodes, string content, decimal? amount, bool isAutoRelease, string auditInstructions, User user)
        {
            Checkup checkupRS = null;
            try
            {

                if (userCodes != null && userCodes.Length > 0)
                {
                    foreach (string userCode in userCodes)
                    {
                        User checkupUser = userMgrE.CheckAndLoadUser(userCode);
                        Checkup checkup = new Checkup();

                        DateTime now = DateTime.Now;
                        checkup.CheckupDate = checkupDate;
                        checkup.CheckupUser = checkupUser.Code;
                        checkup.Department = checkupUser.Department;
                        checkup.Dept2 = checkupUser.Dept2;
                        checkup.JobNo = checkupUser.JobNo;
                        checkup.CheckupUserNm = checkupUser.Name;
                        checkup.Status = ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CREATE;
                        checkup.Type = type;
                        checkup.CheckupProject = checkupProjectMgrE.LoadCheckupProject(checkupProjectCode);
                        checkup.CreateDate = now;
                        checkup.CreateUser = user.Code;
                        checkup.CreateUserNm = user.Name;

                        if (isAutoRelease || !string.IsNullOrEmpty(auditInstructions))
                        {
                            checkup.Status = ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT;
                            checkup.SubmitDate = now;
                            checkup.SubmitUser = user.Code;
                            checkup.SubmitUserNm = user.Name;
                        }

                        if (!string.IsNullOrEmpty(auditInstructions))
                        {
                            checkup.Status = ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL;
                            checkup.AuditInstructions = auditInstructions;
                            checkup.ApprovalDate = now;
                            checkup.ApprovalUser = user.Code;
                            checkup.ApprovalUserNm = user.Name;
                        }

                        checkup.Content = content;
                        checkup.Amount = amount;

                        checkup.LastModifyDate = now;
                        checkup.LastModifyUser = user.Code;
                        checkup.LastModifyUserNm = user.Name;
                        this.CreateCheckup(checkup);
                        checkupRS = checkup;
                        //����֪ͨ
                        if (isAutoRelease || !string.IsNullOrEmpty(auditInstructions))
                        {
                            Remind(checkup);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("e=" + e.Message, e);
            }
            return checkupRS;
        }

        [Transaction(TransactionMode.Requires)]
        public void SubmitCheckup(int id, decimal? amount, string content, User user)
        {
            Checkup checkup = this.LoadCheckup(id);
            try
            {
                if (checkup.Status != ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CREATE)
                {
                    throw new BusinessErrorException("ISI.Error.StatusErrorWhenSubmit", checkup.Status);
                }

                DateTime now = DateTime.Now;
                checkup.Status = ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT;
                checkup.Content = content;
                checkup.Amount = amount;
                checkup.SubmitDate = now;
                checkup.SubmitUser = user.Code;
                checkup.SubmitUserNm = user.Name;
                checkup.LastModifyDate = now;
                checkup.LastModifyUser = user.Code;
                checkup.LastModifyUserNm = user.Name;
                this.UpdateCheckup(checkup);

                //����֪ͨ
                Remind(checkup);
            }
            catch (Exception e)
            {
                log.Error("Id=" + checkup.Id + ",e=" + e.Message, e);
            }
        }


        [Transaction(TransactionMode.Requires)]
        public void SubmitCheckup(Summary summary, User user, DateTime effdate)
        {
            try
            {
                StringBuilder content = new StringBuilder("�ύ�" + summary.Count + " ���׼�" + summary.StandardQty);
                content.Append(" ���׼�" + summary.Qty + " ��");
                if (summary.Excellent != 0)
                {
                    content.Append("���ţ�" + summary.Excellent + " ��");
                }
                if (summary.Moderate != 0)
                {
                    content.Append("������" + summary.Moderate + " ��");
                }
                if (summary.Poor != 0)
                {
                    content.Append("���" + summary.Poor + " ��");
                }

                if (summary.Diff > 0)
                {
                    content.Append("�����ڱ�׼��" + summary.Diff + " ��");
                }
                else if (summary.Diff < 0)
                {
                    content.Append("�����ڱ�׼��" + -summary.Diff + " ��");
                }

                //�Զ�����
                if (summary.IsCheckup && summary.Diff != 0)
                {
                    Checkup checkup = new Checkup();
                    checkup.CheckupProject = this.checkupProjectMgrE.LoadCheckupProject(ISIConstants.CODE_MASTER_SUMMARY_CHECKUPPROJECT);

                    checkup.Amount = summary.CheckupAmount;
                    checkup.LastModifyDate = effdate;
                    checkup.LastModifyUser = user.Code;
                    checkup.LastModifyUserNm = user.Name;
                    checkup.CheckupDate = effdate;
                    checkup.CreateDate = effdate;
                    checkup.CreateUser = user.Code;
                    checkup.CreateUserNm = user.Name;
                    checkup.SubmitDate = effdate;
                    checkup.SubmitUser = user.Code;
                    checkup.SubmitUserNm = user.Name;
                    checkup.Type = ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECT_TYPE_CADRE;
                    checkup.SummaryCode = summary.Code;
                    checkup.Department = summary.Company;
                    checkup.Dept2 = summary.Dept2;
                    checkup.JobNo = summary.JobNo;
                    checkup.CheckupUser = summary.CreateUser;
                    checkup.CheckupUserNm = summary.CreateUserNm;
                    checkup.Status = ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT;

                    checkup.Content = content.ToString();

                    this.CreateCheckup(checkup);

                    //����֪ͨ
                    Remind(checkup);
                }
                else
                {
                    //�ʼ�֪ͨ
                    string mailTo = userSubscriptionMgrE.FindEmail(new string[] { summary.UserCode });
                    StringBuilder body = new StringBuilder();
                    body.Append("<span style='font-size:15px;'>����!</span>");
                    body.Append(ISIConstants.EMAIL_SEPRATOR);
                    body.Append(ISIConstants.EMAIL_SEPRATOR);
                    body.Append("<span style='font-size:15px;'>������ţ�" + summary.Code + "</span>");
                    body.Append(ISIConstants.EMAIL_SEPRATOR);
                    body.Append("<span style='font-size:15px;'>" + content.ToString() + "</span>");
                    body.Append(ISIConstants.EMAIL_SEPRATOR);
                    if (summary.ApproveDate.HasValue)
                    {
                        body.Append("<span style='font-size:15px;'>�����ˣ�" + summary.ApproveUserNm + "</span>");
                        body.Append(ISIConstants.EMAIL_SEPRATOR);
                        body.Append("<span style='font-size:15px;'>����ʱ�䣺" + summary.ApproveDate.Value.ToString("yyyy-MM-dd HH:mm") + "</span>");
                    }
                    this.userSubscriptionMgrE.Remind("�¶�����������", body, mailTo);
                }
            }
            catch (Exception e)
            {
                log.Error("e=" + e.Message, e);
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        private void Remind(Checkup checkup)
        {
            StringBuilder users = new StringBuilder();
            EntityPreference isRemind = this.entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_CHECKUPSUBMITREMIND);

            if (isRemind != null && isRemind.Value.ToUpper() == "TRUE"
                    && !string.IsNullOrEmpty(checkup.CheckupUser))
            {
                users.Append(checkup.CheckupUser);
            }
            if (checkup.Type == ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECT_TYPE_EMPLOYEE)
            {
                string employeeCheckupCC = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_EMPLOYEECHECKUPCC).Value;
                if (!string.IsNullOrEmpty(employeeCheckupCC))
                {
                    users.Append(ISIConstants.ISI_USER_SEPRATOR + employeeCheckupCC);
                }
            }
            else if (checkup.Type == ISIConstants.CODE_MASTER_ISI_CHECKUPPROJECT_TYPE_CADRE)
            {
                string cadreCheckupCC = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_CADRECHECKUPCC).Value;
                if (!string.IsNullOrEmpty(cadreCheckupCC))
                {
                    users.Append(ISIConstants.ISI_USER_SEPRATOR + cadreCheckupCC);
                }
            }
            if (users.Length > 0)
            {
                string[] userCodeArray = users.ToString().Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries);
                string mailTo = userSubscriptionMgrE.FindEmail(userCodeArray);

                string subject = string.Empty;

                StringBuilder body = new StringBuilder();

                body.Append("<span style='font-size:15px;'>����!</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                if (checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CANCEL)
                {
                    subject = "�����ѳ���";
                    body.Append("&nbsp;&nbsp;<span style='font-size:15px;'>" + checkup.CancelUserNm + "&nbsp;��&nbsp;" + checkup.CancelDate.Value.ToString("yyyy-MM-dd HH:mm") + "&nbsp;�����˶����Ŀ���</span>");
                }
                else if (checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL)
                {
                    subject = "��������׼";
                    body.Append("&nbsp;&nbsp;<span style='font-size:15px;'>" + checkup.ApprovalUserNm + "&nbsp;��׼��&nbsp;" + checkup.SubmitUserNm + "&nbsp;��&nbsp;" + checkup.SubmitDate.Value.ToString("yyyy-MM-dd HH:mm") + "&nbsp;�����Ŀ���</span>");
                    body.Append(ISIConstants.EMAIL_SEPRATOR);
                    body.Append("<span style='font-size:15px;'>��ʾ��" + checkup.AuditInstructions + "</span>");
                }
                else
                {
                    subject = "��������";
                    body.Append("&nbsp;&nbsp;<span style='font-size:15px;'>" + checkup.SubmitUserNm + "&nbsp;��&nbsp;" + checkup.SubmitDate.Value.ToString("yyyy-MM-dd HH:mm") + "&nbsp;���������˿���</span>");
                }
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("<span style='font-size:15px;'>������Ŀ: " + checkup.CheckupProject.Name + "</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("<span style='font-size:15px;'>���˽��: " + (checkup.Amount.HasValue ? checkup.Amount.Value.ToString("0.## Ԫ") : string.Empty) + "</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                if (!string.IsNullOrEmpty(checkup.SummaryCode))
                {
                    body.Append("<span style='font-size:15px;'>�������: " + checkup.SummaryCode + "</span>");
                    body.Append(ISIConstants.EMAIL_SEPRATOR);
                }
                body.Append("<span style='font-size:15px;'>��������: " + "</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                if (!string.IsNullOrEmpty(checkup.Content))
                {
                    body.Append("<p style='font-size:15px;'>" + checkup.Content.Replace(ISIConstants.TEXT_SEPRATOR, ISIConstants.EMAIL_SEPRATOR).Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") + "</p>");
                }
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                if (checkup.Status != ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CANCEL && checkup.Status != ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL)
                {
                    body.Append("<span style='font-size:15px;'>��ע: �������˻�δ��׼������״̬�μ�ISIϵͳ��</span>");
                }
                this.userSubscriptionMgrE.Remind(subject, body, mailTo);
            }
        }
        [Transaction(TransactionMode.Requires)]
        public void ApproveCheckup(IList<Checkup> checkupList, User user)
        {
            foreach (Checkup c in checkupList)
            {
                this.ApproveCheckup(c.Id, c.Amount, c.AuditInstructions, user);
            }
        }
        [Transaction(TransactionMode.Requires)]
        public void ApproveCheckup(int id, decimal? amount, string auditInstructions, User user)
        {
            Checkup checkup = this.LoadCheckup(id);
            try
            {
                if (checkup.Status != ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT
                        && checkup.Status != ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL)
                {
                    throw new BusinessErrorException("ISI.Error.StatusErrorWhenApproval", checkup.Status);
                }

                if (checkup.Status == ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL
                        && ((checkup.Amount.HasValue && checkup.Amount.Value == amount) || (!checkup.Amount.HasValue && amount == 0))
                        && checkup.AuditInstructions == auditInstructions)
                {
                    return;
                }

                DateTime now = DateTime.Now;
                checkup.Status = ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_APPROVAL;
                checkup.AuditInstructions = auditInstructions;
                checkup.ApprovalDate = now;
                checkup.ApprovalUser = user.Code;
                checkup.ApprovalUserNm = user.Name;
                checkup.Amount = amount;

                checkup.LastModifyDate = now;
                checkup.LastModifyUser = user.Code;
                checkup.LastModifyUserNm = user.Name;
                this.UpdateCheckup(checkup);

                Remind(checkup);

                CloseSummary(auditInstructions, user, checkup, now);
            }
            catch (Exception e)
            {
                log.Error("Id=" + checkup.Id + ",e=" + e.Message, e);
            }
        }

        private void CloseSummary(string auditInstructions, User user, Checkup checkup, DateTime now)
        {
            if (!string.IsNullOrEmpty(checkup.SummaryCode))
            {
                var summary = summaryDao.LoadSummary(checkup.SummaryCode);
                if (summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_APPROVAL)
                {
                    summary.Status = ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CLOSE;
                    summary.CloseDate = now;
                    summary.CloseUser = user.Code;
                    summary.CloseUserNm = user.Name;
                    summary.UltimatelyDesc = auditInstructions;

                    if (checkup.Status != ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CANCEL)
                    {
                        summary.UltimatelyAmount = checkup.Amount;
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(auditInstructions))
                        {
                            summary.UltimatelyDesc = user.Name + " ��ȡ������";
                        }
                    }
                    summary.UltimatelyDate = now;
                    summary.UltimatelyApproveUser = user.Code;
                    summary.UltimatelyApproveUserNm = user.Name;

                    summary.LastModifyDate = now;
                    summary.LastModifyUser = user.Code;
                    summary.LastModifyUserNm = user.Name;
                    summaryDao.UpdateSummary(summary);
                }
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelCheckup(int id, User user)
        {
            Checkup checkup = this.CheckAndLoadCheckup(id);

            if (checkup.Status != ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CREATE
                        && checkup.Status != ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE)
            {
                DateTime nowDate = DateTime.Now;
                checkup.Status = ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CANCEL;
                checkup.CancelDate = nowDate;
                checkup.CancelUser = user.Code;
                checkup.CancelUserNm = user.Name;
                checkup.LastModifyDate = nowDate;
                checkup.LastModifyUser = user.Code;
                checkup.LastModifyUserNm = user.Name;
                this.UpdateCheckup(checkup);

                CloseSummary(checkup.AuditInstructions, user, checkup, nowDate);

                Remind(checkup);
            }
            else
            {
                throw new BusinessErrorException("ISI.Error.StatusErrorWhenCancel", checkup.Status);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseCheckup(IList<Checkup> checkupList, User user)
        {
            foreach (Checkup c in checkupList)
            {
                this.CloseCheckup(c.Id, user);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseCheckup(int id, User user)
        {
            Checkup checkup = this.CheckAndLoadCheckup(id);

            if (checkup.Status != ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CREATE
                        && checkup.Status != ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE)
            {
                DateTime nowDate = DateTime.Now;
                checkup.Status = ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_CLOSE;
                checkup.CloseDate = nowDate;
                checkup.CloseUser = user.Code;
                checkup.CloseUserNm = user.Name;
                checkup.LastModifyDate = nowDate;
                checkup.LastModifyUser = user.Code;
                checkup.LastModifyUserNm = user.Name;
                this.UpdateCheckup(checkup);
            }
            else
            {
                throw new BusinessErrorException("ISI.Error.StatusErrorWhenClose", checkup.Status);
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        public Checkup CheckAndLoadCheckup(int id)
        {
            Checkup checkup = this.LoadCheckup(id);
            if (checkup != null)
            {
                return checkup;
            }
            else
            {
                throw new BusinessErrorException("ISI.Error.CheckupIdNotExist");
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void Publish(User user)
        {
            try
            {
                //string users = entityPreferenceMgrE.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_CHECKUPREMIND).Value;

                string mailTo = userSubscriptionMgrE.FindEmailByPermission(new string[] { ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_RECEIVEPUBLISHCHECKUP });

                if (string.IsNullOrEmpty(mailTo)) return;
                string subject = "�¶ȿ����Ѿ�����";
                StringBuilder body = new StringBuilder();
                body.Append("<span style='font-size:15px;'>����:</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("&nbsp;&nbsp;<span style='font-size:15px;'>");
                body.Append(user.Name + " �Ѿ�������ϡ�</span>");

                userSubscriptionMgrE.Remind(subject, body, mailTo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }


        [Transaction(TransactionMode.Unspecified)]
        public void CloseRemind(User user)
        {
            try
            {
                string mailTo = userSubscriptionMgrE.FindEmailByPermission(new string[] { ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_RECEIVECLOSEREMINDCHECKUP });

                if (string.IsNullOrEmpty(mailTo)) return;
                string subject = "�¶ȼ�Ч���˽�������";
                StringBuilder body = new StringBuilder();

                body.Append("<span style='font-size:15px;'>����:</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("&nbsp;&nbsp;<span style='font-size:15px;'>");
                body.Append("�¶ȼ�Ч�����Ѿ����������Խ���ISIϵͳ��ѯ���˽�����</span>");

                userSubscriptionMgrE.Remind(subject, body, mailTo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void CheckupApproveRemind()
        {
            try
            {
                string mailTo = userSubscriptionMgrE.FindEmailByPermission(new string[] { ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_APPROVEREMINDCHECKUP });

                if (string.IsNullOrEmpty(mailTo)) return;
                string subject = "�¶ȿ�����������";
                StringBuilder body = new StringBuilder();

                body.Append("<span style='font-size:15px;'>����:</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("&nbsp;&nbsp;<span style='font-size:15px;'>");
                body.Append("ISIϵͳ������������������</span>");

                userSubscriptionMgrE.Remind(subject, body, mailTo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void CheckupRemind()
        {
            try
            {
                string mailTo = this.userSubscriptionMgrE.FindEmailByPermission(new string[] { ISIConstants.PERMISSION_PAGE_ISI_CHECKUP_VALUE_REMINDCHECKUP });

                if (string.IsNullOrEmpty(mailTo)) return;
                string subject = "�¶ȿ��˿�ʼ����";
                StringBuilder body = new StringBuilder();

                body.Append("<span style='font-size:15px;'>����:</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("&nbsp;&nbsp;<span style='font-size:15px;'>");
                body.Append("�뾡���ύ������Ϣ,����5�Ž�����</span>");

                userSubscriptionMgrE.Remind(subject, body, mailTo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }
        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class CheckupMgrE : com.Sconit.ISI.Service.Impl.CheckupMgr, ICheckupMgrE
    {
    }
}

#endregion Extend Class