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

public partial class Facility_FacilityDistributionDetail_Search : SearchModuleBase
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
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityDistributionDetail));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityDistributionDetail)).SetProjection(Projections.Count("Id"));

        selectCriteria.CreateAlias("FacilityDistribution", "d");
        selectCountCriteria.CreateAlias("FacilityDistribution", "d");

        if (this.tbFCID.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("d.FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("d.FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbSupplierName.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("d.SupplierName", this.tbSupplierName.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("d.SupplierName", this.tbSupplierName.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbCustomerName.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("d.CustomerName", this.tbCustomerName.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("d.CustomerName", this.tbCustomerName.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbPurchaseContractCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("d.PurchaseContractCode", this.tbPurchaseContractCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("d.PurchaseContractCode", this.tbPurchaseContractCode.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbDistributionContractCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("d.DistributionContractCode", this.tbDistributionContractCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("d.DistributionContractCode", this.tbDistributionContractCode.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.ddlType.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Type", this.ddlType.SelectedValue));
            selectCountCriteria.Add(Expression.Eq("Type", this.ddlType.SelectedValue));
        }

        if (this.tbBillStartDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Ge("BillDate", DateTime.Parse(this.tbBillStartDate.Text.Trim())));
            selectCountCriteria.Add(Expression.Ge("BillDate", DateTime.Parse(this.tbBillStartDate.Text.Trim())));
        }
        if (this.tbBillEndDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Le("BillDate", DateTime.Parse(this.tbBillEndDate.Text)));
            selectCountCriteria.Add(Expression.Le("BillDate", DateTime.Parse(this.tbBillEndDate.Text)));
        }

        if (this.tbPayStartDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Ge("PayDate", DateTime.Parse(this.tbPayStartDate.Text.Trim())));
            selectCountCriteria.Add(Expression.Ge("PayDate", DateTime.Parse(this.tbPayStartDate.Text.Trim())));
        }
        if (this.tbPayEndDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Le("PayDate", DateTime.Parse(this.tbPayEndDate.Text)));
            selectCountCriteria.Add(Expression.Le("PayDate", DateTime.Parse(this.tbPayEndDate.Text)));
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
