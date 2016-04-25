using System;
using System.Collections;
using System.Collections.Generic;
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
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;

public partial class Facility_FacilityMaintain_StartMain : MainModuleBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucList.StartEvent += new System.EventHandler(this.Start_Render);

        if (!IsPostBack)
        {

        }
    }

    //The event handler when user click button "Search" button
    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.Visible = true;
        this.ucList.InitPageParameter((string)((object[])sender)[0]);
    }

    //The event handler when user click link "Edit" link of ucList
    void Start_Render(object sender, EventArgs e)
    {
        this.ucSearch.Visible = true;
        this.ucList.Visible = false;
    }


}
