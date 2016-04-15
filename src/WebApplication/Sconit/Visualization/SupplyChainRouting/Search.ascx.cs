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
using com.Sconit.Web;
using System.Collections.Generic;
using com.Sconit.Entity.Procurement;
using NHibernate.Expression;
using com.Sconit.Entity.View;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;

public partial class Visualization_SupplyChainRouting_Search : SearchModuleBase
{
    public event EventHandler SearchEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code;
        if (this.CurrentUser.Permissions.Select(p => p.Code).Contains("Menu.Application.GeneralCode"))
        {
            this.btnCheck.Visible = true;
        }
        else
        {
            this.btnCheck.Visible = false;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            DoSearch();
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage(ex);
        }
    }

    protected override void DoSearch()
    {
        if (SearchEvent != null)
        {
            string flowCode = this.tbFlow.Text.Trim() != string.Empty ? this.tbFlow.Text.Trim() : string.Empty;
            string itemCode = this.tbItemCode.Text.Trim() != string.Empty ? this.tbItemCode.Text.Trim() : string.Empty;
            int upOrdown = this.rblupOrdown.SelectedIndex;

            if (flowCode == string.Empty || itemCode == string.Empty)
            {
                return;
            }

            SearchEvent(new object[] { flowCode, itemCode, upOrdown }, null);
        }
    }

    protected override void InitPageParameter(IDictionary<string, string> actionParameter)
    {
        if (actionParameter.ContainsKey("Flow"))
        {
            this.tbFlow.Text = actionParameter["Flow"];
        }
        if (actionParameter.ContainsKey("ItemCode"))
        {
            this.tbItemCode.Text = actionParameter["ItemCode"];
        }
    }

    protected void btnCheck_Click(object sender, EventArgs e)
    {
        try
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FlowView));
            criteria.CreateAlias("Flow", "f");
            criteria.Add(Expression.Eq("f.Type", BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_DISTRIBUTION));
            IList<FlowView> list = TheCriteriaMgr.FindAll<FlowView>(criteria);

            foreach (FlowView flowView in list)
            {
                TheSupplyChainMgr.GenerateSupplyChain(flowView.Flow.Code, flowView.FlowDetail.Item.Code);
            }
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage(ex);
            return;
        }
        ShowSuccessMessage("Visualization.SupplyChainRouting.NoLoop");
    }



}
