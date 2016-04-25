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

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
          
        }
    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        System.Web.UI.WebControls.Button btn = (System.Web.UI.WebControls.Button)sender;
        if (SearchEvent != null)
        {
            if (btn == this.btnExport)
            {

                object criteriaParam = this.CollectParam(true);
                SearchEvent(criteriaParam, null);

            }
            else
            {
                DoSearch();
            }
        }
    }
    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

        if (actionParameter.ContainsKey("Code"))
        {
            this.tbCode.Text = actionParameter["Code"];
        }
        if (actionParameter.ContainsKey("Description"))
        {
            this.tbDescription.Text = actionParameter["Description"];
        }
       
    }


    private object CollectParam(bool isExport)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(MaintainPlan));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(MaintainPlan)).SetProjection(Projections.Count("Code"));
        if (this.tbCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Code", this.tbCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Code", this.tbCode.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbDescription.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Description", this.tbDescription.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Description", this.tbDescription.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbCategory.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FacilityCategory", this.tbCategory.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FacilityCategory", this.tbCategory.Text.Trim(), MatchMode.Anywhere));
        }

        return new object[] { selectCriteria, selectCountCriteria, isExport, true };
    }
    protected override void DoSearch()
    {

        if (SearchEvent != null)
        {
            #region DetachedCriteria


            object criteriaParam = this.CollectParam(false);
            SearchEvent(criteriaParam, null);
            #endregion
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }
}
