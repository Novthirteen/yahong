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

public partial class Facility_MouldMaster_TabNavigator : com.Sconit.Web.ModuleBase
{

    public event EventHandler lbFacilityMasterClickEvent;
    public event EventHandler lbFacilityMaintainClickEvent;
    public event EventHandler lbFacilityTransClickEvent;
    public event EventHandler lbFacilityAttachmentClickEvent;
    public event EventHandler lbFacilityTemplateClickEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lbFacilityMaster_Click(object sender, EventArgs e)
    {
        if (lbFacilityMasterClickEvent != null)
        {
            lbFacilityMasterClickEvent(this, e);
        }

        this.tab_lbFacilityMaster.Attributes["class"] = "ajax__tab_active";
        this.tab_lbFacilityMaintain.Attributes["class"] = "ajax__tab_inactive";
        this.tab_lbFacilityTrans.Attributes["class"] = "ajax__tab_inactive";
        this.tab_lbFacilityAttachment.Attributes["class"] = "ajax__tab_inactive";
        this.tab_lbFacilityTemplate.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbFacilityMaintain_Click(object sender, EventArgs e)
    {
        if (lbFacilityMaintainClickEvent != null)
        {
            lbFacilityMaintainClickEvent(this, e);

            this.tab_lbFacilityMaster.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityMaintain.Attributes["class"] = "ajax__tab_active";
            this.tab_lbFacilityTrans.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityAttachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityTemplate.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbFacilityTrans_Click(object sender, EventArgs e)
    {
        if (lbFacilityTransClickEvent != null)
        {
            lbFacilityTransClickEvent(this, e);

            this.tab_lbFacilityMaster.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityMaintain.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityTrans.Attributes["class"] = "ajax__tab_active";
            this.tab_lbFacilityAttachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityTemplate.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbFacilityAttachment_Click(object sender, EventArgs e)
    {
        if (lbFacilityAttachment != null)
        {
            lbFacilityAttachmentClickEvent(this, e);
            
            this.tab_lbFacilityMaster.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityMaintain.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityTrans.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityAttachment.Attributes["class"] = "ajax__tab_active";
            this.tab_lbFacilityTemplate.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbFacilityTemplate_Click(object sender, EventArgs e)
    {
        if (lbFacilityTemplate != null)
        {
            lbFacilityTemplateClickEvent(this, e);

            this.tab_lbFacilityMaster.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityMaintain.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityTrans.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityAttachment.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityTemplate.Attributes["class"] = "ajax__tab_active";
        }
    }

    public void UpdateAttachmentTitle(string taskCode)
    {
        lbFacilityAttachment.Text = "${Facility.FacilityMaster.Attachment}(<font color='blue'>" + this.TheFacilityMasterMgr.GetFacilityMasterAttachmentCount(taskCode) + "</font>)";
    }

    public void UpdateTemplateTitle(string taskCode)
    {
        lbFacilityTemplate.Text = "${Facility.Template}(<font color='blue'>" + this.TheFacilityMasterMgr.GetFacilityCategoryAttachmentCount(taskCode) + "</font>)";
    }

    public void UpdateView(string taskCode)
    {
        lbFacilityMaster_Click(this, null);
        UpdateAttachmentTitle(taskCode);

        string categoryCode = TheFacilityMasterMgr.LoadFacilityMaster(taskCode).Category;
        UpdateTemplateTitle(categoryCode);
    }

}
