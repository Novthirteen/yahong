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
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;

public partial class ISI_Status_List : ListModuleBase
{
    public bool IsToDoList
    {
        get
        {
            return ViewState["IsToDoList"] != null ? (bool)ViewState["IsToDoList"] : false;
        }
        set
        {
            ViewState["IsToDoList"] = value;
        }
    }
    public string Type
    {
        get
        {
            return (string)ViewState["Type"];
        }
        set
        {
            ViewState["Type"] = value;
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
    /// <summary>
    /// 本周周一
    /// </summary>
    public DateTime Monday
    {
        get
        {
            return ViewState["Monday"] != null ? (DateTime)ViewState["Monday"] : ISIUtil.GetMondayDate();
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
            return ViewState["LastMonday"] != null ? (DateTime)ViewState["LastMonday"] : Monday.AddDays(-7);
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
            return ViewState["LastLastMonday"] != null ? (DateTime)ViewState["LastLastMonday"] : LastMonday.AddDays(-7);
        }
        set
        {
            ViewState["LastLastMonday"] = value;
        }
    }
    /// <summary>
    /// 附件扩展名
    /// </summary>
    public string FileExtensions
    {
        get
        {
            return (string)ViewState["FileExtensions"];
        }
        set
        {
            ViewState["FileExtensions"] = value;
        }
    }
    /// <summary>
    ///  附件文件大小
    /// </summary>
    public int ContentLength
    {
        get
        {
            return (int)ViewState["ContentLength"];
        }
        set
        {
            ViewState["ContentLength"] = value;
        }
    }
    public bool IsHighlight
    {
        get
        {
            return (bool)ViewState["IsHighlight"];
        }
        set
        {
            ViewState["IsHighlight"] = value;
        }
    }

    public bool IsStatus
    {
        get
        {
            return (bool)ViewState["IsStatus"];
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
    public bool IsMine
    {
        get
        {
            return ViewState["IsMine"] != null ? (bool)ViewState["IsMine"] : false;
        }
        set
        {
            ViewState["IsMine"] = value;
        }
    }

    public bool IsFocus
    {
        get
        {
            return (bool)ViewState["IsFocus"];
        }
        set
        {
            ViewState["IsFocus"] = value;
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

    public EventHandler EditEvent;

    public EventHandler NewEvent;
    public override void UpdateView()
    {
        UpdateView(string.Empty);
    }
    public void UpdateView(string taskCode)
    {
        if (this.Type == ISIConstants.ISI_TASK_TYPE_PROJECT_ISSUE)
        {
            this.GV_List.Columns[0].HeaderText = "${ISI.TSK.PrjIss.Subject}";
            this.GV_List.Columns[1].HeaderText = "${ISI.TSK.PrjIss.Desc1}";
            this.GV_List.Columns[2].HeaderText = "${ISI.TSK.PrjIss.ExpectedResults}";
        }
        if (this.Type == ISIConstants.ISI_TASK_TYPE_ENGINEERING_CHANGE)
        {
            this.GV_List.Columns[0].HeaderText = "${ISI.TSK.Enc.Subject}";
            this.GV_List.Columns[1].HeaderText = "${ISI.TSK.Enc.Desc1}";
            this.GV_List.Columns[2].HeaderText = "${ISI.TSK.Enc.ExpectedResults}";
        }
        else
        {
            this.GV_List.Columns[0].HeaderText = "${ISI.Status.Subject}";
            this.GV_List.Columns[1].HeaderText = "${Common.Business.Description}";
            this.GV_List.Columns[2].HeaderText = "${ISI.TSK.ExpectedResults}";
        }

        //this.GV_List.FindPager().CurrentPageIndex = 1;
        this.GV_List.Execute();
        this.SetAnchor(taskCode);
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string taskCode = ((LinkButton)sender).CommandArgument;
            EditEvent(taskCode, e);
        }
    }

    protected void lbtnNew_Click(object sender, EventArgs e)
    {
        if (NewEvent != null)
        {
            string argument = ((LinkButton)sender).CommandArgument;
            string[] arg = argument.Substring(1, argument.Length - 2).Split(new string[] { "}{" }, StringSplitOptions.None);

            NewEvent(arg, e);
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
            Response.ClearHeaders();
            Response.Buffer = false;
            Response.ContentType = attachment.ContentType;
            Response.AddHeader("Content-Disposition", "attachment;filename=" + Server.UrlEncode(attachment.FileName));
            Response.AddHeader("Content-Length", FI.Length.ToString());
            Response.Filter.Close();
            Response.WriteFile(FI.FullName);
            Response.Flush();
            Response.End();
        }
    }


    protected void GV_List_DataBound(object sender, EventArgs e)
    {
        var gridView = (com.Sconit.Control.GridView)sender;
        IList<string> taskCodeApplyList = new List<string>();
        IList<string> taskCodeFormTypeList = new List<string>();
        IList<string> taskCodeList = new List<string>();
        foreach (GridViewRow row in gridView.Rows)
        {
            HiddenField hfIsApply = (HiddenField)row.FindControl("hfIsApply");
            HiddenField hfFormType = (HiddenField)row.FindControl("hfFormType");
            LinkButton lbtnEdit = (LinkButton)row.FindControl("lbtnEdit");
            if (hfIsApply != null && bool.Parse(hfIsApply.Value))
            {
                taskCodeApplyList.Add(lbtnEdit.CommandArgument);
            }
            if (!string.IsNullOrEmpty(hfFormType.Value))
            {
                taskCodeFormTypeList.Add(lbtnEdit.CommandArgument);
            }
            taskCodeList.Add(lbtnEdit.CommandArgument);
        }

        if (IsStatus)
        {
            gridView.Columns[0].HeaderStyle.Width = new Unit(4, UnitType.Percentage);
            gridView.Columns[0].ItemStyle.Width = new Unit(4, UnitType.Percentage);
            gridView.Columns[1].HeaderStyle.Width = new Unit(14, UnitType.Percentage);
            gridView.Columns[1].ItemStyle.Width = new Unit(14, UnitType.Percentage);
            gridView.Columns[5].HeaderStyle.Width = new Unit(2, UnitType.Percentage);
            gridView.Columns[5].ItemStyle.Width = new Unit(2, UnitType.Percentage);
            gridView.Columns[6].HeaderStyle.Width = new Unit(62, UnitType.Percentage);
            gridView.Columns[6].ItemStyle.Width = new Unit(62, UnitType.Percentage);
            gridView.Columns[8].HeaderStyle.Width = new Unit(18, UnitType.Percentage);
            gridView.Columns[8].ItemStyle.Width = new Unit(18, UnitType.Percentage);
            gridView.Columns[2].Visible = false;
            gridView.Columns[3].Visible = false;
            gridView.Columns[4].Visible = false;
            gridView.Columns[7].Visible = false;
        }
        else
        {
            gridView.Columns[0].HeaderStyle.Width = new Unit(0, UnitType.Percentage);
            gridView.Columns[0].ItemStyle.Width = new Unit(0, UnitType.Percentage);
            gridView.Columns[1].HeaderStyle.Width = new Unit(22, UnitType.Percentage);
            gridView.Columns[1].ItemStyle.Width = new Unit(22, UnitType.Percentage);
            gridView.Columns[5].HeaderStyle.Width = new Unit(0, UnitType.Percentage);
            gridView.Columns[5].ItemStyle.Width = new Unit(0, UnitType.Percentage);
            gridView.Columns[6].HeaderStyle.Width = new Unit(22, UnitType.Percentage);
            gridView.Columns[6].ItemStyle.Width = new Unit(22, UnitType.Percentage);
            gridView.Columns[8].HeaderStyle.Width = new Unit(15, UnitType.Percentage);
            gridView.Columns[8].ItemStyle.Width = new Unit(15, UnitType.Percentage);

            gridView.Columns[2].Visible = true;
            gridView.Columns[3].Visible = true;
            gridView.Columns[4].Visible = true;
            gridView.Columns[7].Visible = true;
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
        IList<Cost> costList = null;
        if (taskCodeApplyList.Count > 0)
        {
            taskApplyList = this.TheTaskApplyMgr.GetTaskApply(taskCodeApplyList);
        }
        if (taskCodeFormTypeList.Count > 0)
        {
            costList = this.TheCostMgr.GetCost(taskCodeFormTypeList);
        }
        if (taskApplyList != null && taskApplyList.Count > 0
                        || taskStatusDic != null && taskStatusDic.Count > 0
                        || commentDic != null && commentDic.Count > 0
                        || attachmentDetailDic != null && attachmentDetailDic.Count > 0
                        || costList != null && costList.Count > 0)
        {
            foreach (GridViewRow row in gridView.Rows)
            {
                //var statusDiv = (System.Web.UI.HtmlControls.HtmlGenericControl)(row.FindControl("statusDiv"));
                //var spanComplete = (System.Web.UI.HtmlControls.HtmlGenericControl)(row.FindControl("spanComplete"));
                //var spanApprove = (System.Web.UI.HtmlControls.HtmlGenericControl)(row.FindControl("spanApprove"));

                LinkButton lbtnEdit = (LinkButton)row.FindControl("lbtnEdit");

                if (attachmentDetailDic != null && attachmentDetailDic.Count > 0 && attachmentDetailDic.Keys.Contains(lbtnEdit.CommandArgument))
                {
                    GetAttachmentDesc(attachmentDetailDic[lbtnEdit.CommandArgument], row);
                }

                if (taskStatusDic != null && taskStatusDic.Count > 0 && taskStatusDic.Keys.Contains(lbtnEdit.CommandArgument))
                {
                    Label lblStatusDesc = (Label)row.FindControl("lblStatusDesc");
                    //GetStatusDesc(taskStatusDic[lbtnEdit.CommandArgument], lblStatusDesc, statusDiv, spanComplete, spanApprove);
                    GetStatusDesc(taskStatusDic[lbtnEdit.CommandArgument], lblStatusDesc);
                }
                else
                {
                    //this.SetCount(statusDiv, spanComplete, spanApprove, 0, 0);
                }

                if (commentDic != null && commentDic.Count > 0 && commentDic.Keys.Contains(lbtnEdit.CommandArgument))
                {
                    Label lblComment = ((Label)row.FindControl("lblComment"));
                    GetCommentDesc(commentDic[lbtnEdit.CommandArgument], lblComment);
                }

                if (taskApplyList != null && taskApplyList.Count > 0)
                {
                    var thisRowTaskApplyList = taskApplyList.Where(t => t.TaskCode == lbtnEdit.CommandArgument).ToList();
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


                //输出表单明细
                if (costList != null && costList.Count > 0)
                {
                    HiddenField hfFormType = (HiddenField)row.FindControl("hfFormType");
                    if (!string.IsNullOrEmpty(hfFormType.Value))
                    {
                        var thisRowCostList = costList.Where(t => t.TaskCode == lbtnEdit.CommandArgument).ToList();
                        if (thisRowCostList != null && thisRowCostList.Count > 0)
                        {
                            StringBuilder descBufer = new StringBuilder();
                            Label lblDesc = (Label)row.FindControl("lblDesc");
                            foreach (var cost in thisRowCostList)
                            {
                                descBufer.Append(ISIConstants.EMAIL_SEPRATOR);
                                Append(descBufer, "WFS.Cost.UserName", cost.User);
                                Append(descBufer, "WFS.Cost.Desc", cost.Desc1);
                                if (hfFormType.Value == ISIConstants.CODE_MASTER_WFS_FORMTYPE_1)
                                {
                                    Append(descBufer, "WFS.Cost.ExtNo", cost.ExtNo);
                                    AppendAmount(descBufer, "WFS.Cost.NoTaxAmount", cost.NoTaxAmount);
                                    AppendAmount(descBufer, "WFS.Cost.Taxes", cost.Taxes);
                                    AppendAmount(descBufer, "WFS.Cost.TotalAmount", cost.TotalAmount);
                                }
                                else if (hfFormType.Value == ISIConstants.CODE_MASTER_WFS_FORMTYPE_5)
                                {
                                    //品名
                                    descBufer.Append(cost.Item);
                                    if (cost.Qty.HasValue)
                                    {
                                        descBufer.Append(cost.Qty.Value.ToString("    0.########"));
                                    }
                                    if (!String.IsNullOrEmpty(cost.Uom))
                                    {
                                        descBufer.Append(cost.Uom);
                                    }
                                }
                                else if (hfFormType.Value == ISIConstants.CODE_MASTER_WFS_FORMTYPE_4)
                                {
                                    Append(descBufer, "WFS.Cost.ApplicationDate", cost.EndDate);
                                    Append(descBufer, "WFS.Cost.StartAddr", cost.StartAddr);
                                    Append(descBufer, "WFS.Cost.EndAddr", cost.EndAddr);
                                }
                                else
                                {
                                    if (hfFormType.Value == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2)
                                    {
                                        Append(descBufer, "WFS.Cost.StartDate", cost.StartDate);
                                        Append(descBufer, "WFS.Cost.EndDate", cost.EndDate);
                                        Append(descBufer, "WFS.Cost.StartAddr", cost.StartAddr);
                                        Append(descBufer, "WFS.Cost.EndAddr", cost.EndAddr);
                                        Append(descBufer, "WFS.Cost.Vehicle", cost.Vehicle);

                                        AppendAmount(descBufer, "WFS.Cost.Allowance", cost.Allowance);
                                        AppendAmount(descBufer, "WFS.Cost.Fare", cost.Fare);
                                        AppendAmount(descBufer, "WFS.Cost.Quarterage", cost.Quarterage);
                                        AppendAmount(descBufer, "WFS.Cost.Haulage", cost.Haulage);
                                        Append(descBufer, "WFS.Cost.ExtNo", cost.ExtNo);
                                        AppendAmount(descBufer, "WFS.Cost.NoTaxAmount", cost.NoTaxAmount);
                                        AppendAmount(descBufer, "WFS.Cost.Taxes", cost.Taxes);
                                        AppendAmount(descBufer, "WFS.Cost.TotalAmount", cost.TotalAmount);
                                    }
                                    else if (hfFormType.Value == ISIConstants.CODE_MASTER_WFS_FORMTYPE_3)
                                    {
                                        Append(descBufer, "WFS.Cost.StartDate", cost.StartDate);
                                        Append(descBufer, "WFS.Cost.EndDate", cost.EndDate);
                                    }
                                }
                            }

                            if (descBufer.Length > 0)
                            {
                                lblDesc.Text += descBufer.ToString();
                            }
                        }
                    }
                }
            }
        }
    }

    private void SetStartedUser(TaskStatusView task, Label lblStartedUser, TableCell cell)
    {
        lblStartedUser.Text = string.Empty;
        if (task.StartedUserCount > 0)
        {
            string assignStartUserNm = task.AssignStartUserNm;
            if (task.StartedUserCount > 0 && string.IsNullOrEmpty(assignStartUserNm))
            {
                assignStartUserNm = this.TheUserSubscriptionMgr.GetUserName(task.StartedUser);
            }
            if (!IsStatus && task.StartedUserCount > 6)
            {
                lblStartedUser.Text += ISIUtil.GetUserName(task.GetAssignStartUserNm(6, this.CurrentUser.Code), this.CurrentUser.Name, "blue");//.Replace(", ", "<br/>");
                cell.Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                cell.Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                cell.Attributes.Add("title", GetStartUser(assignStartUserNm));
            }
            else
            {
                lblStartedUser.Text += ISIUtil.GetUserName(assignStartUserNm, this.CurrentUser.Name, "blue");//.Replace(", ", "<br />");
            }
        }

        if (task.IsWF && !string.IsNullOrEmpty(task.ApprovalUserNm))
        {
            if (lblStartedUser.Text != string.Empty)
            {
                lblStartedUser.Text += "<hr>";
            }

            lblStartedUser.Text += ISIUtil.GetUserName(task.ApprovalUserNm, this.CurrentUser.Name, task.Level, task.ApprovalLevel, task.CurrentApprovalUserNm, "blue", "fuchsia");//.Replace(", ", "<br/>");
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.Header)
        {
            int i = 0;

            e.Row.Cells[i++].ToolTip = "${ISI.Status.CreateDate}";
            e.Row.Cells[i++].ToolTip = "${ISI.Status.RefTaskCount}";
            e.Row.Cells[i++].ToolTip = "${ISI.Status.OrderByPlanStartDate}";
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
            bool isISIAdmin = !IsMine && this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN);
            bool isWFAdmin = !IsMine && this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_WF_TASK_VALUE_WFADMIN);
            bool isTaskFlowAdmin = !IsMine && this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_TASKFLOWADMIN);
            bool isCloser = !IsMine && this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_CLOSE);
            bool isAssigner = !IsMine && this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ASSIGN);
            bool isViewer = !IsMine && this.CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_VIEW);
            bool isDeleteAttachment = !IsMine && this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_VALUE_DELETEATTACHMENT);

            Image imFlag = (Image)(e.Row.FindControl("imFlag"));
            if (ISIUtil.Contains(task.FocusUser, this.CurrentUser.Code))
            {
                imFlag.ImageUrl = "~/Images/ISI/redflag1.png";
            }
            else
            {
                imFlag.ImageUrl = "~/Images/ISI/whiteflag1.png";
            }
            imFlag.Attributes["onclick"] = "UpdateFocus(this, \"" + task.TaskCode + "\")";

            Label lblTaskSubTypeCode = ((Label)(e.Row.FindControl("lblTaskSubTypeCode")));
            if (!string.IsNullOrEmpty(task.TaskSubTypeCode))
            {
                lblTaskSubTypeCode.Text = "&#91;" + task.TaskSubTypeCode + "&#93;" + (!string.IsNullOrEmpty(task.FailureModeDesc) ? "<br>" + ISIUtil.SetHighlight(task.FailureModeDesc, this.IsHighlight, this.Desc) : string.Empty);
                lblTaskSubTypeCode.ToolTip = task.TaskSubTypeDesc;
            }
            Label lblStatus = ((Label)(e.Row.FindControl("lblStatus")));
            lblStatus.Text = "${ISI.Status." + task.Status + "}";
            Label lblLevel = ((Label)(e.Row.FindControl("lblLevel")));
            lblLevel.Visible = task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN || task.Level.HasValue;
            //e.Row.FindControl("downLoadDiv").Visible = this.TheTaskMgr.HasAttachmentPermission(task.StartedUser, task.AssignUser, task.AssignUpUser, isISIAdmin, isTaskFlowAdmin, isViewer, isAssigner, isCloser, isDeleteAttachment, this.CurrentUser.Code);
            lblLevel.Text = ISIUtil.GetLevelDesc(task.Level);

            Label lblSubject = ((Label)(e.Row.FindControl("lblSubject")));
            if (!string.IsNullOrEmpty(task.Subject))
            {
                lblSubject.Text = ISIUtil.SetHighlight(task.Subject.Trim(), this.IsHighlight, this.Desc);
            }

            Label lblStartedUser = ((Label)(e.Row.FindControl("lblStartedUser")));
            SetStartedUser(task, lblStartedUser, e.Row.Cells[5]);

            TextBox tbEndDate = (TextBox)e.Row.FindControl("tbEndDate");
            TextBox tbStartDate = (TextBox)e.Row.FindControl("tbStartDate");
            Label lblEndDate = (Label)e.Row.FindControl("lblEndDate");
            Label lblStartDate = (Label)e.Row.FindControl("lblStartDate");
            Label lblPlanStartDate = (Label)e.Row.FindControl("lblPlanStartDate");
            DateTime consult = DateTime.Now;
            if (task.PlanStartDate.HasValue)
            {
                if (task.PlanStartDate.Value < consult && (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN
                                || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT))
                {
                    lblPlanStartDate.ForeColor = System.Drawing.Color.Red;
                }
                lblPlanStartDate.Visible = true;
                lblStartDate.Visible = true;
                tbStartDate.Visible = true;
                string planCompleteDate = string.Empty;
                if (task.PlanCompleteDate.HasValue)
                {
                    planCompleteDate = task.PlanCompleteDate.Value.ToString("yyyy-MM-dd");
                }
                tbStartDate.Attributes["onchange"] = "UpdatePlanStartDate(\"" + task.TaskCode + "\",\"" + tbStartDate.ClientID + "\",\"" + task.PlanStartDate.Value.ToString("yyyy-MM-dd") + "\",\"" + planCompleteDate + "\",\"" + lblStartDate.ClientID + "\")";
            }

            if (task.PlanCompleteDate.HasValue)
            {

                if (task.CompleteDate.HasValue
                        && (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE
                                    || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CLOSE))
                {
                    consult = task.CompleteDate.Value;
                }
                if (task.PlanStartDate.HasValue && task.PlanCompleteDate.HasValue)
                {
                    e.Row.Cells[2].ToolTip = "${ISI.Status.Deadline}: " + task.PlanStartDate.Value.ToString("yyyy-MM-dd") + "/" + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd");
                    //((Label)e.Row.FindControl("lblEndDate")).Text = task.PlanCompleteDate.Value.ToString("yyyy-MM-dd");

                    if (task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CANCEL && task.Status != ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE)
                    {
                        if (task.PlanCompleteDate.Value.CompareTo(consult) <= 0)
                        {
                            ((Label)e.Row.FindControl("lblDeadline")).Text = "<span style='color: Red;'>${ISI.Status.Deadline}:</span>";
                        }
                        else
                        {
                            double startPercent = task.StartPercent.HasValue ? double.Parse(task.StartPercent.Value.ToString()) : 0.7;
                            TimeSpan ts = task.PlanCompleteDate.Value.Subtract(task.PlanStartDate.Value);
                            int milliseconds = (int)Math.Ceiling(ts.TotalMilliseconds * startPercent);
                            if (task.PlanStartDate.Value.AddMilliseconds(milliseconds).CompareTo(consult) > 0)
                            {
                                ((Label)e.Row.FindControl("lblDeadline")).Text = "<span style='color: Green;'>${ISI.Status.Deadline}:</span>";
                            }
                            else
                            {
                                ((Label)e.Row.FindControl("lblDeadline")).Text = "<span style='color: #FFCC00;'>${ISI.Status.Deadline}:</span>";
                            }
                        }
                    }
                    tbEndDate.Visible = true;
                    string planStartDate = string.Empty;
                    if (task.PlanStartDate.HasValue)
                    {
                        planStartDate = task.PlanStartDate.Value.ToString("yyyy-MM-dd");
                    }
                    tbEndDate.Attributes["onchange"] = "UpdatePlanCompleteDate(\"" + task.TaskCode + "\",\"" + tbEndDate.ClientID + "\",\"" + task.PlanCompleteDate.Value.ToString("yyyy-MM-dd") + "\",\"" + planStartDate + "\",\"" + lblEndDate.ClientID + "\")";
                }
                else
                {
                    //e.Row.FindControl("lblDeadline").Visible = false;
                }
            }

            if (!string.IsNullOrEmpty(task.Color))
            {
                e.Row.Cells[7].Attributes["style"] = "background-color:" + task.Color;
            }
            if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_URGENT)
            {
                ((LinkButton)e.Row.FindControl("lbtnEdit")).ForeColor = System.Drawing.Color.Red;
            }
            else if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_HIGH)
            {
                ((LinkButton)e.Row.FindControl("lbtnEdit")).ForeColor = System.Drawing.Color.MediumVioletRed;
            }
            else if (task.Priority == ISIConstants.CODE_MASTER_ISI_PRIORITY_LOW)
            {
                ((LinkButton)e.Row.FindControl("lbtnEdit")).ForeColor = System.Drawing.Color.Black;
            }
            ((LinkButton)e.Row.FindControl("lbtnEdit")).ToolTip = task.Priority;

