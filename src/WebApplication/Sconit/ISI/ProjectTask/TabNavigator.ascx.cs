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

public partial class ISI_ProjectTask_TabNavigator : com.Sconit.Web.ModuleBase
{
    public int ProjectTaskId
    {
        get
        {
            return (int)ViewState["ProjectTaskId"];
        }
        set
        {
            ViewState["ProjectTaskId"] = value;
        }
    }

    public event EventHandler lblMstrClickEvent;
    public event EventHandler lblAttachmentClickEvent;

    public void UpdateView(int id)
    {
        lbMstr_Click(this, null);
        this.ProjectTaskId = id;
        UpdateAttachmentTitle(this.ProjectTaskId);
    }

    public void UpdateAttachmentTitle(int id)
    {
        lblAttachment.Text = "${ISI.ProjectTask.Attachment}(<font color='blue'>" + this.TheAttachmentDetailMgr.GetProjectAttachmentCount(id.ToString()) + "</font>)";
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lbMstr_Click(object sender, EventArgs e)
    {
        if (lblMstrClickEvent != null)
        {
            lblMstrClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_active";
            this.tab_attachment.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbAttachment_Click(object sender, EventArgs e)
    {
        if (lblAttachmentClickEvent != null)
        {
            lblAttachmentClickEvent(this, e);

            this.tab_mstr.Attributes["class"] = "ajax__tab_inactive";
            this.tab_attachment.Attributes["class"] = "ajax__tab_active";

        }
    }
}
