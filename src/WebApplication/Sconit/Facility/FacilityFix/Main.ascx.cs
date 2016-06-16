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

public partial class Facility_FacilityFix_Main : MainModuleBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucFinish.FinishBackEvent += new System.EventHandler(this.FinishBack_Render);
        this.ucList.FixStartEvent += new System.EventHandler(this.ListStart_Render);
        this.ucList.FixFinishEvent += new System.EventHandler(this.ListFinish_Render);
        this.ucTrans.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucFinish.BackEvent += new System.EventHandler(this.Back_Render);

        if (!IsPostBack)
        {
            if (this.Action == BusinessConstants.PAGE_LIST_ACTION)
            {
                ucSearch.QuickSearch(this.ActionParameter);
            }
            if (this.Action == BusinessConstants.PAGE_NEW_ACTION)
            {
                New_Render(this, null);
            }
            if (this.Action == BusinessConstants.PAGE_EDIT_ACTION)
            {
                ListFinish_Render(this.ActionParameter["Code"], null);
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

    //The event handler when user click button "New" button
    void New_Render(object sender, EventArgs e)
    {
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
    }

    //The event handler when user click link "Start" link of ucList
    void ListStart_Render(object sender, EventArgs e)
    {
        this.ucStart.Visible = true;
        this.ucFinish.Visible = false;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucStart.InitPageParameter((string)sender);
    }

    //The event handler when user click link "Finish" link of ucList
    void ListFinish_Render(object sender, EventArgs e)
    {
        this.ucStart.Visible = false;
        this.ucFinish.Visible = true;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucFinish.InitPageParameter((string)sender);
    }

    //The event handler when user click button "Back" button of ucFinish
    void FinishBack_Render(object sender, EventArgs e)
    {
        this.ucStart.Visible = false;
        this.ucFinish.Visible = false;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucTrans.InitPageParameter(((int)sender).ToString());
        this.ucTrans.Visible = true;
    }

    //The event handler when user click button "Back" button of ucFinish
    void Back_Render(object sender, EventArgs e)
    {
        this.ucStart.Visible = false;
        this.ucFinish.Visible = false;
        this.ucSearch.Visible = true;
        this.ucList.Visible = true;
        this.ucTrans.Visible = false;
        this.ucList.UpdateView();
    }


    //void Finish_Render(object sender, EventArgs e)
    //{
    //    InitPageParameter();
    //}

    //public void InitPageParameter()
    //{
    //    this.ucSearch.Visible = true;
    //    this.ucList.Visible = true;
    //    this.ucNew.Visible = false;
    //    this.ucFinish.Visible = false;

    //    IDictionary<string, string> mpDic = new Dictionary<string, string>();
    //    mpDic.Add("FCID", this.FCID);
    //    this.ucSearch.QuickSearch(mpDic);
    //}
}
