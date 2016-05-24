using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using com.Sconit.Service.Ext.Criteria;
using NHibernate.Expression;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Persistence;
using NHibernate;
using Castle.Facilities.NHibernateIntegration;
using Castle.Services.Transaction;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;

/// <summary>
/// Summary description for ISIProxy
/// </summary>
namespace com.Sconit.Web
{
    public class FilterMgrProxy
    {
        private IFilterMgrE FilterMgr
        {
            get
            {
                return ServiceLocator.GetService<IFilterMgrE>("FilterMgr.service");
            }
        }

        public FilterMgrProxy()
        {
        }

        public void CreateFilter(Filter filter)
        {
            FilterMgr.CreateFilter(filter);
        }

        public Filter LoadFilter(int id)
        {
            return FilterMgr.LoadFilter(id);
        }

        public void UpdateFilter(Filter filter)
        {
            FilterMgr.UpdateFilter(filter);
        }

        public void DeleteFilter(Filter filter)
        {
            FilterMgr.DeleteFilter(filter);
        }
    }


    public class TaskStatusMgrProxy
    {
        private ITaskStatusMgrE TaskStatusMgr
        {
            get
            {
                return ServiceLocator.GetService<ITaskStatusMgrE>("TaskStatusMgr.service");
            }
        }
        private ITaskMgrE TaskMgr
        {
            get
            {
                return ServiceLocator.GetService<ITaskMgrE>("TaskMgr.service");
            }
        }

        public TaskStatusMgrProxy()
        {
        }

        public void CreateTaskStatus(TaskStatus taskStatus)
        {
            TaskMgr.CreateTaskStatus(taskStatus);
        }

        public TaskStatus LoadTaskStatus(int id)
        {
            return TaskStatusMgr.LoadTaskStatus(id);
        }

        public void UpdateTaskStatus(TaskStatus taskStatus)
        {
            TaskMgr.UpdateTaskStatus(taskStatus);
        }

        public void DeleteTaskStatus(TaskStatus taskStatus)
        {
            TaskStatusMgr.DeleteTaskStatus(taskStatus);
        }
    }

    public class CheckupMgrProxy
    {
        private ICheckupMgrE CheckupMgr
        {
            get
            {
                return ServiceLocator.GetService<ICheckupMgrE>("CheckupMgr.service");
            }
        }

        public CheckupMgrProxy()
        {
        }

        public void CreateCheckup(Checkup checkup)
        {
            CheckupMgr.CreateCheckup(checkup);
        }

        public Checkup LoadCheckup(int id)
        {
            return CheckupMgr.LoadCheckup(id);
        }

        public void UpdateCheckup(Checkup checkup)
        {
            CheckupMgr.UpdateCheckup(checkup);
        }

        public void DeleteCheckup(Checkup checkup)
        {
            CheckupMgr.DeleteCheckup(checkup);
        }
    }

    public class TaskAddressMgrProxy
    {
        private ITaskAddressMgrE TaskAddressMgr
        {
            get
            {
                return ServiceLocator.GetService<ITaskAddressMgrE>("TaskAddressMgr.service");
            }
        }

        public TaskAddressMgrProxy()
        {
        }

        public void CreateTaskAddress(TaskAddress taskAddress)
        {
            TaskAddressMgr.CreateTaskAddress(taskAddress);
        }

        public TaskAddress LoadTaskAddress(string code)
        {
            return TaskAddressMgr.LoadTaskAddress(code);
        }

        public void UpdateTaskAddress(TaskAddress taskAddress)
        {
            TaskAddressMgr.UpdateTaskAddress(taskAddress);
        }

        public void DeleteTaskAddress(TaskAddress taskAddress)
        {
            TaskAddressMgr.DeleteTaskAddress(taskAddress);
        }
    }

    public class SchedulingMgrProxy
    {
        private ISchedulingMgrE SchedulingMgr
        {
            get
            {
                return ServiceLocator.GetService<ISchedulingMgrE>("SchedulingMgr.service");
            }
        }

        public SchedulingMgrProxy()
        {
        }

