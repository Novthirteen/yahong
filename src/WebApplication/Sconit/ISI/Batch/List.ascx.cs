using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using NHibernate.Expression;
using com.Sconit.Utility;
using com.Sconit.Entity.Exception;
using System.Text;

public partial class ISI_Approve_List : ListModuleBase
{
    public bool IsHighlight
    {
        get
        {
            return ViewState["IsHighlight"] != null ? (bool)ViewState["IsHighlight"] : false;
        }
        set
        {
            ViewState["IsHighlight"] = value;
        }
    }
    public string ModuleType
    {
        get
        {
            return (string)ViewState["ModuleType"];
        }
        set
        {
            ViewState["ModuleType"] = value;
        }
    }

    public string Desc
    {
        get
        {
            return (string)ViewState["Desc"];
        }
        set
        {
            ViewState["Desc"] = value;
        }
    }
    public bool IsStatus
    {
        get
        {
            return ViewState["IsStatus"] != null ? (bool)ViewState["IsStatus"] : false;
        }
        set
        {
            ViewState["IsStatus"] = value;
        }
    }
    public DateTime? StartDate
    {
        get
        {
            return (DateTime?)ViewState["StartDate"];
        }
        set
        {
            ViewState["StartDate"] = value;
        }
    }
    public DateTime? EndDate
    {
        get
        {
            return (DateTime?)ViewState["EndDate"];
        }
        set
        {
            ViewState["EndDate"] = value;
        }
    }
    /// <summary>
    /// 本周周一
    /// </summary>
    public DateTime Monday
    {
        get
        {
            return (DateTime)ViewState["Monday"];
        }
        set
        {
            ViewState["Monday"] = value;
        }
    }
    /// <summary>
    /// 上周周一
    /// </summary>
    public DateTime LastMonday
    {
        get
        {
            return (DateTime)ViewState["LastMonday"];
        }
        set
        {
            ViewState["LastMonday"] = value;
        }
    }
    /// <summary>
    /// 上周周一
    /// </summary>
    public DateTime LastLastMonday
    {
        get
        {
            return (DateTime)ViewState["LastLastMonday"];
        }
        set
        {
            ViewState["LastLastMonday"] = value;
        }
    }
    public string TargetUser
    {
        get
        {
            return (string)ViewState["TargetUser"];
        }
        set
        {
            ViewState["TargetUser"] = value;
        }
    }
    public string SrcUser
    {
        get
        {
            return (string)ViewState["SrcUser"];
        }
        set
        {
            ViewState["SrcUser"] = value;
        }
    }
    public EventHandler EditEvent;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Monday = GetMondayDate();
            LastMonday = Monday.AddDays(-7);
            LastLastMonday = LastMonday.AddDays(-7);
        }
    }
    /// <summary> 
    /// 计算本周的周一日期 
    /// </summary> 
    /// <returns></returns> 
    public static DateTime GetMondayDate()
    {
        return DateTime.Parse(GetMondayDate(DateTime.Now).ToString("yyyy-MM-dd 00:00:00"));
    }
    /// <summary> 
    /// 计算某日起始日期（礼拜一的日期） 
    /// </summary> 
    /// <param name="someDate">该周中任意一天</param> 
    /// <returns>返回礼拜一日期，后面的具体时、分、秒和传入值相等</returns> 
    public static DateTime GetMondayDate(DateTime someDate)
    {
        int i = someDate.DayOfWeek - DayOfWeek.Monday;
        if (i == -1) i = 6;// i值 > = 0 ，因为枚举原因，Sunday排在最前，此时Sunday-Monday=-1，必须+7=6。 
        TimeSpan ts = new TimeSpan(i, 0, 0, 0);
        return someDate.Subtract(ts);
    }
    public override void UpdateView()
    {
    }

    public void InitPageParameter(object sender)
    {
        object[] param = (object[])sender;
        int i = 0;
        string type = (string)param[i++];
        string taskSubType = (string)param[i++];
        string assignUser = (string)param[i++];
        string startUser = (string)param[i++];

        this.SrcUser = startUser;


        string flag = (string)param[i++];
        string color = (string)param[i++];
        bool first = (bool)param[i++];
        string priority = (string)param[i++];
        IList<string> statusList = (IList<string>)param[i++];
        IList<string> orgList = (IList<string>)param[i++];
        string phase = (string)param[i++];
        string seq = (string)param[i++];
        string targetUser = (string)param[i++];
        this.TargetUser = targetUser;
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskStatusView));
        if (!string.IsNullOrEmpty(flag))
        {
            selectCriteria.Add(Expression.Eq("Flag", flag));

        }
        if (!string.IsNullOrEmpty(color))
        {
            selectCriteria.Add(Expression.Eq("Color", color));

        }
        if (!string.IsNullOrEmpty(priority))
        {
            selectCriteria.Add(Expression.Eq("Priority", priority));

        }
        if (taskSubType != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("TaskSubTypeCode", taskSubType));

        }
        if (statusList != null && statusList.Count > 0)
        {
            selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));

        }
        if (!string.IsNullOrEmpty(assignUser))
        {
            selectCriteria.Add(Expression.Eq("AssignUser", assignUser));

        }

        if (!string.IsNullOrEmpty(startUser))
        {
            if (!first)
            {
                #region 不控制是否第一责任人
                selectCriteria.Add(
                    Expression.Or(
                            Expression.And(Expression.Or(Expression.IsNull("SchedulingStartUser"), Expression.Eq("SchedulingStartUser", string.Empty)),
                                           Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                         Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                                                      Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("AssignStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere))))))),
                            Expression.And(Expression.And(Expression.IsNotNull("SchedulingStartUser"), Expression.Not(Expression.Eq("SchedulingStartUser", string.Empty))),
                                           Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                         Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                                                      Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere),
                                                                                                    Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                                                                  Expression.Like("SchedulingStartUser", ISIConstants.ISI_USER_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)))))))));
                #endregion
            }
            else
            {
                #region 只查找第一责任人
                selectCriteria.Add(
                    Expression.Or(
                            Expression.And(Expression.Or(Expression.IsNull("SchedulingStartUser"), Expression.Eq("SchedulingStartUser", string.Empty)),
                                           Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                         Expression.Or(Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("AssignStartUser", startUser),
                                                                                      Expression.Like("AssignStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                                                    )))),
                            Expression.And(Expression.And(Expression.IsNotNull("SchedulingStartUser"), Expression.Not(Expression.Eq("SchedulingStartUser", string.Empty))),
                                           Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                         Expression.Or(Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_LEVEL_SEPRATOR, MatchMode.Anywhere),
                                                                        Expression.Or(Expression.Eq("SchedulingStartUser", startUser),
                                                                                      Expression.Like("SchedulingStartUser", ISIConstants.ISI_LEVEL_SEPRATOR + startUser + ISIConstants.ISI_USER_SEPRATOR, MatchMode.Anywhere)
                                                                                                    ))))));

                #endregion
            }
        }

        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT)
        {
            //PLN000001578中提出需要修改
            //2013-03-12谢总提出修改，项目跟踪要包含项目问题
            selectCriteria.Add(Expression.Or(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT), Expression.Or(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE), Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE))));

            if (!string.IsNullOrEmpty(phase))
            {
                selectCriteria.Add(Expression.Eq("Phase", phase));
            }
            if (!string.IsNullOrEmpty(seq))
            {
                selectCriteria.Add(Expression.Eq("Seq", seq));
            }
        }
        else
        {

            if (orgList != null && orgList.Count > 0)
            {
                selectCriteria.Add(Expression.In("Org", orgList.ToArray<string>()));
            }
            selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT)));
            selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)));
            selectCriteria.Add(Expression.Not(Expression.Eq("Type", ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)));
        }

        if (!string.IsNullOrEmpty(type))
        {
            selectCriteria.Add(Expression.Eq("Type", type));
        }

        #region 权限的过滤
        if (!this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN)
                && !this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN))
        {
            if (!(CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW)
                    || CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE)
                    || CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN)))
            {
                #region 非观察人
                /*
                DetachedCriteria[] tstCrieteria = ISIUtil.GetTaskSubTypePermissionCriteria(this.CurrentUser.Code,
                                ISIConstants.CODE_MASTER_ISI_TYPE_VALUE_TASKSUBTYPE);

                //创建人
                selectCriteria.Add(
                        Expression.Or(
                            Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                                Subqueries.PropertyIn("tst.Code", tstCrieteria[1])));

                selectCountCriteria.Add(
                        Expression.Or(
                            Subqueries.PropertyIn("tst.Code", tstCrieteria[0]),
                                Subqueries.PropertyIn("tst.Code", tstCrieteria[1])));
                */

                ISIUtil.SetNoVierUserCriteria(selectCriteria, this.CurrentUser);

                #endregion
            }
            else
            {
                #region 观察人

                string[] propertyNames = new string[] { "Type", "CreateUser", "SubmitUser", "AssignStartUser", "SchedulingStartUser", "AssignUpUser", "StartUpUser", "CloseUpUser", "TaskSubTypeAssignUser", "ViewUser", "ECUser", "TaskSubTypeCode" };
                ISIUtil.SetVierUserCriteria(selectCriteria, this.CurrentUser.Code, propertyNames);

                #endregion
            }
        }

        #endregion

        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ISSUE || this.ModuleType == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            selectCriteria.AddOrder(Order.Asc("TaskSubTypeCode"));
            selectCriteria.AddOrder(Order.Asc("Phase"));
            selectCriteria.AddOrder(Order.Asc("Seq"));
        }

        IList<TaskStatusView> taskList = TheCriteriaMgr.FindAll<TaskStatusView>(selectCriteria);

        this.GV_List.DataSource = taskList;
        this.GV_List.DataBind();
    }


    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            EditEvent(code, e);
        }
    }
    public void Open()
    {
        try
        {
            IList<string> taskList = this.PopulateSelectedData(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE);
            if (taskList != null && taskList.Count > 0)
            {
                int count = this.TheTaskMgr.BatchOpenTask(taskList, this.CurrentUser);
                this.ShowSuccessMessage("ISI.Batch.Open.Successfully", count.ToString());
            }
            else
            {
                this.ShowSuccessMessage("Common.Message.Record.Not.Select");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    public void Reject()
    {
        try
        {
            IList<string> taskList = this.PopulateSelectedData(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE);
            if (taskList != null && taskList.Count > 0)
            {
                int count = this.TheTaskMgr.BatchRejectTask(taskList, this.CurrentUser);
                this.ShowSuccessMessage("ISI.Batch.Reject.Successfully", count.ToString());
            }
            else
            {
                this.ShowSuccessMessage("Common.Message.Record.Not.Select");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    public void Close()
    {
        try
        {
            IList<string> taskList = this.PopulateSelectedData(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE);
            if (taskList != null && taskList.Count > 0)
            {
                int count = this.TheTaskMgr.BatchCloseTask(taskList, this.CurrentUser);
                this.ShowSuccessMessage("ISI.Batch.Close.Successfully", count.ToString());
            }
            else
            {
                this.ShowSuccessMessage("Common.Message.Record.Not.Select");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    public void Complete()
    {
        try
        {
            IList<string> taskList = this.PopulateSelectedData(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS);
            if (taskList != null && taskList.Count > 0)
            {
                int count = this.TheTaskMgr.BatchCompleteTask(taskList, this.CurrentUser);
                this.ShowSuccessMessage("ISI.Batch.Complete.Successfully", count.ToString());
            }
            else
            {
                this.ShowSuccessMessage("Common.Message.Record.Not.Select");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    public void Submit()
    {
        try
        {
            IList<string> taskList = this.PopulateSelectedData(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE);
            if (taskList != null && taskList.Count > 0)
            {
                int count = this.TheTaskMgr.BatchSubmitTask(taskList, this.CurrentUser);
                this.ShowSuccessMessage("ISI.Batch.Submit.Successfully", count.ToString());
            }
            else
            {
                this.ShowSuccessMessage("Common.Message.Record.Not.Select");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    public void Cancel()
    {
        try
        {
            IList<string> taskList = this.PopulateSelectedData(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT);
            if (taskList != null && taskList.Count > 0)
            {
                int count = this.TheTaskMgr.BatchCancelTask(taskList, this.CurrentUser);
                this.ShowSuccessMessage("ISI.Batch.Cancel.Successfully", count.ToString());
            }
            else
            {
                this.ShowSuccessMessage("Common.Message.Record.Not.Select");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    public void Delete()
    {
        try
        {
            IList<string> taskList = this.PopulateSelectedData(ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE);
            if (taskList != null && taskList.Count > 0)
            {
                int count = this.TheTaskMgr.BatchDeleteTask(taskList, this.CurrentUser);
                this.ShowSuccessMessage("ISI.Batch.Delete.Successfully", count.ToString());
            }
            else
            {
                this.ShowSuccessMessage("Common.Message.Record.Not.Select");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    public void Batch(string create, bool isCancl, bool isComplete, string complete, bool isOpen)
    {
        try
        {
            IList<string> taskList = this.PopulateSelectedData(string.Empty);
            if (taskList != null && taskList.Count > 0)
            {
                int count = this.TheTaskMgr.BatchTask(taskList, create, isCancl, isComplete, complete, isOpen, this.CurrentUser);
                this.ShowSuccessMessage("ISI.Batch.Successfully", count.ToString());
            }
            else
            {
                this.ShowSuccessMessage("Common.Message.Record.Not.Select");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    public void Replace(string srcUser, string targetUser)
    {
        try
        {
            this.SrcUser = srcUser;
            this.TargetUser = targetUser;
            IList<TaskMstr> taskList = this.PopulateSelectedData(new string[] { ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE,ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT,ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL,
                                      ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN,ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS,ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE,ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE});
            if (taskList != null && taskList.Count > 0)
            {
                int count = this.TheTaskMgr.BatchReplaceTask(taskList, srcUser, targetUser, this.CurrentUser);
                this.ShowSuccessMessage("ISI.Batch.Replace.Successfully", count.ToString());
            }
            else
            {
                this.ShowSuccessMessage("Common.Message.Record.Not.Select");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    public IList<TaskMstr> PopulateSelectedData(string[] status)
    {
        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {
            IList<TaskMstr> taskList = new List<TaskMstr>();
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                CheckBox checkBoxGroup = row.FindControl("CheckBoxGroup") as CheckBox;
                if (checkBoxGroup.Checked)
                {
                    HiddenField hfStatus = row.FindControl("hfStatus") as HiddenField;
                    if (status != null && status.Length > 0 && !status.Contains(hfStatus.Value))
                    {
                        continue;
                    }
                    HiddenField hfId = row.FindControl("hfId") as HiddenField;
                    TaskMstr task = this.TheTaskMstrMgr.CheckAndLoadTaskMstr(hfId.Value);

                    taskList.Add(task);
                }
            }
            return taskList;
        }

        return null;
    }

    public IList<string> PopulateSelectedData(string status)
    {
        if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
        {
            IList<string> taskList = new List<string>();
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                CheckBox checkBoxGroup = row.FindControl("CheckBoxGroup") as CheckBox;
                if (checkBoxGroup.Checked)
                {
                    HiddenField hfStatus = row.FindControl("hfStatus") as HiddenField;
                    if (!string.IsNullOrEmpty(status) && status != hfStatus.Value)
                    {
                        continue;
                    }
                    HiddenField hfId = row.FindControl("hfId") as HiddenField;
                    taskList.Add(hfId.Value);
                }
            }
            return taskList;
        }

        return null;
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i == 4 || i == 5 || i == 6 || i == 8) continue;
                e.Row.Cells[i].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            }

            TaskStatusView task = (TaskStatusView)e.Row.DataItem;

            Label lblTaskSubTypeCode = ((Label)(e.Row.FindControl("lblTaskSubTypeCode")));
            if (!string.IsNullOrEmpty(task.TaskSubTypeCode))
            {
                lblTaskSubTypeCode.Text = "&#91;" + task.TaskSubTypeCode + "&#93;";
            }
            Label lblStatus = ((Label)(e.Row.FindControl("lblStatus")));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage("ISI.Status." + task.Status, this.CurrentUser);

            Label lblStartedUser = ((Label)(e.Row.FindControl("lblStartedUser")));
            SetStartedUser(task, lblStartedUser, e.Row.Cells[6]);

            if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
            {
                ((Label)e.Row.FindControl("lblTaskCode")).ForeColor = System.Drawing.Color.Red;
                ((Label)e.Row.FindControl("lblTaskCode")).ToolTip = task.Priority;
            }
            if (!string.IsNullOrEmpty(task.ExpectedResults))
            {
                ((Label)e.Row.FindControl("lblExpectedResults")).Text = ISIUtil.SetHighlight(task.ExpectedResults, false, string.Empty);
            }
            if (!string.IsNullOrEmpty(task.Color))
            {
                e.Row.Cells[8].Attributes["style"] = "background-color:" + task.Color;
            }
            StringBuilder desc = new StringBuilder();
            if (!string.IsNullOrEmpty(task.Desc1))
            {
                desc.Append(ISIUtil.SetHighlight(task.Desc1, false, string.Empty));
            }
            if (!string.IsNullOrEmpty(task.Desc2))
            {
                desc.Append((!string.IsNullOrEmpty(task.Desc1) ? "<br/>" : string.Empty) + "<span style='color:#0000E5;'>" + "${ISI.Status.Desc2}" + "</span>&#58;&nbsp;" + ISIUtil.SetHighlight(task.Desc2, false, string.Empty));
            }
            ((Label)e.Row.FindControl("lblDesc")).Text = desc.ToString();

            if (task.SubmitDate.HasValue)
            {
                e.Row.Cells[4].ToolTip = task.SubmitDate.Value.ToString("yyyy-MM-dd HH:mm");
            }
            else
            {
                e.Row.Cells[4].ToolTip = task.CreateDate.ToString("yyyy-MM-dd HH:mm");
            }
            if (task.AssignDate.HasValue)
            {
                e.Row.Cells[4].ToolTip = task.AssignDate.Value.ToString("yyyy-MM-dd HH:mm");
            }

        }
    }


    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        var gridView = (GridView)sender;
        IList<string> taskCodeApplyList = new List<string>();
        IList<string> taskCodeList = new List<string>();
        foreach (GridViewRow row in gridView.Rows)
        {
            HiddenField hfIsApply = (HiddenField)row.FindControl("hfIsApply");
            Label lblTaskCode = (Label)row.FindControl("lblTaskCode");
            if (hfIsApply != null && bool.Parse(hfIsApply.Value))
            {
                taskCodeApplyList.Add(lblTaskCode.Text);
            }
            taskCodeList.Add(lblTaskCode.Text);
        }

        IDictionary<string, IList<object>> taskStatusDic = null;
        IDictionary<string, IList<object>> commentDic = null;
        IDictionary<string, IList<object>> attachmentDetailDic = null;
        if (taskCodeList.Count > 0)
        {
            //进展
            taskStatusDic = this.TheTaskStatusMgr.GetTaskStatus(taskCodeList, Monday, LastMonday, LastLastMonday);

            //评论
            commentDic = this.TheCommentDetailMgr.GetComment(taskCodeList, Monday, LastMonday, LastLastMonday);

            //附件
            attachmentDetailDic = this.TheAttachmentDetailMgr.GetAttachmentDetail(taskCodeList, Monday, LastMonday, LastLastMonday);
        }

        IList<TaskApply> taskApplyList = null;
        if (taskCodeApplyList.Count > 0)
        {
            taskApplyList = this.TheTaskApplyMgr.GetTaskApply(taskCodeApplyList);
        }

        if (taskApplyList != null && taskApplyList.Count > 0 || taskStatusDic != null && taskStatusDic.Count > 0 || commentDic != null && commentDic.Count > 0 || attachmentDetailDic != null && attachmentDetailDic.Count > 0)
        {
            foreach (GridViewRow row in gridView.Rows)
            {
                //var statusDiv = (System.Web.UI.HtmlControls.HtmlGenericControl)(row.FindControl("statusDiv"));
                //var spanComplete = (System.Web.UI.HtmlControls.HtmlGenericControl)(row.FindControl("spanComplete"));
                //var spanApprove = (System.Web.UI.HtmlControls.HtmlGenericControl)(row.FindControl("spanApprove"));

                Label lblTaskCode = (Label)row.FindControl("lblTaskCode");

                if (taskStatusDic != null && taskStatusDic.Count > 0 && taskStatusDic.Keys.Contains(lblTaskCode.Text))
                {
                    Label lblStatusDesc = (Label)row.FindControl("lblStatusDesc");
                    //GetStatusDesc(taskStatusDic[lbtnEdit.CommandArgument], lblStatusDesc, statusDiv, spanComplete, spanApprove);
                    GetStatusDesc(taskStatusDic[lblTaskCode.Text], lblStatusDesc);
                }
                else
                {
                    //this.SetCount(statusDiv, spanComplete, spanApprove, 0, 0);
                }

                if (commentDic != null && commentDic.Count > 0 && commentDic.Keys.Contains(lblTaskCode.Text))
                {
                    Label lblComment = ((Label)row.FindControl("lblComment"));
                    GetCommentDesc(commentDic[lblTaskCode.Text], lblComment);
                }

                if (taskApplyList != null && taskApplyList.Count > 0)
                {
                    var thisRowTaskApplyList = taskApplyList.Where(t => t.TaskCode == lblTaskCode.Text).ToList();
                    if (thisRowTaskApplyList != null && thisRowTaskApplyList.Count > 0)
                    {
                        StringBuilder descBufer = new StringBuilder();
                        this.TheTaskApplyMgr.OutputApply(descBufer, thisRowTaskApplyList, this.CurrentUser.UserLanguage);
                        Label lblDesc = (Label)row.FindControl("lblDesc");
                        if (lblDesc.Text.Length > 0)
                        {
                            lblDesc.Text += ISIConstants.EMAIL_SEPRATOR;
                        }
                        lblDesc.Text += descBufer.ToString();
                    }
                }
            }
        }
    }


    private void GetStatusDesc(IList<object> ojbectList, Label lblStatusDesc)
    {
        StringBuilder html = new StringBuilder();
        if (ojbectList != null && ojbectList.Count > 0)
        {
            IList<IList<TaskStatus>> taskStatusListList = (IList<IList<TaskStatus>>)ojbectList[0];
            int currentStatusCount = (int)ojbectList[1];
            int statusCount = (int)ojbectList[2];

            IList<TaskStatus> allStatus = (IList<TaskStatus>)ojbectList[3];
            if (taskStatusListList != null && taskStatusListList.Count > 0)
            {
                IList<TaskStatus> taskStatusList = null;
                IList<TaskStatus> taskStatusTipList = null;
                if (this.IsStatus)
                {
                    if (this.StartDate.HasValue && this.EndDate.HasValue)
                    {
                        taskStatusList = allStatus.Where(t => t.LastModifyDate < this.EndDate.Value.AddDays(1) && t.LastModifyDate >= this.StartDate.Value).ToList();
                    }
                    else if (this.StartDate.HasValue)
                    {
                        taskStatusList = allStatus.Where(t => t.LastModifyDate >= this.StartDate.Value).ToList();
                    }
                    else if (this.EndDate.HasValue)
                    {
                        taskStatusList = allStatus.Where(t => t.LastModifyDate < this.EndDate.Value.AddDays(1)).ToList();
                    }
                }
                else
                {
                    taskStatusList = taskStatusListList[0];

                    if (taskStatusListList.Count > 1)
                    {
                        taskStatusTipList = taskStatusListList[1];
                    }
                }
                foreach (var status in taskStatusList)
                {
                    html.Append("<div>");
                    html.Append("<span style='color:#0000E5;'>");
                    html.Append(status.LastModifyUserNm);
                    html.Append("&#40;");

                    html.Append("<span style='color:");
                    if (status.LastModifyDate >= this.Monday)
                    {
                        html.Append("fuchsia");
                    }
                    else
                    {
                        html.Append("#0000E5");
                    }
                    html.Append(";'>");
                    html.Append(status.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));
                    html.Append("</span>");

                    html.Append("&#41;</span>&#58;&nbsp;");
                    html.Append("<br><span style='background:#DFD3D3'>${ISI.TSK." + status.Type + "}</span>&nbsp;");
                    html.Append(ISIUtil.SetHighlight(status.Desc, this.IsHighlight, this.Desc));
                    html.Append("</div>");
                }
                html.Append("<span style='color:#0000E5;'>&#40;");
                if (currentStatusCount > 0)
                {
                    html.Append("<span style='color:fuchsia;'><b>");
                    html.Append(currentStatusCount);
                    html.Append("</b></span>");
                }
                else
                {
                    html.Append("<span style='color:#0000E5;'>");
                    html.Append(currentStatusCount);
                    html.Append("</span>");
                }
                html.Append("&#47;");
                html.Append(statusCount);
                html.Append("&#41;</span>");

                if (taskStatusTipList != null && taskStatusTipList.Count > 0)
                {
                    lblStatusDesc.Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                    lblStatusDesc.Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                    lblStatusDesc.Attributes.Add("title", GetTaskStatus(taskStatusTipList));
                }

                lblStatusDesc.Text = html.ToString();
            }
        }
    }
    private string GetTaskStatus(IList<TaskStatus> taskStatus)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.Status.StatusDesc}] body=[<table width=100%>");

        foreach (var status in taskStatus)
        {
            detail.Append("<tr><td><span style='color:#0000E5;'>" + status.CreateUserNm + "&#40;");
            detail.Append(status.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));
            detail.Append("&#41;</span>&#58;&nbsp;");
            detail.Append(ISIUtil.SetHighlight(status.Desc.Replace("[", "&#91;").Replace("]", "&#93;"), this.IsHighlight, Desc));
            detail.Append("</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }

    private void GetCommentDesc(IList<object> ojbectList, Label lblComment)
    {
        StringBuilder html = new StringBuilder();
        if (ojbectList != null && ojbectList.Count > 0)
        {
            IList<IList<Comment>> commentListList = (IList<IList<Comment>>)ojbectList[0];
            int currentCommentCount = (int)ojbectList[1];
            int commentCount = (int)ojbectList[2];
            IList<Comment> allComment = (IList<Comment>)ojbectList[3];
            if (commentListList != null && commentListList.Count > 0)
            {
                IList<Comment> commentList = null;
                IList<Comment> commentTipList = null;

                if (this.IsStatus)
                {
                    if (this.StartDate.HasValue && this.EndDate.HasValue)
                    {
                        commentList = allComment.Where(t => t.CreateDate < this.EndDate.Value.AddDays(1) && t.CreateDate >= this.StartDate.Value).ToList();
                    }
                    else if (this.StartDate.HasValue)
                    {
                        commentList = allComment.Where(t => t.CreateDate >= this.StartDate.Value).ToList();

                    }
                    else if (this.EndDate.HasValue)
                    {
                        commentList = allComment.Where(t => t.CreateDate < this.EndDate.Value.AddDays(1)).ToList();
                    }
                }
                else
                {
                    commentList = commentListList[0];

                    if (commentListList.Count > 1)
                    {
                        commentTipList = commentListList[1];
                    }
                }
                foreach (var comment in commentList)
                {
                    html.Append("<div>");
                    html.Append("<span style='color:#0000E5;'>");
                    html.Append(comment.CreateUser);
                    html.Append("&#40;");

                    html.Append("<span style='color:");
                    if (comment.CreateDate >= this.Monday)
                    {
                        html.Append("fuchsia");
                    }
                    else
                    {
                        html.Append("#0000E5");
                    }
                    html.Append(";'>");
                    html.Append(comment.CreateDate.ToString("yyyy-MM-dd HH:mm"));
                    html.Append("</span>");

                    html.Append("&#41;</span>&#58;&nbsp;");
                    html.Append(ISIUtil.SetHighlight(comment.Value, this.IsHighlight, this.Desc));
                    html.Append("</div>");
                }
                html.Append("<span style='color:#0000E5;'>&#40;");
                if (currentCommentCount > 0)
                {
                    html.Append("<span style='color:fuchsia;'><b>");
                    html.Append(currentCommentCount);
                    html.Append("</b></span>");
                }
                else
                {
                    html.Append("<span style='color:#0000E5;;'>");
                    html.Append(currentCommentCount);
                    html.Append("</span>");
                }
                html.Append("&#47;");
                html.Append(commentCount);
                html.Append("&#41;</span>");

                if (commentTipList != null && commentTipList.Count > 0)
                {
                    lblComment.Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                    lblComment.Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                    lblComment.Attributes.Add("title", GetComments(commentTipList));
                }

                lblComment.Text = html.ToString();
            }
        }
    }

    private string GetComments(IList<Comment> comments)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.Status.Comment}] body=[<table width=100%>");
        foreach (var comment in comments)
        {
            detail.Append("<tr><td><span style='color:#0000E5;'>");
            detail.Append(comment.CreateUser);
            detail.Append("&#40;");

            detail.Append(comment.CreateDate.ToString("yyyy-MM-dd HH:mm"));

            detail.Append("&#41;</span>&#58;&nbsp;");
            detail.Append(ISIUtil.SetHighlight(comment.Value.Replace("[", "&#91;").Replace("]", "&#93;"), this.IsHighlight, Desc));
            detail.Append("</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }


    private void SetStartedUser(TaskStatusView task, Label lblStartedUser, TableCell cell)
    {
        if (task.StartedUserCount > 0)
        {
            string assignStartUserNm = task.AssignStartUserNm;
            if (task.StartedUserCount > 0 && string.IsNullOrEmpty(assignStartUserNm))
            {
                assignStartUserNm = this.TheUserSubscriptionMgr.GetUserName(task.StartedUser);
            }

            var userCodeArr = task.StartedUser.Split(ISIConstants.ISI_SEPRATOR, StringSplitOptions.RemoveEmptyEntries).ToList();

            IDictionary<int, string> indexColor = new Dictionary<int, string>();
            if (!string.IsNullOrEmpty(SrcUser))
            {
                int index1 = userCodeArr.IndexOf(SrcUser);
                indexColor.Add(index1, "red");
            }
            if (!string.IsNullOrEmpty(TargetUser))
            {
                int index2 = userCodeArr.IndexOf(TargetUser);
                indexColor.Add(index2, "blue");
            }

            lblStartedUser.Text = ISIUtil.GetUserName(assignStartUserNm, indexColor);//.Replace(", ", "<br />");
        }
        else
        {
            lblStartedUser.Text = string.Empty;
        }
    }
}


