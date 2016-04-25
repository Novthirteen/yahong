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
using com.Sconit.Web;

public partial class Facility_FacilityBatchMaintain_TabNavigator : ModuleBase
{
    public event EventHandler lbMaintainStartCliclEvent;
    public event EventHandler lbMaintainFinishCliclEvent;


    public void SelectFirstTab()
    {
        lbMaintainStart_Click(this, null);
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void lbMaintainStart_Click(object sender, EventArgs e)
    {
        if (lbMaintainStartCliclEvent != null)
        {
            lbMaintainStartCliclEvent(this, e);
        }

        this.tab_start.Attributes["class"] = "ajax__tab_active";
        this.tab_finish.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbMaintainFinish_Click(object sender, EventArgs e)
    {
        if (lbMaintainFinishCliclEvent != null)
        {
            lbMaintainFinishCliclEvent(this, e);
        }

        this.tab_start.Attributes["class"] = "ajax__tab_inactive";
        this.tab_finish.Attributes["class"] = "ajax__tab_active";
    }
}
