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
using com.Sconit.ISI.Entity;

public partial class ISI_Bill_TabNavigator : com.Sconit.Web.ModuleBase
{
    public string PSIType
    {
        get
        {
            return (string)ViewState["PSIType"];
        }
        set
        {
            ViewState["PSIType"] = value;
        }
    }
    public event EventHandler lbMouldClickEvent;
    public event EventHandler lbMouldAttachmentClickEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lbMould_Click(object sender, EventArgs e)
    {
        if (lbMouldClickEvent != null)
        {
            lbMouldClickEvent(this, e);
        }

        this.tab_lbMould.Attributes["class"] = "ajax__tab_active";
        
        this.tab_lbMouldAttachment.Attributes["class"] = "ajax__tab_inactive";

    }


    protected void lbMouldAttachment_Click(object sender, EventArgs e)
    {
        if (lbMouldAttachmentClickEvent != null)
        {
            lbMouldAttachmentClickEvent(this, e);

            this.tab_lbMould.Attributes["class"] = "ajax__tab_inactive";
            
            this.tab_lbMouldAttachment.Attributes["class"] = "ajax__tab_active";
        }
    }

    public void UpdateAttachmentTitle(string taskCode)
    {
        lbMouldAttachment.Text = "${PSI.Bill.Attachment}(<font color='blue'>" + this.TheAttachmentDetailMgr.GetAttachmentCount(taskCode, typeof(Mould).FullName) + "</font>)";
    }

    public void UpdateView(string taskCode)
    {
        lbMould_Click(this, null);
        UpdateAttachmentTitle(taskCode);
    }
}
