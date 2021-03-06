﻿using System;
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
using com.Sconit.Entity;
using com.Sconit.Service.Ext.Distribution;
using com.Sconit.Service.Ext.Procurement;
using com.Sconit.Service.Ext.MasterData.Impl;

public partial class MasterData_Flow_New : NewModuleBase
{
    private Flow flow;
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        Controls_TextBox tbRefFlow = (Controls_TextBox)this.FV_Flow.FindControl("tbRefFlow");
        Controls_TextBox tbPartyFrom = (Controls_TextBox)this.FV_Flow.FindControl("tbPartyFrom");
        Controls_TextBox tbPartyTo = (Controls_TextBox)this.FV_Flow.FindControl("tbPartyTo");

        Controls_TextBox tbRejectLocationFrom = (Controls_TextBox)this.FV_Flow.FindControl("tbRejectLocationFrom");
        Controls_TextBox tbRejectLocationTo = (Controls_TextBox)this.FV_Flow.FindControl("tbRejectLocationTo");
        Controls_TextBox tbInspectLocationFrom = (Controls_TextBox)this.FV_Flow.FindControl("tbInspectLocationFrom");
        Controls_TextBox tbInspectLocationTo = (Controls_TextBox)this.FV_Flow.FindControl("tbInspectLocationTo");

        
        tbPartyFrom.ServiceParameter = "string:" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION + ",string:" + this.CurrentUser.Code;
        tbPartyFrom.DataBind();

        tbPartyTo.ServiceParameter = "string:" + BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION + ",string:" + this.CurrentUser.Code;
        tbPartyTo.DataBind();

        tbRefFlow.ServiceParameter = "string:" + this.CurrentUser.Code;
        tbRefFlow.DataBind();

        tbRejectLocationFrom.ServiceParameter = "string:" + this.CurrentUser.Code + ",string:" + BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_REJECT + ",bool:false";
        tbRejectLocationFrom.DataBind();
        tbRejectLocationTo.ServiceParameter = "string:" + this.CurrentUser.Code + ",string:" + BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_REJECT + ",bool:false";
        tbRejectLocationTo.DataBind();
        tbInspectLocationFrom.ServiceParameter = "string:" + this.CurrentUser.Code + ",string:" + BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_INSPECT + ",bool:false";
        tbInspectLocationFrom.DataBind();
        tbInspectLocationTo.ServiceParameter = "string:" + this.CurrentUser.Code + ",string:" + BusinessConstants.CODE_MASTER_LOCATION_TYPE_VALUE_INSPECT + ",bool:false";
        tbInspectLocationTo.DataBind();
        
        #region 生产类型flow不限制库位
        Controls_TextBox tbLocFrom = (Controls_TextBox)this.FV_Flow.FindControl("tbLocFrom");
        Controls_TextBox tbLocTo = (Controls_TextBox)this.FV_Flow.FindControl("tbLocTo");
        tbLocFrom.ServiceParameter = "string:" + this.CurrentUser.Code + ",string:";
        tbLocFrom.DataBind();
        tbLocTo.ServiceParameter = "string:" + this.CurrentUser.Code + ",string:";
        tbLocTo.DataBind();
        #endregion
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_Flow_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        flow = (Flow)e.InputParameters[0];

        Controls_TextBox tbRefFlow = (Controls_TextBox)this.FV_Flow.FindControl("tbRefFlow");
        Controls_TextBox tbPartyFrom = (Controls_TextBox)this.FV_Flow.FindControl("tbPartyFrom");
        Controls_TextBox tbPartyTo = (Controls_TextBox)this.FV_Flow.FindControl("tbPartyTo");
        Controls_TextBox tbShipFrom = (Controls_TextBox)this.FV_Flow.FindControl("tbShipFrom");
        Controls_TextBox tbShipTo = (Controls_TextBox)this.FV_Flow.FindControl("tbShipTo");
        Controls_TextBox tbLocFrom = (Controls_TextBox)this.FV_Flow.FindControl("tbLocFrom");
        Controls_TextBox tbLocTo = (Controls_TextBox)this.FV_Flow.FindControl("tbLocTo");
        Controls_TextBox tbRejectLocationFrom = (Controls_TextBox)this.FV_Flow.FindControl("tbRejectLocationFrom");
        Controls_TextBox tbRejectLocationTo = (Controls_TextBox)this.FV_Flow.FindControl("tbRejectLocationTo");
        Controls_TextBox tbInspectLocationFrom = (Controls_TextBox)this.FV_Flow.FindControl("tbInspectLocationFrom");
        Controls_TextBox tbInspectLocationTo = (Controls_TextBox)this.FV_Flow.FindControl("tbInspectLocationTo");
        
        com.Sconit.Control.CodeMstrDropDownList ddlOrderTemplate = (com.Sconit.Control.CodeMstrDropDownList)(this.FV_Flow.FindControl("ddlOrderTemplate"));
        com.Sconit.Control.CodeMstrDropDownList ddlReceiptTemplate = (com.Sconit.Control.CodeMstrDropDownList)(this.FV_Flow.FindControl("ddlReceiptTemplate"));
        com.Sconit.Control.CodeMstrDropDownList ddlHuTemplate = (com.Sconit.Control.CodeMstrDropDownList)(this.FV_Flow.FindControl("ddlHuTemplate"));
        
