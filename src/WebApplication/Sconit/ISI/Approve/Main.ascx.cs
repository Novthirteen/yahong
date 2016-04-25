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

public partial class ISI_Approve_Main : MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucList.SummaryEvent += new System.EventHandler(this.ListSummary_Render);
        this.ucList.EditEvent += new System.EventHandler(this.ListEdit_Render);
        this.ucEdit.BackEvent += new System.EventHandler(this.EditBack_Render);
        this.ucSearch.ExportEvent += new EventHandler(ucSearch_ExportEvent);
        this.ucSearch.ApproveEvent += new EventHandler(ucSearch_ApproveEvent);
        this.ucSearch.CloseEvent += new EventHandler(ucSearch_CloseEvent);
        this.ucSummary.BackEvent += new System.EventHandler(this.EditBack_Render);

        if (!IsPostBack)
        {
            if (this.Action == BusinessConstants.PAGE_LIST_ACTION)
            {
                ucSearch.QuickSearch(this.ActionParameter);
            }
        }
    }

    void ucSearch_ApproveEvent(object sender, EventArgs e)
    {
        if (this.ucList.Visible)
        {
            this.ucList.Approve();
        }
    }

    void ucSearch_CloseEvent(object sender, EventArgs e)
    {
        if (this.ucList.Visible)
        {
            this.ucList.Close();
        }
    }


    void ucSearch_ExportEvent(object sender, EventArgs e)
    {
        this.ucList.IsExport = true;
        this.ucList.InitPageParameter(sender);
        this.ucList.Visible = true;
        this.ucList.Export();
    }

    //The event handler when user click button "Search" button
    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.InitPageParameter(sender);
        this.ucList.Visible = true;
    }
    //The event handler when user click link "Edit" link of ucList
    void ListSummary_Render(object sender, EventArgs e)
    {
        this.ucSummary.Visible = true;
        this.ucEdit.Visible = false;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucSummary.InitPageParameter((string)sender);
    }


    //The event handler when user click link "Edit" link of ucList
    void ListEdit_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = true;
        this.ucSummary.Visible = false;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucEdit.InitPageParameter((string)sender);
    }

    //The event handler when user click button "Back" button of ucEdit
    void EditBack_Render(object sender, EventArgs e)
    {
        this.ucEdit.Visible = false;
        this.ucSummary.Visible = false;
        this.ucSearch.Visible = true;
        this.Search_Render(ucSearch.CollectParam(null), e);
        //this.ucList.Visible = true;
        //this.ucList.UpdateView();

    }
}
