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
using System.Collections.Generic;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity;

public partial class ISI_CheckupProject_Search : SearchModuleBase
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
            this.tbCheckup.Text = actionParameter["Code"];
        }
        if (actionParameter.ContainsKey("Desc_"))
        {
            this.tbDesc.Text = actionParameter["Desc_"];
        }
    }

    protected override void DoSearch()
    {
        string code = this.tbCheckup.Text.Trim();
        string desc_ = this.tbDesc.Text.Trim();
        string type = this.ddlType.SelectedValue;
        bool isActive = this.cbIsActive.Checked;

        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(CheckupProject));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(CheckupProject)).SetProjection(Projections.Count("Code"));
            if (code != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Code", code, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Code", code, MatchMode.Anywhere));
            }

            if (desc_ != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Desc", desc_, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Desc", desc_, MatchMode.Anywhere));
            }
            if (type != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Type",type));
                selectCountCriteria.Add(Expression.Eq("Type",type));
            }
            selectCriteria.Add(Expression.Eq("IsActive", isActive));
            selectCountCriteria.Add(Expression.Eq("IsActive", isActive));

            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
            #endregion
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }
}