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
using System.Collections.Generic;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;

public partial class MasterData_ItemDisCon_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

        if (actionParameter.ContainsKey("Item"))
        {
            this.tbItem.Text = actionParameter["Item"];
        }
        if (actionParameter.ContainsKey("DisConItem"))
        {
            this.tbDisConItem.Text = actionParameter["DisConItem"];
        }
    }

    protected override void DoSearch()
    {
        string item = this.tbItem.Text.Trim();
        string disConItem = this.tbDisConItem.Text.Trim();
        string startDate = this.tbStartDate.Text.Trim();
        string endDate = this.tbEndDate.Text.Trim();

        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(ItemDiscontinue));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(ItemDiscontinue)).SetProjection(Projections.Count("Id"));
            if (item != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Item", item, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Item", item, MatchMode.Anywhere));
            }

            if (disConItem != string.Empty)
            {
                selectCriteria.Add(Expression.Like("DiscontinueItem", disConItem, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("DiscontinueItem", disConItem, MatchMode.Anywhere));
            }


            if (startDate != string.Empty)
            {
                selectCriteria.Add(Expression.Like("StartDate", startDate, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("StartDate", startDate, MatchMode.Anywhere));
            }

            if (endDate != string.Empty)
            {
                selectCriteria.Add(Expression.Like("EndDate", endDate, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("EndDate", endDate, MatchMode.Anywhere));
            }
            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
            #endregion
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }
}
