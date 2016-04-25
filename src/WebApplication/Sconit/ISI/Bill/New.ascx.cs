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
using LeanEngine.Entity;

public partial class ISI_Bill_New : NewModuleBase
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
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    private Mould mould;

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
    protected void checkCodeExists(object source, ServerValidateEventArgs args)
    {
        string prjCode = ((Controls_TextBox)(this.FV_Bill.FindControl("tbPrjCode"))).Text;

        IList<Mould> mouldList = this.TheMouldMgr.GetMould(prjCode, this.PSIType);
        if (mouldList != null && mouldList.Count > 0)
        {
            args.IsValid = false;
        }
    }

    protected void checkSupplierContractNoExists(object source, ServerValidateEventArgs args)
    {
        string supplierContractNo = ((TextBox)(this.FV_Bill.FindControl("tbSupplierContractNo"))).Text;

        IList<Mould> mouldList = this.TheMouldMgr.GetMouldList(supplierContractNo);
        if (mouldList != null && mouldList.Count > 0)
        {
            args.IsValid = false;
        }
    }
    public void PageCleanup()
    {
        lgd.InnerText = "${PSI.Bill.New" + this.PSIType + "}";
        //((TextBox)(this.FV_Bill.FindControl("tbFCID"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbQty"))).Text = "1";
        ((TextBox)(this.FV_Bill.FindControl("tbDesc1"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbQS"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbRemark"))).Text = string.Empty;

        ((Controls_TextBox)(this.FV_Bill.FindControl("tbPrjCode"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_Bill.FindControl("tbCustomer"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_Bill.FindControl("tbSupplier"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_Bill.FindControl("tbPOUser"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_Bill.FindControl("tbSOUser"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_Bill.FindControl("tbMouldUser"))).Text = string.Empty;

        ((TextBox)(this.FV_Bill.FindControl("tbSOContractNo"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOAmount"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOAmount1"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOBillDate1"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOAmount2"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOBillDate2"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOAmount3"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOBillDate3"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOPayDate1"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOPayDate2"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOPayDate3"))).Text = string.Empty;

        ((TextBox)(this.FV_Bill.FindControl("tbSupplierContractNo"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierAmount"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierAmount1"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierBillDate1"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierAmount2"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierBillDate2"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierAmount3"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierBillDate3"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierPayDate1"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierPayDate2"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierPayDate3"))).Text = string.Empty;

        ((TextBox)(this.FV_Bill.FindControl("tbSOAmount4"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOBillDate4"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSOPayDate4"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierAmount4"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierBillDate4"))).Text = string.Empty;
        ((TextBox)(this.FV_Bill.FindControl("tbSupplierPayDate4"))).Text = string.Empty;
    }

    protected void ODS_Bill_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        mould = (Mould)e.InputParameters[0];
        mould.FCID = this.TheNumberControlMgr.GenerateNumber(DateTime.Now.Year.ToString(), 4);
        mould.Type = this.PSIType;
        mould.Code = this.TheNumberControlMgr.GenerateNumber(BusinessConstants.CODE_PREFIX_BILL);
        if (!string.IsNullOrEmpty(mould.Remark))
        {
            mould.Remark = mould.Remark.Trim();
        }
        if (!string.IsNullOrEmpty(mould.Desc1))
        {
            mould.Desc1 = mould.Desc1.Trim();
        }
        if (!string.IsNullOrEmpty(mould.SOContractNo))
        {
            mould.SOContractNo = mould.SOContractNo.Trim();
        }
        if (!string.IsNullOrEmpty(mould.SupplierContractNo))
        {
            mould.SupplierContractNo = mould.SupplierContractNo.Trim();
        }

        if (mould.SOAmount1.HasValue && mould.SOAmount1.Value > 0 || mould.SOAmount2.HasValue && mould.SOAmount2.Value > 0 || mould.SOAmount4.HasValue && mould.SOAmount4.Value > 0 || mould.SOAmount3.HasValue && mould.SOAmount3.Value > 0)
        {
            decimal amount = 0;
            if (mould.SOAmount1.HasValue && mould.SOAmount1.Value > 0)
            {
                amount += mould.SOAmount1.Value;
            }
            if (mould.SOAmount2.HasValue && mould.SOAmount2.Value > 0)
            {
                amount += mould.SOAmount2.Value;
            }
            if (mould.SOAmount4.HasValue && mould.SOAmount4.Value > 0)
            {
                amount += mould.SOAmount4.Value;
            }
            if (mould.SOAmount3.HasValue && mould.SOAmount3.Value > 0)
            {
                amount += mould.SOAmount3.Value;
            }
            if (!mould.SOAmount.HasValue || mould.SOAmount == 0)
            {
                mould.SOAmount = amount;
            }
            else if (mould.SOAmount != amount)
            {
                this.ShowErrorMessage("PSI.Bill.SOAmountNoEquals", mould.SOAmount.Value.ToString("0.########"), amount.ToString("0.########"));
                e.Cancel = true;
            }
        }


        if (mould.POAmount1.HasValue && mould.POAmount1.Value > 0 || mould.POAmount2.HasValue && mould.POAmount2.Value > 0 || mould.POAmount4.HasValue && mould.POAmount4.Value > 0 || mould.POAmount3.HasValue && mould.POAmount3.Value > 0)
        {
            decimal amount = 0;
            if (mould.POAmount1.HasValue && mould.POAmount1.Value > 0)
            {
                amount += mould.POAmount1.Value;
            }
            if (mould.POAmount2.HasValue && mould.POAmount2.Value > 0)
            {
                amount += mould.POAmount2.Value;
            }
            if (mould.POAmount4.HasValue && mould.POAmount4.Value > 0)
            {
                amount += mould.POAmount4.Value;
            }
            if (mould.POAmount3.HasValue && mould.POAmount3.Value > 0)
            {
                amount += mould.POAmount3.Value;
            }
            if (!mould.POAmount.HasValue || mould.POAmount == 0)
            {
                mould.POAmount = amount;
            }
            else if (mould.POAmount != amount)
            {
                this.ShowErrorMessage("PSI.Bill.POAmountNoEquals", mould.POAmount.Value.ToString("0.########"), amount.ToString("0.########"));
                e.Cancel = true;
            }
        }

        mould.SOBilledAmount = null;
        mould.SOPayAmount = null;
        mould.SOBilledAmount1 = null;
        mould.SOPayAmount1 = null;
        mould.SOBilledAmount2 = null;
        mould.SOPayAmount2 = null;
        mould.SOBilledAmount4 = null;
        mould.SOPayAmount4 = null;
        mould.SOBilledAmount3 = null;
        mould.SOPayAmount3 = null;

        mould.POBilledAmount = null;
        mould.POPayAmount = null;
        mould.POBilledAmount1 = null;
        mould.POPayAmount1 = null;
        mould.POBilledAmount2 = null;
        mould.POPayAmount2 = null;
        mould.POBilledAmount4 = null;
        mould.POPayAmount4 = null;
        mould.POBilledAmount3 = null;
        mould.POPayAmount3 = null;

        mould.CreateDate = DateTime.Now;
        mould.CreateUser = this.CurrentUser.Code;
        mould.CreateUserNm = this.CurrentUser.Name;
        mould.LastModifyDate = DateTime.Now;
        mould.LastModifyUser = this.CurrentUser.Code;
        mould.LastModifyUserNm = this.CurrentUser.Name;
        mould.Status = ISIConstants.CODE_MASTER_PSI_BILL_STATUS_CREATE;

        mould.Supplier = ((Controls_TextBox)(this.FV_Bill.FindControl("tbSupplier"))).Text.Trim();
        if (!string.IsNullOrEmpty(mould.Supplier))
        {
            mould.SupplierName = this.TheSupplierMgr.LoadSupplier(mould.Supplier).Name;
        }
        mould.Customer = ((Controls_TextBox)(this.FV_Bill.FindControl("tbCustomer"))).Text.Trim();
        if (!string.IsNullOrEmpty(mould.Customer))
        {
            mould.CustomerName = this.TheCustomerMgr.LoadCustomer(mould.Customer).Name;
        }
        mould.PrjCode = ((Controls_TextBox)(this.FV_Bill.FindControl("tbPrjCode"))).Text.Trim();
        if (!string.IsNullOrEmpty(mould.PrjCode))
        {
            mould.PrjDesc = this.TheTaskSubTypeMgr.LoadTaskSubType(mould.PrjCode).Name;
        }
        mould.MouldUser = ((Controls_TextBox)(this.FV_Bill.FindControl("tbMouldUser"))).Text.Trim();
        if (!string.IsNullOrEmpty(mould.MouldUser))
        {
            mould.MouldUserNm = this.TheUserMgr.LoadUser(mould.MouldUser).Name;
        }
        mould.SOUser = ((Controls_TextBox)(this.FV_Bill.FindControl("tbSOUser"))).Text.Trim();
        if (!string.IsNullOrEmpty(mould.SOUser))
        {
            mould.SOUserNm = this.TheUserMgr.LoadUser(mould.SOUser).Name;
        }
        mould.POUser = ((Controls_TextBox)(this.FV_Bill.FindControl("tbPOUser"))).Text.Trim();
        if (!string.IsNullOrEmpty(mould.POUser))
        {
            mould.POUserNm = this.TheUserMgr.LoadUser(mould.POUser).Name;
        }

    }

    protected void ODS_Bill_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(mould.Code, e);
            ShowSuccessMessage("PSI.Bill.Add" + this.PSIType + ".Successfully");
        }
    }

    protected void FV_Bill_DataBound(object sender, EventArgs e)
    {
    }
}
