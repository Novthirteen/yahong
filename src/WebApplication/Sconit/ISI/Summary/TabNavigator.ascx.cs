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

public partial class ISI_Summary_TabNavigator : com.Sconit.Web.ModuleBase
{
    public string SummaryCode
    {
        get
        {
            return (string)ViewState["SummaryCode"];
        }
        set
        {
            ViewState["SummaryCode"] = value;
        }
    }

    public event EventHandler lblMstrClickEvent;
    public event EventHandler lblAttachmentClickEvent;

    public void UpdateView(string summaryCode)
    {
        lbMstr_Click(this, null);
        this.SummaryCode = summaryCode;
        UpdateAttachmentTitle(this.SummaryCode);
    }

    public void UpdateAttachmentTitle(string summaryCode)
    {
        lblAttachment.Text = "${ISI.Summary.Attachment}(<font color='blue'>" + this.TheAttachmentDetailMgr.GetAttachmentCount(summaryCode) + "</font>)";
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
