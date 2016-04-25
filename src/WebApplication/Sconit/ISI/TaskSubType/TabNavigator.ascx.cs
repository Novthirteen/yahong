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
using com.Sconit.Utility;
using com.Sconit.ISI.Entity.Util;
public partial class ISI_TaskSubType_TabNavigator : com.Sconit.Web.ModuleBase
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
    public event EventHandler lblMstrClickEvent;
    public event EventHandler lblBudgetClickEvent;
    public event EventHandler lblProcessClickEvent;
    public event EventHandler lblFormClickEvent;
    public event EventHandler lblAttachmentClickEvent;

    public void UpdateView(string taskSubTypeCode)
    {
        //if (this.ModuleType == ISIConstants.ISI_TASK_TYPE_WORKFLOW)
        {
            //lbProcess_Click(this, null);
        }
        //else
        {
            lbMstr_Click(this, null);
            this.TaskSubTypeCode = taskSubTypeCode;
            UpdateAttachmentTitle(this.TaskSubTypeCode);
        }
    }

    public void UpdateAttachmentTitle(string taskSubTypeCode)
    {
        lblAttachment.Text = "${ISI.TaskSubType.Attachment}(<font color='blue'>" + this.TheAttachmentDetailMgr.GetTaskSubTypeAttachmentCount(taskSubTypeCode) + "</font>)";
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lbMstr_Click(object sender, EventArgs e)
    {
        if (lblMstrClickEvent != null)
        {
            lblMstrClickEvent(this, e);
        }

        this.tab_mstr.Attributes["class"] = "ajax__tab_active";
        this.tab_process.Attributes["class"] = "ajax__tab_inactive";
        this.tab_form.Attributes["class"] = "ajax__tab_inactive";
        this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
        this.tab_budget.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbProcess_Click(object sender, EventArgs e)
    {
        if (lblProcessClickEvent != null)
        {
            lblProcessClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_active";
            this.tab_form.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_budget.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbForm_Click(object sender, EventArgs e)
    {
        if (lblFormClickEvent != null)
        {
            lblFormClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_inactive";
            this.tab_form.Attributes["class"] = "ajax__tab_active";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_budget.Attributes["class"] = "ajax__tab_inactive";
        }
    }
    protected void lbBudget_Click(object sender, EventArgs e)
    {
        if (lblProcessClickEvent != null)
        {
            lblBudgetClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_inactive";
            this.tab_form.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_budget.Attributes["class"] = "ajax__tab_active";
        }
    }

    protected void lbAttachment_Click(object sender, EventArgs e)
    {
        if (lblAttachmentClickEvent != null)
        {
            lblAttachmentClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_inactive";
            this.tab_form.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_active";
            this.tab_budget.Attributes["class"] = "ajax__tab_inactive";
        }
    }
}
