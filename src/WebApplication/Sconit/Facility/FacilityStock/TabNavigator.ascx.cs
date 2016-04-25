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

public partial class Facility_FacilityStock_TabNavigator : com.Sconit.Web.ModuleBase
{

    public event EventHandler lbFacilityStockMasterClickEvent;
    public event EventHandler lbFacilityStockDetailClickEvent;
    public event EventHandler lbFacilityAttachmentClickEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lbFacilityStockMaster_Click(object sender, EventArgs e)
    {
        if (lbFacilityStockMasterClickEvent != null)
        {
            lbFacilityStockMasterClickEvent(this, e);
        }

        this.tab_lbFacilityStockMaster.Attributes["class"] = "ajax__tab_active";
        this.tab_lbFacilityStockDetail.Attributes["class"] = "ajax__tab_inactive";
        this.tab_lbFacilityAttachment.Attributes["class"] = "ajax__tab_inactive";
    }
    protected void lbFacilityAttachment_Click(object sender, EventArgs e)
    {
        if (lbFacilityAttachment != null)
        {
            lbFacilityAttachmentClickEvent(this, e);
           this.tab_lbFacilityAttachment.Attributes["class"] = "ajax__tab_active";
           this.tab_lbFacilityStockDetail.Attributes["class"] = "ajax__tab_inactive";
           this.tab_lbFacilityStockMaster.Attributes["class"] = "ajax__tab_inactive";
        }
    }
    public void UpdateAttachmentTitle(string taskCode)
    {
        lbFacilityAttachment.Text = "${Facility.FacilityStock.Attachment}(<font color='blue'>" + this.TheFacilityMasterMgr.GetFacilityStockAttachmentCount(taskCode) + "</font>)";
    }

    protected void lbFacilityStockDetail_Click(object sender, EventArgs e)
    {
        if (lbFacilityStockDetailClickEvent != null)
        {
            lbFacilityStockDetailClickEvent(this, e);

            this.tab_lbFacilityStockMaster.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityStockDetail.Attributes["class"] = "ajax__tab_active";
            this.tab_lbFacilityAttachment.Attributes["class"] = "ajax__tab_inactive";
        }
    }


    public void UpdateView(string taskCode)
    {
        lbFacilityStockMaster_Click(this, null);
        UpdateAttachmentTitle(taskCode);
    }

}
