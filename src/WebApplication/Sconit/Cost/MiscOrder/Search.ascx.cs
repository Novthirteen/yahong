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
using com.Sconit.Entity.MasterData;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Web;
using com.Sconit.Entity;
using NHibernate.Expression;
using System.Collections.Generic;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.Cost;


public partial class Cost_MiscOrder_MiscOrder_Search : SearchModuleBase
{
    public event EventHandler EditEvent;
    public event EventHandler BackEvent;
    public event EventHandler NewEvent;
    public event EventHandler SearchEvent;

    public string ModuleType
    {
        get
        {
            return (string)ViewState["ModuleType"];
        }
        set
        {
            ViewState["ModuleType"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        this.tbMiscOrderRegion.ServiceParameter = "string:" + this.CurrentUser.Code;
        if (!IsPostBack)
        {
            this.startDate.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            this.tbEndDate.Text = DateTime.Today.ToString("yyyy-MM-dd");

            List<CostElement> costElements = new List<CostElement>();
            costElements.Add(new CostElement());
            costElements.AddRange(TheCostElementMgr.GetAllCostElement().ToList());
            this.ddlCostElement.DataSource = costElements;
            this.ddlCostElement.DataBind();

            this.ddlCostGroup.DataSource = TheCostGroupMgr.GetAllCostGroup();
            this.ddlCostGroup.DataBind();

            List<CodeMaster> codeMstrs = new List<CodeMaster>();
            codeMstrs.Add(new CodeMaster());
            codeMstrs.AddRange(TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_STOCK_OUT_REASON).ToList());
            codeMstrs.AddRange(TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_STOCK_IN_REASON).ToList());
            this.ddlReason.DataSource = codeMstrs;
            this.ddlReason.DataBind();
        }
    }
    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        //todo

    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }

    protected override void DoSearch()
    {
        string code = this.tbMiscOrderCode.Text != string.Empty ? this.tbMiscOrderCode.Text.Trim() : string.Empty;
        string type = this.ddlType.SelectedValue;

        string tbRegion = this.tbMiscOrderRegion.Text != string.Empty ? this.tbMiscOrderRegion.Text.Trim() : string.Empty;
        string tbLocation = this.tbMiscOrderLocation.Text != string.Empty ? this.tbMiscOrderLocation.Text.Trim() : string.Empty;
        string tbEffectDate = this.tbMiscOrderEffectDate.Text != string.Empty ? this.tbMiscOrderEffectDate.Text.Trim() : string.Empty;
        string tbStartDate = this.startDate.Text != string.Empty ? this.startDate.Text.Trim() : string.Empty;
        string tbEndDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
        string tbItem = this.tbItem.Text.Trim();
        string ddlStatus = this.ddlStatus.SelectedValue;
        string ddlCostGroup = this.ddlCostGroup.SelectedValue;
        string ddlCostElement = this.ddlCostElement.SelectedValue;

        if (tbItem != string.Empty)
        {
            rblListFormat.SelectedIndex = 1;
        }

        #region DetachedCriteria
        if (rblListFormat.SelectedIndex == 0)
        {
            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(MiscOrder));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(MiscOrder))
                .SetProjection(Projections.Count("OrderNo"));
            selectCriteria.CreateAlias("Location", "l");
            selectCountCriteria.CreateAlias("Location", "l");

            if (code != string.Empty)
            {
                selectCriteria.Add(Expression.Like("OrderNo", code, MatchMode.End));
                selectCountCriteria.Add(Expression.Like("OrderNo", code, MatchMode.End));
            }
            if (type != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Type", type));
                selectCountCriteria.Add(Expression.Eq("Type", type));

            }
            if (tbRegion != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("l.Region.Code", tbRegion));
                selectCountCriteria.Add(Expression.Eq("l.Region.Code", tbRegion));

            }
            if (tbLocation != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Location.Code", tbLocation));
                selectCountCriteria.Add(Expression.Eq("Location.Code", tbLocation));

            }

            if (tbEffectDate != string.Empty)
            {
                DateTime tmpEffectDate = DateTime.Parse(tbEffectDate);
                selectCriteria.Add(Expression.Eq("EffectiveDate", tmpEffectDate));
                selectCountCriteria.Add(Expression.Eq("EffectiveDate", tmpEffectDate));
            }

            if (tbStartDate != string.Empty)
            {
                DateTime tmpStartDate = DateTime.Parse(tbStartDate);
                selectCriteria.Add(Expression.Gt("CreateDate", tmpStartDate));
                selectCountCriteria.Add(Expression.Gt("CreateDate", tmpStartDate));

            }
            if (tbEndDate != string.Empty)
            {
                DateTime tmpEndDate = DateTime.Parse(tbEndDate);
                selectCriteria.Add(Expression.Lt("CreateDate", tmpEndDate));
                selectCountCriteria.Add(Expression.Lt("CreateDate", tmpEndDate));
            }

            if (ddlCostElement != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("CostElement", ddlCostElement));
                selectCountCriteria.Add(Expression.Eq("CostElement", ddlCostElement));
            }

            selectCriteria.Add(Expression.Eq("Status", ddlStatus));
            selectCountCriteria.Add(Expression.Eq("Status", ddlStatus));

            selectCriteria.Add(Expression.Eq("CostGroup", ddlCostGroup));
            selectCountCriteria.Add(Expression.Eq("CostGroup", ddlCostGroup));

            if (type != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Type", type));
                selectCountCriteria.Add(Expression.Eq("Type", type));
            }

            SearchEvent((new object[] { selectCriteria, selectCountCriteria, 0 }), null);
        }
        else
        {
            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(MiscOrderDetail));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(MiscOrderDetail))
                .SetProjection(Projections.Count("Id"));

            selectCriteria.CreateAlias("MiscOrder", "m");
            selectCountCriteria.CreateAlias("MiscOrder", "m");

            selectCriteria.CreateAlias("m.Location", "l");
            selectCountCriteria.CreateAlias("m.Location", "l");

            if (code != string.Empty)
            {
                selectCriteria.Add(Expression.Like("m.OrderNo", code, MatchMode.End));
                selectCountCriteria.Add(Expression.Like("m.OrderNo", code, MatchMode.End));
            }
            if (type != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("m.Type", type));
                selectCountCriteria.Add(Expression.Eq("m.Type", type));

            }
            if (tbRegion != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("l.Region.Code", tbRegion));
                selectCountCriteria.Add(Expression.Eq("l.Region.Code", tbRegion));
            }
            if (tbLocation != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("l.Code", tbLocation));
                selectCountCriteria.Add(Expression.Eq("l.Code", tbLocation));
            }

            if (tbEffectDate != string.Empty)
            {
                DateTime tmpEffectDate = DateTime.Parse(tbEffectDate);
                selectCriteria.Add(Expression.Eq("m.EffectiveDate", tmpEffectDate));
                selectCountCriteria.Add(Expression.Eq("m.EffectiveDate", tmpEffectDate));
            }

            if (tbStartDate != string.Empty)
            {
                DateTime tmpStartDate = DateTime.Parse(tbStartDate);
                selectCriteria.Add(Expression.Gt("m.CreateDate", tmpStartDate));
                selectCountCriteria.Add(Expression.Gt("m.CreateDate", tmpStartDate));

            }

            if (tbEndDate != string.Empty)
            {
                DateTime tmpEndDate = DateTime.Parse(tbEndDate);
                selectCriteria.Add(Expression.Lt("m.CreateDate", tmpEndDate));
                selectCountCriteria.Add(Expression.Lt("m.CreateDate", tmpEndDate));
            }

            if (tbItem != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Item.Code", tbItem, MatchMode.Anywhere));
                selectCountCriteria.Add(Expression.Like("Item.Code", tbItem, MatchMode.Anywhere));
            }

            if (ddlCostElement != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("m.CostElement", ddlCostElement));
                selectCountCriteria.Add(Expression.Eq("m.CostElement", ddlCostElement));
            }

            selectCriteria.Add(Expression.Eq("m.Status", ddlStatus));
            selectCountCriteria.Add(Expression.Eq("m.Status", ddlStatus));

            selectCriteria.Add(Expression.Eq("m.CostGroup", ddlCostGroup));
            selectCountCriteria.Add(Expression.Eq("m.CostGroup", ddlCostGroup));

            SearchEvent((new object[] { selectCriteria, selectCountCriteria, 1 }), null);
        }
        #endregion

    }
}
