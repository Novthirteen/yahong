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
using com.Sconit.Facility.Entity;
using com.Sconit.Control;
using System.Collections.Generic;
using com.Sconit.Entity;

public partial class Facility_FacilityDistribution_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    private FacilityDistribution facilityDistribution;

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
        ((TextBox)(this.FV_FacilityDistribution.FindControl("tbFCID"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_FacilityDistribution.FindControl("tbCustomerName"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_FacilityDistribution.FindControl("tbSupplierName"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistribution.FindControl("tbDistributionContractCode"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistribution.FindControl("tbPurchaseContractCode"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistribution.FindControl("tbDistributionContractAmount"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistribution.FindControl("tbPurchaseContractAmount"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistribution.FindControl("tbDistributionContact"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistribution.FindControl("tbPurchaseContact"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityDistribution.FindControl("tbRemark"))).Text = string.Empty;
    }

    protected void ODS_FacilityDistribution_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        facilityDistribution = (FacilityDistribution)e.InputParameters[0];
       
        //FacilityDistribution.Item = TheItemMgr.LoadItem(itemCode);
        facilityDistribution.PurchaseBilledAmount = 0;
        facilityDistribution.PurchasePayAmount = 0;
        facilityDistribution.DistributionBilledAmount = 0;
        facilityDistribution.DistributionBilledAmount = 0;
        facilityDistribution.CreateDate = DateTime.Now;
        facilityDistribution.LastModifyDate = DateTime.Now;
        facilityDistribution.CreateUser = this.CurrentUser.Code;
        facilityDistribution.LastModifyUser = this.CurrentUser.Code;
        facilityDistribution.Status = FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CREATE;
        //FacilityDistribution.Status = Busin
        //FacilityDistribution.IsAllocate = true;
        //FacilityDistribution.PassRate = 100;
        //FacilityDistribution.AllocateType = ((CodeMstrDropDownList)(this.FV_FacilityDistribution.FindControl("ddlAllocateType"))).SelectedValue;
    }

    protected void ODS_FacilityDistribution_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(facilityDistribution.Id, e);
            ShowSuccessMessage("Facility.FacilityDistribution.AddFacilityDistribution.Successfully");
        }
    }

    protected void checkPurchaseContractExists(object source, ServerValidateEventArgs args)
    {
        string purchaseContractCode = ((TextBox)(this.FV_FacilityDistribution.FindControl("tbPurchaseContractCode"))).Text;

        IList<FacilityDistribution> FacilityDistributionList = TheFacilityDistributionMgr.GetFacilityDistributionList(purchaseContractCode);
        if (FacilityDistributionList != null && FacilityDistributionList.Count > 0)
        {
            args.IsValid = false;
        }
    }

    protected void checkCodeExists(object source, ServerValidateEventArgs args)
    {
        string code = ((TextBox)(this.FV_FacilityDistribution.FindControl("tbCode"))).Text;

        IList<FacilityDistribution> FacilityDistributionList = TheFacilityDistributionMgr.GetFacilityDistributionListByCode(code);
        if (FacilityDistributionList != null && FacilityDistributionList.Count > 0)
        {
            args.IsValid = false;
        }
    }

}