            ((LinkButton)e.Row.FindControl("lbtnEdit")).Text = ISIUtil.SetHighlight(task.TaskCode, this.IsHighlight, this.Desc);
            StringBuilder desc = new StringBuilder();
            if (!string.IsNullOrEmpty(task.SupplierCode))
            {
                desc.Append("<span style='color:#0000E5;'>" + "${Common.Business.Supplier}" + "</span>&#58;&nbsp;");
                desc.Append(ISIUtil.SetHighlight(task.SupplierDesc, this.IsHighlight, this.Desc));
            }
            /*
            if (task.Type == ISIConstants.ISI_TASK_TYPE_IMPROVE && task.Amount.HasValue)
            {
                if (desc.Length > 0)
                {
                    desc.Append("<br/>");
                }
                desc.Append("<span style='color:#0000E5;'>" + "${ISI.TSK.ImpAmount}" + "</span>&#58;&nbsp;");
                desc.Append(task.Amount.Value.ToString("0.########"));
            }
             */
            if (!string.IsNullOrEmpty(task.Desc1))
            {
                if (desc.Length > 0)
                {
                    desc.Append("<br/>");
                }
                desc.Append(ISIUtil.SetHighlight(task.Desc1, this.IsHighlight, this.Desc));
            }

            if (!string.IsNullOrEmpty(task.Desc2))
            {
                desc.Append((!string.IsNullOrEmpty(task.Desc1) ? "<br/>" : string.Empty) + "<span style='color:#0000E5;'>" + "${ISI.Status.Desc2}" + "</span>&#58;&nbsp;" + ISIUtil.SetHighlight(task.Desc2, this.IsHighlight, this.Desc));
            }