        com.Sconit.Control.CodeMstrDropDownList ddlCreateHuOption = (com.Sconit.Control.CodeMstrDropDownList)this.FV_Flow.FindControl("ddlCreateHuOption");


        if (tbRefFlow != null && tbRefFlow.Text.Trim() != string.Empty)
        {
            flow.ReferenceFlow = TheFlowMgr.CheckAndLoadFlow(tbRefFlow.Text.Trim()).Code;
        }

        if (tbPartyFrom != null && tbPartyFrom.Text.Trim() != string.Empty)
        {
            flow.PartyFrom = ThePartyMgr.LoadParty(tbPartyFrom.Text.Trim());
            flow.PartyTo = flow.PartyFrom;
        }
        if (tbPartyTo != null && tbPartyTo.Text.Trim() != string.Empty)
        {
            flow.PartyTo = ThePartyMgr.LoadParty(tbPartyTo.Text.Trim());
        }
        if (tbShipFrom != null && tbShipFrom.Text.Trim() != string.Empty)
        {
            flow.ShipFrom = TheAddressMgr.LoadShipAddress(tbShipFrom.Text.Trim());
        }

        if (tbShipTo != null && tbShipTo.Text.Trim() != string.Empty)
        {
            flow.ShipTo = TheAddressMgr.LoadShipAddress(tbShipTo.Text.Trim());
        }
        if (tbLocFrom != null && tbLocFrom.Text.Trim() != string.Empty)
        {
            flow.LocationFrom = TheLocationMgr.LoadLocation(tbLocFrom.Text.Trim());
        }
        if (tbLocTo != null && tbLocTo.Text.Trim() != string.Empty)
        {
            flow.LocationTo = TheLocationMgr.LoadLocation(tbLocTo.Text.Trim());
        }
        if (ddlOrderTemplate.SelectedIndex != -1)
        {
            flow.OrderTemplate = ddlOrderTemplate.SelectedValue;
        }
        if (ddlReceiptTemplate.SelectedIndex != -1)
        {
            flow.ReceiptTemplate = ddlReceiptTemplate.SelectedValue;
        }
        if (ddlHuTemplate.SelectedIndex != -1)
        {
            flow.HuTemplate = ddlHuTemplate.SelectedValue;
        }
        if (ddlCreateHuOption.SelectedIndex != -1)
        {
            flow.CreateHuOption = ddlCreateHuOption.SelectedValue;
        }

        if (tbRejectLocationFrom != null && tbRejectLocationFrom.Text.Trim() != string.Empty)
        {
            Location rejectLocation = TheLocationMgr.LoadLocation(tbRejectLocationFrom.Text.Trim());
            if (rejectLocation.Region.CostGroup != ((Region)flow.PartyFrom).CostGroup)
            {
                ShowErrorMessage("MasterData.Flow.RejectLocationFrom.CostGroup.Error");
                e.Cancel = true;
            }
            flow.RejectLocationFrom = tbRejectLocationFrom.Text.Trim();
        }
        else
        {
            string regionRejectLocation = ((Region)flow.PartyFrom).RejectLocation;
            if (regionRejectLocation == null || regionRejectLocation == string.Empty)
            {
                ShowErrorMessage("MasterData.Flow.RejectLocationFrom.Empty");
                e.Cancel = true;
            }
            flow.RejectLocationFrom = regionRejectLocation;
        }
        if (tbRejectLocationTo != null && tbRejectLocationTo.Text.Trim() != string.Empty)
        {
            Location rejectLocation = TheLocationMgr.LoadLocation(tbRejectLocationTo.Text.Trim());
            if (rejectLocation.Region.CostGroup != ((Region)flow.PartyTo).CostGroup)
            {
                ShowErrorMessage("MasterData.Flow.RejectLocationTo.CostGroup.Error");
                e.Cancel = true;
            }
            flow.RejectLocationTo = tbRejectLocationTo.Text.Trim();
        }
        else
        {
            string regionRejectLocation = ((Region)flow.PartyTo).RejectLocation;
            if (regionRejectLocation == null || regionRejectLocation == string.Empty)
            {
                ShowErrorMessage("MasterData.Flow.RejectLocationTo.Empty");
                e.Cancel = true;
            }
            flow.RejectLocationTo = regionRejectLocation;
        }

