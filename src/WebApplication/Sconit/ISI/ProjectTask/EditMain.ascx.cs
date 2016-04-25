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

public partial class ISI_ProjectTask_EditMain : MainModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler UpdateTitleEvent;

    public Int32 ProjectTaskId
    {
        get
        {
            if (ViewState["ProjectTaskId"] == null)
            {
                return 0;
            }
            else
            {
                return (Int32)ViewState["ProjectTaskId"];
            }
        }
        set
        {
            ViewState["ProjectTaskId"] = value;
        }
    }

    public void InitPageParameter(int projectTaskId)
    {
        this.ProjectTaskId = projectTaskId;

        //this.ucTabNavigator.Visible = true;
        // this.ucEdit.Visible = true;
        //this.ucEdit.InitPageParameter(taskCode);

        this.ucAttachment.Visible = false;
        this.ucTabNavigator.UpdateView(this.ProjectTaskId);
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        this.ucEdit.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucEdit.UpdateTitleEvent += new System.EventHandler(this.EditUpdateTitle_Render);
        this.ucAttachment.BackEvent += new System.EventHandler(this.BackNoEdit_Render);
        this.ucAttachment.UpdateAttacmentTitleEvent += new System.EventHandler(this.UpdateAttacmentTitle_Render);
        this.ucTabNavigator.lblMstrClickEvent += new System.EventHandler(this.TabMstrClick_Render);
        this.ucTabNavigator.lblAttachmentClickEvent += new System.EventHandler(this.TabAttachmentClick_Render);

        if (!IsPostBack)
        {

        }
        this.ucEdit.ProjectTaskId = this.ProjectTaskId;
        this.ucAttachment.TaskCode = this.ProjectTaskId.ToString();
        this.ucAttachment.AttachmentType = typeof(ProjectTask).FullName;

        this.ucTabNavigator.ProjectTaskId = this.ProjectTaskId;
    }


    void EditUpdateTitle_Render(object sender, EventArgs e)
    {
        if (this.UpdateTitleEvent != null)
        {
            this.UpdateTitleEvent(sender, e);
        }
    }

    protected void UpdateAttacmentTitle_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateAttachmentTitle(this.ProjectTaskId);
    }

    protected void BackNoEdit_Render(object sender, EventArgs e)
    {
        this.ucTabNavigator.UpdateView(this.ProjectTaskId);
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
        this.ucAttachment.Visible = false;
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter(this.ProjectTaskId);
    }

    protected void TabAttachmentClick_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucAttachment.Visible = true;
        this.ucAttachment.InitPageParameter(ProjectTaskId, typeof(ProjectTask).FullName);
    }

}
