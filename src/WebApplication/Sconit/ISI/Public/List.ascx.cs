using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using System.Text;

public partial class ISI_Public_List : ListModuleBase
{
    public EventHandler EditEvent;

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string taskCode = ((LinkButton)sender).CommandArgument;
            EditEvent(taskCode, e);
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code = ((LinkButton)sender).CommandArgument;
        TaskMstr task = this.TheTaskMstrMgr.LoadTaskMstr(code);
        try
        {
            TheTaskMgr.DeleteTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Delete" + task.Type + ".Successfully", code);
            UpdateView();
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            //TheTaskMgr.DeleteTaskMstr(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Delete" + task.Type + ".Fail", code);
        }
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

    #region 处理Tip
    private string GetStartUser(string users)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.Status.StartUser}] body=[<table width=100%>");
        detail.Append("<tr><td>" + users.Replace("<br>", "、") + "</td></tr>");
        detail.Append("</table>]");
        return detail.ToString();
    }


    private string GetRefTasks(IList<Task> refTasks)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.TSK.RefTask}] body=[<table width=100%>");

        foreach (var refTask in refTasks)
        {
            detail.Append("<tr><td><span style='color:#0000E5;'>" + refTask.Subject + "&#40;" + refTask.Code + "&#41;</span>&#58;&nbsp;" + (refTask.Desc1 + (!string.IsNullOrEmpty(refTask.Desc2) ? (!string.IsNullOrEmpty(refTask.Desc1) ? "<br/>" : string.Empty) + "<span style='font-style:italic;'>" + "${ISI.Status.Desc2}" + "</span>&#58;&nbsp;" + refTask.Desc2 : string.Empty)).Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") + "</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }

    private string GetAttachments(IList<AttachmentDetail> attachments)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.Status.Attachment}] body=[<table width=100%>");

        foreach (var attachment in attachments)
        {
            detail.Append("<tr><td><span style='color:#0000E5;'>" + attachment.CreateUserNm + "&#40;" + attachment.CreateDate.ToString("yyyy-MM-dd HH:mm") + "&#41;</span>&#58;&nbsp;" + attachment.FileName + "</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }

    private string GetComments(IList<Comment> comments)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.Status.Comment}] body=[<table width=100%>");
        comments.RemoveAt(0);
        foreach (var comment in comments)
        {
            detail.Append("<tr><td><span style='color:#0000E5;'>" + comment.CreateUser + "&#40;" + comment.CreateDate.ToString("yyyy-MM-dd HH:mm") + "&#41;</span>&#58;&nbsp;" + comment.Value.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") + "</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }

    private string GetTaskStatus(IList<TaskStatus> taskStatus)
    {
        StringBuilder detail = new StringBuilder();
        detail.Append("cssbody=[obbd] cssheader=[obhd] header=[${ISI.Status.StatusDesc}] body=[<table width=100%>");
        taskStatus.RemoveAt(0);
        foreach (var status in taskStatus)
        {
            detail.Append("<tr><td><span style='color:#0000E5;'>" + status.CreateUserNm + "&#40;" + status.LastModifyDate.ToString("yyyy-MM-dd HH:mm") + "&#41;</span>&#58;&nbsp;" + status.Desc.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") + "</td></tr>");
        }
        detail.Append("</table>]");
        return detail.ToString();
    }

    #endregion

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        TaskStatusView task = (TaskStatusView)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 0; i < e.Row.Cells.Count; i++)
            {
                if (i == 3 || i == 4 || i == 5 || i == 7) continue;
                e.Row.Cells[i].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            }
            Label lblStatus = ((Label)(e.Row.FindControl("lblStatus")));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(task.Status, this.CurrentUser);
            Label lblSubject = ((Label)(e.Row.FindControl("lblSubject")));
            if (!string.IsNullOrEmpty(task.Subject))
            {
                lblSubject.Text = task.Subject.Trim();
            }

            if (!string.IsNullOrEmpty(lblSubject.Text))
            {
                lblSubject.Text += "<br>";
            }
            lblSubject.Text = task.CreateDate.ToString("yyyy-MM-dd");

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
                if (task.PlanStartDate.HasValue && task.PlanCompleteDate.HasValue && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL)
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
            StringBuilder desc = new StringBuilder();
            if (!string.IsNullOrEmpty(task.Desc1))
            {
                desc.Append(task.Desc1.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
            }
            if (!string.IsNullOrEmpty(task.Desc2))
            {
                desc.Append((!string.IsNullOrEmpty(task.Desc1) ? "<br/>" : string.Empty) + "<span style='color:#0000E5;'>" + "${ISI.Status.Desc2}" + "</span>&#58;&nbsp;" + task.Desc2.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>"));
            }

            if (task.RefTaskCount.HasValue && task.RefTaskCount.Value > 0)
            {
                desc.Append("<br/><span style='color:#0000E5;'>${ISI.TSK.RefTask}</span>&#58;&nbsp;" + task.RefTaskCount.Value);

                var refTask = this.TheTaskMstrMgr.GetRefTask(task.TaskCode, 0, 5);
                e.Row.Cells[1].Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                e.Row.Cells[1].Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                e.Row.Cells[1].Attributes.Add("title", GetRefTasks(refTask));
            }

            ((Label)e.Row.FindControl("lblDesc")).Text = desc.ToString();

            if (!string.IsNullOrEmpty(task.ExpectedResults))
            {
                ((Label)e.Row.FindControl("lblExpectedResults")).Text = task.ExpectedResults.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>");
            }
            if (!string.IsNullOrEmpty(task.StatusDesc))
            {
                ((Label)e.Row.FindControl("lblStatusDesc")).Text = "<span style='color:#0000E5;'>" + task.CreateUserNm + "&#40;" + task.StatusDate.Value.ToString("yyyy-MM-dd HH:mm") + "&#41;</span>&#58;&nbsp;" + task.StatusDesc.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") + (task.StatusCount.HasValue && task.StatusCount.Value > 1 ? "<span style='color:#0000E5;'>&#40;" + task.StatusCount.Value + "&#41;</span>" : string.Empty);
                if (task.StatusCount.HasValue && task.StatusCount.Value > 1)
                {
                    var taskStatus = this.TheTaskStatusMgr.GetTaskStatus(task.TaskCode, 0, 5);
                    e.Row.Cells[6].Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                    e.Row.Cells[6].Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                    e.Row.Cells[6].Attributes.Add("title", GetTaskStatus(taskStatus));
                }
            }
            else if (task.StatusDate.HasValue)
            {
                ((Label)e.Row.FindControl("lblStatusDesc")).Text = "<span style='color:#0000E5;'>" + task.StatusDate.Value.ToString("yyyy-MM-dd HH:mm") + "</span>";
            }
            if (!string.IsNullOrEmpty(task.Comment) && task.CommentCreateDate.HasValue)
            {
                ((Label)e.Row.FindControl("lblComment")).Text = "<span style='color:#0000E5;'>" + task.CommentCreateUserNm + "&#40;" + task.CommentCreateDate.Value.ToString("yyyy-MM-dd HH:mm") + "&#41;</span>&#58;&nbsp;" + task.Comment.Replace(ISIConstants.TEXT_SEPRATOR, "<br/>").Replace(ISIConstants.TEXT_SEPRATOR2, "<br/>") + (task.CommentCount.HasValue && task.CommentCount.Value > 1 ? "<span style='color:#0000E5;'>&#40;" + task.CommentCount.Value + "&#41;</span>" : string.Empty);

                if (task.CommentCount.HasValue && task.CommentCount.Value > 1)
                {
                    var comments = this.TheCommentDetailMgr.GetComment(task.TaskCode, 0, 6);
                    ((Label)e.Row.FindControl("lblComment")).Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                    ((Label)e.Row.FindControl("lblComment")).Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                    ((Label)e.Row.FindControl("lblComment")).Attributes.Add("title", GetComments(comments));
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
            //if (task.AttachmentCount.HasValue && task.AttachmentCount.Value > 0)
            //{
            //    LinkButton lbtnDownLoad = (LinkButton)e.Row.FindControl("lbtnDownLoad");
            //    lbtnDownLoad.Text = "<br><span style='color:#0000E5;'>${ISI.Status.Attachment}：" + ISIUtil.GetStrLength(task.FileName, 16) + "&#40;" + task.AttachmentCount.Value + "&#41;</span>";
            //    lbtnDownLoad.Visible = true;

            //    var attachmentList = this.TheAttachmentDetailMgr.GetAttachment(task.TaskCode, 0, 10);
            //    lbtnDownLoad.Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
            //    lbtnDownLoad.Attributes.Add("onmouseout", "this.style.backgroundColor=e");
            //    lbtnDownLoad.Attributes.Add("title", GetAttachments(attachmentList));
            //}
        }
    }

    public void Export()
    {
        this.ExportXLS(GV_List);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            
        }
    }

    public class Attachment
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; }
    }
}
