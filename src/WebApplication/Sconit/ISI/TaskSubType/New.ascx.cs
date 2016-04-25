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

public partial class ISI_TaskSubType_New : NewModuleBase
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

    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
    public event EventHandler NewEvent;

    private TaskSubType taskSubType;
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void FV_TaskSubType_DataBound(object sender, EventArgs e)
    {
        ((Controls_TextBox)(this.FV_TaskSubType.FindControl("tbParent"))).ServiceParameter = "string:" + this.ModuleType;
        ((Controls_TextBox)(this.FV_TaskSubType.FindControl("tbParent"))).DataBind();
        this.FV_TaskSubType.FindControl("fs").Visible = this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT;
        if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT)
        {
            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlProjectType"))).IncludeBlankOption = false;
            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlProjectType"))).DataBind();

            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlColor"))).IncludeBlankOption = false;
            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlColor"))).DataBind();

            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlECType"))).IncludeBlankOption = false;
            ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlECType"))).DataBind();

            ((Literal)this.FV_TaskSubType.FindControl("ltlAssignUser")).Text = "${ISI.TaskSubType.TeamLeader}:";
            ((Literal)this.FV_TaskSubType.FindControl("lblIsReport")).Text = "${ISI.TaskSubType.IsProjectReport}:";
        }
    }

    protected void FV_TaskSubType_OnDataBinding(object sender, EventArgs e)
    {

    }

    public void PageCleanup()
    {
        ((TextBox)(this.FV_TaskSubType.FindControl("tbCode"))).Text = string.Empty;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbDesc"))).Text = string.Empty;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbSeq"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_TaskSubType.FindControl("tbParent"))).Text = string.Empty;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsActive"))).Checked = true;
        ((CheckBox)(this.FV_TaskSubType.FindControl("cbIsCost"))).Checked = this.ModuleType == ISIConstants.ISI_TASK_TYPE_GENERAL || this.ModuleType == ISIConstants.ISI_TASK_TYPE_PROJECT;

        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsAutoAssign"))).Checked = false;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsAutoStart"))).Checked = false;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsAutoComplete"))).Checked = false;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsAutoStatus"))).Checked = false;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsAutoClose"))).Checked = false;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsPublic"))).Checked = false;
        //((com.Sconit.Control.CodeMstrDropDownList)this.FV_TaskSubType.FindControl("ddlType")).SelectedIndex = 0;

        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsQuote"))).Checked = true;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsInitiation"))).Checked = true;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsStart"))).Checked = true;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsOpen"))).Checked = true;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbOpenTime"))).Text = "0";
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsReport"))).Checked = true;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbStartPercent"))).Text = "0.7";
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsCompleteUp"))).Checked = true;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbCompleteUpTime"))).Text = "0";
        ((com.Sconit.Control.CodeMstrDropDownList)this.FV_TaskSubType.FindControl("ddlProjectType")).SelectedIndex = 0;
        ((com.Sconit.Control.CodeMstrDropDownList)this.FV_TaskSubType.FindControl("ddlColor")).SelectedIndex = 0;

        ((com.Sconit.Control.CodeMstrDropDownList)this.FV_TaskSubType.FindControl("ddlOrg")).SelectedIndex = 0;

        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsAssignUp"))).Checked = false;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsStartUp"))).Checked = false;
        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsCloseUp"))).Checked = false;

        ((CheckBox)(this.FV_TaskSubType.FindControl("ckIsEC"))).Checked = true;
        ((com.Sconit.Control.CodeMstrDropDownList)this.FV_TaskSubType.FindControl("ddlECType")).SelectedIndex = 0;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbECUser"))).Text = string.Empty;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbDesc2"))).Text = string.Empty;

        ((TextBox)(this.FV_TaskSubType.FindControl("tbAssignUser"))).Text = string.Empty;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbStartUser"))).Text = string.Empty;

        ((TextBox)(this.FV_TaskSubType.FindControl("tbAssignUpTime"))).Text = TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_ASSIGN_UP_TIME).Value;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbStartUpTime"))).Text = TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_START_UP_TIME).Value;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbCloseUpTime"))).Text = TheEntityPreferenceMgr.LoadEntityPreference(ISIConstants.ENTITY_PREFERENCE_CODE_ISI_CLOSE_UP_TIME).Value;

        ((TextBox)(this.FV_TaskSubType.FindControl("tbAssignUpUser"))).Text = string.Empty;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbStartUpUser"))).Text = string.Empty;
        ((TextBox)(this.FV_TaskSubType.FindControl("tbCloseUpUser"))).Text = string.Empty;

    }

    protected void ODS_TaskSubType_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        taskSubType = (TaskSubType)e.InputParameters[0];

        if (taskSubType != null)
        {
            taskSubType.Code = taskSubType.Code.Trim();
            string parent = ((Controls_TextBox)(this.FV_TaskSubType.FindControl("tbParent"))).Text.Trim();

            if (!string.IsNullOrEmpty(parent))
            {
                taskSubType.Parent = this.TheTaskSubTypeMgr.LoadTaskSubType(parent);
            }
            taskSubType.ECUser = ISIUtil.GetUser(taskSubType.ECUser);
            taskSubType.StartUser = ISIUtil.GetUser(taskSubType.StartUser);
            taskSubType.AssignUser = ISIUtil.GetUser(taskSubType.AssignUser);
            taskSubType.ViewUser = ISIUtil.GetUser(taskSubType.ViewUser);
            taskSubType.StartUpUser = ISIUtil.GetUser(taskSubType.StartUpUser);
            taskSubType.AssignUpUser = ISIUtil.GetUser(taskSubType.AssignUpUser);
            taskSubType.CloseUpUser = ISIUtil.GetUser(taskSubType.CloseUpUser);

            taskSubType.IsTrace = true;
            taskSubType.IsAssignUser = true;

            taskSubType.Type = this.ModuleType;//((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlType"))).SelectedValue;
            taskSubType.ProjectType = ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlProjectType"))).SelectedValue;
            taskSubType.Color = ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlColor"))).SelectedValue;

            taskSubType.Org = ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlOrg"))).SelectedValue;
            //taskSubType.FormType = ISIConstants.CODE_MASTER_WFS_FORMTYPE_1;
            taskSubType.ECType = ((CodeMstrDropDownList)(this.FV_TaskSubType.FindControl("ddlECType"))).SelectedValue;

            DateTime now = DateTime.Now;
            taskSubType.CreateDate = now;
            taskSubType.CreateUser = this.CurrentUser.Code;
            taskSubType.LastModifyDate = now;
            taskSubType.LastModifyUser = this.CurrentUser.Code;
        }
    }

    protected void ODS_TaskSubType_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(new string[] { taskSubType.Code, taskSubType.Desc }, e);
            ShowSuccessMessage("ISI.TaskSubType.AddTaskSubType.Successfully", taskSubType.Code);
        }
    }

    protected void checkTaskSubType(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;

        switch (cv.ID)
        {
            case "cvInsert":
                if (TheTaskSubTypeMgr.ExistsCode(args.Value))
                {
                    //ShowErrorMessage("ISI.TaskSubType.CodeExist", args.Value);
                    args.IsValid = false;
                }
                break;
            default:
                break;
        }
    }


    protected void checkUser(object source, ServerValidateEventArgs args)
    {
        CustomValidator cv = (CustomValidator)source;

        switch (cv.ID)
        {
            case "cvStartUser":
            case "cvAssignUser":
            case "cvECUser":
            case "cvViewUser":
            case "cvStartUpUser":
            case "cvAssignUpUser":
            case "cvCloseUpUser":
                string invalidUser = TheTaskMgr.GetInvalidUser(args.Value, this.CurrentUser.Code);
                if (!string.IsNullOrEmpty(invalidUser))
                {
                    cv.ErrorMessage = this.TheLanguageMgr.TranslateMessage("ISI.Error.UserNotExist", this.CurrentUser, new string[] { invalidUser });
                    args.IsValid = false;
                }
                break;
            default:
                break;
        }
    }
}
