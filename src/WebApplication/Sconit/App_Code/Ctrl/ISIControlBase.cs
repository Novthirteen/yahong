using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.Utility;

/// <summary>
/// ISIControl 的摘要说明
/// </summary>
namespace com.Sconit.Web
{
    public abstract class ISIControlBase : ControlBase
    {
        public ISIControlBase()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        #region ISI
        protected IAccountMgrE TheAccountMgr { get { return ServiceLocator.GetService<IAccountMgrE>("AccountMgr.service"); } }
        protected IBudgetMgrE TheBudgetMgr { get { return ServiceLocator.GetService<IBudgetMgrE>("BudgetMgr.service"); } }
        protected IBudgetDetMgrE TheBudgetDetMgr { get { return ServiceLocator.GetService<IBudgetDetMgrE>("BudgetDetMgr.service"); } }
        protected ISmtpMgrE TheSmtpMgr { get { return ServiceLocator.GetService<ISmtpMgrE>("SmtpMgr.service"); } }
        protected ITaskMstrMgrE TheTaskMstrMgr { get { return GetService<ITaskMstrMgrE>("TaskMstrMgr.service"); } }
        protected ITaskMgrE TheTaskMgr { get { return GetService<ITaskMgrE>("TaskMgr.service"); } }
        protected ITaskDetailMgrE TheTaskDetailMgr { get { return GetService<ITaskDetailMgrE>("TaskDetailMgr.service"); } }
        protected ITaskReportMgrE TheTaskReportMgr { get { return GetService<ITaskReportMgrE>("TaskReportMgr.service"); } }
        protected IUserSubscriptionMgrE TheUserSubscriptionMgr { get { return GetService<IUserSubscriptionMgrE>("UserSubscriptionMgr.service"); } }
        protected IAttachmentDetailMgrE TheAttachmentDetailMgr { get { return GetService<IAttachmentDetailMgrE>("AttachmentDetailMgr.service"); } }
        protected ICommentDetailMgrE TheCommentDetailMgr { get { return GetService<ICommentDetailMgrE>("CommentDetailMgr.service"); } }
        protected IFailureModeMgrE TheFailureModeMgr { get { return GetService<IFailureModeMgrE>("FailureModeMgr.service"); } }
        protected ISchedulingMgrE TheSchedulingMgr { get { return GetService<ISchedulingMgrE>("SchedulingMgr.service"); } }
        protected ITaskAddressMgrE TheTaskAddressMgr { get { return GetService<ITaskAddressMgrE>("TaskAddressMgr.service"); } }
        protected ITaskStatusMgrE TheTaskStatusMgr { get { return GetService<ITaskStatusMgrE>("TaskStatusMgr.service"); } }
        protected ITaskSubTypeMgrE TheTaskSubTypeMgr { get { return GetService<ITaskSubTypeMgrE>("TaskSubTypeMgr.service"); } }
        protected IWFDetailMgrE TheWFDetailMgr { get { return GetService<IWFDetailMgrE>("WFDetailMgr.service"); } }
        protected ICheckupProjectMgrE TheCheckupProjectMgr { get { return GetService<ICheckupProjectMgrE>("CheckupProjectMgr.service"); } }
        protected ICheckupMgrE TheCheckupMgr { get { return GetService<ICheckupMgrE>("CheckupMgr.service"); } }
        protected IProjectTaskMgrE TheProjectTaskMgr { get { return GetService<IProjectTaskMgrE>("ProjectTaskMgr.service"); } }

        protected IWorkflowMgrE TheWorkflowMgr { get { return GetService<IWorkflowMgrE>("WorkflowMgr.service"); } }

        protected IApplyMgrE TheApplyMgr { get { return GetService<IApplyMgrE>("ApplyMgr.service"); } }

        protected ITaskApplyMgrE TheTaskApplyMgr { get { return GetService<ITaskApplyMgrE>("TaskApplyMgr.service"); } }
        protected IProcessInstanceMgrE TheProcessInstanceMgr { get { return GetService<IProcessInstanceMgrE>("ProcessInstanceMgr.service"); } }
        protected IProcessDefinitionMgrE TheProcessDefinitionMgr { get { return GetService<IProcessDefinitionMgrE>("ProcessDefinitionMgr.service"); } }
        protected IProcessApplyMgrE TheProcessApplyMgr { get { return GetService<IProcessApplyMgrE>("ProcessApplyMgr.service"); } }

        protected IFilterMgrE TheFilterMgr { get { return GetService<IFilterMgrE>("FilterMgr.service"); } }

        protected IEvaluationMgrE TheEvaluationMgr { get { return GetService<IEvaluationMgrE>("EvaluationMgr.service"); } }
        protected ISummaryMgrE TheSummaryMgr { get { return GetService<ISummaryMgrE>("SummaryMgr.service"); } }
        protected ISummaryDetMgrE TheSummaryDetMgr { get { return GetService<ISummaryDetMgrE>("SummaryDetMgr.service"); } }

        protected IMouldMgrE TheMouldMgr { get { return GetService<IMouldMgrE>("MouldMgr.service"); } }
        protected IMouldDetailMgrE TheMouldDetailMgr { get { return GetService<IMouldDetailMgrE>("MouldDetailMgr.service"); } }

        #endregion

    }
}