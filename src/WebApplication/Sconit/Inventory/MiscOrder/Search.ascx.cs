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


public partial class MasterData_MiscOrder_Search : SearchModuleBase
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
            if (this.ModuleType == BusinessConstants.CODE_MASTER_MISC_ORDER_TYPE_VALUE_GI)
            {
                this.ddlReason.Code = BusinessConstants.CODE_MASTER_STOCK_OUT_REASON;
            }
            else
            {
                this.ddlReason.Code = BusinessConstants.CODE_MASTER_STOCK_IN_REASON;
            }
            this.ddlReason.DataBind();
            this.startDate.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            this.tbEndDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
        }
    }
    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        //todo

    }
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
        this.IsExport = (sender == this.btnExport);
    }
    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }
    protected override void DoSearch()
    {
        string code = this.tbMiscOrderCode.Text != string.Empty ? this.tbMiscOrderCode.Text.Trim() : string.Empty;
        string type = this.ModuleType;

        string tbRegion = this.tbMiscOrderRegion.Text != string.Empty ? this.tbMiscOrderRegion.Text.Trim() : string.Empty;
        string tbLocation = this.tbMiscOrderLocation.Text != string.Empty ? this.tbMiscOrderLocation.Text.Trim() : string.Empty;
        string tbEffectDate = this.tbMiscOrderEffectDate.Text != string.Empty ? this.tbMiscOrderEffectDate.Text.Trim() : string.Empty;
        string tbStartDate = this.startDate.Text != string.Empty ? this.startDate.Text.Trim() : string.Empty;
        string tbEndDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;
        string tbItem = this.tbItem.Text.Trim();
        //string subjectCode = this.tbSubjectCode.Text != string.Empty ? this.tbSubjectCode.Text.Trim() : string.Empty;
        //string costCenterCode = this.tbCostCenterCode.Text != string.Empty ? this.tbCostCenterCode.Text.Trim() : string.Empty;

        if (tbItem != string.Empty)
        {
            rblListFormat.SelectedIndex = 1;
        }

        if (SearchEvent != null)
        {
            #region DetachedCriteria
            if (rblListFormat.SelectedIndex == 0)
            {
                DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(MiscOrder));
                DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(MiscOrder))
                    .SetProjection(Projections.Count("OrderNo"));
                selectCriteria.CreateAlias("Location", "l");
                selectCountCriteria.CreateAlias("Location", "l");

                //selectCriteria.CreateAlias("SubjectList", "s");
                //selectCountCriteria.CreateAlias("SubjectList", "s");

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
                    selectCriteria.Add(Expression.Lt("CreateDate", tmpEndDate.AddDays(1)));
                    selectCountCriteria.Add(Expression.Lt("CreateDate", tmpEndDate.AddDays(1)));
                }

                //if (costCenterCode != string.Empty)
                //{
                //    selectCriteria.Add(Expression.Eq("s.CostCenterCode", costCenterCode));
                //    selectCountCriteria.Add(Expression.Eq("s.CostCenterCode", costCenterCode));
                //}
                //if (subjectCode != string.Empty)
                //{
                //    selectCriteria.Add(Expression.Eq("s.SubjectCode", subjectCode));
                //    selectCountCriteria.Add(Expression.Eq("s.SubjectCode", subjectCode));
                //}
                selectCriteria.Add(Expression.Or(Expression.IsNull("Reason"),
                    Expression.Eq("Reason", ddlReason.SelectedValue)));

                selectCountCriteria.Add(Expression.Or(Expression.IsNull("Reason"),
                    Expression.Eq("Reason", ddlReason.SelectedValue)));

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
                    selectCriteria.Add(Expression.Lt("m.CreateDate", tmpEndDate.AddDays(1)));
                    selectCountCriteria.Add(Expression.Lt("m.CreateDate", tmpEndDate.AddDays(1)));
                }

                if (tbItem != string.Empty)
                {
                    selectCriteria.Add(Expression.Like("Item.Code", tbItem, MatchMode.Anywhere));
                    selectCountCriteria.Add(Expression.Like("Item.Code", tbItem, MatchMode.Anywhere));
                }

                selectCriteria.Add(Expression.Or(Expression.IsNull("m.Reason"),
                    Expression.Eq("m.Reason", ddlReason.SelectedValue)));

                selectCountCriteria.Add(Expression.Or(Expression.IsNull("m.Reason"),
                    Expression.Eq("m.Reason", ddlReason.SelectedValue)));

                SearchEvent((new object[] { selectCriteria, selectCountCriteria, 1 }), null);
            }
            #endregion
        }
    }
}
