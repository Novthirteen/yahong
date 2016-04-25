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

public partial class Facility_FacilityCategory_TabNavigator : com.Sconit.Web.ModuleBase
{

    public event EventHandler lbFacilityCategoryClickEvent;
    public event EventHandler lbFacilityCategoryAttachmentClickEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lbFacilityCategory_Click(object sender, EventArgs e)
    {
        if (lbFacilityCategoryClickEvent != null)
        {
            lbFacilityCategoryClickEvent(this, e);
        }

        this.tab_lbFacilityCategory.Attributes["class"] = "ajax__tab_active";
        this.tab_lbFacilityCategoryAttachment.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbFacilityCategoryAttachment_Click(object sender, EventArgs e)
    {
        if (lbFacilityCategoryAttachmentClickEvent != null)
        {
            lbFacilityCategoryAttachmentClickEvent(this, e);

            this.tab_lbFacilityCategory.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityCategoryAttachment.Attributes["class"] = "ajax__tab_active";
        }
    }

    public void UpdateAttachmentTitle(string taskCode)
    {
        lbFacilityCategoryAttachment.Text = "${ISI.TSK.Attachment}(<font color='blue'>" + this.TheFacilityMasterMgr.GetFacilityCategoryAttachmentCount(taskCode) + "</font>)";
    }


    public void UpdateView()
    {
        lbFacilityCategory_Click(this, null);
    }

}
