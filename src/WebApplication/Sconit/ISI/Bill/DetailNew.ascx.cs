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
using com.Sconit.Web;
using System.IO;
using System.Drawing;
using System.Drawing.Drawing2D;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity;
using com.Sconit.Control;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Bill_DetailNew : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;
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
    protected string MouldCode
    {
        get
        {
            return (string)ViewState["MouldCode"];
        }
        set
        {
            ViewState["MouldCode"] = value;
        }
    }
    protected string PSIBillDetailPhase
    {
        get
        {
            return (string)ViewState["PSIBillDetailPhase"];
        }
        set
        {
            ViewState["PSIBillDetailPhase"] = value;
        }
    }

    private MouldDetail mouldDetail;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    public void PageCleanup()
    {
        ((TextBox)(this.FV_BillDetail.FindControl("tbInvoice"))).Text = string.Empty;
        ((TextBox)(this.FV_BillDetail.FindControl("tbBillDate"))).Text = string.Empty;
        ((TextBox)(this.FV_BillDetail.FindControl("tbPayDate"))).Text = string.Empty;
        ((TextBox)(this.FV_BillDetail.FindControl("tbBillAmount"))).Text = string.Empty;
        ((TextBox)(this.FV_BillDetail.FindControl("tbPayAmount"))).Text = string.Empty;
        ((TextBox)(this.FV_BillDetail.FindControl("tbRemark"))).Text = string.Empty;
    }

    protected void ODS_MouldDetail_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        mouldDetail = (MouldDetail)e.InputParameters[0];
        mouldDetail.Code = this.MouldCode;
        mouldDetail.Phase = this.PSIBillDetailPhase;
        mouldDetail.Type = ((CodeMstrDropDownList)(this.FV_BillDetail.FindControl("ddlType"))).SelectedValue;
        Mould mould = this.TheMouldMgr.LoadMould(this.MouldCode);
        if (mould.Status == ISIConstants.CODE_MASTER_PSI_BILL_STATUS_SOCOMPLETE
            && mouldDetail.Type == ISIConstants.CODE_MASTER_PSIBILLDETAIL_TYPE_SO)
        {
            ShowErrorMessage("PSI.MouldDetail.AddMouldDetail.Fail", this.TheLanguageMgr.TranslateMessage("ISI.Mould." + mould.Status, this.CurrentUser), this.TheLanguageMgr.TranslateMessage(mouldDetail.Type, this.CurrentUser));
            e.Cancel = true;
        }
        if (mould.Status == ISIConstants.CODE_MASTER_PSI_BILL_STATUS_POCOMPLETE
           && mouldDetail.Type == ISIConstants.CODE_MASTER_PSIBILLDETAIL_TYPE_PO)
        {
            ShowErrorMessage("PSI.MouldDetail.AddMouldDetail.Fail", this.TheLanguageMgr.TranslateMessage("ISI.Mould." + mould.Status, this.CurrentUser), this.TheLanguageMgr.TranslateMessage(mouldDetail.Type, this.CurrentUser));
            e.Cancel = true;
        }

        if (!string.IsNullOrEmpty(mouldDetail.Remark))
        {
            mouldDetail.Remark = mouldDetail.Remark.Trim();
        }

        mouldDetail.CreateDate = DateTime.Now;
        mouldDetail.LastModifyDate = DateTime.Now;
        mouldDetail.CreateUser = this.CurrentUser.Code;
        mouldDetail.CreateUserNm = this.CurrentUser.Name;
        mouldDetail.LastModifyUser = this.CurrentUser.Code;
        mouldDetail.LastModifyUserNm = this.CurrentUser.Name;
    }

    protected void ODS_MouldDetail_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(mouldDetail.Id, e);
            ShowSuccessMessage("PSI.MouldDetail.AddMouldDetail.Successfully");
        }
    }
    public void InitPageParameter(string phase, string code)
    {
        this.MouldCode = code;
        this.PSIBillDetailPhase = phase;
        PageCleanup();
    }

    protected void FV_BillDetail_DataBound(object sender, EventArgs e)
    {
        ((Literal)(this.FV_BillDetail.FindControl("lblPhase"))).Text = "${PSI.MouldDetail.Phase." + this.PSIBillDetailPhase + "}";
    }
}
