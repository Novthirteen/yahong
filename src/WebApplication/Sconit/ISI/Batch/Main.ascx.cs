using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity;
using NHibernate.Expression;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Batch_Main : MainModuleBase
{
    public string ModuleType
    {
        get
        {
            return (string)ViewState["ModuleType"];
        }
        set
        {
            ViewState["ModuleType"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucSearch.ReplaceEvent += new EventHandler(ucSearch_ReplaceEvent);
        this.ucSearch.DeleteEvent += new EventHandler(ucSearch_DeleteEvent);
        this.ucSearch.CancelEvent += new EventHandler(ucSearch_CancelEvent);
        this.ucSearch.CompleteEvent += new EventHandler(ucSearch_CompleteEvent);
        this.ucSearch.CloseEvent += new EventHandler(ucSearch_CloseEvent);
        this.ucSearch.RejectEvent += new EventHandler(ucSearch_RejectEvent);
        this.ucSearch.OpenEvent += new EventHandler(ucSearch_OpenEvent);
        this.ucSearch.BatchEvent += new EventHandler(ucSearch_BatchEvent);

        if (!IsPostBack)
        {

            if (this.Action == BusinessConstants.PAGE_LIST_ACTION)
            {
                ucSearch.QuickSearch(this.ActionParameter);
            }
            if (this.ModuleParameter.ContainsKey("ModuleType"))
            {
                this.ModuleType = this.ModuleParameter["ModuleType"];
            }

            this.ucList.ModuleType = this.ModuleType;
            this.ucSearch.ModuleType = this.ModuleType;
        }
    }

    void ucSearch_DeleteEvent(object sender, EventArgs e)
    {
        if (this.ucList.Visible)
        {
            this.ucList.Delete();
        }
    }
    void ucSearch_CancelEvent(object sender, EventArgs e)
    {
        if (this.ucList.Visible)
        {
            this.ucList.Cancel();
        }
    }
    void ucSearch_CloseEvent(object sender, EventArgs e)
    {
        if (this.ucList.Visible)
        {
            this.ucList.Close();
        }
    }
    void ucSearch_RejectEvent(object sender, EventArgs e)
    {
        if (this.ucList.Visible)
        {
            this.ucList.Reject();
        }
    }
    void ucSearch_OpenEvent(object sender, EventArgs e)
    {
        if (this.ucList.Visible)
        {
            this.ucList.Open();
        }
    }

    void ucSearch_CompleteEvent(object sender, EventArgs e)
    {
        if (this.ucList.Visible)
        {
            this.ucList.Complete();
        }
    }

    void ucSearch_BatchEvent(object sender, EventArgs e)
    {
        if (this.ucList.Visible)
        {
            this.ucList.Batch((string)(((object[])sender)[0]), (bool)(((object[])sender)[1]), (bool)(((object[])sender)[2]), (string)(((object[])sender)[3]), (bool)(((object[])sender)[4]));
        }
    }
    void ucSearch_ReplaceEvent(object sender, EventArgs e)
    {
        if (this.ucList.Visible)
        {
            this.ucList.Replace((string)(((object[])sender)[0]), (string)(((object[])sender)[1]));
        }
    }

    //The event handler when user click button "Search" button
    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.InitPageParameter(sender);
        this.ucList.Visible = true;
    }
}
