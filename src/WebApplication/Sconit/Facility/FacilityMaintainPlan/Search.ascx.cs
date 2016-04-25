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
using com.Sconit.Facility.Entity;

public partial class Facility_MaintainPlan_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;
    public event EventHandler BackEvent;

    public string FCID
    {
        get
        {
            return (string)ViewState["FCID"];
        }
        set
        {
            ViewState["FCID"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
          
        }
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
       
    }

    protected override void DoSearch()
    {

        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityMaintainPlan));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityMaintainPlan)).SetProjection(Projections.Count("Id"));

            selectCriteria.Add(Expression.Eq("FacilityMaster.FCID", this.FCID));
            selectCountCriteria.Add(Expression.Eq("FacilityMaster.FCID", this.FCID));
            
            if (this.tbCode.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("MaintainPlan.Code", this.tbCode.Text.Trim(), MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("MaintainPlan.Code", this.tbCode.Text.Trim(), MatchMode.Anywhere));
            }

            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
            #endregion
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }
}
