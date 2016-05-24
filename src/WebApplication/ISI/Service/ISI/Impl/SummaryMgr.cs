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

//TODO: Add other using statements here.

namespace com.Sconit.ISI.Service.Impl
{
    [Transactional]
    public class SummaryMgr : SummaryBaseMgr, ISummaryMgr
    {
        public IHqlMgrE hqlMgrE { get; set; }
        public ICriteriaMgrE criteriaMgrE { get; set; }

        public IEvaluationMgrE evaluationMgrE { get; set; }
        public ISummaryDetMgrE summaryDetMgrE { get; set; }

        public INumberControlMgrE numberControlMgrE { get; set; }
        public ICheckupMgrE checkupMgrE { get; set; }

        private static log4net.ILog log = log4net.LogManager.GetLogger("Log.ISI");

        #region Customized Methods

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

                foreach (var summaryDet in summaryDetList)
                {
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
                summary.Dept2 = user.Dept2;
                summary.JobNo = user.JobNo;
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

                foreach (var summaryDet in summaryDetList)
                {
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
                if (user.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE))
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

                foreach (var summaryDet in summaryDetList)
                {
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
        public IList<Evaluation> GetNoSummary(string date, User user)
        {
            try
            {
                StringBuilder hql = new StringBuilder();
                hql.Append("from Evaluation e ");
                hql.Append("where e.IsActive =1 and ");
                if (!user.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) && !user.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE))
                {
                    hql.Append("e.UserCode = '" + user.Code + "' and ");
                }
                hql.Append("e.UserCode not in  ");
                hql.Append("        (select s.UserCode from Summary s  ");
                hql.Append("            where s.Date ='" + DateTime.Parse(date) + "'  ");
                if (!user.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYADMIN) && !user.HasPermission(ISIConstants.CODE_MASTER_SUMMARY_VALUE_SUMMARYAPPROVE))
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
        public IList<SummaryDet> GetSummaryDet(string userCode, string date)
        {
            return hqlMgrE.FindAll<SummaryDet>("from SummaryDet sd where sd.SummaryCode in (select s.Code from Summary s where s.Status!='" + ISIConstants.CODE_MASTER_SUMMARY_STATUS_VALUE_CANCEL + "' and s.UserCode='" + userCode + "' and Date= '" + date + "') order by sd.Id asc ");
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