        public void CreateScheduling(Scheduling scheduling)
        {
            SchedulingMgr.CreateScheduling(scheduling);
        }

        public Scheduling LoadScheduling(int id)
        {
            return SchedulingMgr.LoadScheduling(id);
        }

        public void UpdateScheduling(Scheduling scheduling)
        {
            SchedulingMgr.UpdateScheduling(scheduling);
        }

        public void DeleteScheduling(Scheduling scheduling)
        {
            SchedulingMgr.DeleteScheduling(scheduling);
        }
    }

    public class UserSubscriptionMgrProxy
    {
        private IUserSubscriptionMgrE UserSubscriptionMgr
        {
            get
            {
                return ServiceLocator.GetService<IUserSubscriptionMgrE>("UserSubscriptionMgr.service");
            }
        }

        public UserSubscriptionMgrProxy()
        {
        }

        public IList<UserSubView> LoadUserSubscription(string userCode)
        {
            if (string.IsNullOrEmpty(userCode)) return null;
            return UserSubscriptionMgr.GetUserAllTaskSubType(userCode);
        }
    }


    public class TaskReportMgrProxy
    {
        private ITaskReportMgrE TaskReportMgr
        {
            get
            {
                return ServiceLocator.GetService<ITaskReportMgrE>("TaskReportMgr.service");
            }
        }

        public TaskReportMgrProxy()
        {
        }

        public IList<TaskReportView> LoadTaskReport(string userCode)
        {
            if (string.IsNullOrEmpty(userCode)) return null;
            return TaskReportMgr.GetUserAllTaskSubType(userCode);
        }
    }

    public class TaskSubTypeMgrProxy
    {
        private ITaskSubTypeMgrE TaskSubTypeMgr
        {
            get
            {
                return ServiceLocator.GetService<ITaskSubTypeMgrE>("TaskSubTypeMgr.service");
            }
        }

        private ICriteriaMgrE CriteriaMgr
        {
            get
            {
                return ServiceLocator.GetService<ICriteriaMgrE>("CriteriaMgr.service");
            }
        }

        public TaskSubTypeMgrProxy()
        {
        }

        public void CreateTaskSubType(TaskSubType taskSubType)
        {
            TaskSubTypeMgr.CreateTaskSubType(taskSubType);
        }

        public TaskSubType LoadTaskSubType(string code)
        {
            return TaskSubTypeMgr.LoadTaskSubType(code);
        }

        public void UpdateTaskSubType(TaskSubType taskSubType)
        {
            TaskSubTypeMgr.UpdateTaskSubType(taskSubType);
        }

        public void DeleteTaskSubType(TaskSubType taskSubType)
        {
            // TaskMstr tm = CriteriaMgr.FindAll<TaskMstr>(("from TaskMstr as t where t.TaskSubType=?",taskSubType.Code);
            TaskSubTypeMgr.DeleteTaskSubType(taskSubType.Code);
        }
    }

    public class FailureModeMgrProxy
    {
        private IFailureModeMgrE FailureModeMgr
        {
            get
            {
                return ServiceLocator.GetService<IFailureModeMgrE>("FailureModeMgr.service");
            }
        }

        public FailureModeMgrProxy()
        {
        }

        public void CreateFailureMode(FailureMode failureMode)
        {
            FailureModeMgr.CreateFailureMode(failureMode);
        }

        public FailureMode LoadFailureMode(string code)
        {
            return FailureModeMgr.LoadFailureMode(code);
        }

        public void UpdateFailureMode(FailureMode failureMode)
        {
            FailureModeMgr.UpdateFailureMode(failureMode);
        }

        public void DeleteFailureMode(FailureMode failureMode)
        {
            FailureModeMgr.DeleteFailureMode(failureMode);
        }
    }

    public class TaskMstrMgrProxy
    {
        private ITaskMstrMgrE TaskMstrMgr
        {
            get
            {
                return ServiceLocator.GetService<ITaskMstrMgrE>("TaskMstrMgr.service");
            }
        }

        public TaskMstrMgrProxy()
        {
        }

