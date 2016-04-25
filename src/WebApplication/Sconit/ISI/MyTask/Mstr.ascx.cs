using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using NHibernate.Expression;

public partial class ISI_Mstr_Main : MainModuleBase
{
    public string TabAction
    {
        get { return (string)ViewState["TabAction"]; }
        set { ViewState["TabAction"] = value; }
    }
    public event EventHandler UpdateTitleEvent;
    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucList.EditEvent += new System.EventHandler(this.ListEdit_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucEdit.UpdateTitleEvent += new System.EventHandler(this.EditUpdateTitle_Render);

        ucSearch.tabAction = TabAction;
        ucList.tabAction = TabAction;
        if (!IsPostBack)
        {
            //if (this.Action == BusinessConstants.PAGE_LIST_ACTION)
            {
                //ucSearch.GenerateTree();
                //ucSearch.QuickSearch(this.ActionParameter);
            }
        }
    }

    public void InitPageParameter()
    {
        this.ucEdit.Visible = false;
        this.ucSearch.Visible = true;
        ucSearch.QuickSearch(this.ActionParameter);
    }

    void EditUpdateTitle_Render(object sender, EventArgs e)
    {
        if (this.UpdateTitleEvent != null)
        {
            this.UpdateTitleEvent(sender, e);
        }
    }

    //The event handler when user click button "Back" button of ucEdit
    void EditBack_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucSearch.Visible = true;
        this.ucSearch.Refresh(sender, e);
        this.ucList.Visible = true;
        this.ucList.UpdateView();
    }

    //The event handler when user click link "Edit" link of ucList
    void ListEdit_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucEdit.ModuleType = ((string[])sender)[1];
        this.ucEdit.InitPageParameter(((string[])sender)[0]);
    }

    //The event handler when user click button "Search" button
    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.SetSearchCriteria((DetachedCriteria)((object[])sender)[0], (DetachedCriteria)((object[])sender)[1], (IDictionary<string, string>)((object[])sender)[2]);
        this.ucList.Visible = true;
        this.ucList.UpdateView();
        if (UpdateTitleEvent != null)
        {
            UpdateTitleEvent(sender, e);
        }
    }

}
