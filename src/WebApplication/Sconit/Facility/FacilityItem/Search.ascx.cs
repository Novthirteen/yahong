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

public partial class Facility_FacilityItem_Search : SearchModuleBase
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
    private object CollectParam(bool isExport)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityItem));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityItem)).SetProjection(Projections.Count("Id"));
        //selectCriteria.CreateAlias("FacilityMaster", "f");
        //selectCountCriteria.CreateAlias("FacilityMaster", "f");
        if (this.tbFCID.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbItemCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Item.Code", this.tbItemCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Item.Code", this.tbItemCode.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.ddlAllocateType.SelectedValue != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("AllocateType", this.ddlAllocateType.SelectedValue));
            selectCountCriteria.Add(Expression.Eq("AllocateType", this.ddlAllocateType.SelectedValue));
        }
        if (cbIsInitQty.Checked)
        {
            selectCriteria.Add(Expression.Not(Expression.Eq("InitQty", Decimal.Zero)));
            selectCountCriteria.Add(Expression.Not(Expression.Eq("InitQty", Decimal.Zero)));
        }
        if (cbIsWarn.Checked)
        {
            selectCriteria.Add(Expression.Eq("IsWarn", cbIsWarn.Checked));
            selectCountCriteria.Add(Expression.Eq("IsWarn", cbIsWarn.Checked));
        }
        selectCriteria.Add(Expression.Eq("IsActive", this.cbIsActive.Checked));
        selectCountCriteria.Add(Expression.Eq("IsActive", this.cbIsActive.Checked));
        //if (this.tbCategory.Text.Trim() != string.Empty)
        //{
        //    selectCriteria.Add(Expression.Like("f.Category", this.tbCategory.Text.Trim(), MatchMode.Anywhere));
        //    selectCountCriteria.Add(Expression.Like("f.Category", this.tbCategory.Text.Trim(), MatchMode.Anywhere));
        //}
        //if (this.tbAssetNo.Text.Trim() != string.Empty)
        //{
        //    selectCriteria.Add(Expression.Like("f.AssetNo", this.tbAssetNo.Text.Trim(), MatchMode.Anywhere));
        //    selectCountCriteria.Add(Expression.Like("f.AssetNo", this.tbAssetNo.Text.Trim(), MatchMode.Anywhere));
        //}
        //if (this.cbIsAllocate.Checked)
        //{
        //    selectCriteria.Add(Expression.Eq("IsAllocate", true));
        //    selectCountCriteria.Add(Expression.Eq("IsAllocate", true));
        //}
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
