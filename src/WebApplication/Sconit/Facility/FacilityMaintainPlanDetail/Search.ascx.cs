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
using com.Sconit.Entity;

public partial class Facility_FacilityMaintainPlanDetail_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;

    protected void Page_Load(object sender, EventArgs e)
    {


    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;
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
    private object CollectParam(bool isExport)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityMaintainPlan));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityMaintainPlan)).SetProjection(Projections.Count("Id"));

        selectCriteria.CreateAlias("FacilityMaster", "f");
        selectCountCriteria.CreateAlias("FacilityMaster", "f");

        selectCriteria.CreateAlias("MaintainPlan", "p");
        selectCountCriteria.CreateAlias("MaintainPlan", "p");



        if (this.tbFCID.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("f.FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("f.FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbName.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("f.Name", this.tbName.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("f.Name", this.tbName.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbAssetNo.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("f.AssetNo", this.tbAssetNo.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("f.AssetNo", this.tbAssetNo.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbMaintainPlanCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("p.Code", this.tbMaintainPlanCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("p.Code", this.tbMaintainPlanCode.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbMaintainPlanDescription.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("p.Description", this.tbMaintainPlanDescription.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("p.Description", this.tbMaintainPlanDescription.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbStartDate.Text.Trim() != string.Empty || this.tbEndDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.IsNotNull("NextMaintainDate"));
            selectCountCriteria.Add(Expression.IsNotNull("NextMaintainDate"));
       
            if (this.tbStartDate.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Ge("NextMaintainDate", Convert.ToDateTime(this.tbStartDate.Text.Trim())));
                selectCountCriteria.Add(Expression.Ge("NextMaintainDate", Convert.ToDateTime(this.tbStartDate.Text.Trim())));
            }
            if (this.tbEndDate.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Le("NextMaintainDate", Convert.ToDateTime(this.tbEndDate.Text.Trim())));
                selectCountCriteria.Add(Expression.Le("NextMaintainDate", Convert.ToDateTime(this.tbEndDate.Text.Trim())));
            }

        }

        return new object[] { selectCriteria, selectCountCriteria, isExport, true };
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

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
