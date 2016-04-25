using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using NHibernate.Expression;

public partial class ISI_Task_Main : MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucSearch.ExportEvent += new EventHandler(ucSearch_ExportEvent);
        this.ucList.EditEvent += new System.EventHandler(this.ListEdit_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);

        if (!IsPostBack)
        {

            if (this.Action == BusinessConstants.PAGE_LIST_ACTION)
            {
                ucSearch.QuickSearch(this.ActionParameter);
            }
        }
    }

    //The event handler when user click button "Back" button of ucEdit
    void EditBack_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucSearch.Visible = true;
        this.ucList.Visible = true;
        this.ucList.UpdateView();
    }

    //The event handler when user click link "Edit" link of ucList
    void ListEdit_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucEdit.InitPageParameter((string)sender);
    }

    void ucSearch_ExportEvent(object sender, EventArgs e)
    {
        this.ucList.SetSearchCriteria((DetachedCriteria)((object[])sender)[0], (DetachedCriteria)((object[])sender)[1], (IDictionary<string, string>)((object[])sender)[2]);
        this.ucList.Visible = true;
        this.ucList.UpdateView();
        this.ucList.Export();
    }

    //The event handler when user click button "Search" button
    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.SetSearchCriteria((DetachedCriteria)((object[])sender)[0], (DetachedCriteria)((object[])sender)[1], (IDictionary<string, string>)((object[])sender)[2]);
        this.ucList.Visible = true;
        this.ucList.UpdateView();
    }
}