            ((LinkButton)e.Row.FindControl("lbtnNew")).Text = "${ISI.TSK.RefTask}" + (task.RefTaskCount.HasValue && task.RefTaskCount.Value > 0 ? "&#58;&nbsp;" + task.RefTaskCount.Value.ToString() : string.Empty);

            if (task.RefTaskCount.HasValue && task.RefTaskCount.Value > 0)
            {
                //desc.Append("<br/><span style='color:#0000E5;'>${ISI.TSK.RefTask}</span>&#58;&nbsp;" + task.RefTaskCount.Value);

                var refTask = this.TheTaskMstrMgr.GetRefTask(task.TaskCode, 0, 5);
                e.Row.Cells[1].Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                e.Row.Cells[1].Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                e.Row.Cells[1].Attributes.Add("title", GetRefTasks(refTask));
            }
            Label lblDesc = (Label)e.Row.FindControl("lblDesc");
            StringBuilder descBufer = new StringBuilder();
            if (!string.IsNullOrEmpty(this.Desc) && desc.ToString().Contains(this.Desc))
            {
                descBufer.Append(desc.ToString());
            }
            else
            {
                descBufer.Append(ISIUtil.GetHide(task.TaskCode, desc.ToString()));
            }
            Append(descBufer, "ISI.Status.CostCenter", task.CostCenter);
            Append(descBufer, "ISI.Status.Account1", task.Account1Name);
            Append(descBufer, "ISI.Status.Account2", task.Account2Name);
            Append(descBufer, "WFS.Cost.Voucher", task.Voucher);
            Append(descBufer, "ISI.Status.Payee", task.Payee);
            Append(descBufer, "ISI.Status.Amount", task.Amount, StringHelper.MoneyCn(task.Amount));
            Append(descBufer, "ISI.Status.Taxes", task.Taxes, StringHelper.MoneyCn(task.Taxes));
            Append(descBufer, "ISI.Status.TotalAmount", task.TotalAmount, StringHelper.MoneyCn(task.TotalAmount));
            if (task.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_2 || task.FormType == ISIConstants.CODE_MASTER_WFS_FORMTYPE_3)
            {
                AppendQty(descBufer, "ISI.Status.Hours", task.Qty);
            }
            else
            {
                AppendQty(descBufer, "ISI.Status.Qty", task.Qty);
            }
            if(!string.IsNullOrEmpty(task.BackYards))
            {
                Append(descBufer, "ISI.TSK.BackYards", task.BackYards);
            }
            lblDesc.Text = descBufer.ToString();

