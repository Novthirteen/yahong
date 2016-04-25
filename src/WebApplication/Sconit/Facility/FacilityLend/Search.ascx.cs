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

public partial class Facility_FacilityLend_Search : SearchModuleBase
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
        DoSearch();
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {

        if (actionParameter.ContainsKey("FCID"))
        {
            this.tbFCID.Text = actionParameter["FCID"];
        }
        if (actionParameter.ContainsKey("Name"))
        {
            this.tbName.Text = actionParameter["Desc"];
        }
       
    }

    protected override void DoSearch()
    {

        if (SearchEvent != null)
        {
            #region DetachedCriteria

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityMaster));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityMaster)).SetProjection(Projections.Count("FCID"));
            if (this.tbFCID.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
            }

            if (this.tbName.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Name", this.tbName.Text.Trim(), MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Name", this.tbName.Text.Trim(), MatchMode.Anywhere));
            }

            if (this.tbCategory.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Category", this.tbCategory.Text.Trim(), MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Category", this.tbCategory.Text.Trim(), MatchMode.Anywhere));
            }
            if (this.tbAssetNo.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("AssetNo", this.tbAssetNo.Text.Trim(), MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("AssetNo", this.tbAssetNo.Text.Trim(), MatchMode.Anywhere));
            }
            selectCriteria.Add(Expression.In("Status", new List<string> { com.Sconit.Entity.FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE.ToString(), com.Sconit.Entity.FacilityConstants.CODE_MASTER_FACILITY_STATUS_LEND.ToString() }));
            selectCountCriteria.Add(Expression.In("Status", new List<string> { com.Sconit.Entity.FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE.ToString(), com.Sconit.Entity.FacilityConstants.CODE_MASTER_FACILITY_STATUS_LEND.ToString() }));


            SearchEvent((new object[] { selectCriteria, selectCountCriteria }), null);
            #endregion
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }
}
