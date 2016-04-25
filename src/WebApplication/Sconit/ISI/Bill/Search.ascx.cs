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
using com.Sconit.ISI.Entity;
using com.Sconit.Entity;
using Geekees.Common.Controls;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Bill_Search : SearchModuleBase
{
    public string PSIType
    {
        get
        {
            return (string)ViewState["PSIType"];
        }
        set
        {
            ViewState["PSIType"] = value;
        }
    }
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
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(Mould));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(Mould)).SetProjection(Projections.Count("Code"));
        if (this.tbCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Code", this.tbCode.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Code", this.tbCode.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbDesc1.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Or(Expression.Like("Desc1", this.tbDesc1.Text.Trim(), MatchMode.Anywhere), Expression.Like("Remark", this.tbDesc1.Text.Trim(), MatchMode.Anywhere)));
            selectCountCriteria.Add(Expression.Or(Expression.Like("Desc1", this.tbDesc1.Text.Trim(), MatchMode.Anywhere), Expression.Like("Remark", this.tbDesc1.Text.Trim(), MatchMode.Anywhere)));
        }
        if (this.tbFCID.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));
        }

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
        if (this.tbPrjCode.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("PrjCode", this.tbPrjCode.Text.Trim()));
            selectCountCriteria.Add(Expression.Eq("PrjCode", this.tbPrjCode.Text.Trim()));
        }
        if (this.tbMouldUser.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("MouldUser", this.tbMouldUser.Text.Trim()));
            selectCountCriteria.Add(Expression.Eq("MouldUser", this.tbMouldUser.Text.Trim()));
        }
        if (this.tbSOUser.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("SOUser", this.tbSOUser.Text.Trim()));
            selectCountCriteria.Add(Expression.Eq("SOUser", this.tbSOUser.Text.Trim()));
        }
        if (this.tbPOUser.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("POUser", this.tbPOUser.Text.Trim()));
            selectCountCriteria.Add(Expression.Eq("POUser", this.tbPOUser.Text.Trim()));
        }
        if (this.tbSOContractNo.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("SOContractNo", this.tbSOContractNo.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("SOContractNo", this.tbSOContractNo.Text.Trim(), MatchMode.Anywhere));
        }
        if (this.tbSupplierContractNo.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Like("SupplierContractNo", this.tbSupplierContractNo.Text.Trim(), MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("SupplierContractNo", this.tbSupplierContractNo.Text.Trim(), MatchMode.Anywhere));
        }
        selectCriteria.Add(Expression.Eq("Type", this.PSIType));
        selectCountCriteria.Add(Expression.Eq("Type", this.PSIType));

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

        //权限
        if (!this.CurrentUser.HasPermission(ISIConstants.PERMISSION_PAGE_VALUE_VIEWPSIBILL))
        {
            selectCriteria.Add(Expression.Or(Expression.Or(Expression.Eq("SOUser", this.CurrentUser.Code), Expression.Eq("POUser", this.CurrentUser.Code)),
                                            Expression.Eq("CreateUser", this.CurrentUser.Code)));
            selectCountCriteria.Add(Expression.Or(Expression.Or(Expression.Eq("SOUser", this.CurrentUser.Code), Expression.Eq("POUser", this.CurrentUser.Code)),
                                            Expression.Eq("CreateUser", this.CurrentUser.Code)));
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
        IList<CodeMaster> statusList = this.TheCodeMasterMgr.GetCachedCodeMasterAsc(ISIConstants.CODE_MASTER_PSI_BILL_STATUS);

        foreach (CodeMaster status in statusList)
        {
            this.astvMyTree.RootNode.AppendChild(new ASTreeViewLinkNode(this.TheLanguageMgr.TranslateMessage("ISI.Status." + status.Value, CurrentUser), status.Value, string.Empty));
        }
        this.astvMyTree.RootNode.ChildNodes[0].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[1].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[2].CheckedState = ASTreeViewCheckboxState.Checked;
        this.astvMyTree.RootNode.ChildNodes[3].CheckedState = ASTreeViewCheckboxState.Checked;

        this.astvMyTree.InitialDropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[2].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser);
        this.astvMyTree.DropdownText = this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[0].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[1].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[2].NodeValue, CurrentUser) + "," + this.TheLanguageMgr.TranslateMessage("ISI.Status." + this.astvMyTree.RootNode.ChildNodes[3].NodeValue, CurrentUser);

    }


}

