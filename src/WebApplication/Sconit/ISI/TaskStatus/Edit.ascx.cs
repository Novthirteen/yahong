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
using com.Sconit.Entity.MasterData;
using com.Sconit.Control;
using System.Collections.Generic;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_TaskStatus_Edit : EditModuleBase
{
    public event EventHandler BackEvent;


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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }

    public void InitPageParameter(int id)
    {
        this.ucInfo.InitPageParameter(this.ModuleType, this.TaskCode);
        this.ODS_TaskStatus.SelectParameters["Id"].DefaultValue = id.ToString();
        this.ODS_TaskStatus.DeleteParameters["Id"].DefaultValue = id.ToString();
        this.ODS_TaskStatus.DataBind();
    }

    protected void FV_TaskStatus_DataBound(object sender, EventArgs e)
    {
        TaskStatus taskStatus = (TaskStatus)((FormView)sender).DataItem;
        UpdateView(taskStatus);
    }

    private void UpdateView(TaskStatus taskStatus)
    {
        com.Sconit.Control.CodeMstrDropDownList ddlFlag = (com.Sconit.Control.CodeMstrDropDownList)this.FV_TaskStatus.FindControl("ddlFlag");
        ddlFlag.Items.Remove(ListItem.FromString(ISIConstants.CODE_MASTER_ISI_FLAG_DI1));
        ddlFlag.Items.Remove(ListItem.FromString(ISIConstants.CODE_MASTER_ISI_FLAG_DI5));

        if (taskStatus.Flag != string.Empty)
        {
            ddlFlag.SelectedValue = taskStatus.Flag;
        }

        com.Sconit.Control.CodeMstrDropDownList ddlColor = (com.Sconit.Control.CodeMstrDropDownList)this.FV_TaskStatus.FindControl("ddlColor");
        if (taskStatus.Flag != string.Empty)
        {
            ddlColor.SelectedValue = taskStatus.Color;
        }
        ((TextBox)this.FV_TaskStatus.FindControl("tbStartDate")).Text = taskStatus.StartDate.ToString("yyyy-MM-dd");
        ((TextBox)this.FV_TaskStatus.FindControl("tbEndDate")).Text = taskStatus.EndDate.ToString("yyyy-MM-dd");

        int updateday =
            int.Parse(
                this.TheEntityPreferenceMgr.LoadEntityPreference(
                    ISIConstants.ENTITY_PREFERENCE_CODE_ISI_TASKSTATUSUPDATEDAY).Value);

        if (!(taskStatus.LastModifyDate.AddDays(updateday) > DateTime.Now && (taskStatus.CreateUser == this.CurrentUser.Code ||
                        CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN))))
        {
            ((HtmlGenericControl)(this.FV_TaskStatus.FindControl("lgd"))).InnerText = "${ISI.TaskStatus.ViewTaskStatus}";
            ((TextBox)this.FV_TaskStatus.FindControl("tbDesc")).ReadOnly = true;

            ((CheckBox)this.FV_TaskStatus.FindControl("cbIsCurrentStatus")).Enabled = false;
            ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindCreateUser"))).Enabled = false;
            ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindAssignUser"))).Enabled = false;
            ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindStartUser"))).Enabled = false;
            ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindCommentUser"))).Enabled = false;

            ((CodeMstrDropDownList)this.FV_TaskStatus.FindControl("ddlFlag")).Enabled = false;
            ((CodeMstrDropDownList)this.FV_TaskStatus.FindControl("ddlColor")).Enabled = false;
            this.FV_TaskStatus.FindControl("btnSave").Visible = false;

            TextBox tbStartDate = (TextBox)this.FV_TaskStatus.FindControl("tbStartDate");
            TextBox tbEndDate = (TextBox)this.FV_TaskStatus.FindControl("tbEndDate");

            tbStartDate.ReadOnly = true;
            tbStartDate.Attributes.Add("onfocus", "this.blur();");
            tbStartDate.Attributes.Remove("onclick");
            tbStartDate.Attributes.Remove("class");
            tbEndDate.ReadOnly = true;
            tbEndDate.Attributes.Add("onfocus", "this.blur();");
            tbEndDate.Attributes.Remove("onclick");
            tbEndDate.Attributes.Remove("class");
        }
        if (!CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_ISI_VALUE_DELETETASKSTATUS) &&
              !CurrentUser.HasPermission(ISIConstants.CODE_MASTER_ISI_TASK_VALUE_ISIADMIN))
        {
            this.FV_TaskStatus.FindControl("btnDelete").Visible = false;
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_TaskStatus_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("ISI.TaskStatus.UpdateTaskStatus.Successfully");
        btnBack_Click(this, e);
    }

    protected void ODS_TaskStatus_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        TaskStatus taskStatus = (TaskStatus)e.InputParameters[0];
        if (taskStatus != null)
        {
            TaskStatus oldTaskStatus = TheTaskStatusMgr.LoadTaskStatus(taskStatus.Id);
            taskStatus.CreateDate = oldTaskStatus.CreateDate;
            taskStatus.CreateUser = oldTaskStatus.CreateUser;
            taskStatus.CreateUserNm = oldTaskStatus.CreateUserNm;
            taskStatus.TaskCode = oldTaskStatus.TaskCode;
            taskStatus.Desc = taskStatus.Desc.Trim().Trim((char[])ISIConstants.TEXT_SEPRATOR.ToCharArray());
            taskStatus.Flag = ((CodeMstrDropDownList)(this.FV_TaskStatus.FindControl("ddlFlag"))).Text.Trim();
            taskStatus.Color = ((CodeMstrDropDownList)(this.FV_TaskStatus.FindControl("ddlColor"))).Text.Trim();
            taskStatus.IsCurrentStatus = ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsCurrentStatus"))).Checked;

            taskStatus.IsRemindCreateUser = ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindCreateUser"))).Checked;
            taskStatus.IsRemindAssignUser = ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindAssignUser"))).Checked;
            taskStatus.IsRemindStartUser = ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindStartUser"))).Checked;
            taskStatus.IsRemindCommentUser = ((CheckBox)(this.FV_TaskStatus.FindControl("cbIsRemindCommentUser"))).Checked;

            taskStatus.Version = oldTaskStatus.Version;
            taskStatus.LastModifyDate = DateTime.Now;
            taskStatus.LastModifyUser = this.CurrentUser.Code;
            taskStatus.LastModifyUserNm = this.CurrentUser.Name;
        }
    }

    protected void ODS_TaskStatus_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        /*
        TaskStatus taskStatus = (TaskStatus)e.InputParameters[0];
        //IList<TaskStatus> taskStatusList = TheTaskStatusMgr.GetTaskStatusByParent(taskStatus.Code);
        //if (taskStatusList != null && taskStatusList.Count > 0)
        
        if (TheTaskStatusMgr.IsRef(taskStatus.Code))
        {
            ShowErrorMessage("ISI.TaskStatus.DeleteTaskStatus.Ref.Fail", TaskStatusCode.ToString());
            e.Cancel = true;
        }*/
    }
    protected void ODS_TaskStatus_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("ISI.TaskStatus.DeleteTaskStatus.Successfully");
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("ISI.TaskStatus.DeleteTaskStatus.Fail");
            e.ExceptionHandled = true;
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
}
