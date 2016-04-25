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

public partial class ISI_Scheduling_TabNavigator : ModuleBase
{
    public event EventHandler lbViewClickEvent;
    public event EventHandler lbGeneralClickEvent;
    public event EventHandler lbSpecialClickEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void lbView_Click(object sender, EventArgs e)
    {
        if (lbViewClickEvent != null)
        {
            lbViewClickEvent(this, e);
        }

        this.tab_view.Attributes["class"] = "ajax__tab_active";
        this.tab_general.Attributes["class"] = "ajax__tab_inactive";
        this.tab_special.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbGeneral_Click(object sender, EventArgs e)
    {
        if (lbGeneralClickEvent != null)
        {
            lbGeneralClickEvent(this, e);
        }

        this.tab_view.Attributes["class"] = "ajax__tab_inactive";
        this.tab_general.Attributes["class"] = "ajax__tab_active";
        this.tab_special.Attributes["class"] = "ajax__tab_inactive";
    }

    protected void lbSpecial_Click(object sender, EventArgs e)
    {
        if (lbSpecialClickEvent != null)
        {
            lbSpecialClickEvent(this, e);
        }

        this.tab_view.Attributes["class"] = "ajax__tab_inactive";
        this.tab_general.Attributes["class"] = "ajax__tab_inactive";
        this.tab_special.Attributes["class"] = "ajax__tab_active";
    }
}
