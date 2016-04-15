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
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.Utility;
using com.Sconit.Entity.Distribution;

public partial class Distribution_OrderIssue_FlowInfo : EditModuleBase
{
    public event EventHandler ShipBackEvent;
    public event EventHandler ReplyBackEvent;

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

    public string OrderNo
    {
        get
        {
            return (string)ViewState["OrderNo"];
        }
        set
        {
            ViewState["OrderNo"] = value;
        }
    }

    public void InitPageParameter(InProcessLocation ip)
    {
        this.BindData(ip);
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
        }
    }

    private void BindData(InProcessLocation ip)
    {
        //PartyFrom
        this.tbPartyFrom.ReadonlyTextBox_DataBind(ip.PartyFrom.Code, ip.PartyFrom.Name);

        //PartyTo
        this.tbPartyTo.ReadonlyTextBox_DataBind(ip.PartyTo.Code, ip.PartyTo.Name);

        //ShipFrom
        if (ip.ShipFrom != null)
            this.tbShipFrom.ReadonlyTextBox_DataBind(ip.ShipFrom.Code, ip.ShipFrom.Address);

        //ShipTo
        if (ip.ShipTo != null)
            this.tbShipTo.ReadonlyTextBox_DataBind(ip.ShipTo.Code, ip.ShipTo.Address);

        //Carrier
        //todo

        //DockDescription
        this.tbDockDescription.ReadonlyTextBox_DataBind(null, ip.DockDescription);
    }

    public void InitPageParameter(string orderNo)
    {
        OrderHead orderHead = TheOrderHeadMgr.LoadOrderHead(orderNo);
        this.tbPartyFrom.ReadonlyTextBox_DataBind(orderHead.PartyFrom.Code, orderHead.PartyFrom.Name);
        this.tbPartyTo.ReadonlyTextBox_DataBind(orderHead.PartyTo.Code, orderHead.PartyTo.Name);
        if (orderHead.ShipFrom != null)
        {
            this.tbShipFrom.ReadonlyTextBox_DataBind(orderHead.ShipFrom.Code, orderHead.ShipFrom.Address);
        }
        if (orderHead.ShipTo != null)
        {
            this.tbShipTo.ReadonlyTextBox_DataBind(orderHead.ShipTo.Code, orderHead.ShipTo.Address);
        }
        if (orderHead.Carrier != null)
        {
            this.tbCarrier.ReadonlyTextBox_DataBind(orderHead.Carrier.Code, orderHead.Carrier.Name);
        }
        this.tbDockDescription.ReadonlyTextBox_DataBind(null, orderHead.DockDescription);

        this.btnReply.Enabled = true;
        this.tbReply.Enabled = true;
        if (orderHead.Status != BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT)
        {
            this.btnReply.Enabled = false;
            this.tbReply.Enabled = false;
            this.btnReply.Text = TheLanguageMgr.TranslateContent("MasterData.Order.OrderHead.IsConfirmed", this.CurrentUser.UserLanguage);
        }
        if (orderHead.Memo != null)
        {
            this.tbReply.Text = orderHead.Memo;
        }

        this.OrderNo = orderNo;
    }

    protected void btnReply_Click(object sender, EventArgs e)
    {
        OrderHead orderHead = TheOrderHeadMgr.LoadOrderHead(this.OrderNo);
        if (this.tbReply.Text.Trim() != string.Empty)
        {
            orderHead.Memo = this.tbReply.Text.Trim();
            TheOrderHeadMgr.UpdateOrderHead(orderHead);
        }
        if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT)
        {
            TheOrderMgr.StartOrder(orderHead, this.CurrentUser);
        }
        List<string> orderNos = new List<string>();
        orderNos.Add(this.OrderNo);
        if (this.ReplyBackEvent != null)
        {
            this.ReplyBackEvent(new object[] { orderNos }, e);
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (this.ShipBackEvent != null)
        {
            this.ShipBackEvent(this, e);
        }
    }
}
