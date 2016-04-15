﻿using System;
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
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Web;
using com.Sconit.Entity;
using NHibernate.Expression;
using System.Collections.Generic;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.Cost;


public partial class Cost_MiscOrder_Main : MainModuleBase
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

    public Cost_MiscOrder_Main()
    {
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        this.ucSearch.NewEvent += new System.EventHandler(this.New_Render);
        this.ucSearch.SearchEvent += new System.EventHandler(this.Search_Render);
        this.ucList.ViewEvent += new System.EventHandler(this.ListView_Render);
        this.ucNew.BackEvent += new System.EventHandler(this.EditBack_Render);

        this.ucList.ModuleType = BusinessConstants.CODE_MASTER_MISC_ORDER_TYPE_VALUE_VALUE;
    }

    void Search_Render(object sender, EventArgs e)
    {
        this.ucList.IsExport = this.ucSearch.IsExport;
        this.ucList.SetSearchCriteria((DetachedCriteria)((object[])sender)[0], (DetachedCriteria)((object[])sender)[1]);
        this.ucList.SelectedIndex = (int)((object[])sender)[2];
        this.ucList.Visible = true;
        this.ucList.UpdateView();
        this.CleanMessage();
    }

    void New_Render(object sender, EventArgs e)
    {
        this.ucList.Visible = false;
        this.ucSearch.Visible = false;

        this.ucNew.Visible = true;
        this.ucNew.UpdateView();
        this.CleanMessage();
    }
    void ListView_Render(object sender, EventArgs e)
    {
        this.ucNew.Visible = true;
        this.ucSearch.Visible = false;
        this.ucList.Visible = false;
        this.ucNew.InitPageParameter((string)sender);

    }
    void EditBack_Render(object sender, EventArgs e)
    {
        this.ucSearch.Visible = true;
        this.ucList.Visible = true;
        //this.ucSearch.DoSearch();
        //this.ucList.UpdateView();
        this.ucNew.Visible = false;
    }

}
