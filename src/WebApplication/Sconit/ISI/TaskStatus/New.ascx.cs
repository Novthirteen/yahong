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
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Web;
using com.Sconit.Control;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_TaskStatus_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
    public event EventHandler NewEvent;

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

    private TaskStatus taskStatus;
    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {

        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void FV_TaskStatus_OnDataBinding(object sender, EventArgs e)
    {

    }

    public void PageCleanup()
    {
        ((TextBox)(this.FV_TaskStatus.FindControl("tbDesc"))).Text = string.Empty;
        com.Sconit.Control.CodeMstrDropDownList ddlFlag = (com.Sconit.Control.CodeMstrDropDownList)(this.FV_TaskStatus.FindControl("ddlFlag"));
        com.Sconit.Control.CodeMstrDropDownList ddlColor = (com.Sconit.Control.CodeMstrDropDownList)(this.FV_TaskStatus.FindControl("ddlColor"));
        ddlFlag.Items.Remove(ListItem.FromString(ISIConstants.CODE_MASTER_ISI_FLAG_DI1));
        ddlFlag.Items.Remove(ListItem.FromString(ISIConstants.CODE_MASTER_ISI_FLAG_DI5));

        object[] last = this.TheTaskStatusMgr.GetLastTaskStatus(this.TaskCode);
        if (last != null && last.Length > 0)
        {
            if (!string.IsNullOrEmpty((string)last[0]) && (string)last[0] != ISIConstants.CODE_MASTER_ISI_FLAG_DI1 && (string)last[0] != ISIConstants.CODE_MASTER_ISI_FLAG_DI5)
            {
                ddlFlag.SelectedValue = (string)last[0];
            }
            if (!string.IsNullOrEmpty((string)last[1]))
            {
                ddlColor.SelectedValue = (string)last[1];
            }
        }
        else
        {
            ddlFlag.SelectedIndex = 0;
            ddlColor.SelectedIndex = 0;
        }

        this.ucInfo.InitPageParameter(this.ModuleType, this.TaskCode);
        // ((CodeMstrDropDownList)(this.FV_TaskStatus.FindControl("ddlFlag"))).SelectedIndex = 0;
        ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsCurrentStatus"))).Checked = true;
        ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindCreateUser"))).Checked = true;
        ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindAssignUser"))).Checked = false;
        ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindStartUser"))).Checked = false;
        ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindCommentUser"))).Checked = true;
        ((TextBox)(this.FV_TaskStatus.FindControl("tbStartDate"))).Text = string.Empty;
        ((TextBox)(this.FV_TaskStatus.FindControl("tbEndDate"))).Text = DateTime.Now.ToString("yyyy-MM-dd");
    }

    protected void ODS_TaskStatus_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        taskStatus = (TaskStatus)e.InputParameters[0];

        if (taskStatus != null)
        {
            taskStatus.TaskCode = this.TaskCode;
            taskStatus.Desc = taskStatus.Desc.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());
            taskStatus.Flag = ((CodeMstrDropDownList)(this.FV_TaskStatus.FindControl("ddlFlag"))).Text.Trim();
            taskStatus.Color = ((CodeMstrDropDownList)(this.FV_TaskStatus.FindControl("ddlColor"))).Text.Trim();
            taskStatus.IsCurrentStatus = ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsCurrentStatus"))).Checked;

            taskStatus.IsRemindCreateUser = ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindCreateUser"))).Checked;
            taskStatus.IsRemindAssignUser = ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindAssignUser"))).Checked;
            taskStatus.IsRemindStartUser = ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindStartUser"))).Checked;
            taskStatus.IsRemindCommentUser = ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindCommentUser"))).Checked;

            DateTime now = DateTime.Now;
            taskStatus.CreateDate = now;
            taskStatus.CreateUser = this.CurrentUser.Code;
            taskStatus.CreateUserNm = this.CurrentUser.Name;
            taskStatus.LastModifyDate = now;
            taskStatus.LastModifyUser = this.CurrentUser.Code;
            taskStatus.LastModifyUserNm = this.CurrentUser.Name;
        }
    }


    protected void Save_ServerValidate(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;
        string startDate = ((TextBox)(this.FV_TaskStatus.FindControl("tbStartDate"))).Text.Trim();
        string endDate = ((TextBox)(this.FV_TaskStatus.FindControl("tbEndDate"))).Text.Trim();
        switch (cv.ID)
        {
            case "cvEndDate":
                if (endDate.CompareTo(startDate) < 0)
                {
                    cv.ErrorMessage = "${Common.StarDate.EndDate.Compare}";
                    args.IsValid = false;
                }
                break;
            default:
                break;
        }
    }

    protected void ODS_TaskStatus_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(taskStatus.Id, e);
            ShowSuccessMessage("ISI.TaskStatus.AddTaskStatus.Successfully");
        }
    }
}
