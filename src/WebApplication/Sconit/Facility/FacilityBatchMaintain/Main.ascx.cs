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
using com.Sconit.Entity;

public partial class Facility_FacilityBatchMaintain_Main : MainModuleBase
{

    public Facility_FacilityBatchMaintain_Main()
    {

    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucNavigator.lbMaintainStartCliclEvent += new System.EventHandler(this.TabMaintainStartClick_Render);
        this.ucNavigator.lbMaintainFinishCliclEvent += new System.EventHandler(this.TabMaintainFinishClick_Render);

        if (!IsPostBack)
        {

        }
    }

    protected void TabMaintainStartClick_Render(object sender, EventArgs e)
    {
        this.ucStart.Visible = true;
        this.ucFinish.Visible = false;
    }

    protected void TabMaintainFinishClick_Render(object sender, EventArgs e)
    {
        this.ucStart.Visible = false;
        this.ucFinish.Visible = true;
    }
}
