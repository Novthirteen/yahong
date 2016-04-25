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

public partial class ISI_BillDetail_Search : SearchModuleBase
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
                if (this.rblListFormat.SelectedIndex == 1)
                {
                    object criteriaParam = this.CollectParam(true);
                    SearchEvent(criteriaParam, null);
                }
                else
                {
                    object criteriaParam = this.CollectParam(true);
                    SearchEvent(criteriaParam, null);
                }
            }
            else
            {
                DoSearch();
            }
        }
    }
    private object CollectParam(bool isExport)
    {
        if (this.rblListFormat.SelectedIndex == 0)
        {
            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(BillView));
            DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(BillView)).SetProjection(Projections.Count("PrjCode"));

            if (this.tbSupplier.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Supplier", this.tbSupplier.Text.Trim(), MatchMode.Anywhere));

            }
            if (this.tbCustomer.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Customer", this.tbCustomer.Text.Trim(), MatchMode.Anywhere));

            }
            if (this.tbPrjCode.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("PrjCode", this.tbPrjCode.Text.Trim()));

            }
            if (this.tbSOUser.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("SOUser", this.tbSOUser.Text.Trim()));

            }
            if (this.tbSOContractNo.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("SOContractNo", this.tbSOContractNo.Text.Trim(), MatchMode.Anywhere));

            }

            if (this.ddlType.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Type", this.ddlType.SelectedValue));

            }


            return new object[] { selectCriteria, selectCountCriteria, isExport, true };
        }
        else
        {
            DetachedCriteria selectDetailCriteria = DetachedCriteria.For(typeof(MouldDetail));
            DetachedCriteria selectDetailCountCriteria = DetachedCriteria.For(typeof(MouldDetail)).SetProjection(Projections.Count("Id"));

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(Mould));
            if (this.tbCode.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Code", this.tbCode.Text.Trim(), MatchMode.Anywhere));

            }
            if (this.tbDesc1.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Or(Expression.Like("Desc1", this.tbDesc1.Text.Trim(), MatchMode.Anywhere), Expression.Like("Remark", this.tbDesc1.Text.Trim(), MatchMode.Anywhere)));
            }
            if (this.tbFCID.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("FCID", this.tbFCID.Text.Trim(), MatchMode.Anywhere));

            }

            if (this.tbSupplier.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Supplier", this.tbSupplier.Text.Trim(), MatchMode.Anywhere));

            }
            if (this.tbCustomer.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("Customer", this.tbCustomer.Text.Trim(), MatchMode.Anywhere));

            }
            if (this.tbPrjCode.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("PrjCode", this.tbPrjCode.Text.Trim()));

            }
            if (this.tbMouldUser.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("MouldUser", this.tbMouldUser.Text.Trim()));

            }
            if (this.tbSOUser.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("SOUser", this.tbSOUser.Text.Trim()));

            }
            if (this.tbPOUser.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("POUser", this.tbPOUser.Text.Trim()));

            }
            if (this.tbSOContractNo.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("SOContractNo", this.tbSOContractNo.Text.Trim(), MatchMode.Anywhere));

            }
            if (this.tbSupplierContractNo.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Like("SupplierContractNo", this.tbSupplierContractNo.Text.Trim(), MatchMode.Anywhere));
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

            }
            #endregion


            if (this.ddlType.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Eq("Type", this.ddlType.SelectedValue));

            }

            if (this.tbBillStartDate.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Ge("BillDate", DateTime.Parse(this.tbBillStartDate.Text.Trim())));

            }
            if (this.tbBillEndDate.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Le("BillDate", DateTime.Parse(this.tbBillEndDate.Text)));

            }

            if (this.tbPayStartDate.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Ge("PayDate", DateTime.Parse(this.tbPayStartDate.Text.Trim())));

            }
            if (this.tbPayEndDate.Text.Trim() != string.Empty)
            {
                selectCriteria.Add(Expression.Le("PayDate", DateTime.Parse(this.tbPayEndDate.Text)));

            }

            selectCriteria.SetProjection(Projections.ProjectionList().Add(Projections.GroupProperty("Code")));

            selectDetailCriteria.Add(
                            Subqueries.PropertyIn("Code", selectCriteria));
            selectDetailCountCriteria.Add(
                            Subqueries.PropertyIn("Code", selectCriteria));

            return new object[] { selectDetailCriteria, selectDetailCountCriteria, isExport, false };
        }
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

    }

}
