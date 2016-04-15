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
using com.Sconit.Utility;
using com.Sconit.Web;
using NHibernate.Expression;
using com.Sconit.Entity;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections.Generic;

public partial class Cost_MiscOrder_List : ListModuleBase
{
    public EventHandler ViewEvent;
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

    public int SelectedIndex
    {
        get
        {
            return ViewState["SelectedIndex"] == null ? 0 : (int)ViewState["SelectedIndex"];
        }
        set
        {
            ViewState["SelectedIndex"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
    }

    public override void UpdateView()
    {
        if (SelectedIndex == 0)
        {
            this.GV_List.Execute();
            this.list.Visible = true;
            this.detail.Visible = false;
            if (this.IsExport)
            {
                //this.ExportXLS(this.GV_List);
            }
        }
        else
        {
            this.gv_Detail.Execute();
            this.list.Visible = false;
            this.detail.Visible = true;
            if (this.IsExport)
            {
                //this.ExportXLS(this.gv_Detail);
            }
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblReason = ((Label)(e.Row.FindControl("lblReason")));
            if (lblReason != null)
            {
                lblReason.Text = GetCodeMaster(lblReason.ToolTip);
            }
        }
    }

    protected void GV_List_Detail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            MiscOrderDetail orderDetail = (MiscOrderDetail)e.Row.DataItem;
            Label lblCost = ((Label)e.Row.FindControl("lblCost"));
            lblCost.Text = "0";
            if (!orderDetail.IsBlankDetail)
            {
                decimal? cost = this.TheCostDetailMgr.CalculateItemUnitCost(orderDetail.Item.Code, orderDetail.MiscOrder.CostGroup,
                    orderDetail.MiscOrder.EffectiveDate.Year, orderDetail.MiscOrder.EffectiveDate.Month - 1);
                if (cost.HasValue)
                {
                    lblCost.Text = (cost.Value * orderDetail.Qty).ToString("0.########");
                }
            }

            Label lblReason = ((Label)(e.Row.FindControl("lblReason")));
            if (lblReason != null)
            {
                lblReason.Text = GetCodeMaster(lblReason.ToolTip);
            }
        }
    }

    protected void lbtnView_Click(object sender, EventArgs e)
    {
        if (ViewEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            ViewEvent(code, e);
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
    }

    private string GetCodeMaster(string codeValue)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(CodeMaster));
        selectCriteria.Add(Expression.Eq("Value", codeValue));

        IList<CodeMaster> codemstrs = TheCriteriaMgr.FindAll<CodeMaster>(selectCriteria);
        if (codemstrs != null && codemstrs.Count > 0)
        {
            return codemstrs[0].Description;
        }
        return string.Empty;
    }
}
