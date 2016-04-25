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
using com.Sconit.Entity;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Entity.Util;
using Geekees.Common.Controls;
using NHibernate.Transform;

public partial class ISI_BillIO_Search : SearchModuleBase
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
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(BillIODet))
                        .SetProjection(Projections.Distinct(Projections.ProjectionList()
                            .Add(Projections.GroupProperty("OrgType").As("OrgType"))
                            .Add(Projections.GroupProperty("Org").As("Org"))
                            .Add(Projections.GroupProperty("OrgName").As("OrgName"))
                            .Add(Projections.Sum("Amount").As("Amount"))
                            .Add(Projections.Sum("BilledAmount").As("BilledAmount"))
                            .Add(Projections.Sum("PayAmount").As("PayAmount"))
                            .Add(Projections.Sum("Diff").As("Diff"))
                            ));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(BillIODet)).SetProjection(Projections.Distinct(Projections.ProjectionList()
                            .Add(Projections.GroupProperty("OrgType").As("OrgType"))
                            .Add(Projections.GroupProperty("Org").As("Org"))
                            .Add(Projections.GroupProperty("OrgName").As("OrgName")))).SetProjection(Projections.CountDistinct("Org"));
        
        if (this.tbSupplier.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Supplier", this.tbSupplier.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Supplier", this.tbSupplier.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbCustomer.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Customer", this.tbCustomer.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Customer", this.tbCustomer.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.ddlOrgType.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("OrgType", this.ddlOrgType.SelectedValue));
            selectCountCriteria.Add(Expression.Eq("OrgType", this.ddlOrgType.SelectedValue));
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
            selectCriteria.Add(Expression.In("Type", statusList.ToArray<string>()));
            selectCountCriteria.Add(Expression.In("Type", statusList.ToArray<string>()));
        }
        #endregion

        selectCriteria.SetResultTransformer(Transformers.AliasToBean(typeof(BillIODet)));

        return new object[] { selectCriteria, selectCountCriteria, isExport };

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
        IList<CodeMaster> statusList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_PSI_BILL_TYPE);

        foreach (CodeMaster status in statusList)
        {
            this.astvMyTree.RootNode.AppendChild(new ASTreeViewLinkNode(this.TheLanguageMgr.TranslateMessage("PSI.Bill.Type." + status.Value, CurrentUser), status.Value, string.Empty));
        }

        this.astvMyTree.RootNode.ChildNodes[0].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[3].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[4].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage("PSI.Bill.Type." + this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("PSI.Bill.Type." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("PSI.Bill.Type." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage("PSI.Bill.Type." + this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("PSI.Bill.Type." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("PSI.Bill.Type." + this.astvMyTree.RootNode.ChildNodes[4].NodeValue, CurrentUser);

    }
}
