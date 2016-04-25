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
using Geekees.Common.Controls;
using com.Sconit.ISI.Entity.Util;

public partial class Facility_FacilityDistribution_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;
    public event EventHandler NewEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
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
    private object CollectParam(bool isExport)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityDistribution));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityDistribution)).SetProjection(Projections.Count("Id"));
        if (this.tbFCID.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
        }

        if (this.tbSupplierName.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("SupplierName", this.tbSupplierName.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("SupplierName", this.tbSupplierName.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbCustomerName.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("CustomerName", this.tbCustomerName.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("CustomerName", this.tbCustomerName.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbPurchaseContractCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("PurchaseContractCode", this.tbPurchaseContractCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("PurchaseContractCode", this.tbPurchaseContractCode.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbDistributionContractCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("DistributionContractCode", this.tbDistributionContractCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("DistributionContractCode", this.tbDistributionContractCode.Text.Trim(), MatchMode.Anywhere));
        }
        #region status
        IList<string> statusList = new List<string>();
        List<ASTreeViewNode> statusNodes = this.astvMyTree.GetCheckedNodes();
        foreach (ASTreeViewNode node in statusNodes)
        {
            statusList.Add(node.NodeValue);
        }
        if (statusList != null && statusList.Count > 0)
        {
            selectCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
            selectCountCriteria.Add(Expression.In("Status", statusList.ToArray<string>()));
        }
        #endregion

        if (this.tbCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Code", this.tbCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Code", this.tbCode.Text.Trim(), MatchMode.Anywhere));
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

    private void GenerateTree()
    {
        IList<CodeMaster> statusList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc("FacilityDistributionStatus");

        foreach (CodeMaster status in statusList)
        {
            this.astvMyTree.RootNode.AppendChild(new ASTreeViewLinkNode(this.TheLanguageMgr.TranslateMessage(status.Value, CurrentUser), status.Value, string.Empty));
        }
        this.astvMyTree.RootNode.ChildNodes[0].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[1].CheckedState = ASTreeViewCheckboxState.Checked;

        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage(this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser);

    }

}
