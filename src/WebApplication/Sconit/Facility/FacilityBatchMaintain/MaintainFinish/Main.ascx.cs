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

public partial class Facility_FacilityMaintain_FinishMain : MainModuleBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucList.FinishEvent += new System.EventHandler(this.Finish_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);

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
    void Finish_Render(object sender, EventArgs e)
    {
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter((string)sender);
    }

    //The event handler when user click link "Edit" link of ucList
    void EditBack_Render(object sender, EventArgs e)
    {
        this.ucSearch.Visible = true;
        this.ucList.Visible = false;
        this.ucEdit.Visible = false;
    }

}
