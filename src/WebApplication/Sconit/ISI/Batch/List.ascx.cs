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
    public EventHandler EditEvent;
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {


        }
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
        string flag = (string)param[i++];
        string color = (string)param[i++];
        bool first = (bool)param[i++];
        string priority = (string)param[i++];
        IList<string> statusList = (IList<string>)param[i++];
        IList<string> orgList = (IList<string>)param[i++];
        string phase = (string)param[i++];
        string seq = (string)param[i++];

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

    protected void GV_List_DataBound(object sender, EventArgs e)
    {

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

            Label lblStatusDesc = (Label)e.Row.FindControl("lblStatusDesc");
            if (!string.IsNullOrEmpty(task.StatusDesc))
            {
                lblStatusDesc.Text = GetStatusDesc(task);
            }
            else if (task.StatusDate.HasValue)
            {
                lblStatusDesc.Text = "<span style='color:#0000E5;'>" + task.StatusDate.Value.ToString("yyyy-MM-dd HH:mm") + "</span>";
            }

            if (!string.IsNullOrEmpty(task.Comment) && task.CommentCreateDate.HasValue)
            {
                ((Label)e.Row.FindControl("lblComment")).Text = GetCommentDesc(task);

            }
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

    private void SetStartedUser(TaskStatusView task, Label lblStartedUser, TableCell cell)
    {
        if (task.StartedUserCount > 0)
        {
            string assignStartUserNm = task.AssignStartUserNm;
            if (task.StartedUserCount > 0 && string.IsNullOrEmpty(assignStartUserNm))
            {
                assignStartUserNm = this.TheUserSubscriptionMgr.GetUserName(task.StartedUser);
            }
            if (task.StartedUserCount > 6)
            {
                lblStartedUser.Text = ISIUtil.GetUserName(task.GetAssignStartUserNm(6, this.CurrentUser.Code), this.CurrentUser.Name, "blue");//.Replace(", ", "<br/>");
            }
            else
            {
                lblStartedUser.Text = ISIUtil.GetUserName(assignStartUserNm, this.CurrentUser.Name, "blue");//.Replace(", ", "<br />");
            }
        }
        else
        {
            lblStartedUser.Text = string.Empty;
        }
    }

    private string GetCommentDesc(TaskStatusView task)
    {
        StringBuilder html = new StringBuilder();
        html.Append("<span style='color:#0000E5;'>");
        html.Append(task.CommentCreateUserNm);
        html.Append("&#40;");
        if (task.CurrentCommentCount.HasValue && task.CurrentCommentCount.Value > 0)
        {
            html.Append("<span style='color:fuchsia;'>");
        }
        html.Append(task.CommentCreateDate.Value.ToString("yyyy-MM-dd HH:mm"));
        if (task.CurrentCommentCount.HasValue && task.CurrentCommentCount.Value > 0)
        {
            html.Append("</span>");
        }
        html.Append("&#41;</span>&#58;&nbsp;");
        html.Append(ISIUtil.SetHighlight(task.Comment, false, string.Empty));

        if (task.CommentCount.HasValue && task.CommentCount.Value > 1)
        {
            html.Append("<span style='color:#0000E5;'>&#40;");
            if (task.CurrentCommentCount.HasValue && task.CurrentCommentCount > 0)
            {
                if (task.CurrentCommentCount.Value == 1)
                {
                    html.Append(task.CurrentCommentCount.Value.ToString());
                }
                else
                {
                    html.Append("<span style='color:fuchsia;'><b>");
                    html.Append(task.CurrentCommentCount.Value);
                    html.Append("</b></span>");
                }
                html.Append("&#47;");
            }

            html.Append(task.CommentCount.Value);
            html.Append("&#41;</span>");
        }
        return html.ToString();
    }
    private string GetStatusDesc(TaskStatusView task)
    {
        StringBuilder html = new StringBuilder();
        html.Append("<span style='color:#0000E5;'>");
        html.Append(task.StatusUserNm);
        html.Append("&#40;");
        if (task.CurrentStatusCount.HasValue && task.CurrentStatusCount.Value > 0)
        {
            html.Append("<span style='color:fuchsia;'>");
        }
        html.Append(task.StatusDate.Value.ToString("yyyy-MM-dd HH:mm"));
        if (task.CurrentStatusCount.HasValue && task.CurrentStatusCount.Value > 0)
        {
            html.Append("</span>");
        }
        html.Append("&#41;</span>&#58;&nbsp;");
        html.Append(ISIUtil.SetHighlight(task.StatusDesc, false, string.Empty));

        if (task.StatusCount.HasValue && task.StatusCount.Value > 1)
        {
            html.Append("<span style='color:#0000E5;'>&#40;");
            if (task.CurrentStatusCount.HasValue && task.CurrentStatusCount > 0)
            {
                if (task.CurrentStatusCount.Value == 1)
                {
                    html.Append(task.CurrentStatusCount.Value.ToString());
                }
                else
                {
                    html.Append("<span style='color:fuchsia;'><b>");
                    html.Append(task.CurrentStatusCount.Value);
                    html.Append("</b></span>");
                }
                html.Append("&#47;");
            }

            html.Append(task.StatusCount.Value);
            html.Append("&#41;</span>");
        }
        return html.ToString();
    }

}