            StringBuilder expectedResultsBufer = new StringBuilder();
            if (!string.IsNullOrEmpty(task.ExpectedResults))
            {
                expectedResultsBufer.Append(ISIUtil.SetHighlight(task.ExpectedResults, this.IsHighlight, this.Desc));
            }

            if (!string.IsNullOrEmpty(task.WorkHoursUserNm))
            {
                if (expectedResultsBufer.Length > 0)
                {
                    expectedResultsBufer.Append("<br>");
                }
                expectedResultsBufer.Append("${ISI.TSK.WorkHoursUser}: " + task.WorkHoursUserNm);
            }
            ((Label)e.Row.FindControl("lblExpectedResults")).Text = expectedResultsBufer.ToString();

            //Label lblStatusDesc = (Label)e.Row.FindControl("lblStatusDesc");
            /*
            if (task.StatusDate.HasValue)
            {
                lblStatusDesc.Text = "<span style='color:#0000E5;'>" + task.StatusDate.Value.ToString("yyyy-MM-dd HH:mm") + "</span>";
            }
            */
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

            #region  按钮
            var statusDiv = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("statusDiv"));
            statusDiv.InnerHtml += "<span class='link' id='lnkStatus" + task.TaskCode + "' name='lnkStatus" + task.TaskCode + "' onclick=\"javascript:ShowStatus('" + task.TaskCode + "','" + task.Subject + "'," + e.Row.RowIndex + "," + task.CurrentStatusCount1 + "," + task.StatusCount1 + ",'" + task.Flag + "','" + task.Color + "','" + false + "');\">${ISI.Status.Status}</span>";

            var spanComplete = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanComplete"));
            spanComplete.InnerHtml += "&nbsp;&nbsp;<span class='link' id='lnkCompleteStatus" + task.TaskCode + "' name='lnkCompleteStatus" + task.TaskCode + "' onclick=\"javascript:ShowStatus('" + task.TaskCode + "','" + task.Subject + "'," + e.Row.RowIndex + "," + task.CurrentStatusCount1 + "," + task.StatusCount1 + ",'" + task.Flag + "','" + task.Color + "','" + true + "');\">${ISI.TSK.Button.Complete}</span>";

            var spanStart = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanStart"));
            spanStart.InnerHtml += "<span class='link' id='lnkStart" + task.TaskCode + "' name='lnkStart" + task.TaskCode + "' onclick=\"javascript:StartTask('" + task.TaskCode + "'," + e.Row.RowIndex + ");\">${Common.Button.Start}</span>";

            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
            {
                //statusDiv.Visible= false;//.Attributes["style"] = "display: none;";

                if (task.CreateUser == this.CurrentUser.Code || isISIAdmin)
                {
                    //LinkButton lbtnSubmit = (LinkButton)e.Row.FindControl("lbtnSubmit");
                    //lbtnSubmit.OnClientClick = "return confirm('" + task.TaskCode + " ${Common.Button.Submit.Confirm}')";
                    //lbtnSubmit.Visible = true;
                    var spanSubmit = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanSubmit"));
                    spanSubmit.Visible = true;
                    spanSubmit.InnerHtml = "<span class='link' id='lnkSubmit" + task.TaskCode + "' name='lnkSubmit" + task.TaskCode + "' onclick=\"javascript:SubmitTask('" + task.TaskCode + "'," + e.Row.RowIndex + ");\">${Common.Button.Submit}</span>";
                    if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_CREATE)
                    {
                        var spanDelete = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanDelete"));
                        spanDelete.Visible = true;
                        spanDelete.InnerHtml = "<span class='link' id='lnkDelete" + task.TaskCode + "' name='lnkDelete" + task.TaskCode + "' onclick=\"javascript:DeleteTask('" + task.TaskCode + "');\">${Common.Button.Delete}</span>";
                    }
                    if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_RETURN)
                    {
                        var spanCancel = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanCancel"));
                        spanCancel.Visible = true;
                        spanCancel.InnerHtml = "<span class='link' id='lnkCancel" + task.TaskCode + "' name='lnkCancel" + task.TaskCode + "' onclick=\"javascript:CancelTask('" + task.TaskCode + "'," + e.Row.RowIndex + ");\">${Common.Button.Cancel}</span>";
                    }

                    tbEndDate.ReadOnly = false;
                    //tbEndDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";

                    tbStartDate.ReadOnly = false;
                    //tbStartDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                    if (tbEndDate.Visible && tbStartDate.Visible)
                    {
                        tbStartDate.Attributes.Add("onclick", "var " + tbEndDate.ClientID + "=$dp.$('" + tbEndDate.ClientID + "');WdatePicker({startDate:'%y-%M-%d 08:00:00',qsEnabled:true,quickSel:['%y-01-01 08:00:00','%y-02-01 08:00:00','%y-%M-01 08:00:00','%y-%M-15 08:00:00'],dateFmt:'yyyy-MM-dd',maxDate:'#F{$dp.$D(\\'" + tbEndDate.ClientID + "\\')}' })");
                        tbEndDate.Attributes.Add("onclick", "WdatePicker({doubleCalendar:true,startDate:'%y-%M-%d 16:30:00',qsEnabled:true,quickSel:['%y-%M-15 16:30:00','%y-%M-%ld 16:30:00','%y-{%M+1}-%ld 16:30:00','%y-12-%ld 16:30:00','{%y+1}-01-%ld 16:30:00'],dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\\'" + tbStartDate.ClientID + "\\')}'})");
                    }
                    else
                    {
                        tbEndDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                        tbStartDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                    }
                }

                Label lblSubmitUserNm = (Label)e.Row.FindControl("lblSubmitUserNm");
                lblSubmitUserNm.Text = task.CreateUserNm;
            }
            bool isAssign = false;
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
            {
                //statusDiv.Attributes["style"] = "display: none;";

                if (isISIAdmin
                         || isTaskFlowAdmin
                         || task.CreateUser == this.CurrentUser.Code
                         || task.SubmitUser == this.CurrentUser.Code)
                {
                    var spanCancel = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanCancel"));
                    spanCancel.Visible = true;
                    spanCancel.InnerHtml = "<span class='link' id='lnkCancel" + task.TaskCode + "' name='lnkCancel" + task.TaskCode + "' onclick=\"javascript:CancelTask('" + task.TaskCode + "'," + e.Row.RowIndex + ");\">${Common.Button.Cancel}</span>";

                    tbEndDate.ReadOnly = false;
                    //tbEndDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";

                    tbStartDate.ReadOnly = false;
                    //tbStartDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                    if (tbEndDate.Visible && tbStartDate.Visible)
                    {
                        tbStartDate.Attributes.Add("onclick", "var " + tbEndDate.ClientID + "=$dp.$('" + tbEndDate.ClientID + "');WdatePicker({startDate:'%y-%M-%d 08:00:00',qsEnabled:true,quickSel:['%y-01-01 08:00:00','%y-02-01 08:00:00','%y-%M-01 08:00:00','%y-%M-15 08:00:00'],dateFmt:'yyyy-MM-dd',maxDate:'#F{$dp.$D(\\'" + tbEndDate.ClientID + "\\')}' })");
                        tbEndDate.Attributes.Add("onclick", "WdatePicker({doubleCalendar:true,startDate:'%y-%M-%d 16:30:00',qsEnabled:true,quickSel:['%y-%M-15 16:30:00','%y-%M-%ld 16:30:00','%y-{%M+1}-%ld 16:30:00','%y-12-%ld 16:30:00','{%y+1}-01-%ld 16:30:00'],dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\\'" + tbStartDate.ClientID + "\\')}'})");
                    }
                    else
                    {
                        tbStartDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                        tbEndDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                    }
                }
                if (task.IsWF && task.Level != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE)
                {
                    //审批人     
                    isAssign = task.IsAssignUser && (isWFAdmin || ISIUtil.Contains(task.DeptUser, this.CurrentUser.Code) || ISIUtil.Contains(task.CostCenterUser, this.CurrentUser.Code));
                }
                else
                {
                    isAssign = this.TheTaskMgr.HasAssignPermission(task.CreateUser, task.SubmitUser, task.Status, task.Type, isISIAdmin, isTaskFlowAdmin, isAssigner, this.CurrentUser.Code, task.ECUser, task.TaskSubTypeAssignUser, task.AssignUpUser, task.IsAutoAssign, task.IsWF);
                }
                WFPermission wfPermission = this.TheTaskMgr.ProcessPermission(task.Status, task.TaskCode, task.Level, isWFAdmin, this.CurrentUser.Code);

                if (task.IsWF && task.Level.HasValue && task.Level.Value != ISIConstants.CODE_MASTER_WFS_LEVEL_COMPLETE && wfPermission.IsApprove)
                {
                    var spanApprove = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanApprove"));
                    spanApprove.Attributes["style"] = "";
                    if (!wfPermission.IsAccountCtrl)
                    {
                        if (task.Level.Value == ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE)
                        {
                            spanApprove.InnerHtml += "<span class='link' id='lnkApprove" + task.TaskCode + "' name='lnkApprove" + task.TaskCode + "' onclick=\"javascript:ShowApprove('" + task.TaskCode + "','" + task.Subject + "'," + e.Row.RowIndex + "," + task.CurrentStatusCount1 + "," + task.StatusCount1 + "," + 7 + ");\">${ISI.Button.Approve}</span>";
                        }
                        else
                        {
                            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT)
                            {
                                spanApprove.InnerHtml += "<span class='link' id='lnkApprove" + task.TaskCode + "' name='lnkApprove" + task.TaskCode + "' onclick=\"javascript:ShowApprove('" + task.TaskCode + "','" + task.Subject + "'," + e.Row.RowIndex + "," + task.CurrentStatusCount1 + "," + task.StatusCount1 + "," + 1 + ");\">${ISI.Button.Approve}</span>";
                            }
                            else
                            {
                                spanApprove.InnerHtml += "<span class='link' id='lnkApprove" + task.TaskCode + "' name='lnkApprove" + task.TaskCode + "' onclick=\"javascript:ShowApprove('" + task.TaskCode + "','" + task.Subject + "'," + e.Row.RowIndex + "," + task.CurrentStatusCount1 + "," + task.StatusCount1 + "," + 9 + ");\">${ISI.Button.Approve}</span>";
                            }
                        }

                    }

                    LinkButton lbtnEdit2 = (LinkButton)e.Row.FindControl("lbtnEdit2");
                    if (wfPermission.IsApprove)
                    {
                        Label lblLevelDesc1 = ((Label)(e.Row.FindControl("lblLevelDesc1")));
                        lblLevelDesc1.Visible = true;
                        lblLevelDesc1.Text = wfPermission.Desc1;
                        lbtnEdit2.Visible = wfPermission.IsCtrl;
                    }
                    lbtnEdit2.Visible = lbtnEdit2.Visible || task.IsCtrl;
                }
                else if (isAssign)
                {
                    //LinkButton lbtnAssign = (LinkButton)e.Row.FindControl("lbtnAssign");
                    string assignStartUserNm = task.AssignStartUserNm;
                    if (task.StartedUserCount > 0 && string.IsNullOrEmpty(assignStartUserNm))
                    {
                        assignStartUserNm = this.TheUserSubscriptionMgr.GetUserName(task.StartedUser);
                    }
                    //lbtnAssign.OnClientClick = "return confirm('" + task.TaskCode + " ${Common.Button.Confirm.Assign.For," + assignStartUserNm + "}')";
                    //lbtnAssign.Visible = true;

                    var spanAssign = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanAssign"));
                    spanAssign.Visible = true;
                    spanAssign.InnerHtml = "<span class='link' id='lnkAssign" + task.TaskCode + "' name='lnkAssign" + task.TaskCode + "' onclick=\"javascript:AssignTask('" + task.TaskCode + "'," + e.Row.RowIndex + ",'" + task.TaskCode + " " + this.TheLanguageMgr.TranslateMessage("Common.Button.Confirm.Assign.For", this.CurrentUser, assignStartUserNm) + "');\">${Common.Button.Assign}</span>";

                    tbEndDate.ReadOnly = false;
                    //tbEndDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";

                    tbStartDate.ReadOnly = false;
                    //tbStartDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                    if (tbEndDate.Visible && tbStartDate.Visible)
                    {
                        tbStartDate.Attributes.Add("onclick", "var " + tbEndDate.ClientID + "=$dp.$('" + tbEndDate.ClientID + "');WdatePicker({startDate:'%y-%M-%d 08:00:00',qsEnabled:true,quickSel:['%y-01-01 08:00:00','%y-02-01 08:00:00','%y-%M-01 08:00:00','%y-%M-15 08:00:00'],dateFmt:'yyyy-MM-dd',maxDate:'#F{$dp.$D(\\'" + tbEndDate.ClientID + "\\')}' })");
                        tbEndDate.Attributes.Add("onclick", "WdatePicker({doubleCalendar:true,startDate:'%y-%M-%d 16:30:00',qsEnabled:true,quickSel:['%y-%M-15 16:30:00','%y-%M-%ld 16:30:00','%y-{%M+1}-%ld 16:30:00','%y-12-%ld 16:30:00','{%y+1}-01-%ld 16:30:00'],dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\\'" + tbStartDate.ClientID + "\\')}'})");
                    }
                    else
                    {
                        tbEndDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                        tbStartDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                    }
                }
            }
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INAPPROVE || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INDISPUTE)
            {
                //审批人
                WFPermission wfPermission = this.TheTaskMgr.ProcessPermission(task.Status, task.TaskCode, task.Level, isWFAdmin, this.CurrentUser.Code);
                //上一步审批人
                //WFPermission wfPrePermission = this.TheTaskMgr.PreProcessPermission(task.Status, task.TaskCode, task.PreLevel, task.Level, isWFAdmin, this.CurrentUser.Code);

                //if ((wfPermission.IsApprove || wfPrePermission.IsApprove) && !wfPermission.IsCtrl && !wfPrePermission.IsCtrl)
                if (wfPermission.IsApprove)
                {
                    Label lblLevelDesc1 = ((Label)(e.Row.FindControl("lblLevelDesc1")));
                    lblLevelDesc1.Visible = true;
                    lblLevelDesc1.Text = wfPermission.Desc1;
                    LinkButton lbtnEdit2 = (LinkButton)e.Row.FindControl("lbtnEdit2");
                    lbtnEdit2.Visible = wfPermission.IsCtrl;
                    if (task.Level.HasValue)
                    {
                        var spanApprove = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanApprove"));
                        spanApprove.Attributes["style"] = "";
                        if (!wfPermission.IsAccountCtrl)
                        {
                            if (task.Level.Value == ISIConstants.CODE_MASTER_WFS_LEVEL_ULTIMATE)
                            {
                                spanApprove.InnerHtml += "<span class='link' id='lnkApprove" + task.TaskCode + "' name='lnkApprove" + task.TaskCode + "' onclick=\"javascript:ShowApprove('" + task.TaskCode + "','" + task.Subject + "'," + e.Row.RowIndex + "," + task.CurrentStatusCount1 + "," + task.StatusCount1 + "," + 7 + ");\">${ISI.Button.Approve}</span>";
                            }
                            else
                            {
                                spanApprove.InnerHtml += "<span class='link' id='lnkApprove" + task.TaskCode + "' name='lnkApprove" + task.TaskCode + "' onclick=\"javascript:ShowApprove('" + task.TaskCode + "','" + task.Subject + "'," + e.Row.RowIndex + "," + task.CurrentStatusCount1 + "," + task.StatusCount1 + "," + 9 + ");\">${ISI.Button.Approve}</span>";
                            }
                        }
                    }

                    //Style="text-decoration: none"

                    /*
                    if (wfPermission.IsCtrl)
                    {
                        Label lblLevelDesc2 = ((Label)(e.Row.FindControl("lblLevelDesc2")));
                        lblLevelDesc2.Visible = true;
                        lblLevelDesc2.ForeColor = System.Drawing.Color.Blue;
                        lblLevelDesc2.Text += "<${ISI.Status.IsCtrl}>";
                    }
                    */
                }

            }

            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
            {
                if (this.TheTaskMgr.HasPermissionByProcess(task.Status, task.StartedUser, task.StartUpUser, task.CreateUser, task.SubmitUser, isAssigner, isISIAdmin, isTaskFlowAdmin, this.CurrentUser.Code))
                {
                    //LinkButton lbtnStart = (LinkButton)e.Row.FindControl("lbtnStart");
                    //lbtnStart.OnClientClick = "return confirm('" + task.TaskCode + " ${ISI.TSK.Confirm.Start}')";
                    //lbtnStart.Visible = true;

                    spanStart.Attributes["style"] = "";
                }

                if (isISIAdmin || isTaskFlowAdmin
                         || task.CreateUser == this.CurrentUser.Code
                         || task.SubmitUser == this.CurrentUser.Code
                         || (task.Type == ISIConstants.ISI_TASK_TYPE_IMPROVE && task.CreateUser != this.CurrentUser.Code && (ISIUtil.Contains(task.TaskSubTypeAssignUser, this.CurrentUser.Code) || ISIUtil.Contains(task.AssignUpUser, this.CurrentUser.Code))))
                {
                    var spanCancel = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanCancel"));
                    spanCancel.Visible = true;
                    spanCancel.InnerHtml = "<span class='link' id='lnkCancel" + task.TaskCode + "' name='lnkCancel" + task.TaskCode + "' onclick=\"javascript:CancelTask('" + task.TaskCode + "'," + e.Row.RowIndex + ");\">${Common.Button.Cancel}</span>";
                }

                if (this.TheTaskMgr.HasAssignPermission(task.CreateUser, task.SubmitUser, task.Status, task.Type, isISIAdmin, isTaskFlowAdmin, isAssigner, this.CurrentUser.Code, task.ECUser, task.TaskSubTypeAssignUser, task.AssignUpUser, task.IsAutoAssign, task.IsWF))
                {
                    tbEndDate.ReadOnly = false;
                    //tbEndDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";

                    tbStartDate.ReadOnly = false;
                    //tbStartDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                    if (tbEndDate.Visible && tbStartDate.Visible)
                    {
                        tbStartDate.Attributes.Add("onclick", "var " + tbEndDate.ClientID + "=$dp.$('" + tbEndDate.ClientID + "');WdatePicker({startDate:'%y-%M-%d 08:00:00',qsEnabled:true,quickSel:['%y-01-01 08:00:00','%y-02-01 08:00:00','%y-%M-01 08:00:00','%y-%M-15 08:00:00'],dateFmt:'yyyy-MM-dd',maxDate:'#F{$dp.$D(\\'" + tbEndDate.ClientID + "\\')}' })");
                        tbEndDate.Attributes.Add("onclick", "WdatePicker({doubleCalendar:true,startDate:'%y-%M-%d 16:30:00',qsEnabled:true,quickSel:['%y-%M-15 16:30:00','%y-%M-%ld 16:30:00','%y-{%M+1}-%ld 16:30:00','%y-12-%ld 16:30:00','{%y+1}-01-%ld 16:30:00'],dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\\'" + tbStartDate.ClientID + "\\')}'})");
                    }
                    else
                    {
                        tbEndDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                        tbStartDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                    }
                }
            }
            if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE)
            {
                //LinkButton lbtnClose = (LinkButton)e.Row.FindControl("lbtnClose");
                //lbtnClose.OnClientClick = "return confirm('" + task.TaskCode + " ${Common.Button.Close.Confirm}')";

                var spanClose = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanClose"));
                var spanReject = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanReject"));
                spanReject.InnerHtml = "<span class='link' id='lnkReject" + task.TaskCode + "' name='lnkReject" + task.TaskCode + "' onclick=\"javascript:RejectTask('" + task.TaskCode + "'," + e.Row.RowIndex + ");\">${Common.Button.Reject}</span>";

                if (this.TheTaskMgr.HasPermissionByComplete(task.Status, task.Type,
                                                    task.CreateUser, task.SubmitUser, task.StartedUser,
                                                    task.ECUser, task.TaskSubTypeAssignUser, task.AssignUpUser,
                                                    task.CloseUpUser, task.CloseUpLevel,
                                                    isISIAdmin, isISIAdmin, isCloser, this.CurrentUser.Code))
                {
                    //lbtnClose.Visible = true;
                    spanClose.Visible = true;
                    spanClose.InnerHtml = "<span class='link' id='lnkClose" + task.TaskCode + "' name='lnkClose" + task.TaskCode + "' onclick=\"javascript:CloseTask('" + task.TaskCode + "'," + e.Row.RowIndex + ");\">${Common.Button.Close}</span>";

                    spanReject.Visible = true;
                }
            }

            if (this.TheTaskMgr.HasPermissionByClose(task.Status, task.Type, task.IsWF, task.CreateUser, task.SubmitUser, task.AssignUser, task.AssignUpUser, task.CloseUpUser, isAssigner, isISIAdmin, isCloser, this.CurrentUser.Code))
            {
                var spanOpen = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("spanOpen"));
                spanOpen.InnerHtml = "<span class='link' id='lnkOpen" + task.TaskCode + "' name='lnkOpen" + task.TaskCode + "' onclick=\"javascript:OpenTask('" + task.TaskCode + "'," + e.Row.RowIndex + ");\">${ISI.TSK.Button.Open}</span>";
                spanOpen.Visible = true;
            }

            if ((task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN && this.TheTaskMgr.HasPermissionByProcess(task.Status, task.StartedUser, task.StartUpUser, task.CreateUser, task.SubmitUser, isAssigner, isISIAdmin, isTaskFlowAdmin, this.CurrentUser.Code))
                       || (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS
                                        && (this.TheTaskMgr.HasPermissionByProcess(task.Status, task.StartedUser, task.StartUpUser, task.CreateUser, task.SubmitUser, isAssigner, isISIAdmin, isTaskFlowAdmin, this.CurrentUser.Code)
                                            || this.TheTaskMgr.HasAssignPermission(task.CreateUser, task.SubmitUser, task.Status, task.Type, isISIAdmin, isTaskFlowAdmin, isAssigner, this.CurrentUser.Code, task.ECUser, task.TaskSubTypeAssignUser, task.AssignUpUser, task.IsAutoAssign, task.IsWF)))
                            || (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_COMPLETE
                                        && (this.TheTaskMgr.HasPermissionByComplete(task.Status, task.Type, task.CreateUser, task.SubmitUser, task.StartedUser,
                                                                                      task.ECUser, task.TaskSubTypeAssignUser,
                                                                                      task.AssignUpUser, task.CloseUpUser, task.CloseUpLevel,
                                                                                        isISIAdmin, isTaskFlowAdmin, isCloser, this.CurrentUser.Code)
                                                                || ISIUtil.Contains(task.StartedUser, this.CurrentUser.Code)
                                                                || task.CreateUser == this.CurrentUser.Code
                                                                || task.SubmitUser == this.CurrentUser.Code)))
            {
                statusDiv.Attributes["style"] = "";
                if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_ASSIGN)
                {
                    spanComplete.Attributes["style"] = "";
                }
                if (task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS)
                {
                    spanComplete.Attributes["style"] = "";
                    if (this.TheTaskMgr.HasAssignPermission(task.CreateUser, task.SubmitUser, task.Status, task.Type, isISIAdmin, isTaskFlowAdmin, isAssigner, this.CurrentUser.Code, task.ECUser, task.TaskSubTypeAssignUser, task.AssignUpUser, task.IsAutoAssign, task.IsWF))
                    {
                        tbEndDate.ReadOnly = false;
                        //tbEndDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";

                        tbStartDate.ReadOnly = false;
                        //tbStartDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                        if (tbStartDate.Visible && tbEndDate.Visible)
                        {
                            tbStartDate.Attributes.Add("onclick", "var " + tbEndDate.ClientID + "=$dp.$('" + tbEndDate.ClientID + "');WdatePicker({startDate:'%y-%M-%d 08:00:00',qsEnabled:true,quickSel:['%y-01-01 08:00:00','%y-02-01 08:00:00','%y-%M-01 08:00:00','%y-%M-15 08:00:00'],dateFmt:'yyyy-MM-dd',maxDate:'#F{$dp.$D(\\'" + tbEndDate.ClientID + "\\')}' })");
                            tbEndDate.Attributes.Add("onclick", "WdatePicker({doubleCalendar:true,startDate:'%y-%M-%d 16:30:00',qsEnabled:true,quickSel:['%y-%M-15 16:30:00','%y-%M-%ld 16:30:00','%y-{%M+1}-%ld 16:30:00','%y-12-%ld 16:30:00','{%y+1}-01-%ld 16:30:00'],dateFmt:'yyyy-MM-dd',minDate:'#F{$dp.$D(\\'" + tbStartDate.ClientID + "\\')}'})");
                        }
                        else
                        {
                            tbEndDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                            tbStartDate.Attributes["onclick"] = "WdatePicker({dateFmt:'yyyy-MM-dd'})";
                        }
                    }
                }
            }

            /*
            if ((task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_INPROCESS || task.Status == ISIConstants.CODE_MASTER_ISI_STATUS_VALUE_SUBMIT) &&
                this.TheTaskMgr.HasAssignPermission(task.CreateUser, task.TSubmitUser, task.Status, task.Type, isISIAdmin, isTaskFlowAdmin, isAssigner, this.CurrentUser.Code, task.TaskSubTypeAssignUser, task.AssignUpUser, task.IsAutoAssign, task.IsWF)
                )
            {
                var assignDiv = (System.Web.UI.HtmlControls.HtmlGenericControl)(e.Row.FindControl("assignDiv"));

                string userNames = this.TheUserSubscriptionMgr.GetUserName(task.AssignStartUser);
                userNames = ISIUtil.GetUserMerge(task.AssignStartUser, userNames);

                assignDiv.InnerHtml = "<a id='lnkAssign" + task.TaskCode + "' name='lnkAssign" + task.TaskCode + "' href='#' onclick=\"javascript:ShowAssign('" + task.TaskCode + "','" + task.TaskSubTypeCode + "','" + userNames + "'," + e.Row.RowIndex + ");\">${ISI.Status.Assign}</a>";
                assignDiv.Visible = true;
            }
            */

            #endregion
        }
    }

    private void Append(StringBuilder descBufer, string title, decimal? obj, string money)
    {
        Append(descBufer, title, obj, money, "C");
    }

    private void AppendQty(StringBuilder descBufer, string title, decimal? obj)
    {
        Append(descBufer, title, obj, string.Empty, "0.########");
    }
    private void Append(StringBuilder descBufer, string title, decimal? obj, string money, string format)
    {
        if (obj.HasValue && obj.Value != 0)
        {
            descBufer.Append((descBufer.Length > 0 ? "<br>" : string.Empty));
            descBufer.Append("<span style='color:#0000E5;'>");
            descBufer.Append("${" + title + "}");
            descBufer.Append("</span>&#58;&nbsp;");
            descBufer.Append(obj.Value.ToString(format) + (!string.IsNullOrEmpty(money) ? " " + money : string.Empty));
        }
    }
    private void Append(StringBuilder descBufer, string title, DateTime? obj)
    {
        if (obj.HasValue)
        {
            this.Append(descBufer, title, obj.Value.ToString("yyyy-MM-dd"));
        }
    }

    private void Append(StringBuilder descBufer, string title, int? obj)
    {
        if (obj.HasValue && obj.Value != 0)
        {
            this.Append(descBufer, title, obj.Value.ToString());
        }
    }
    private void AppendAmount(StringBuilder descBufer, string title, decimal? obj)
    {
        if (obj.HasValue && obj.Value != 0)
        {
            Append(descBufer, title, obj, StringHelper.MoneyCn(obj), "C");
        }
    }
    private void Append(StringBuilder descBufer, string title, string obj)
    {
        if (!string.IsNullOrEmpty(obj))
        {
            descBufer.Append((descBufer.Length > 0 ? "<br>" : string.Empty));
            descBufer.Append("<span style='color:#0000E5;'>");
            descBufer.Append("${" + title + "}");
            descBufer.Append("</span>&#58;&nbsp;");
            descBufer.Append(obj);
        }
    }
    private void GetAttachmentDesc(IList<object> ojbectList, GridViewRow row)
    {

        StringBuilder html = new StringBuilder();
        if (ojbectList != null && ojbectList.Count > 0)
        {
            IList<IList<AttachmentDetail>> attachmentDetailListList = (IList<IList<AttachmentDetail>>)ojbectList[0];
            int currentAttachmentDetailCount = (int)ojbectList[1];
            int attachmentDetailCount = (int)ojbectList[2];
            if (attachmentDetailListList != null && attachmentDetailListList.Count > 0)
            {
                IList<AttachmentDetail> attachmentDetailList = null;
                IList<AttachmentDetail> attachmentDetailTipList = null;

                if (attachmentDetailListList.Count > 0)
                {
                    attachmentDetailList = attachmentDetailListList[0];
                }
                if (attachmentDetailListList.Count > 1)
                {
                    attachmentDetailTipList = attachmentDetailListList[1];
                }

                /*
                if (!downLoadDiv.Visible)
                {
                    attachmentDetailList = attachmentDetailList.Where(file => file.CreateUser == this.CurrentUser.Code).ToList();
                    if (attachmentDetailList == null || attachmentDetailList.Count == 0)
                    {
                        return;
                    }
                    else
                    {
                        downLoadDiv.Visible = true;
                    }
                }
                */
                for (int i = 0; i < attachmentDetailList.Count; i++)
                {
                    if (i == 10) break;
                    LinkButton lbtnDownLoad = (LinkButton)row.FindControl("lbtnDownLoad" + (i + 1));
                    lbtnDownLoad.CommandArgument = "{" + attachmentDetailList[i].FileName + "}{" + attachmentDetailList[i].ContentType + "}{" + attachmentDetailList[i].Path + "}";
                    lbtnDownLoad.Text = GetAttachmentDesc(attachmentDetailList[i].FileName, this.Desc, i < currentAttachmentDetailCount, currentAttachmentDetailCount, attachmentDetailCount, i == 0, i == attachmentDetailList.Count - 1);
                    lbtnDownLoad.Visible = true;
                }
                if (attachmentDetailTipList != null && attachmentDetailTipList.Count > 0)
                {
                    string title = GetAttachments(attachmentDetailTipList, currentAttachmentDetailCount);
                    System.Web.UI.HtmlControls.HtmlContainerControl downLoadDiv = (System.Web.UI.HtmlControls.HtmlContainerControl)row.FindControl("downLoadDiv");
                    downLoadDiv.Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
                    downLoadDiv.Attributes.Add("onmouseout", "this.style.backgroundColor=e");
                    downLoadDiv.Attributes.Add("title", title);
                }
            }
        }
    }
    private string GetAttachmentDesc(string fileName, string desc, bool isHighlight, int currentAttachmentCount, int attachmentCount, bool isFirst, bool isLast)
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
            if (currentAttachmentCount > 0)
            {
                html.Append(currentAttachmentCount);
                html.Append("&#47;");
            }
            html.Append(attachmentCount);
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


    /*
    /// <summary>
    /// 废弃
    /// </summary>
    /// <param name="task"></param>
    /// <param name="lblStatusDesc"></param>
    private void GetStatusDesc(TaskStatusView task, Label lblStatusDesc)
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
        html.Append("&#41;</span>&#58;");
        if (task.IsWF)
        {
            html.Append("<br><span style='background:#DFD3D3'>${ISI.TSK." + task.TraceType + "}</span>&nbsp;");
        }
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
        lblStatusDesc.Text = html.ToString();

    }*/

    private void SetCount(System.Web.UI.HtmlControls.HtmlGenericControl statusDiv,
                                        System.Web.UI.HtmlControls.HtmlGenericControl spanComplete,
                                        System.Web.UI.HtmlControls.HtmlGenericControl spanApprove,
                                        int currentStatusCount, int statusCount)
    {
        if (statusDiv != null && statusDiv.InnerHtml.Length > 0)
        {
            statusDiv.InnerHtml.Replace("##########", currentStatusCount.ToString());
            statusDiv.InnerHtml.Replace("%%%%%%%%%%", currentStatusCount.ToString());
        }
        if (spanComplete != null && spanComplete.InnerHtml.Length > 0)
        {
            spanComplete.InnerHtml.Replace("##########", currentStatusCount.ToString());
            spanComplete.InnerHtml.Replace("%%%%%%%%%%", currentStatusCount.ToString());
        }
        if (spanApprove != null && spanApprove.InnerHtml.Length > 0)
        {
            spanApprove.InnerHtml.Replace("##########", currentStatusCount.ToString());
            spanApprove.InnerHtml.Replace("%%%%%%%%%%", currentStatusCount.ToString());
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

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string[] arg = ((LinkButton)sender).CommandArgument.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        string code = arg[0];
        int displayIndex = int.Parse(arg[1]);
        try
        {
            this.TheTaskMgr.DeleteTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Delete.Successfully", code);
            UpdateView();
            SetAnchor(displayIndex);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Delete.Fail", code);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void lbtnCancel_Click(object sender, EventArgs e)
    {
        string[] arg = ((LinkButton)sender).CommandArgument.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        string code = arg[0];
        int displayIndex = int.Parse(arg[1]);
        try
        {
            this.TheTaskMgr.CancelTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Cancel.Successfully", code);
            UpdateView();
            SetAnchor(displayIndex);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Cancel.Fail", code);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        string[] arg = ((LinkButton)sender).CommandArgument.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        string code = arg[0];
        int displayIndex = int.Parse(arg[1]);
        try
        {
            this.TheTaskMgr.ConfirmTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Confirm.Successfully", code);
            UpdateView();
            SetAnchor(displayIndex);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Confirm.Fail", code);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void lbtnSubmit_Click(object sender, EventArgs e)
    {
        string[] arg = ((LinkButton)sender).CommandArgument.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        string code = arg[0];
        int displayIndex = int.Parse(arg[1]);
        try
        {
            this.TheTaskMgr.SubmitTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Submit.Successfully", code);
            UpdateView();
            SetAnchor(displayIndex);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Submit.Fail", code);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
    protected void lbtnAssign_Click(object sender, EventArgs e)
    {
        string[] arg = ((LinkButton)sender).CommandArgument.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        string code = arg[0];
        int displayIndex = int.Parse(arg[1]);
        try
        {
            TheTaskMgr.AssignTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Assign.Successfully", code);
            UpdateView();
            SetAnchor(displayIndex);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Assign.Fail", code);
        }
    }
    protected void lbtnComplete_Click(object sender, EventArgs e)
    {
        string[] arg = ((LinkButton)sender).CommandArgument.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        string code = arg[0];
        int displayIndex = int.Parse(arg[1]);
        try
        {
            TheTaskMgr.CompleteTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Complete.Successfully", code);
            UpdateView();
            SetAnchor(displayIndex);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Complete.Fail", code);
        }
    }

    protected void lbtnClose_Click(object sender, EventArgs e)
    {
        string[] arg = ((LinkButton)sender).CommandArgument.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        string code = arg[0];
        int displayIndex = int.Parse(arg[1]);
        try
        {
            TheTaskMgr.CloseTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Close.Successfully", code);
            UpdateView();
            SetAnchor(displayIndex);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Close.Fail", code);
        }
    }

    public void SetAnchor()
    {
        SetAnchor(0);
    }

    private void SetAnchor(int displayIndex)
    {
        if (displayIndex != 0)
        {
            displayIndex -= 1;
            LinkButton lbtnEdit = (LinkButton)this.GV_List.Rows[displayIndex].FindControl("lbtnEdit");
            //Response.Write("<script language='JavaScript'>this.location.href='#" + displayIndex + "'</script>");
            Page.ClientScript.RegisterStartupScript(GetType(), "js", " <script language='JavaScript'>this.location.href='#" + lbtnEdit.CommandArgument + "'</script>");
            return;
        }
        //Response.Write("<script language='JavaScript'>this.location.href='#hideImg'</script>");
        Page.ClientScript.RegisterStartupScript(GetType(), "js", " <script language='JavaScript'>this.location.href='#hideImg'</script>");

    }
    private void SetAnchor(string taskCode)
    {
        if (!string.IsNullOrEmpty(taskCode))
        {
            //Response.Write("<script language='JavaScript'>this.location.href='#" + displayIndex + "'</script>");
            Page.ClientScript.RegisterStartupScript(GetType(), "js", " <script language='JavaScript'>this.location.href='#" + taskCode + "'</script>");
            return;
        }
    }
    protected void lbtnReject_Click(object sender, EventArgs e)
    {
        string[] arg = ((LinkButton)sender).CommandArgument.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        string code = arg[0];
        int displayIndex = int.Parse(arg[1]);
        try
        {
            TheTaskMgr.RejectTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Reject.Successfully", code);
            UpdateView();
            SetAnchor(displayIndex);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Reject.Fail", code);
        }
    }

    protected void lbtnOpen_Click(object sender, EventArgs e)
    {
        string[] arg = ((LinkButton)sender).CommandArgument.Split(new char[] { '{', '}' }, StringSplitOptions.RemoveEmptyEntries);
        string code = arg[0];
        int displayIndex = int.Parse(arg[1]);
        try
        {
            TheTaskMgr.OpenTask(code, this.CurrentUser);
            ShowSuccessMessage("ISI.TSK.Open.Successfully", code);
            UpdateView();
            SetAnchor(displayIndex);
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowSuccessMessage("ISI.TSK.Open.Fail", code);
        }
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
    #endregion

    public void Export()
    {
        this.ExportXLS(GV_List);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.IsToDoList)
        {
            gp.ToolTip = "${ToDoList.GridView.NoRecordFound}";
        }
        else
        {
            gp.ToolTip = string.Empty;
        }
        if (!IsPostBack)
        {
            //ddlFlag.Items.Remove(ListItem.FromString(ISIConstants.CODE_MASTER_ISI_FLAG_DI1));
            //ddlFlag.Items.Remove(ListItem.FromString(ISIConstants.CODE_MASTER_ISI_FLAG_DI5));

            FileExtensions = this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ISI_FILEEXTENSION).Value;
            ContentLength = int.Parse(this.TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ISI_CONTENTLENGTH).Value);

            Monday = ISIUtil.GetMondayDate();
            LastMonday = Monday.AddDays(-7);
            LastLastMonday = LastMonday.AddDays(-7);
        }
    }

    public class Attachment
    {
        public string FileName { get; set; }
        public string Path { get; set; }
        public string ContentType { get; set; }
    }
}