        public void CreateTaskMstr(TaskMstr taskMstr)
        {
            TaskMstrMgr.CreateTaskMstr(taskMstr);
        }

        public TaskMstr LoadTaskMstr(string code)
        {
            if (code == null) return null;
            return TaskMstrMgr.LoadTaskMstr(code);
        }

        public TaskMstr LoadTaskMstr(string code, bool includeDetail)
        {
            return TaskMstrMgr.LoadTaskMstr(code, includeDetail);
        }

        public void UpdateTaskMstr(TaskMstr task)
        {
            TaskMstrMgr.UpdateTaskMstr(task);
        }

        public void DeleteTaskMstr(TaskMstr task)
        {
            TaskMstrMgr.DeleteTaskMstr(task);
        }
    }

    public class CheckupProjectMgrProxy
    {
        private ICheckupProjectMgrE CheckupProjectMgr
        {
            get
            {
                return ServiceLocator.GetService<ICheckupProjectMgrE>("CheckupProjectMgr.service");
            }
        }

        public CheckupProjectMgrProxy()
        {
        }

        public void CreateCheckupProject(CheckupProject checkupProject)
        {
            CheckupProjectMgr.CreateCheckupProject(checkupProject);
        }

        public CheckupProject LoadCheckupProject(string code)
        {
            return CheckupProjectMgr.LoadCheckupProject(code);
        }

        public void UpdateCheckupProject(CheckupProject checkupProject)
        {
            CheckupProjectMgr.UpdateCheckupProject(checkupProject);
        }

        public void DeleteCheckupProject(CheckupProject checkupProject)
        {
            CheckupProjectMgr.DeleteCheckupProject(checkupProject);
        }
    }

    public class SummaryMgrProxy
    {
        private ISummaryMgrE SummaryMgr
        {
            get
            {
                return ServiceLocator.GetService<ISummaryMgrE>("SummaryMgr.service");
            }
        }

        public SummaryMgrProxy()
        {
        }

        public void CreateSummary(Summary checkupProject)
        {
            SummaryMgr.CreateSummary(checkupProject);
        }

        public Summary LoadSummary(string code)
        {
            return SummaryMgr.LoadSummary(code);
        }

        public void UpdateSummary(Summary checkupProject)
        {
            SummaryMgr.UpdateSummary(checkupProject);
        }

        public void DeleteSummary(Summary checkupProject)
        {
            SummaryMgr.DeleteSummary(checkupProject);
        }
    }

    public class ProjectTaskMgrProxy
    {
        private IProjectTaskMgrE ProjectTaskMgr
        {
            get
            {
                return ServiceLocator.GetService<IProjectTaskMgrE>("ProjectTaskMgr.service");
            }
        }

        public ProjectTaskMgrProxy()
        {
        }

        public void CreateProjectTask(ProjectTask projectTask)
        {
            ProjectTaskMgr.CreateProjectTask(projectTask);
        }

        public ProjectTask LoadProjectTask(int id)
        {
            if (id == 0) return null;
            return ProjectTaskMgr.LoadProjectTask(id);
        }


        public void UpdateProjectTask(ProjectTask task)
        {
            ProjectTaskMgr.UpdateProjectTask(task);
        }

        public void DeleteProjectTask(ProjectTask task)
        {
            ProjectTaskMgr.DeleteProjectTask(task);
        }
    }

    public class ApplyMgrProxy
    {
        private IApplyMgrE ApplyMgr
        {
            get
            {
                return ServiceLocator.GetService<IApplyMgrE>("ApplyMgr.service");
            }
        }

        public ApplyMgrProxy()
        {
        }

        public void CreateApply(Apply apply)
        {
            ApplyMgr.CreateApply(apply);
        }

        public Apply LoadApply(string code)
        {
            return ApplyMgr.LoadApply(code);
        }

        public void UpdateApply(Apply apply)
        {
            ApplyMgr.UpdateApply(apply);
        }

        public void DeleteApply(Apply apply)
        {
            ApplyMgr.DeleteApply(apply);
        }
    }

    
}