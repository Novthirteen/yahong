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
using com.Sconit.ISI.Entity;

public partial class ISI_TaskAddress_Search : SearchModuleBase
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

        if (actionParameter.ContainsKey("Code"))
        {
            this.tbCode.Text = actionParameter["Code"];
        }
        if (actionParameter.ContainsKey("Description"))
        {
            this.tbDesc.Text = actionParameter["Description"];
        }
    }

    protected override void DoSearch()
    {
        string code = this.tbCode.Text.Trim();
        string desc = this.tbDesc.Text.Trim();
        string parentCode = this.tbParent.Text.Trim();
        
        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(TaskAddress));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(TaskAddress)).SetProjection(Projections.Count("Code"));
            if (code != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Code", code));
                selectCountCriteria.Add(Expression.Eq("Code", code));
            }

            if (desc != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Description", desc, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Description", desc, MatchMode.Anywhere));
            }
            if (parentCode != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Parent.Code", parentCode));
                selectCountCriteria.Add(Expression.Eq("Parent.Code", parentCode));
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
