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

public partial class Facility_FacilitySell_Main : MainModuleBase
{
    //protected string FCID
    //{
    //    get
    //    {
    //        return (string)ViewState["FCID"];
    //    }
    //    set
    //    {
    //        ViewState["FCID"] = value;
    //    }
    //}

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        //this.ucSearch.NewEvent += new System.EventHandler(this.New_Render);
        this.ucList.EditEvent += new System.EventHandler(this.ListEdit_Render);
        //this.ucList.TransferEvent += new System.EventHandler(this.ListTransfer_Render);
        //this.ucNew.BackEvent += new System.EventHandler(this.NewBack_Render);
        //this.ucNew.CreateEvent += new System.EventHandler(this.CreateBack_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.Back_Render);
        //this.ucEdit.EditEvent += new System.EventHandler(this.Edit_Render);
        this.ucTrans.BackEvent += new System.EventHandler(this.Back_Render);
        this.ucEdit.FinishBackEvent += new System.EventHandler(this.EditBack_Render);

        if (!IsPostBack)
        {
            if (this.Action == BusinessConstants.PAGE_LIST_ACTION)
            {
                ucSearch.QuickSearch(this.ActionParameter);
            }
            if (this.Action == BusinessConstants.PAGE_EDIT_ACTION)
            {
                ListEdit_Render(this.ActionParameter["Code"], null);
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
    //void New_Render(object sender, EventArgs e)
    //{
    //    this.ucSearch.Visible = false;
    //    this.ucList.Visible = false;
    //    //this.ucNew.Visible = true;
    //    this.ucNew.PageCleanup();
    //}

    //The event handler when user click button "Back" button of ucNew
    //void NewBack_Render(object sender, EventArgs e)
    //{
    //    this.ucNew.Visible = false;
    //    this.ucSearch.Visible = true;
    //    this.ucList.Visible = true;
    //    this.ucTransfer.Visible = false;
    //    this.ucList.UpdateView();
    //}

    //The event handler when user click button "Save" button of ucNew
    void CreateBack_Render(object sender, EventArgs e)
    {
        //this.ucNew.Visible = false;
        this.ucEdit.Visible = true;
        //this.ucTransfer.Visible = false;
        this.ucEdit.InitPageParameter((string)sender);
    }

    //The event handler when user click link "Edit" link of ucList
    void ListEdit_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        //this.ucTransfer.Visible = false;
        this.ucEdit.InitPageParameter((string)sender);
    }

    //void ListTransfer_Render(object sender, EventArgs e)
    //{
    //    //this.ucTransfer.Visible = true;
    //    this.ucEdit.Visible = false;
    //    this.ucSearch.Visible = false;
    //    this.ucList.Visible = false;
    //    this.ucTransfer.InitPageParameter((string)sender);
    //}

    //The event handler when user click button "Back" button of ucFinish
    void EditBack_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucTrans.InitPageParameter(((int)sender).ToString());
        this.ucTrans.Visible = true;
    }

    //The event handler when user click button "Back" button of ucFinish
    void Back_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucSearch.Visible = true;
        this.ucList.Visible = true;
        this.ucTrans.Visible = false;
        this.ucList.UpdateView();
    }

    //void Edit_Render(object sender, EventArgs e)
    //{
    //    InitPageParameter();
    //}

    //public void InitPageParameter()
    //{
    //    this.ucSearch.Visible = true;
    //    this.ucList.Visible = true;
    //    this.ucNew.Visible = false;
    //    this.ucEdit.Visible = false;

    //    IDictionary<string, string> mpDic = new Dictionary<string, string>();
    //    mpDic.Add("FCID", this.FCID);
    //    this.ucSearch.QuickSearch(mpDic);
    //}
}
