using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Castle.Services.Transaction;
using com.Sconit.ISI.Persistence;
using com.Sconit.Service.Ext.Hql;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Utility;
using com.Sconit.Entity.MasterData;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.Exception;
using com.Sconit.Service.Ext.MasterData;
using System.Linq;
using com.Sconit.ISI.Service.Util;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class SummaryMgr : SummaryBaseMgr, ISummaryMgr
    {
        public virtual string appDataFolder { get; set; }
        public IUserMgrE uesrMgrE { get; set; }
        public IAttachmentDetailMgrE attachmentDetailMgrE { get; set; }
        public IHqlMgrE hqlMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }

        public IEvaluationMgrE evaluationMgrE { get; set; }
        public ISummaryDetMgrE summaryDetMgrE { get; set; }
        public IUserSubscriptionMgrE userSubscriptionMgrE { get; set; }

        public INumberControlMgrE numberControlMgrE { get; set; }
        public ICheckupMgrE checkupMgrE { get; set; }

        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        #region Customized Methods

        [Transaction(TransactionMode.RequiresNew)]
        public void SummaryRemind1(DateTime date, int startDays, int endDays, int approveDays)
        {
            try
            {
                var evaluationList = evaluationMgrE.GetEvaluation(date);
                if (evaluationList == null || evaluationList.Count == 0) return;

                string mailTo = userSubscriptionMgrE.FindEmail(evaluationList.Select(e => e.UserCode).ToArray());

                if (string.IsNullOrEmpty(mailTo)) return;
                string subject = "月度自评提醒";
                StringBuilder body = new StringBuilder();

                body.Append("<span style='font-size:15px;'>您好:</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("<span style='font-size:15px;'>请务必" + endDays + "日前在系统提交月度自评。否则系统将关闭提交功能，造成无月度绩效加分。</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("<span style='font-size:15px;'>请注意" + approveDays + "日执委会开始考核，并且系统自动统计。</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                userSubscriptionMgrE.Remind(subject, body, mailTo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.RequiresNew)]
        public void SummaryRemind2(IList<Evaluation> evaluationList)
        {
            try
            {
                string mailTo = userSubscriptionMgrE.FindEmailByPermission(new string[] { ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE });

                if (string.IsNullOrEmpty(mailTo)) return;
                string subject = "月度自评审批提醒";
                StringBuilder body = new StringBuilder();

                body.Append("<span style='font-size:15px;'>您好:</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("<span style='font-size:15px;'>请对各干部月度自评进行审批。</span>");

                OutputNoEvaluation(evaluationList, body);

                body.Append(ISIConstants.EMAIL_SEPRATOR);
                userSubscriptionMgrE.Remind(subject, body, mailTo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        /// <summary>
        /// 通知月度自评管理员还有哪些人未提交月度自评
        /// </summary>
        /// <param name="evaluationList"></param>
        [Transaction(TransactionMode.RequiresNew)]
        public void SummaryRemind3(IList<Evaluation> evaluationList)
        {
            try
            {
                string mailTo = userSubscriptionMgrE.FindEmailByPermission(new string[] { ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN });

                if (string.IsNullOrEmpty(mailTo)) return;
                string subject = "月度自评未提交人";
                StringBuilder body = new StringBuilder();

                body.Append("<span style='font-size:15px;'>您好:</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);

                OutputNoEvaluation(evaluationList, body);

                body.Append(ISIConstants.EMAIL_SEPRATOR);
                userSubscriptionMgrE.Remind(subject, body, mailTo);
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        /// <summary>
        /// 导出发符经理
        /// </summary>
        /// <param name="evaluationList"></param>
        [Transaction(TransactionMode.RequiresNew)]
        public void SummaryRemind4(DateTime date, IList<Evaluation> evaluationList, User user)
        {
            try
            {
                string mailTo = userSubscriptionMgrE.FindEmailByPermission(new string[] { ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN });

                if (string.IsNullOrEmpty(mailTo)) return;
                string subject = "月度自评自动导出";
                StringBuilder body = new StringBuilder();

                body.Append("<span style='font-size:15px;'>您好:</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);

                OutputNoEvaluation(evaluationList, body);

                body.Append(ISIConstants.EMAIL_SEPRATOR);
                if (user != null)
                {
                    body.Append(ISIConstants.EMAIL_SEPRATOR);
                    body.Append(user.Name);
                    body.Append(ISIConstants.EMAIL_SEPRATOR);
                    body.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                    body.Append(ISIConstants.EMAIL_SEPRATOR);
                }
                string fileName = GeneratePDF(appDataFolder, date);

                userSubscriptionMgrE.Remind(subject, body, mailTo, new string[] { fileName });
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public string GeneratePDF(string appDataFolder, DateTime date)
        {
            try
            {
                IList<Summary> summaryList = this.GetSummaryByDate(date);
                if (summaryList != null && summaryList.Count > 0)
                {
                    IList<SummaryDet> summaryDetList = this.GetSummaryDet(date);
                    IList<string> fileNameList = new List<string>();

                    string path = ISIUtil.GetPath(DateTime.Now, false, true);
                    string appDataAllFolder = appDataFolder + path;
                    if (!Directory.Exists(appDataAllFolder))//判断是否存在
                    {
                        Directory.CreateDirectory(appDataAllFolder);//创建新路径
                    }

                    BaseFont bfChinese = BaseFont.CreateFont(@"C:\WINDOWS\Fonts\SIMKAI.TTF", BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED);
                    Font chTileFont1 = new Font(bfChinese, 20);
                    Font chTileFont2 = new Font(bfChinese, 15);
                    Font chFont = new Font(bfChinese, 10);
                    Font chFontBlue = new Font(bfChinese, 10);
                    chFontBlue.Color = BaseColor.BLUE;
                    Image logo = Image.GetInstance(appDataFolder + @"../Images/OEM/YFGM/logo.png");
                    logo.Alignment = Image.UNDERLYING;// ALIGN_LEFT;
                    logo.ScalePercent(50);

                    foreach (var summary in summaryList)
                    {
                        try
                        {
                            var summaryDetUserList = summaryDetList.Where(sd => sd.SummaryCode == summary.Code).ToList();
                            if (summaryDetUserList != null && summaryDetUserList.Count > 0)
                            {
                                Document document = new Document();

                                //string fileNamePDF = XlsHelper.GetTempDirectory() + summary.UserName + summary.Date.ToString("yyyy年MM月") + "月度自评(" + summary.CreateDate.ToString("yyyyMMddHHmmss") + ").pdf";
                                string fileNamePDF = appDataAllFolder + summary.UserName + summary.Date.ToString("yyyy年MM月") + "月度自评(" + summary.CreateDate.ToString("yyyyMMddHHmmss") + ").pdf";

                                PdfWriter.GetInstance(document, new FileStream(fileNamePDF, FileMode.Create));
                                document.Open();
                                try
                                {
                                    //logo.SetAbsolutePosition(0, 0);
                                    document.Add(logo);
                                    Paragraph paragraph = new Paragraph((summary.Position == ISIConstants.CODE_MASTER_USER_ISIPOSITION_PLATFORMMANAGER ? summary.Position : string.Empty) + "月度自评表", chTileFont1);

                                    paragraph.Alignment = Element.ALIGN_CENTER;
                                    document.Add(paragraph);

                                    PdfPTable table = new PdfPTable(6);
                                    table.WidthPercentage = 100;
                                    table.SplitLate = false;
                                    table.SplitRows = true;
                                    PdfPCell cell;

                                    cell = new PdfPCell(new Phrase("   "));

                                    cell.Colspan = 6;
                                    cell.BorderWidth = 0;
                                    table.AddCell(cell);

                                    cell = new PdfPCell(new Phrase("部门", chTileFont2));
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    table.AddCell(cell);
                                    if (summary.Position == ISIConstants.CODE_MASTER_USER_ISIPOSITION_PLATFORMMANAGER)
                                    {
                                        cell = new PdfPCell(new Phrase(summary.Dept2, chTileFont2));
                                    }
                                    else
                                    {
                                        cell = new PdfPCell(new Phrase(summary.Department, chTileFont2));
                                    }
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase("自评人", chTileFont2));
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase(summary.UserName, chTileFont2));
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase("日期", chTileFont2));
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    table.AddCell(cell);
                                    cell = new PdfPCell(new Phrase(summary.Date.ToString("yyyy年MM月"), chTileFont2));
                                    cell.VerticalAlignment = Element.ALIGN_MIDDLE;
                                    cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                    table.AddCell(cell);
                                    if (!string.IsNullOrEmpty(summary.Desc))
                                    {
                                        cell = new PdfPCell(new Paragraph(summary.Desc, chFont));
                                        cell.Colspan = 6;
                                        cell.BorderWidthBottom = 0;
                                        table.AddCell(cell);

                                    }
                                    foreach (var summaryDetUser in summaryDetUserList)
                                    {
                                        if (!string.IsNullOrEmpty(summaryDetUser.Subject))
                                        {
                                            cell = new PdfPCell(new Paragraph("标题：" + summaryDetUser.Subject, chFontBlue));
                                            cell.Colspan = 6;
                                            cell.BorderWidthBottom = 0;
                                            table.AddCell(cell);
                                        }
                                        cell = new PdfPCell(new Paragraph(summaryDetUser.Conment, chFont));
                                        cell.BorderWidthTop = 0;
                                        cell.Colspan = 6;
                                        table.AddCell(cell);
                                    }
                                    table.HeaderRows = 1;
                                    table.HorizontalAlignment = Element.ALIGN_LEFT;
                                    document.Add(table);
                                    fileNameList.Add(fileNamePDF);
                                }
                                catch (Exception e)
                                {
                                    log.Error(e.Message, e);
                                }
                                finally
                                {
                                    document.Close();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            log.Error(e.Message, e);
                        }
                    }
                    if (fileNameList != null && fileNameList.Count > 0)
                    {
                        string file = CreateAttachmentDetail(date, fileNameList, path, appDataAllFolder);

                        return file;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error(e.Message, e);
            }

            return string.Empty;
        }
        [Transaction(TransactionMode.Requires)]
        private string CreateAttachmentDetail(DateTime date, IList<string> fileNameList, string path, string appDataAllFolder)
        {
            string fileName = "月度自评" + date.ToString("yyyy年MM月") + "(" + DateTime.Now.ToString("yyyyMMddHHmmss") + ")" + ".zip";

            string file = XlsHelper.WriteToLocalZip(fileName, appDataAllFolder, fileNameList);
            FileInfo fileInfo = new FileInfo(file);
            AttachmentDetail attachment = new AttachmentDetail();
            attachment.TaskCode = path;
            attachment.Size = decimal.Parse((file.Length / 1024.0).ToString());
            attachment.CreateUser = this.uesrMgrE.GetMonitorUser().Code;
            attachment.CreateUserNm = this.uesrMgrE.GetMonitorUser().Name;
            attachment.CreateDate = DateTime.Now;
            attachment.FileName = fileInfo.Name;
            attachment.FileExtension = fileInfo.Extension;
            attachment.ContentType = "application/x-zip-compressed";
            attachment.ModuleType = ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN;
            attachment.Path = path + fileInfo.Name;
            this.attachmentDetailMgrE.CreateAttachmentDetail(attachment);
            return file;
        }
        public void OutputNoEvaluation(IList<Evaluation> evaluationList, StringBuilder body)
        {
            if (evaluationList != null && evaluationList.Count > 0)
            {
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("<span style='font-size:15px;'>未提交月度自评的干部如下：</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                for (int i = 0; i < evaluationList.Count; i++)
                {
                    var evaluation = evaluationList[i];
                    if (i != 0)
                    {
                        body.Append("、");
                    }
                    body.Append(evaluation.UserName);
                }
            }
            else
            {
                body.Append(ISIConstants.EMAIL_SEPRATOR);
                body.Append("<span style='font-size:15px;'>所有干部已全部提交月度自评。</span>");
                body.Append(ISIConstants.EMAIL_SEPRATOR);
            }
        }
        //TODO: Add other methods here.
        [Transaction(TransactionMode.Unspecified)]
        public Summary LoadSummary(string userCode, string date)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Summary));
            criteria.Add(Expression.Not(Expression.Eq("Status", ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CANCEL)));
            criteria.Add(Expression.Eq("UserCode", userCode));
            criteria.Add(Expression.Eq("Date", DateTime.Parse(date)));

            var summaryList = this.criteriaMgrE.FindAll<Summary>(criteria);
            if (summaryList != null && summaryList.Count > 0)
            {
                return summaryList[0];
            }
            return null;
        }

        [Transaction(TransactionMode.Unspecified)]
        public Summary LoadNextSummary(string Code)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Summary));
            criteria.Add(Expression.Or(Expression.Eq("Status", ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT), Expression.Eq("Status", ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_INAPPROVE)));
            criteria.Add(Expression.Not(Expression.Eq("Code", Code)));

            var summaryList = this.criteriaMgrE.FindAll<Summary>(criteria);
            if (summaryList != null && summaryList.Count > 0)
            {
                return summaryList[0];
            }
            return null;
        }

        [Transaction(TransactionMode.Requires)]
        public void CloseSummary(string summaryCode, User user)
        {
            try
            {
                var summary = this.LoadSummary(summaryCode);
                if (summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_APPROVAL)
                {
                    throw new BusinessErrorException("ISI.Summary.Error.StatusErrorWhenClose", summary.Status, summary.Code);
                }

                DateTime now = DateTime.Now;
                summary.Status = ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CLOSE;
                summary.CloseDate = now;
                summary.CloseUser = user.Code;
                summary.CloseUserNm = user.Name;
                summary.LastModifyDate = now;
                summary.LastModifyUser = user.Code;
                summary.LastModifyUserNm = user.Name;

                this.UpdateSummary(summary);

                if (summary.IsCheckup)
                {
                    DetachedCriteria criteria = DetachedCriteria.For(typeof(Checkup));
                    //criteria.Add(Expression.Eq("CheckupProject", ISIConstants.CODE_MASTER_SUMMARY_CHECKUPPROJECT));
                    criteria.Add(Expression.Eq("Status", ISIConstants.CODE_MASTER_ISI_CHECKUP_STATUS_SUBMIT));
                    criteria.Add(Expression.Eq("SummaryCode", summary.Code));
                    var checkupList = this.criteriaMgrE.FindAll<Checkup>(criteria);
                    if (checkupList != null && checkupList.Count > 0)
                    {
                        foreach (var checkup in checkupList)
                        {
                            this.checkupMgrE.CancelCheckup(checkup.Id, user);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("SummaryCode=" + summaryCode + ",e=" + e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CancelSummary(string summaryCode, User user)
        {
            try
            {
                var summary = this.LoadSummary(summaryCode);
                if (summary.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE && summary.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT && summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_INAPPROVE)
                {
                    throw new BusinessErrorException("ISI.Summary.Error.StatusErrorWhenCancel", summary.Status, summary.Code);
                }

                DateTime now = DateTime.Now;
                summary.Status = ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CANCEL;
                summary.CancelDate = now;
                summary.CancelUser = user.Code;
                summary.CancelUserNm = user.Name;
                summary.LastModifyDate = now;
                summary.LastModifyUser = user.Code;
                summary.LastModifyUserNm = user.Name;

                this.UpdateSummary(summary);

            }
            catch (Exception e)
            {
                log.Error("SummaryCode=" + summaryCode + ",e=" + e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void UpdateSummary(Summary summary, IList<SummaryDet> summaryDetList, User user)
        {
            try
            {
                DateTime now = DateTime.Now;
                summary.Poor = summaryDetList.Where(s => s.Type == ISIConstants.CODE_MASTER_SUMMARY_TYPE_POOR).Count();
                summary.Excellent = summaryDetList.Where(s => s.Type == ISIConstants.CODE_MASTER_SUMMARY_TYPE_EXCELLENT).Count();
                summary.Moderate = summaryDetList.Where(s => s.Type == ISIConstants.CODE_MASTER_SUMMARY_TYPE_MODERATE).Count();
                summary.LastModifyDate = now;
                summary.LastModifyUser = user.Code;
                summary.LastModifyUserNm = user.Name;
                summary.Count = summaryDetList.Count;
                this.UpdateSummary(summary);

                summaryDetList = summaryDetList.OrderBy(sd => sd.Seq).ToList();
                for (int i = 0; i < summaryDetList.Count; i++)
                {
                    var summaryDet = summaryDetList[i];
                    summaryDet.Seq = i + 1;

                    if (summaryDet.Id == 0)
                    {
                        summaryDet.SummaryCode = summary.Code;
                        this.summaryDetMgrE.CreateSummaryDet(summaryDet);
                    }
                    else
                    {
                        this.summaryDetMgrE.UpdateSummaryDet(summaryDet);
                    }
                }

                if (summary.IsAutoRelease)
                {
                    this.SubmitSummary(summary, summaryDetList, user);
                }
            }
            catch (Exception e)
            {
                log.Error("Code=" + summary.Code + ",e=" + e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void CreateSummary(Summary summary, IList<SummaryDet> summaryDetList, User user)
        {
            try
            {
                DateTime now = DateTime.Now;

                summary.UserCode = user.Code;
                summary.UserName = user.Name;
                summary.Department = user.Department;
                summary.Dept2 = user.Dept2;
                summary.JobNo = user.JobNo;
                summary.Position = user.Position;
                summary.Company = user.Company;

                summary.Code = numberControlMgrE.GenerateNumber(summary.Date.ToString("yyyyMM") + summary.UserCode.ToUpper(), 2);

                summary.Status = ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE;
                summary.CreateDate = now;
                summary.CreateUser = user.Code;
                summary.CreateUserNm = user.Name;
                summary.LastModifyDate = now;
                summary.LastModifyUser = user.Code;
                summary.LastModifyUserNm = user.Name;
                summary.Count = summaryDetList.Count;
                this.CreateSummary(summary);
                summaryDetList = summaryDetList.OrderBy(sd => sd.Seq).ToList();
                for (int i = 0; i < summaryDetList.Count; i++)
                {
                    var summaryDet = summaryDetList[i];
                    summaryDet.Seq = i + 1;
                    summaryDet.SummaryCode = summary.Code;
                    this.summaryDetMgrE.CreateSummaryDet(summaryDet);
                }

                if (summary.IsAutoRelease)
                {
                    this.SubmitSummary(summary, summaryDetList, user);
                }
            }
            catch (Exception e)
            {
                log.Error("Code=" + summary.Code + ",e=" + e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public string ApproveSummary2(Summary summary, IList<SummaryDet> summaryDetList, User user)
        {
            this.ApproveSummary(summary, summaryDetList, user);

            var nextSummary = LoadNextSummary(summary.Code);

            if (nextSummary == null)
            {
                return null;
            }
            else
            {
                return nextSummary.Code;
            }
        }

        /// <summary>
        /// 用于查看自动变为审批中
        /// </summary>
        /// <param name="code"></param>
        /// <param name="user"></param>
        [Transaction(TransactionMode.Requires)]
        public void ApproveSummary(string code, User user)
        {
            try
            {
                if (user.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE) && user.Code != "su" && user.Code != "tiansu")
                {
                    var summary = this.LoadSummary(code);
                    if (summary.CreateUser != user.Code && summary.SubmitUser != user.Code && summary.Status == ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT)
                    {
                        summary.Status = ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_INAPPROVE;
                        summary.InApproveDate = DateTime.Now;
                        summary.InApproveUser = user.Code;
                        summary.InApproveUserNm = user.Name;
                        this.UpdateSummary(summary);
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("e=" + e.Message, e);
            }
        }

        [Transaction(TransactionMode.Requires)]
        public void ApproveSummary(Summary summary, IList<SummaryDet> summaryDetList, User user)
        {
            if (summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT && summary.Status != ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_INAPPROVE)
            {
                throw new BusinessErrorException("ISI.Summary.Error.StatusErrorWhenApprove", summary.Status, summary.Code);
            }

            DateTime now = DateTime.Now;
            summary.Status = ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_APPROVAL;
            summary.ApproveDate = now;
            summary.ApproveUser = user.Code;
            summary.ApproveUserNm = user.Name;
            summary.LastModifyDate = now;
            summary.LastModifyUser = user.Code;
            summary.LastModifyUserNm = user.Name;
            summary.Count = summaryDetList.Count;

            if (!summary.IsCheckup || summary.Diff == 0)
            {
                summary.Status = ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CLOSE;
                summary.CloseDate = now;
                summary.CloseUser = user.Code;
                summary.CloseUserNm = user.Name;
            }

            /*
            int qty = summaryDetList.Where(s => s.Type == ISIConstants.CODE_MASTER_SUMMARY_TYPE_EXCELLENT || s.Type == ISIConstants.CODE_MASTER_SUMMARY_TYPE_MODERATE).Count();
            if (summary.Qty < qty)
            {
                summary.Qty = qty;
            }
            */

            summary.Poor = summaryDetList.Where(s => s.Type == ISIConstants.CODE_MASTER_SUMMARY_TYPE_POOR).Count();
            summary.Excellent = summaryDetList.Where(s => s.Type == ISIConstants.CODE_MASTER_SUMMARY_TYPE_EXCELLENT).Count();
            summary.Moderate = summaryDetList.Where(s => s.Type == ISIConstants.CODE_MASTER_SUMMARY_TYPE_MODERATE).Count();

            this.UpdateSummary(summary);

            foreach (var summaryDet in summaryDetList)
            {
                this.summaryDetMgrE.UpdateSummaryDet(summaryDet);
            }

            this.checkupMgrE.SubmitCheckup(summary, user, now);
        }

        [Transaction(TransactionMode.Requires)]
        public void SubmitSummary(Summary summary, IList<SummaryDet> summaryDetList, User user)
        {
            try
            {
                if (summary.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE)
                {
                    throw new BusinessErrorException("ISI.Summary.Error.StatusErrorWhenSubmit", summary.Status, summary.Code);
                }

                DateTime now = DateTime.Now;
                summary.Status = ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_SUBMIT;
                summary.SubmitDate = now;
                summary.SubmitUser = user.Code;
                summary.SubmitUserNm = user.Name;
                summary.LastModifyDate = now;
                summary.LastModifyUser = user.Code;
                summary.LastModifyUserNm = user.Name;
                summary.Count = summaryDetList.Count;
                this.UpdateSummary(summary);

                summaryDetList = summaryDetList.OrderBy(sd => sd.Seq).ToList();
                for (int i = 0; i < summaryDetList.Count; i++)
                {
                    var summaryDet = summaryDetList[i];
                    summaryDet.Seq = i + 1;
                    if (summaryDet.Id <= 0)
                    {
                        summaryDet.SummaryCode = summary.Code;
                        this.summaryDetMgrE.CreateSummaryDet(summaryDet);
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(summaryDet.Subject) && string.IsNullOrEmpty(summaryDet.Conment))
                        {
                            this.summaryDetMgrE.DeleteSummaryDet(summaryDet.Id);
                        }
                        else
                        {
                            this.summaryDetMgrE.UpdateSummaryDet(summaryDet);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("SummaryCode=" + summary.Code + ",e=" + e.Message, e);
            }
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Evaluation> GetNoSummary(string date, User user, bool isHasPermission)
        {
            try
            {
                StringBuilder hql = new StringBuilder();
                hql.Append("from Evaluation e ");
                hql.Append("where e.IsActive =1 and ");
                if (isHasPermission && !user.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) && !user.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE))
                {
                    hql.Append("e.UserCode = '" + user.Code + "' and ");
                }
                hql.Append("e.UserCode not in  ");
                hql.Append("        (select s.UserCode from Summary s  ");
                hql.Append("            where s.Date ='" + DateTime.Parse(date) + "'  ");
                if (isHasPermission && !user.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) && !user.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE))
                {
                    hql.Append("        and  s.UserCode = '" + user.Code + "' ");
                }
                hql.Append("            and s.Status != '" + ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CANCEL + "' and s.Status != '" + ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CREATE + "'  ");
                hql.Append("            ) ");
                hql.Append("order by e.UserCode asc ");
                return hqlMgrE.FindAll<Evaluation>(hql.ToString());
            }
            catch (Exception e)
            {
                log.Error("user=" + user.CodeName + ",date=" + date + ",e=" + e.Message, e);
            }
            return null;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Summary> GetSummary(string userCode, string date)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Summary));
            criteria.Add(Expression.Eq("UserCode", userCode));
            //criteria.Add(Expression.Lt("Date", DateTime.Parse(date)));
            criteria.Add(Expression.Gt("Date", DateTime.Parse(date).AddMonths(-6)));
            criteria.AddOrder(Order.Desc("Date"));
            criteria.AddOrder(Order.Desc("Code"));
            var summaryList = this.criteriaMgrE.FindAll<Summary>(criteria);
            return summaryList;
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<Summary> GetSummaryByDate(DateTime date)
        {
            return this.GetSummaryByDate(string.Empty, date);
        }


        [Transaction(TransactionMode.Unspecified)]
        public IList<Summary> GetSummaryByDate(string userCode, DateTime date)
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(Summary));
            if (!string.IsNullOrEmpty(userCode))
            {
                criteria.Add(Expression.Eq("UserCode", userCode));
            }
            criteria.Add(Expression.Not(Expression.Eq("Status", ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CANCEL)));
            criteria.Add(Expression.Eq("Date", date));
            criteria.AddOrder(Order.Desc("Date"));
            criteria.AddOrder(Order.Desc("Code"));
            var summaryList = this.criteriaMgrE.FindAll<Summary>(criteria);
            return summaryList;
        }
        [Transaction(TransactionMode.Unspecified)]
        public IList<SummaryDet> GetSummaryDet(DateTime date)
        {
            return GetSummaryDet(string.Empty, date.ToString("yyyy-MM-dd"));
        }

        [Transaction(TransactionMode.Unspecified)]
        public IList<SummaryDet> GetSummaryDet(string userCode, string date)
        {
            StringBuilder hql = new StringBuilder("from SummaryDet sd where sd.SummaryCode in (select s.Code from Summary s where s.Status!='" + ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CANCEL + "' ");
            if (!string.IsNullOrEmpty(userCode))
            {
                hql.Append("and s.UserCode='" + userCode + "' ");
            }
            hql.Append("and Date= '" + date + "') order by sd.Id asc ");
            return hqlMgrE.FindAll<SummaryDet>();
        }


        [Transaction(TransactionMode.Unspecified)]
        public Summary TransferEvaluation2Summary(User user)
        {
            Summary summary = new Summary();
            if (user != null)
            {
                var evaluation = evaluationMgrE.LoadEvaluation(user.Code);
                if (evaluation != null)
                {
                    CloneHelper.CopyProperty(evaluation, summary);
                }
            }

            CloneHelper.CopyProperty(user, summary);

            return summary;
        }

        #endregion Customized Methods
    }
}


#region Extend Class

namespace com.Sconit.ISI.Service.Ext.Impl
{
    [Transactional]
    public partial class SummaryMgrE : com.Sconit.ISI.Service.Impl.SummaryMgr, ISummaryMgrE
    {
    }
}

#endregion Extend Class