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
using com.Sconit.Web;
using System.Collections.Generic;
using com.Sconit.Service.Ext.MasterData;
using Geekees.Common.Controls;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Entity.View;

public partial class Reports_InvIOB_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler ExportEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.tbStartDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        }
        this.tbLocation.ServiceParameter = "string:" + this.CurrentUser.Code;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        this.DoSearch();
    }

    protected void tbFinanceYear_TextChange(object sender, EventArgs e)
    {
        DateTime f = DateTime.Parse(this.tbFinanceYear1.Text);
        int year = f.Year;
        int month = f.Month;
        FinanceCalendar financeCalendar = TheFinanceCalendarMgr.GetFinanceCalendar(year, month);

        this.tbStartDate.Text = financeCalendar.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
        this.tbEndDate.Text = financeCalendar.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        if (ExportEvent != null)
        {
            object param = this.CollectParam();
            if (param != null)
                ExportEvent(param, null);
        }
    }

    protected override void DoSearch()
    {
        if (SearchEvent != null)
        {
            object param = this.CollectParam();
            if (param != null)
            {
                SearchEvent(param, null);
            }
        }
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        if (actionParameter.ContainsKey("Location"))
        {
            this.tbItem.Text = actionParameter["Location"];
        }
        if (actionParameter.ContainsKey("Item"))
        {
            this.tbItem.Text = actionParameter["Item"];
        }
        if (actionParameter.ContainsKey("StartDate"))
        {
            this.tbStartDate.Text = actionParameter["StartDate"];
        }
        if (actionParameter.ContainsKey("EndDate"))
        {
            this.tbEndDate.Text = actionParameter["EndDate"];
        }
        if (actionParameter.ContainsKey("ItemDesc"))
        {
            this.tbDesc.Text = actionParameter["ItemDesc"];
        }
        if (actionParameter.ContainsKey("ItemCategory"))
        {
            this.tbItemCategory.Text = actionParameter["ItemCategory"];
        }
    }

    private CriteriaParam CollectParam()
    {
        try
        {
            if (DateTime.Compare(Convert.ToDateTime(this.tbStartDate.Text.Trim()), Convert.ToDateTime(this.tbEndDate.Text.Trim())) > 0)
            {
                ShowErrorMessage("Common.StarDate.EndDate.Compare");
                return null;
            }
        }
        catch (Exception)
        {
            ShowErrorMessage("Common.Business.Error.DateInvalid");
            return null;
        }

        CriteriaParam criteriaParam = new CriteriaParam();
        criteriaParam.Location = this.tbLocation.Text.Trim() != string.Empty ? new string[] { this.tbLocation.Text.Trim() } : null;
        criteriaParam.Item = this.tbItem.Text.Trim() != string.Empty ? this.tbItem.Text.Trim() : null;
        criteriaParam.ItemDesc = this.tbDesc.Text.Trim() != string.Empty ? this.tbDesc.Text.Trim() : null;
        criteriaParam.ItemCategory = this.tbItemCategory.Text.Trim() != string.Empty ? this.tbItemCategory.Text.Trim() : null;

        if (this.tbStartDate.Text.Trim() != string.Empty)
        {
            criteriaParam.StartDate = DateTime.Parse(this.tbStartDate.Text);
        }
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            criteriaParam.EndDate = DateTime.Parse(this.tbEndDate.Text);
        }
        return criteriaParam;
    }

}
