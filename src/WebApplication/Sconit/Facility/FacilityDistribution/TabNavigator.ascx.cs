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
using com.Sconit.Facility.Entity;

public partial class Facility_FacilityDistribution_TabNavigator : com.Sconit.Web.ModuleBase
{

    public event EventHandler lbFacilityDistributionClickEvent;
    public event EventHandler lbFacilityDistributionDetailClickEvent;
    public event EventHandler lbFacilityDistributionAttachmentClickEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lbFacilityDistribution_Click(object sender, EventArgs e)
    {
        if (lbFacilityDistributionClickEvent != null)
        {
            lbFacilityDistributionClickEvent(this, e);
        }

        this.tab_lbFacilityDistribution.Attributes["class"] = "ajax__tab_active";
        this.tab_lbFacilityDistributionDetail.Attributes["class"] = "ajax__tab_inactive";
        this.tab_lbFacilityDistributionAttachment.Attributes["class"] = "ajax__tab_inactive";
      
    }

    protected void lbFacilityDistributionDetail_Click(object sender, EventArgs e)
    {
        if (lbFacilityDistributionDetailClickEvent != null)
        {
            lbFacilityDistributionDetailClickEvent(this, e);

            this.tab_lbFacilityDistribution.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityDistributionDetail.Attributes["class"] = "ajax__tab_active";
            this.tab_lbFacilityDistributionAttachment.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbFacilityDistributionAttachment_Click(object sender, EventArgs e)
    {
        if (lbFacilityDistributionAttachmentClickEvent != null)
        {
            lbFacilityDistributionAttachmentClickEvent(this, e);

            this.tab_lbFacilityDistribution.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityDistributionDetail.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityDistributionAttachment.Attributes["class"] = "ajax__tab_active";
        }
    }

    public void UpdateAttachmentTitle(string taskCode)
    {
        lbFacilityDistributionAttachment.Text = "${Facility.FacilityDistribution.Attachment}(<font color='blue'>" + this.TheFacilityMasterMgr.GetFacilityDistributionAttachmentCount(taskCode) + "</font>)";
    }

    public void UpdateView(string taskCode)
    {
        lbFacilityDistribution_Click(this, null);
        UpdateAttachmentTitle(taskCode);

     
    }
}
