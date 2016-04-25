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

public partial class Facility_FacilityTrans_TabNavigator : com.Sconit.Web.ModuleBase
{


    public event EventHandler lbFacilityTransClickEvent;
    public event EventHandler lbFacilityAttachmentClickEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

  

    protected void lbFacilityTrans_Click(object sender, EventArgs e)
    {
        if (lbFacilityTransClickEvent != null)
        {
            lbFacilityTransClickEvent(this, e);

            this.tab_lbFacilityTrans.Attributes["class"] = "ajax__tab_active";
            this.tab_lbFacilityAttachment.Attributes["class"] = "ajax__tab_inactive";
        }
    }

    protected void lbFacilityAttachment_Click(object sender, EventArgs e)
    {
        if (lbFacilityAttachment != null)
        {
            lbFacilityAttachmentClickEvent(this, e);
            
          
            this.tab_lbFacilityTrans.Attributes["class"] = "ajax__tab_inactive";
            this.tab_lbFacilityAttachment.Attributes["class"] = "ajax__tab_active";
        }
    }

  

    public void UpdateAttachmentTitle(string taskCode)
    {
        lbFacilityAttachment.Text = "${Facility.FacilityMaster.Attachment}(<font color='blue'>" + this.TheFacilityMasterMgr.GetFacilityTransAttachmentCount(taskCode) + "</font>)";
    }

   
    public void UpdateView(string taskCode)
    {
        lbFacilityTrans_Click(this, null);
        UpdateAttachmentTitle(taskCode);

    }

}