        if (tbInspectLocationFrom != null && tbInspectLocationFrom.Text.Trim() != string.Empty)
        {
            Location inspectLocation = TheLocationMgr.LoadLocation(tbInspectLocationFrom.Text.Trim());
            if (inspectLocation.Region.CostGroup != ((Region)flow.PartyFrom).CostGroup)
            {
                ShowErrorMessage("MasterData.Flow.RejectLocationFrom.CostGroup.Error");
                e.Cancel = true;
            }
            flow.InspectLocationFrom = tbInspectLocationFrom.Text.Trim();
        }
        else
        {
            string regionInspectLocation = ((Region)flow.PartyFrom).InspectLocation;
            if (regionInspectLocation == null || regionInspectLocation == string.Empty)
            {
                ShowErrorMessage("MasterData.Flow.InspectLocationFrom.Empty");
                e.Cancel = true;
            }
            flow.InspectLocationFrom = regionInspectLocation;
        }
        if (tbInspectLocationTo != null && tbInspectLocationTo.Text.Trim() != string.Empty)
        {
            Location inspectLocation = TheLocationMgr.LoadLocation(tbInspectLocationTo.Text.Trim());
            if (inspectLocation.Region.CostGroup != ((Region)flow.PartyTo).CostGroup)
            {
                ShowErrorMessage("MasterData.Flow.InspectLocationTo.CostGroup.Error");
                e.Cancel = true;
            }
            flow.InspectLocationTo = tbInspectLocationTo.Text.Trim();
        }
        else
        {
            string regionInspectLocation = ((Region)flow.PartyTo).InspectLocation;
            if (regionInspectLocation == null || regionInspectLocation == string.Empty)
            {
                ShowErrorMessage("MasterData.Flow.InspectLocationTo.Empty");
                e.Cancel = true;
            }
            flow.InspectLocationTo = regionInspectLocation;
        }
        flow.CheckDetailOption = BusinessConstants.CODE_MASTER_CHECK_ORDER_DETAIL_OPTION_VALUE_NOT_CHECK;
        flow.Type = BusinessConstants.CODE_MASTER_FLOW_TYPE_VALUE_PRODUCTION;
        flow.BillSettleTerm = null;
      
        flow.CreateUser = this.CurrentUser;
        flow.CreateDate = DateTime.Now;
        flow.LastModifyUser = this.CurrentUser;
        flow.LastModifyDate = DateTime.Now;
        flow.IsAsnUniqueReceipt = true;
    }

    protected void ODS_Flow_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(flow.Code, e);
            ShowSuccessMessage("MasterData.Flow.AddFlow.Successfully", flow.Code);
        }
    }

    protected void checkFlowExists(object source, ServerValidateEventArgs args)
    {
        String flowCode = ((TextBox)(this.FV_Flow.FindControl("tbCode"))).Text;
        if (TheFlowMgr.LoadFlow(flowCode) != null)
        {
            args.IsValid = false;
        }

    }

    public void PageCleanup()
    {
        ((Controls_TextBox)this.FV_Flow.FindControl("tbRefFlow")).Text = string.Empty;
        ((Controls_TextBox)this.FV_Flow.FindControl("tbPartyFrom")).Text = string.Empty;

        ((Controls_TextBox)this.FV_Flow.FindControl("tbPartyTo")).Text = string.Empty;
        ((Controls_TextBox)this.FV_Flow.FindControl("tbShipFrom")).Text = string.Empty;
        ((Controls_TextBox)this.FV_Flow.FindControl("tbShipTo")).Text = string.Empty;

        ((Controls_TextBox)this.FV_Flow.FindControl("tbLocFrom")).Text = string.Empty;
        ((Controls_TextBox)this.FV_Flow.FindControl("tbLocTo")).Text = string.Empty;
        ((TextBox)(this.FV_Flow.FindControl("tbCode"))).Text = string.Empty;
        ((TextBox)(this.FV_Flow.FindControl("tbDescription"))).Text = string.Empty;
        ((CheckBox)(this.FV_Flow.FindControl("cbIsActive"))).Checked = true;
        ((CheckBox)(this.FV_Flow.FindControl("cbIsAutoCreate"))).Checked = false;
        ((CheckBox)(this.FV_Flow.FindControl("cbIsAutoRelease"))).Checked = false;
        ((CheckBox)(this.FV_Flow.FindControl("cbIsAutoStart"))).Checked = false;
        ((CheckBox)(this.FV_Flow.FindControl("cbNeedPrintOrder"))).Checked = false;
        ((CheckBox)(this.FV_Flow.FindControl("cbAllowExceed"))).Checked = false;
        ((CheckBox)(this.FV_Flow.FindControl("cbIsAutoReceive"))).Checked = false;
        ((CheckBox)(this.FV_Flow.FindControl("cbIsListDetail"))).Checked = true;
        ((CheckBox)(this.FV_Flow.FindControl("cbFulfillUC"))).Checked = true;
        ((com.Sconit.Control.CodeMstrDropDownList)this.FV_Flow.FindControl("ddlCreateHuOption")).SelectedIndex = 0;
        ((Controls_TextBox)this.FV_Flow.FindControl("tbRejectLocationFrom")).Text = string.Empty;
        ((Controls_TextBox)this.FV_Flow.FindControl("tbRejectLocationTo")).Text=string.Empty;
        ((Controls_TextBox)this.FV_Flow.FindControl("tbInspectLocationFrom")).Text=string.Empty;
        ((Controls_TextBox)this.FV_Flow.FindControl("tbInspectLocationTo")).Text=string.Empty;
        
    }
}
