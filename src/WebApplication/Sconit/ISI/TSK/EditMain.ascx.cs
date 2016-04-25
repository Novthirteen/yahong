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

    public void InitPageParameter(String taskCode)
    {
        this.TaskCode = taskCode;
        if (string.IsNullOrEmpty(this.ModuleType) && !string.IsNullOrEmpty(this.TaskCode))
        {
            this.ModuleType = this.TheTaskMstrMgr.LoadTaskMstr(TaskCode).Type;
            this.ucTabNavigator.ModuleType = this.ModuleType;
            this.ucDetail.ModuleType = this.ModuleType;
            this.ucWiki.ModuleType = this.ModuleType;
            this.ucStatus.ModuleType = this.ModuleType;
            this.ucProcess.ModuleType = this.ModuleType;
            this.ucEdit.ModuleType = this.ModuleType;
            this.ucAttachment.ModuleType = this.ModuleType;
            this.ucAttachment.AttachmentType = typeof(TaskMstr).FullName;
            this.ucRefTask.ModuleType = this.ModuleType;
            this.ucProcessInstance.ModuleType = this.ModuleType;
            this.ucResMatrixDet.ModuleType = this.ModuleType;
        }
        this.ucTabNavigator.Visible = true;
        this.ucEdit.Visible = true;
        //this.ucEdit.InitPageParameter(taskCode);
        this.ucProcessInstance.Visible = false;
        this.ucResMatrixDet.Visible = false;
        this.ucDetail.Visible = false;
        this.ucWiki.Visible = false;
        this.ucProcess.Visible = false;
        this.ucStatus.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucRefTask.Visible = false;
        this.ucTabNavigator.UpdateView(this.TaskCode);
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        this.ucEdit.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucEdit.UpdateTitleEvent += new System.EventHandler(this.EditUpdateTitle_Render);
        this.ucEdit.UpdateAttachmentTitleEvent += new System.EventHandler(this.UpdateAttachmentTitle_Render);
        this.ucDetail.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucRefTask.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucWiki.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucProcess.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucAttachment.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucAttachment.UpdateAttacmentTitleEvent += new System.EventHandler(this.UpdateAttacmentTitle_Render);
        this.ucStatus.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucProcessInstance.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucResMatrixDet.BackEvent += new System.EventHandler(this.Back_Render);

        this.ucTabNavigator.lblMstrClickEvent += new System.EventHandler(this.TabMstrClick_Render);
        this.ucTabNavigator.lblDetailClickEvent += new System.EventHandler(this.TabDetailClick_Render);
        this.ucTabNavigator.lblRefTaskClickEvent += new System.EventHandler(this.TabRefTaskClick_Render);
        this.ucTabNavigator.lblWikiClickEvent += new System.EventHandler(this.TabWikiClick_Render);
        this.ucTabNavigator.lblStatusClickEvent += new System.EventHandler(this.TabStatusClick_Render);
        this.ucTabNavigator.lblProcessInstanceClickEvent += new System.EventHandler(this.TabProcessInstanceClick_Render);
        this.ucTabNavigator.lblResMatrixDetClickEvent += new System.EventHandler(this.TabResMatrixClick_Render);

        this.ucTabNavigator.lblProcessClickEvent += new System.EventHandler(this.TabProcessClick_Render);
        this.ucTabNavigator.lblAttachmentClickEvent += new System.EventHandler(this.TabAttachmentClick_Render);

        if (!IsPostBack)
        {
            this.ucTabNavigator.ModuleType = this.ModuleType;
            this.ucDetail.ModuleType = this.ModuleType;
            this.ucRefTask.ModuleType = this.ModuleType;
            this.ucWiki.ModuleType = this.ModuleType;
            this.ucStatus.ModuleType = this.ModuleType;
            this.ucProcessInstance.ModuleType = this.ModuleType;
            this.ucResMatrixDet.ModuleType = this.ModuleType;
            this.ucProcess.ModuleType = this.ModuleType;
            this.ucEdit.ModuleType = this.ModuleType;
            this.ucAttachment.ModuleType = this.ModuleType;
        }

        this.ucDetail.TaskCode = this.TaskCode;
        this.ucRefTask.TaskCode = this.TaskCode;
        this.ucEdit.TaskCode = this.TaskCode;
        this.ucProcess.TaskCode = this.TaskCode;
        this.ucStatus.TaskCode = this.TaskCode;
        this.ucProcessInstance.TaskCode = this.TaskCode;
        this.ucResMatrixDet.TaskCode = this.TaskCode;
        this.ucWiki.TaskCode = this.TaskCode;
        this.ucAttachment.TaskCode = this.TaskCode;
        this.ucTabNavigator.TaskCode = this.TaskCode;
    }


    void EditUpdateTitle_Render(object sender, EventArgs e)
    {
        if (this.UpdateTitleEvent != null)
        {
            this.UpdateTitleEvent(sender, e);
        }
    }


    protected void UpdateAttachmentTitle_Render(object sender, EventArgs e)
    {
        if (!string.IsNullOrEmpty(this.TaskCode))
        {
            string[] obj = (string[])sender;
            this.ucTabNavigator.UpdateAttachmentTitle(obj[0], obj[1], obj[2], obj[3]);
        }
    }

    protected void UpdateAttacmentTitle_Render(object sender, EventArgs e)
    {
        var task = this.TheTaskMstrMgr.CheckAndLoadTaskMstr(this.TaskCode);
        if (task != null)
        {
            this.ucTabNavigator.UpdateAttachmentTitle(task.Code, task.ExtNo, task.ProjectTask.ToString(), task.TaskSubType.Code);
        }
    }

    protected void BackNoEdit_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateView(this.TaskCode);
    }

    protected void Back_Render(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this.TaskCode, e);
        }
    }

    protected void TabMstrClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter(this.TaskCode);
        this.ucDetail.Visible = false;
        this.ucWiki.Visible = false;
        this.ucProcess.Visible = false;
        this.ucStatus.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucRefTask.Visible = false;
        this.ucProcessInstance.Visible = false;
        this.ucResMatrixDet.Visible = false;
    }

    protected void TabRefTaskClick_Render(object sender, EventArgs e)
    {
        //this.ucDetail.TaskCode = TaskCode;
        //this.ucDetail.InitPageParameter();

        this.ucEdit.Visible = false;
        this.ucDetail.Visible = false;
        this.ucWiki.Visible = false;
        this.ucProcess.Visible = false;
        this.ucStatus.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucRefTask.Visible = true;
        this.ucProcessInstance.Visible = false;
        this.ucResMatrixDet.Visible = false;
        this.ucRefTask.InitPageParameter(TaskCode);
    }


    protected void TabDetailClick_Render(object sender, EventArgs e)
    {
        //this.ucDetail.TaskCode = TaskCode;
        //this.ucDetail.InitPageParameter();

        this.ucEdit.Visible = false;
        this.ucDetail.Visible = true;
        this.ucWiki.Visible = false;
        this.ucProcess.Visible = false;
        this.ucStatus.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucRefTask.Visible = false;
        this.ucProcessInstance.Visible = false;
        this.ucResMatrixDet.Visible = false;
        this.ucDetail.InitPageParameter(TaskCode);
    }

    protected void TabWikiClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetail.Visible = false;
        this.ucWiki.Visible = true;
        this.ucProcess.Visible = false;
        this.ucStatus.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucRefTask.Visible = false;
        this.ucProcessInstance.Visible = false;
        this.ucResMatrixDet.Visible = false;
        this.ucWiki.InitPageParameter(TaskCode);
    }

    protected void TabProcessClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetail.Visible = false;
        this.ucWiki.Visible = false;
        this.ucProcess.Visible = true;
        this.ucStatus.Visible = false;
        this.ucAttachment.Visible = false;
        this.ucRefTask.Visible = false;
        this.ucProcessInstance.Visible = false;
        this.ucResMatrixDet.Visible = false;
        this.ucProcess.InitPageParameter(TaskCode);

    }
    protected void TabAttachmentClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetail.Visible = false;
        this.ucWiki.Visible = false;
        this.ucProcess.Visible = false;
        this.ucStatus.Visible = false;
        this.ucAttachment.Visible = true;
        this.ucRefTask.Visible = false;
        this.ucProcessInstance.Visible = false;
        this.ucResMatrixDet.Visible = false;
        this.ucAttachment.InitPageParameter(TaskCode, typeof(TaskMstr).FullName);
    }

    protected void TabProcessInstanceClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetail.Visible = false;
        this.ucWiki.Visible = false;
        this.ucProcess.Visible = false;
        this.ucStatus.Visible = false;
        this.ucProcessInstance.Visible = true;
        this.ucAttachment.Visible = false;
        this.ucRefTask.Visible = false;
        this.ucResMatrixDet.Visible = false;
        this.ucProcessInstance.InitPageParameter(TaskCode, this.ModuleType);
    }

    protected void TabStatusClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetail.Visible = false;
        this.ucWiki.Visible = false;
        this.ucProcess.Visible = false;
        this.ucStatus.Visible = true;
        this.ucAttachment.Visible = false;
        this.ucRefTask.Visible = false;
        this.ucProcessInstance.Visible = false;
        this.ucResMatrixDet.Visible = false;
        this.ucStatus.InitPageParameter(TaskCode, this.ModuleType);
    }

    protected void TabResMatrixClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucDetail.Visible = false;
        this.ucWiki.Visible = false;
        this.ucProcess.Visible = false;
        this.ucStatus.Visible = false;
        this.ucProcessInstance.Visible = false;
        this.ucResMatrixDet.Visible = true;
        this.ucAttachment.Visible = false;
        this.ucRefTask.Visible = false;
        this.ucResMatrixDet.InitPageParameter(TaskCode);
    }

}
