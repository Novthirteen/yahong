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
using com.Sconit.Entity.Distribution;
using com.Sconit.Entity;
using com.Sconit.Service.Ext.Distribution;
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;
using com.Sconit.Service.Ext.MasterData;

public partial class Order_GoodsReceipt_AsnReceipt_List : ListModuleBase
{
    public EventHandler ViewEvent;
    public EventHandler EditEvent;
    public EventHandler CloseEvent;

    public string ModuleType
    {
        get { return (string)ViewState["ModuleType"]; }
        set { ViewState["ModuleType"] = value; }
    }
    public string Action
    {
        get { return (string)ViewState["Action"]; }
        set { ViewState["Action"] = value; }
    }
    public string AsnType
    {
        get { return (string)ViewState["AsnType"]; }
        set { ViewState["AsnType"] = value; }
    }
    public bool IsSupplier
    {
        get { return ViewState["IsSupplier"] != null ? (bool)ViewState["IsSupplier"] : false; }
        set { ViewState["IsSupplier"] = value; }
    }
    public bool IsExport
    {
        get { return ViewState["IsExport"] != null ? (bool)ViewState["IsExport"] : false; }
        set { ViewState["IsExport"] = value; }
    }
    public bool IsGroup
    {
        get { return ViewState["IsGroup"] != null ? (bool)ViewState["IsGroup"] : false; }
        set { ViewState["IsGroup"] = value; }
    }

    public override void UpdateView()
    {
        if (!IsExport)
        {
            if (IsGroup)
            {
                this.GV_List.Execute();
                this.GV_List.Visible = true;
                this.gp.Visible = true;
                this.GV_List_Detail.Visible = false;
                this.gp_Detail.Visible = false;
            }
            else
            {
                this.GV_List_Detail.Execute();
                this.GV_List.Visible = false;
                this.GV_List_Detail.Visible = true;
                this.gp.Visible = false;
                this.gp_Detail.Visible = true;
            }
        }
        else
        {
            string dateTime = DateTime.Now.ToString("ddhhmmss");

            if (IsGroup)
            {
                if (GV_List.Rows.Count > 0)
                {
                    GV_List.Columns.RemoveAt(GV_List.Columns.Count - 1);
                }
                this.ExportXLS(GV_List, "ASNGroup" + dateTime + ".xls");
            }
            else
            {
                this.ExportXLS(GV_List_Detail, "ASNGroup" + dateTime + ".xls");
            }
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        if (!IsPostBack)
        {
            if (this.Action == "Receive")
            {
                this.GV_List.Columns[2].Visible = false;  //类型
                this.GV_List.Columns[9].Visible = false;  //状态
            }
            if (IsSupplier)
            {
                this.GV_List.Columns[2].Visible = false;  //类型
                this.GV_List.Columns[3].Visible = false;  //供应商
                this.GV_List.Columns[4].Visible = false;  //发货地址
            }
        }
    }

    protected void lbtnView_Click(object sender, EventArgs e)
    {
        if (ViewEvent != null)
        {
            string ipNo = ((LinkButton)sender).CommandArgument;
            ViewEvent(ipNo, e);
        }
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string ipNo = ((LinkButton)sender).CommandArgument;
            EditEvent(ipNo, e);
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            InProcessLocation ip = (InProcessLocation)e.Row.DataItem;
            if (e.Row.FindControl("lbtnEdit") != null)
            {
                e.Row.FindControl("lbtnEdit").Visible = this.AsnType != BusinessConstants.CODE_MASTER_INPROCESS_LOCATION_TYPE_VALUE_GAP && ip.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;
            }
            e.Row.Cells[1].Attributes.Add("title", GetDetail(ip));

            Label lblWinTime = (Label)e.Row.FindControl("lblWinTime");
            if (ip.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
            {
                lblWinTime.ForeColor = com.Sconit.Utility.OrderHelper.GetWinTimeColor(ip.CreateDate, ip.WindowTime);
            }
        }
    }

    protected void GV_List_Detail_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (IsExport && e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[3].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[5].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
    }

    private string GetDetail(InProcessLocation ip)
    {
        string detail = "";

        if (ip.InProcessLocationDetails.Count == 0)
        {
            return detail;
        }
        OrderDetail orderDetail = ip.InProcessLocationDetails[0].OrderLocationTransaction.OrderDetail;
        OrderHead orderHead = orderDetail.OrderHead;

        detail += "cssbody=[obbd] cssheader=[obhd] header=[" + orderHead.OrderNo + " | " + (ip.Flow == null ? string.Empty : ip.Flow) +
             "] body=[<table width=100%>";
        var ods = ip.InProcessLocationDetails.Take(20);
        foreach (InProcessLocationDetail od in ods)
        {
            decimal uc = od.OrderLocationTransaction.OrderDetail.UnitCount;
            string Carton = uc.ToString("0.########") + "/" + Math.Ceiling((double)(od.Qty / uc)).ToString("0");
            string ItemCode = od.OrderLocationTransaction.Item.Code;
            //string ItemDesc = od.OrderLocationTransaction.Item.Description.Replace("[", "&#91;").Replace("]", "&#93;");
            string ItemDesc = od.ReferenceItemCode;
            string OrderQty = od.Qty.ToString("0.########");
            string Uom = od.OrderLocationTransaction.Uom.Code;
            detail += "<tr><td>" + ItemCode + "</td><td>" + ItemDesc + "</td><td>" + OrderQty + "</td><td>" + Uom + "</td><td>" + Carton + "</td></tr>";
        }
        detail += "</table>]";
        return detail;
    }
}
