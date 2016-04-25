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
using com.Sconit.Entity;
using com.Sconit.Web;
using com.Sconit.ISI.Entity;

public partial class ISI_TSK_EditMain : MainModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler UpdateTitleEvent;

    public string TaskSubTypeCode
    {
        get
        {
            return (string)ViewState["TaskSubTypeCode"];
        }
        set
        {
            ViewState["TaskSubTypeCode"] = value;
        }
    }
    public string TaskSubTypeDesc
    {
        get
        {
            return (string)ViewState["TaskSubTypeDesc"];
        }
        set
        {
            ViewState["TaskSubTypeDesc"] = value;
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

    public void InitPageParameter(String taskSubTypeCode, string taskSubTypeDesc)
    {
        this.TaskSubTypeCode = taskSubTypeCode;
        this.TaskSubTypeDesc = taskSubTypeDesc;
        if (string.IsNullOrEmpty(this.ModuleType) && !string.IsNullOrEmpty(this.TaskSubTypeCode))
        {
            this.ModuleType = this.TheTaskMstrMgr.LoadTaskMstr(TaskSubTypeCode).Type;
            this.ucTabNavigator.ModuleType = this.ModuleType;

            this.ucProcess.ModuleType = this.ModuleType;
            this.ucForm.ModuleType = this.ModuleType;
            this.ucAttachment.ModuleType = this.ModuleType;
            this.ucAttachment.AttachmentType = typeof(TaskSubType).FullName;
            this.ucEdit.ModuleType = this.ModuleType;
            this.ucBudget.ModuleType = this.ModuleType;
        }

        this.ucTabNavigator.Visible = true;
        this.ucEdit.Visible = true;
        this.ucProcess.Visible = false;
        this.ucForm.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucBudget.Visible = false;
        this.ucTabNavigator.UpdateView(this.TaskSubTypeCode);
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucEdit.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucBudget.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucProcess.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucForm.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucAttachment.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucAttachment.UpdateAttacmentTitleEvent += new System.EventHandler(this.UpdateAttacmentTitle_Render);

        this.ucTabNavigator.lblMstrClickEvent += new System.EventHandler(this.TabMstrClick_Render);
        this.ucTabNavigator.lblBudgetClickEvent += new System.EventHandler(this.TabBudgetClick_Render);
        this.ucTabNavigator.lblProcessClickEvent += new System.EventHandler(this.TabProcessClick_Render);
        this.ucTabNavigator.lblAttachmentClickEvent += new System.EventHandler(this.TabAttachmentClick_Render);
        this.ucTabNavigator.lblFormClickEvent += new System.EventHandler(this.TabFormClick_Render);

        if (!IsPostBack)
        {
            this.ucTabNavigator.ModuleType = this.ModuleType;
            this.ucProcess.ModuleType = this.ModuleType;
            this.ucForm.ModuleType = this.ModuleType;
            this.ucAttachment.ModuleType = this.ModuleType;
            this.ucAttachment.AttachmentType = typeof(TaskSubType).FullName;
            this.ucEdit.ModuleType = this.ModuleType;
            this.ucBudget.ModuleType = this.ModuleType;
        }

        this.ucBudget.TaskSubTypeCode = this.TaskSubTypeCode;
        this.ucEdit.TaskSubTypeCode = this.TaskSubTypeCode;
        this.ucProcess.TaskSubTypeCode = this.TaskSubTypeCode;
        this.ucForm.TaskSubTypeCode = this.TaskSubTypeCode;
        this.ucBudget.TaskSubTypeDesc = this.TaskSubTypeDesc;
        this.ucEdit.TaskSubTypeDesc = this.TaskSubTypeDesc;
        this.ucProcess.TaskSubTypeDesc = this.TaskSubTypeDesc;
        this.ucForm.TaskSubTypeDesc = this.TaskSubTypeDesc;
        this.ucAttachment.TaskCode = this.TaskSubTypeCode;

        this.ucTabNavigator.TaskSubTypeCode = this.TaskSubTypeCode;
        this.ucTabNavigator.TaskSubTypeDesc = this.TaskSubTypeDesc;
    }

    protected void UpdateAttacmentTitle_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateAttachmentTitle(this.TaskSubTypeCode);
    }

    protected void Back_Render(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void TabMstrClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter(this.TaskSubTypeCode, TaskSubTypeDesc);
        this.ucProcess.Visible = false;
        this.ucForm.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucBudget.Visible = false;
    }

    protected void TabBudgetClick_Render(object sender, EventArgs e)
    {
        this.ucBudget.Visible = true;
        this.ucBudget.InitPageParameter(this.TaskSubTypeCode, TaskSubTypeDesc);
        this.ucProcess.Visible = false;
        this.ucForm.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucEdit.Visible = false;
    }

    protected void TabProcessClick_Render(object sender, EventArgs e)
    {
        this.ucProcess.Visible = true;
        this.ucProcess.InitPageParameter(this.TaskSubTypeCode, TaskSubTypeDesc);
        this.ucBudget.Visible = false;
        this.ucForm.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucEdit.Visible = false;
    }

    protected void TabFormClick_Render(object sender, EventArgs e)
    {
        this.ucForm.Visible = true;
        this.ucForm.InitPageParameter(this.TaskSubTypeCode, TaskSubTypeDesc);
        this.ucBudget.Visible = false;
        this.ucProcess.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucEdit.Visible = false;
    }
    protected void TabAttachmentClick_Render(object sender, EventArgs e)
    {
        this.ucAttachment.Visible = true;
        this.ucAttachment.InitPageParameter(this.TaskSubTypeCode, typeof(TaskSubType).FullName);
        this.ucBudget.Visible = false;
        this.ucProcess.Visible = false;
        this.ucForm.Visible = false;
        this.ucEdit.Visible = false;
    }
}
