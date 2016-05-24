using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using System.Text;
using System.IO;
using NHibernate.Transform;
using Geekees.Common.Controls;
using com.Sconit.Entity.MasterData;

public partial class ISI_TSK_RefTask : com.Sconit.Web.MainModuleBase
{
    public event EventHandler BackEvent;
    public EventHandler EditEvent;

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
            return ViewState["Desc"] == null ? string.Empty : (string)ViewState["Desc"];
        }
        set
        {
            ViewState["Desc"] = value;
        }
    }
    public bool IsHighlight
    {
        get
        {
            return ViewState["IsHighlight"] == null ? false : (bool)ViewState["IsHighlight"];
        }
        set
        {
            ViewState["IsHighlight"] = value;
        }
    }
    public string TaskCode
    {
        get
        {
            return (string)ViewState["TaskCode"];
        }
        set
        {
            ViewState["TaskCode"] = value;
        }
    }

    public void InitPageParameter(string taskCode)
    {
        this.TaskCode = taskCode;

        this.lgd.InnerText = "${ISI.TSK." + this.ModuleType + "}" + this.TaskCode;

        this.btnSearch_Click(null, null);

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        tbTaskSubType.ServiceParameter = "string:" + this.CurrentUser.Code;
        tbTaskSubType.DataBind();
        if (!IsPostBack)
        {

        }
    }

    private void PageCleanup()
    {
        this.TaskCode = null;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
            this.PageCleanup();
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        string taskSubType = this.tbTaskSubType.Text.Trim();
        string assignUser = this.tbAssignUser.Text.Trim();
        string startUser = this.tbStartUser.Text.Trim();
        string submitUser = this.tbSubmitUser.Text.Trim();
        DateTime? startTime = null;
        if (this.tbStartDate.Text.Trim() != string.Empty)
        {
            startTime = DateTime.Parse(this.tbStartDate.Text.Trim());
        }
        DateTime? endTime = null;
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            endTime = DateTime.Parse(this.tbEndDate.Text.Trim());
        }


        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskStatusView));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(TaskStatusView))
            .SetProjection(Projections.CountDistinct("TaskCode"));

        selectCriteria.Add(Expression.Eq("BackYards", this.TaskCode));

        if (taskSubType != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("TaskSubTypeCode", taskSubType));
            selectCountCriteria.Add(Expression.Eq("TaskSubTypeCode", taskSubType));
        }
        if (startTime.HasValue)
        {
            selectCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Ge("CreateDate", startTime)));
            selectCountCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Ge("CreateDate", startTime)));
        }
        if (endTime.HasValue)
        {
            selectCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Le("CreateDate", endTime)));
            selectCountCriteria.Add(Expression.Or(Expression.IsNull("CreateDate"), Expression.Le("CreateDate", endTime)));
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
                ISIUtil.SetNoVierUserCriteria(selectCountCriteria, this.CurrentUser);


                #endregion
            }
            else
            {
                #region 观察人

                string[] propertyNames = new string[] { "Type", "CreateUser", "SubmitUser", "AssignStartUser", "SchedulingStartUser", "AssignUpUser", "StartUpUser", "CloseUpUser", "TaskSubTypeAssignUser", "ViewUser", "ECUser", "TaskSubTypeCode" };
                ISIUtil.SetVierUserCriteria(selectCriteria, this.CurrentUser.Code, propertyNames);
                ISIUtil.SetVierUserCriteria(selectCountCriteria, this.CurrentUser.Code, propertyNames);

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

        this.GV_List.DataSource = TheCriteriaMgr.FindAll<TaskStatusView>(selectCriteria, 0, 500);
        this.GV_List.DataBind();

        if ((Button)sender == this.btnExport)
        {
            this.ExportXLS(this.GV_List);
        }
        else
        {
            this.fs.Visible = true;
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.btnSearch_Click(sender, e);
    }

    protected void GV_List_DataBound(object sender, EventArgs e)
    {

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
                cell.Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                cell.Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                cell.Attributes.Add("title", GetStartUser(assignStartUserNm));
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

    protected void lbtnDownLoad_Click(object sender, EventArgs e)
    {
        string argument = ((LinkButton)sender).CommandArgument;

        try
        {
            string[] arg = argument.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);

            if (arg.Length == 3)
            {
                Attachment attachment = new Attachment() { FileName = arg[0], ContentType = arg[1], Path = arg[2] };

                this.DownLoadFile(attachment);
            }
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {

        }
    }

    //  下载文件类
    public void DownLoadFile(Attachment attachment)
    {
        string absolutePath = System.Web.HttpContext.Current.Request.MapPath("App_Data/");
        // 保存文件的虚拟路径
        //string Url = "File\\" + FullFileName;
        // 保存文件的物理路径
        string FullPath = absolutePath + attachment.Path;// HttpContext.Current.Server.MapPath(Url);
        // 初始化FileInfo类的实例，作为文件路径的包装
        FileInfo FI = new FileInfo(FullPath);
        // 判断文件是否存在
        if (FI.Exists)
        {
            // 将文件保存到本机
            Response.Clear();
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Server.UrlEncode(attachment.FileName));
            Response.AddHeader("Content-Length", FI.Length.ToString());
            Response.ContentType = attachment.ContentType;
            Response.Filter.Close();
            Response.WriteFile(FI.FullName);
            Response.End();
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            int i = 0;
            e.Row.Cells[i++].ToolTip = "${ISI.Status.CreateDate}";
            e.Row.Cells[i++].ToolTip = "${ISI.Status.RefTaskCount}";
            e.Row.Cells[i++].ToolTip = "${ISI.Status.AttachmentCount}";
            e.Row.Cells[i++].ToolTip = "${ISI.Status.SubmitDate}";
            e.Row.Cells[i++].ToolTip = "${ISI.Status.AssignDate}";
            e.Row.Cells[i++].ToolTip = "${ISI.Status.StartDate}";
            e.Row.Cells[i++].ToolTip = "${ISI.Status.StatusDate}";
            e.Row.Cells[i++].ToolTip = "${ISI.Status.Color}";
            e.Row.Cells[i++].ToolTip = "${ISI.Status.CommentDate}";
        }
        else if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i == 3 || i == 4 || i == 5 || i == 7) continue;
                e.Row.Cells[i].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            }

            TaskStatusView task = (TaskStatusView)e.Row.DataItem;

            Label lblTaskSubTypeCode = ((Label)(e.Row.FindControl("lblTaskSubTypeCode")));
            if (!string.IsNullOrEmpty(task.TaskSubTypeCode))
            {
                lblTaskSubTypeCode.Text = "&#91;" + task.TaskSubTypeCode + "&#93;";
            }
            Label lblStatus = ((Label)(e.Row.FindControl("lblStatus")));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(task.Status, this.CurrentUser);
            Label lblSubject = ((Label)(e.Row.FindControl("lblSubject")));
            if (!string.IsNullOrEmpty(task.Subject))
            {
                lblSubject.Text = ISIUtil.SetHighlight(task.Subject.Trim(), this.IsHighlight, this.Desc);
            }

            Label lblStartedUser = ((Label)(e.Row.FindControl("lblStartedUser")));
            SetStartedUser(task, lblStartedUser, e.Row.Cells[5]);

            if (task.PlanCompleteDate.HasValue)
            {
                DateTime consult = DateTime.Now;
                e.Row.Cells[2].ToolTip = "${ISI.Status.Deadline}: " + task.PlanStartDate.Value.ToString("yyyy-MM-dd") + "/" + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd");
                if (task.CompleteDate.HasValue
                        && (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE
                                    || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE))
                {
                    consult = task.CompleteDate.Value;
                }
                if (task.PlanStartDate.HasValue && task.PlanCompleteDate.HasValue)
                {
                    if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE)
                    {
                        if (task.PlanCompleteDate.Value.CompareTo(consult) <= 0)
                        {
                            ((Label)e.Row.FindControl("lblEndDate")).Text = "${ISI.Status.Deadline}: <span style='color: Red;'>" + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd") + "</span>";
                        }
                        else
                        {
                            double startPercent = task.StartPercent.HasValue ? double.Parse(task.StartPercent.Value.ToString()) : 0.7;
                            TimeSpan ts = task.PlanCompleteDate.Value.Subtract(task.PlanStartDate.Value);
                            int milliseconds = (int)Math.Ceiling(ts.TotalMilliseconds * startPercent);
                            if (task.PlanStartDate.Value.AddMilliseconds(milliseconds).CompareTo(consult) > 0)
                            {
                                ((Label)e.Row.FindControl("lblEndDate")).Text = "${ISI.Status.Deadline}: <span style='color: Green;'>" + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd") + "</span>";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblEndDate")).Text = "${ISI.Status.Deadline}: <span style='color: #FFCC00;'>" + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd") + "</span>";
                            }
                        }
                    }
                    else
                    {
                        ((Label)e.Row.FindControl("lblEndDate")).Text = "${ISI.Status.Deadline}: " + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd");
                    }
                }
            }
            if (!string.IsNullOrEmpty(task.Color))
            {
                e.Row.Cells[7].Attributes["style"] = "background-color:" + task.Color;
            }
            if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
            {
                //e.Row.Cells[0].ForeColor = System.Drawing.Color.Red;
                //e.Row.Cells[0].ToolTip = task.Priority;
                ((LinkButton)e.Row.FindControl("lbtnEdit")).ForeColor = System.Drawing.Color.Red;
                ((LinkButton)e.Row.FindControl("lbtnEdit")).ToolTip = task.Priority;
            }
            //((LinkButton)e.Row.FindControl("lbtnEdit")).Text = ISIUtil.SetHighlight(task.TaskCode, this.IsHighlight, this.Desc);
            StringBuilder desc = new StringBuilder();
            if (!string.IsNullOrEmpty(task.Desc1))
            {
                desc.Append(ISIUtil.SetHighlight(task.Desc1, this.IsHighlight, this.Desc));
            }
            if (!string.IsNullOrEmpty(task.Desc2))
            {
                desc.Append((!string.IsNullOrEmpty(task.Desc1) ? "<br/>" : string.Empty) + "<span style='color:#0000E5;'>" + "${ISI.Status.Desc2}" + "</span>&#58;&nbsp;" + ISIUtil.SetHighlight(task.Desc2, this.IsHighlight, this.Desc));
            }

            //((LinkButton)e.Row.FindControl("lbtnNew")).Text = "${ISI.TSK.RefTask}" + (task.RefTaskCount.HasValue && task.RefTaskCount.Value > 0 ? "&#58;&nbsp;" + task.RefTaskCount.Value.ToString() : string.Empty);

            if (task.RefTaskCount.HasValue && task.RefTaskCount.Value > 0)
            {
                //desc.Append("<br/><span style='color:#0000E5;'>${ISI.TSK.RefTask}</span>&#58;&nbsp;" + task.RefTaskCount.Value);

                var refTask = this.TheTaskMstrMgr.GetRefTask(task.TaskCode, 0, 5);
                e.Row.Cells[1].Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                e.Row.Cells[1].Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                e.Row.Cells[1].Attributes.Add("title", GetRefTasks(refTask));
            }

            ((Label)e.Row.FindControl("lblDesc")).Text = desc.ToString();

            if (!string.IsNullOrEmpty(task.ExpectedResults))
            {
                ((Label)e.Row.FindControl("lblExpectedResults")).Text = ISIUtil.SetHighlight(task.ExpectedResults, this.IsHighlight, this.Desc);
            }
            Label lblStatusDesc = (Label)e.Row.FindControl("lblStatusDesc");
            if (!string.IsNullOrEmpty(task.StatusDesc))
            {
                lblStatusDesc.Text = GetStatusDesc(task);
                if (task.StatusCount.HasValue && task.StatusCount.Value > 1)
                {
                    var taskStatus = this.TheTaskStatusMgr.GetTaskStatus(task.TaskCode, 0, task.CurrentStatusCount.HasValue && task.CurrentStatusCount.Value > 4 ? task.CurrentStatusCount.Value : 4);
                    e.Row.Cells[6].Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                    e.Row.Cells[6].Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                    e.Row.Cells[6].Attributes.Add("title", GetTaskStatus(taskStatus, task.CurrentStatusCount.HasValue ? task.CurrentStatusCount.Value : 0));
                }
            }
            else if (task.StatusDate.HasValue)
            {
                lblStatusDesc.Text = "<span style='color:#0000E5;'>" + task.StatusDate.Value.ToString("yyyy-MM-dd HH:mm") + "</span>";
            }

            if (!string.IsNullOrEmpty(task.Comment) && task.CommentCreateDate.HasValue)
            {
                ((Label)e.Row.FindControl("lblComment")).Text = GetCommentDesc(task);

                if (task.CommentCount.HasValue && task.CommentCount.Value > 1)
                {
                    var comments = this.TheCommentDetailMgr.GetComment(task.TaskCode, 0, task.CurrentCommentCount.HasValue && task.CurrentCommentCount.Value > 6 ? task.CurrentCommentCount.Value : 6);
                    ((Label)e.Row.FindControl("lblComment")).Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                    ((Label)e.Row.FindControl("lblComment")).Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                    ((Label)e.Row.FindControl("lblComment")).Attributes.Add("title", GetComments(comments, task.CurrentCommentCount.HasValue ? task.CurrentCommentCount.Value : 0));
                }
            }
            if (task.SubmitDate.HasValue)
            {
                e.Row.Cells[3].ToolTip = task.SubmitDate.Value.ToString("yyyy-MM-dd HH:mm");
            }
            else
            {
                e.Row.Cells[3].ToolTip = task.CreateDate.ToString("yyyy-MM-dd HH:mm");
            }
            if (task.AssignDate.HasValue)
            {
                e.Row.Cells[4].ToolTip = task.AssignDate.Value.ToString("yyyy-MM-dd HH:mm");
            }
            

            #region 按钮
            bool isISIAdmin = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN);
            bool isTaskFlowAdmin = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN);
            bool isCloser = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE);
            bool isAssigner = this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN);
            if ((task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS
                                        && this.TheTaskMgr.HasPermissionByProcess(task.Status, task.StartedUser, task.StartUpUser, task.CreateUser, task.SubmitUser, isISIAdmin, isTaskFlowAdmin, this.CurrentUser.Code))
                            || (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE
                                        && (this.TheTaskMgr.HasPermissionByComplete(task.Status, task.Type, task.CreateUser, task.SubmitUser, task.StartedUser,
                                                                                      task.ECUser, task.TaskSubTypeAssignUser,
                                                                                      task.AssignUpUser, task.CloseUpUser, task.CloseUpLevel,
                                                                                        isISIAdmin, isTaskFlowAdmin, isCloser, this.CurrentUser.Code)
                                                                || ISIUtil.Contains(task.StartedUser, this.CurrentUser.Code)
                                                                || task.CreateUser == this.CurrentUser.Code
                                                                || task.SubmitUser == this.CurrentUser.Code)))
            {
                var statusDiv = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("statusDiv"));
                //<a onclick="javascript:ShowStatus('<%# DataBinder.Eval(Container.DataItem, "Code")%>',<%# Container.DisplayIndex %>,<%# DataBinder.Eval(Container.DataItem, "CurrentStatusCount1")%>,<%# DataBinder.Eval(Container.DataItem, "StatusCount1")%>,'<%# DataBinder.Eval(Container.DataItem, "Flag")%>','<%# DataBinder.Eval(Container.DataItem, "Color")%>');" id="lnkStatus<%# DataBinder.Eval(Container.DataItem, "Code") %>" name="lnkStatus<%# DataBinder.Eval(Container.DataItem, "Code") %>" href="#">${ISI.Status.Status}</a>
                statusDiv.InnerHtml = "<a id='lnkStatus" + task.TaskCode + "' name='lnkStatus" + task.TaskCode + "' href='#' onclick=\"javascript:ShowStatus('" + task.TaskCode + "','" + task.Subject + "'," + e.Row.RowIndex + "," + task.CurrentStatusCount1 + "," + task.StatusCount1 + ",'" + task.Flag + "','" + task.Color + "');\">${ISI.Status.Status}</a>";
                statusDiv.Visible = true;
            }
            #endregion
        }
    }

    private string GetAttachmentDesc(string fileName, string desc, bool isHighlight, Int32? currentAttachmentCount, Int32? attachmentCount, bool isFirst, bool isLast)
    {
        StringBuilder html = new StringBuilder();
        if (isFirst)
        {
            html.Append("<span style='color:#0000E5;'>${ISI.Status.Attachment}：");
        }
        if (isHighlight)
        {
            html.Append("<span style='color:fuchsia;'>");
        }
        //html.Append(ISIUtil.SetHighlight(ISIUtil.GetStrLength(task.FileName, 16), this.IsHighlight, this.Desc));
        html.Append(ISIUtil.SetHighlight(fileName, isHighlight, desc));

        if (isLast)
        {
            html.Append("&#40;");
            if (currentAttachmentCount.HasValue && currentAttachmentCount > 0)
            {
                html.Append(currentAttachmentCount.Value);
                html.Append("&#47;");
            }
            html.Append(attachmentCount.Value);
            html.Append("&#41;");
        }
        if (isHighlight)
        {
            html.Append("</span>");
        }
        if (isFirst)
        {
            html.Append("</span>");
        }
        return html.ToString();
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
        html.Append(ISIUtil.SetHighlight(task.Comment, this.IsHighlight, this.Desc));

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
        html.Append(ISIUtil.SetHighlight(task.StatusDesc, this.IsHighlight, this.Desc));

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

    #region 处理Tip
    private string GetStartUser(string users)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.Status.StartUser}] body=[<table width=100%>");
        detail.Append("<tr><td>" + users.Replace("<br/>", "、") + "</td></tr>");
        detail.Append("</table>]");
        return detail.ToString();
    }

    private string GetRefTasks(IList<Task> refTasks)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.TSK.RefTask}] body=[<table width=100%>");

        foreach (var refTask in refTasks)
        {
            detail.Append("<tr><td><span style='color:#0000E5;'>" + refTask.Subject.Replace("[", "&#91;").Replace("]", "&#93;") + "&#40;" + refTask.Code + "&#41;</span>&#58;&nbsp;" + (refTask.Desc1.Replace("[", "&#91;").Replace("]", "&#93;") + (!string.IsNullOrEmpty(refTask.Desc2) ? (!string.IsNullOrEmpty(refTask.Desc1) ? "<br/>" : string.Empty) + "<span style='font-style:italic;'>" + "${ISI.Status.Desc2}" + "</span>&#58;&nbsp;" + refTask.Desc2.Replace("[", "&#91;").Replace("]", "&#93;") : string.Empty)).Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") + "</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }

    private string GetAttachments(IList<AttachmentDetail> attachments, int count)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.Status.Attachment}] body=[<table width=100%>");

        foreach (var attachment in attachments)
        {
            detail.Append("<tr><td><span style='color:#0000E5;'>");
            detail.Append(attachment.CreateUserNm);
            detail.Append("&#40;");
            if (count > 0)
            {
                detail.Append("<span style='color:fuchsia;'>");
            }
            detail.Append(attachment.CreateDate.ToString("yyyy-MM-dd HH:mm"));
            if (count > 0)
            {
                detail.Append("</span>");
                count--;
            }
            detail.Append("&#41;</span>&#58;&nbsp;");
            detail.Append(attachment.FileName.Replace("[", "&#91;").Replace("]", "&#93;"));

            detail.Append("</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }

    private string GetComments(IList<Comment> comments, int count)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.Status.Comment}] body=[<table width=100%>");
        comments.RemoveAt(0);
        foreach (var comment in comments)
        {
            detail.Append("<tr><td><span style='color:#0000E5;'>");
            detail.Append(comment.CreateUser);
            detail.Append("&#40;");
            if (count > 1)
            {
                detail.Append("<span style='color:fuchsia;'>");
            }
            detail.Append(comment.CreateDate.ToString("yyyy-MM-dd HH:mm"));
            if (count > 1)
            {
                detail.Append("</span>");
                count--;
            }
            detail.Append("&#41;</span>&#58;&nbsp;");
            detail.Append(ISIUtil.SetHighlight(comment.Value.Replace("[", "&#91;").Replace("]", "&#93;"), this.IsHighlight, Desc));
            detail.Append("</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }

    private string GetTaskStatus(IList<TaskStatus> taskStatus, int count)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.Status.StatusDesc}] body=[<table width=100%>");
        taskStatus.RemoveAt(0);
        foreach (var status in taskStatus)
        {
            detail.Append("<tr><td><span style='color:#0000E5;'>" + status.CreateUserNm + "&#40;");
            if (count > 1)
            {
                detail.Append("<span style='color:fuchsia;'>");
            }
            detail.Append(status.LastModifyDate.ToString("yyyy-MM-dd HH:mm"));
            if (count > 1)
            {
                detail.Append("</span>");
                count--;
            }
            detail.Append("&#41;</span>&#58;&nbsp;");
            detail.Append(ISIUtil.SetHighlight(status.Desc.Replace("[", "&#91;").Replace("]", "&#93;"), this.IsHighlight, Desc));
            detail.Append("</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }
    #endregion


    public class Attachment
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; }
    }
}
