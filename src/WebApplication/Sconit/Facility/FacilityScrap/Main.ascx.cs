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

public partial class Facility_FacilityScrap_Main : MainModuleBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucList.ScrapEvent += new System.EventHandler(this.ListScrap_Render);
        this.ucScrap.BackEvent += new System.EventHandler(this.Back_Render);
        //this.ucEdit.EditEvent += new System.EventHandler(this.Edit_Render);
        this.ucTrans.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucScrap.FinishBackEvent += new System.EventHandler(this.FinishBack_Render);

        if (!IsPostBack)
        {
            if (this.Action == BusinessConstants.PAGE_LIST_ACTION)
            {
                ucSearch.QuickSearch(this.ActionParameter);
            }
            if (this.Action == BusinessConstants.PAGE_EDIT_ACTION)
            {
                ListScrap_Render(this.ActionParameter["Code"], null);
            }
        }
    }

    //The event handler when user click button "Search" button
    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.SetSearchCriteria((DetachedCriteria)((object[])sender)[0], (DetachedCriteria)((object[])sender)[1]);
        this.ucList.Visible = true;
        this.ucList.UpdateView();
    }


    //The event handler when user click link "Finish" link of ucList
    void ListScrap_Render(object sender, EventArgs e)
    {
        this.ucScrap.Visible = true;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucScrap.InitPageParameter((string)sender);
    }


    //The event handler when user click button "Back" button of ucFinish
    void FinishBack_Render(object sender, EventArgs e)
    {
        this.ucScrap.Visible = false;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucTrans.InitPageParameter(((int)sender).ToString());
        this.ucTrans.Visible = true;
    }

    //The event handler when user click button "Back" button of ucFinish
    void Back_Render(object sender, EventArgs e)
    {
        this.ucScrap.Visible = false;
        this.ucSearch.Visible = true;
        this.ucList.Visible = true;
        this.ucTrans.Visible = false;
        this.ucList.UpdateView();
    }

 
}
