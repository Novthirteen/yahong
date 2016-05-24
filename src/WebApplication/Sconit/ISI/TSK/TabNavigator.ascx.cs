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
using com.Sconit.ISI.Entity;

public partial class ISI_TSK_TabNavigator : com.Sconit.Web.ModuleBase
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

    public event EventHandler lblMstrClickEvent;
    public event EventHandler lblDetailClickEvent;
    public event EventHandler lblRefTaskClickEvent;
    public event EventHandler lblStatusClickEvent;
    public event EventHandler lblAttachmentClickEvent;
    public event EventHandler lblWikiClickEvent;
    public event EventHandler lblProcessClickEvent;
    public event EventHandler lblProcessInstanceClickEvent;
    public void UpdateView(string taskCode)
    {
        lbMstr_Click(this, null);
        this.TaskCode = taskCode;
        var task = this.TheTaskMstrMgr.CheckAndLoadTaskMstr(this.TaskCode);
        UpdateAttachmentTitle(this.TaskCode, task.ExtNo, task.ProjectTask.ToString(), task.TaskSubType.Code);
    }

    public void UpdateAttachmentTitle(string taskCode, string extNo, string projectTaskId, string taskSubType)
    {
        int count = this.TheAttachmentDetailMgr.GetTaskAttachmentCount(taskCode);
        int templates = 0;
        if (!string.IsNullOrEmpty(extNo))
        {
            templates += this.TheAttachmentDetailMgr.GetMaintainPlanAttachmentCount(extNo);
        }
        if (!string.IsNullOrEmpty(projectTaskId) && projectTaskId != "0")
        {
            templates += this.TheAttachmentDetailMgr.GetProjectAttachmentCount(projectTaskId);
        }
        if (!string.IsNullOrEmpty(taskSubType))
        {
            templates += this.TheAttachmentDetailMgr.GetTaskSubTypeAttachmentCount(taskSubType);
        }
        if (count > 0)
        {
            lblAttachment.Text = "${ISI.TSK.Attachment}(<font color='blue'>" + count + "</font>" + (templates > 0 ? "&#47;" + templates : string.Empty) + ")";
        }
        else
        {
            lblAttachment.Text = "${ISI.TSK.Attachment}(" + count + (templates > 0 ? "&#47;" + templates : string.Empty) + ")";
        }
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
        this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
        this.tab_reftask.Attributes["class"] = "ajax__tab_inactive";
        this.tab_processInstance.Attributes["class"] = "ajax__tab_inactive";
        this.tab_status.Attributes["class"] = "ajax__tab_inactive";
        this.tab_process.Attributes["class"] = "ajax__tab_inactive";
        this.tab_detail.Attributes["class"] = "ajax__tab_inactive";
        this.tab_wiki.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbRefTask_Click(object sender, EventArgs e)
    {
        if (lblDetailClickEvent != null)
        {
            lblRefTaskClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_reftask.Attributes["class"] = "ajax__tab_active";
            this.tab_processInstance.Attributes["class"] = "ajax__tab_inactive";
            this.tab_status.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_inactive";
            this.tab_detail.Attributes["class"] = "ajax__tab_inactive";
            this.tab_wiki.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbDetail_Click(object sender, EventArgs e)
    {
        if (lblDetailClickEvent != null)
        {
            lblDetailClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_reftask.Attributes["class"] = "ajax__tab_inactive";
            this.tab_status.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_inactive";
            this.tab_processInstance.Attributes["class"] = "ajax__tab_inactive";
            this.tab_detail.Attributes["class"] = "ajax__tab_active";
            this.tab_wiki.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbAttachment_Click(object sender, EventArgs e)
    {
        if (lblAttachmentClickEvent != null)
        {
            lblAttachmentClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_active";
            this.tab_reftask.Attributes["class"] = "ajax__tab_inactive";
            this.tab_processInstance.Attributes["class"] = "ajax__tab_inactive";
            this.tab_status.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_inactive";
            this.tab_detail.Attributes["class"] = "ajax__tab_inactive";
            this.tab_wiki.Attributes["class"] = "ajax__tab_inactive";
        }
    }
    /*
    protected void lbComment_Click(object sender, EventArgs e)
    {
        if (lblCommentClickEvent != null)
        {
            lblCommentClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_comment.Attributes["class"] = "ajax__tab_active";
            this.tab_status.Attributes["class"] = "ajax__tab_inactive";
            this.tab_processInstance.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_inactive";
            this.tab_detail.Attributes["class"] = "ajax__tab_inactive";
            this.tab_wiki.Attributes["class"] = "ajax__tab_inactive";
        }
    }
    */

    protected void lbProcessInstance_Click(object sender, EventArgs e)
    {
        if (lblProcessInstanceClickEvent != null)
        {
            lblProcessInstanceClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_reftask.Attributes["class"] = "ajax__tab_inactive";
            this.tab_processInstance.Attributes["class"] = "ajax__tab_active";
            this.tab_status.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_inactive";
            this.tab_detail.Attributes["class"] = "ajax__tab_inactive";
            this.tab_wiki.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbStatus_Click(object sender, EventArgs e)
    {
        if (lblStatusClickEvent != null)
        {
            lblStatusClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_reftask.Attributes["class"] = "ajax__tab_inactive";
            this.tab_processInstance.Attributes["class"] = "ajax__tab_inactive";
            this.tab_status.Attributes["class"] = "ajax__tab_active";
            this.tab_process.Attributes["class"] = "ajax__tab_inactive";
            this.tab_detail.Attributes["class"] = "ajax__tab_inactive";
            this.tab_wiki.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbProcess_Click(object sender, EventArgs e)
    {
        if (lblProcessClickEvent != null)
        {
            lblProcessClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_reftask.Attributes["class"] = "ajax__tab_inactive";
            this.tab_processInstance.Attributes["class"] = "ajax__tab_inactive";
            this.tab_status.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_active";
            this.tab_detail.Attributes["class"] = "ajax__tab_inactive";
            this.tab_wiki.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbWiki_Click(object sender, EventArgs e)
    {
        if (lblWikiClickEvent != null)
        {
            lblWikiClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_reftask.Attributes["class"] = "ajax__tab_inactive";
            this.tab_processInstance.Attributes["class"] = "ajax__tab_inactive";
            this.tab_status.Attributes["class"] = "ajax__tab_inactive";
            this.tab_process.Attributes["class"] = "ajax__tab_inactive";
            this.tab_detail.Attributes["class"] = "ajax__tab_inactive";
            this.tab_wiki.Attributes["class"] = "ajax__tab_active";
        }
    }
}
