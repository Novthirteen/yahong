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

public partial class Facility_MaintainPlan_TabNavigator : com.Sconit.Web.ModuleBase
{

    public event EventHandler lbMaintainPlanClickEvent;
    public event EventHandler lbMaintainPlanAttachmentClickEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lbMaintainPlan_Click(object sender, EventArgs e)
    {
        if (lbMaintainPlanClickEvent != null)
        {
            lbMaintainPlanClickEvent(this, e);
        }

        this.tab_lbMaintainPlan.Attributes["class"] = "ajax__tab_active";
        this.tab_lbMaintainPlanAttachment.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbMaintainPlanAttachment_Click(object sender, EventArgs e)
    {
        if (lbMaintainPlanAttachmentClickEvent != null)
        {
            lbMaintainPlanAttachmentClickEvent(this, e);

            this.tab_lbMaintainPlan.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbMaintainPlanAttachment.Attributes["class"] = "ajax__tab_active";
        }
    }

    public void UpdateAttachmentTitle(string taskCode)
    {
        lbMaintainPlanAttachment.Text = "${ISI.TSK.Attachment}(<font color='blue'>" + this.TheAttachmentDetailMgr.GetAttachmentCount(taskCode) + "</font>)";
    }


    public void UpdateView()
    {
        lbMaintainPlan_Click(this, null);
    }

}
