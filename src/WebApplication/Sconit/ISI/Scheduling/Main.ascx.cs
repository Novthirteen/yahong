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
using NHibernate.Expression;
using com.Sconit.Entity;
using System.Collections.Generic;

public partial class ISI_Scheduling_Main : MainModuleBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucTabNavigator.lbViewClickEvent += new System.EventHandler(this.TabViewClick_Render);
        this.ucTabNavigator.lbGeneralClickEvent += new System.EventHandler(this.TabGeneralClick_Render);
        this.ucTabNavigator.lbSpecialClickEvent += new System.EventHandler(this.TabSpecialClick_Render);

        if (!IsPostBack)
        {
            this.ucView.Visible = true;
            this.ucGeneral.Visible = false;
            this.ucSpecial.Visible = false;
            this.ucGeneral.IsSpecial = false;
            this.ucSpecial.IsSpecial = true;
        }
    }

    //The event handler when user click link button to "View" tab
    void TabViewClick_Render(object sender, EventArgs e)
    {
        this.ucView.Visible = true;
        this.ucGeneral.Visible = false;
        this.ucSpecial.Visible = false;
    }

    //The event handler when user click link button to "General" tab
    void TabGeneralClick_Render(object sender, EventArgs e)
    {
        this.ucView.Visible = false;
        this.ucGeneral.Visible = true;
        this.ucSpecial.Visible = false;
    }
    //The event handler when user click link button to "Special" tab
    void TabSpecialClick_Render(object sender, EventArgs e)
    {
        this.ucView.Visible = false;
        this.ucGeneral.Visible = false;
        this.ucSpecial.Visible = true;

    }
}

