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

public partial class MasterData_List : ListModuleBase
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
            com.Sconit.Control.CodeMstrLabel lblReason = ((com.Sconit.Control.CodeMstrLabel)(e.Row.FindControl("lblReason")));
            if (lblReason != null)
            {
                if (this.ModuleType == BusinessConstants.CODE_MASTER_MISC_ORDER_TYPE_VALUE_GI)
                {
                    lblReason.Code = BusinessConstants.CODE_MASTER_STOCK_OUT_REASON;
                }
                else
                {
                    lblReason.Code = BusinessConstants.CODE_MASTER_STOCK_IN_REASON;
                }
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
                    lblCost.Text = (cost.Value * orderDetail.Qty).ToString("0.##");
                }
            }

            com.Sconit.Control.CodeMstrLabel lblReason = ((com.Sconit.Control.CodeMstrLabel)(e.Row.FindControl("lblReason")));
            if (lblReason != null)
            {
                if (this.ModuleType == BusinessConstants.CODE_MASTER_MISC_ORDER_TYPE_VALUE_GI)
                {
                    lblReason.Code = BusinessConstants.CODE_MASTER_STOCK_OUT_REASON;
                }
                else
                {
                    lblReason.Code = BusinessConstants.CODE_MASTER_STOCK_IN_REASON;
                }
            }
        }
    }

}
