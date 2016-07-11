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

public partial class Facility_MouldUseWarn_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;


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
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityMaster));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityMaster)).SetProjection(Projections.Count("FCID"));


        selectCriteria.Add(Expression.Eq("ParentCategory","YH_MJ"));
        selectCountCriteria.Add(Expression.Eq("ParentCategory", "YH_MJ"));
        if (this.tbFCID.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbReferenceCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("ReferenceCode", this.tbReferenceCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("ReferenceCode", this.tbReferenceCode.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.cbIsOverUse.Checked)
        {
            selectCriteria.Add(Expression.GeProperty("UseQty", "WorkLife"));
            selectCountCriteria.Add(Expression.GeProperty("UseQty", "WorkLife"));
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



}
