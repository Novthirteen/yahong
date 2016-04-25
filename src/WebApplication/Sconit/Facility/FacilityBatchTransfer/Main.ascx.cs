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
using NHibernate.Expression;
using System.Collections.Generic;

public partial class Facility_FacilityBatchTransfer_Main : MainModuleBase
{
   
    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucList.TransferEvent += new System.EventHandler(this.Search_Render);
        if (!IsPostBack)
        {
        }
    }


    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.Visible = true;
        this.ucList.InitPageParameter((string)((object[])sender)[0]);

    }



    void Transfer_Render(object sender, EventArgs e)
    {
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter((string)sender);
    }


    //The event handler when user click button "Back" button of ucFinish
    void Back_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucSearch.Visible = true;
        this.ucList.Visible = true;
        this.ucList.UpdateView();
    }
    
}
