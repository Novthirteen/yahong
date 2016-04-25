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
using com.Sconit.Control;
using Geekees.Common.Controls;

public partial class Facility_FacilityTrans_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ((CodeMstrDropDownList)(this.ddlTransType)).SelectedIndex = 0;
            GenerateTree();
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

        if (actionParameter.ContainsKey("FCID"))
        {
            this.tbFCID.Text = actionParameter["FCID"];
        }
        if (actionParameter.ContainsKey("StartDate"))
        {
            this.tbStartDate.Text = actionParameter["StartDate"];
        }
        if (actionParameter.ContainsKey("EndDate"))
        {
            this.tbEndDate.Text = actionParameter["EndDate"];
        }
    }

    private object CollectParam(bool isExport)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityTrans));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityTrans)).SetProjection(Projections.Count("FCID"));
        if (this.tbFCID.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbStartDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Ge("StartDate", DateTime.Parse(this.tbStartDate.Text.Trim())));
            selectCountCriteria.Add(Expression.Ge("StartDate", DateTime.Parse(this.tbStartDate.Text.Trim())));
        }
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Le("EndDate", DateTime.Parse(this.tbEndDate.Text)));
            selectCountCriteria.Add(Expression.Le("EndDate", DateTime.Parse(this.tbEndDate.Text)));
        }


        if (this.ddlTransType.SelectedValue != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("TransType", this.ddlTransType.SelectedValue));
            selectCountCriteria.Add(Expression.Eq("TransType", this.ddlTransType.SelectedValue));
        }

        if (this.tbAssetNo.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("AssetNo", this.tbAssetNo.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("AssetNo", this.tbAssetNo.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbFacilityName.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FacilityName", this.tbFacilityName.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FacilityName", this.tbFacilityName.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbChargePerson.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("ToChargePerson", this.tbChargePerson.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("ToChargePerson", this.tbChargePerson.Text.Trim(), MatchMode.Anywhere));
        }

        #region cateogy
        IList<string> categoryList = new List<string>();
        List<ASTreeViewNode> categoryNodes = this.astvMyTree1.GetCheckedNodes();
        foreach (ASTreeViewNode node in categoryNodes)
        {
            DetachedCriteria criteria = DetachedCriteria.For<FacilityCategory>();
            criteria.Add(Expression.Or(Expression.Eq("Code", node.NodeValue), Expression.Eq("ParentCategory", node.NodeValue)));
            IList<FacilityCategory> facilityCategoryList = TheCriteriaMgr.FindAll<FacilityCategory>(criteria);

            foreach (FacilityCategory category in facilityCategoryList)
            {
                if (!categoryList.Contains(category.Code))
                {
                    categoryList.Add(category.Code);
                }
            }
        }
        if (categoryList != null && categoryList.Count > 0)
        {
            selectCriteria.Add(Expression.In("FacilityCategory", categoryList.ToArray<string>()));
            selectCountCriteria.Add(Expression.In("FacilityCategory", categoryList.ToArray<string>()));
        }

        #endregion
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

    private void GenerateTree()
    {
        #region 类别多选
        IList<FacilityCategory> categoryList = TheFacilityCategoryMgr.GetAllFacilityCategory();
        if (categoryList != null && categoryList.Count > 0)
        {
            foreach (FacilityCategory category in categoryList)
            {
                this.astvMyTree1.RootNode.AppendChild(new ASTreeViewLinkNode(category.Code + "[" + category.Description + "]", category.Code, string.Empty));
            }
        }
        #endregion
    }
}
