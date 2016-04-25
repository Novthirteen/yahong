using System;
using System.IO;
using System.Web.UI.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using com.Sconit.Facility.Entity;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity;
using LeanEngine.Entity;

public partial class Facility_FacilityDistribution_Edit : EditModuleBase
{
    public event EventHandler BackEvent;

    protected Int32 Id
    {
        get
        {
            return (Int32)ViewState["Id"];
        }
        set
        {
            ViewState["Id"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void FV_FacilityDistribution_DataBound(object sender, EventArgs e)
    {
        FacilityDistribution facilityDistribution = (FacilityDistribution)(((FormView)(sender)).DataItem);
        ((HiddenField)(this.FV_FacilityDistribution.FindControl("hfStatus"))).Value = facilityDistribution.Status;

        ((Label)(this.FV_FacilityDistribution.FindControl("tbStatusDesc"))).Text = this.TheLanguageMgr.TranslateMessage(facilityDistribution.Status, this.CurrentUser);

        ((Controls_TextBox)(this.FV_FacilityDistribution.FindControl("tbSupplierName"))).Text = facilityDistribution.SupplierName;
        ((Controls_TextBox)(this.FV_FacilityDistribution.FindControl("tbCustomerName"))).Text = facilityDistribution.CustomerName;


        if (facilityDistribution.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CREATE)
        {
            if (facilityDistribution.PurchaseBilledAmount > 0 || facilityDistribution.PurchasePayAmount > 0
      || facilityDistribution.DistributionBilledAmount > 0 || facilityDistribution.DistributionPayAmount > 0)
            {
                (this.FV_FacilityDistribution.FindControl("btnDelete")).Visible = false;
            }
            else
            {
                (this.FV_FacilityDistribution.FindControl("btnDelete")).Visible = true;
            }
            (this.FV_FacilityDistribution.FindControl("btnSave")).Visible = true;
            (this.FV_FacilityDistribution.FindControl("btnPurchaseComplete")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnDistributionComplete")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnClose")).Visible = false;
        }
        else if (facilityDistribution.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_INPROCESS)
        {

            (this.FV_FacilityDistribution.FindControl("btnDelete")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnSave")).Visible = true;
            (this.FV_FacilityDistribution.FindControl("btnPurchaseComplete")).Visible = true;
            (this.FV_FacilityDistribution.FindControl("btnDistributionComplete")).Visible = true;
            (this.FV_FacilityDistribution.FindControl("btnClose")).Visible = true;
        }
        else if (facilityDistribution.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_PURCHASECOMPLETE)
        {

            (this.FV_FacilityDistribution.FindControl("btnDelete")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnSave")).Visible = true;
            (this.FV_FacilityDistribution.FindControl("btnPurchaseComplete")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnDistributionComplete")).Visible = true;
            (this.FV_FacilityDistribution.FindControl("btnClose")).Visible = true;
        }
        else if (facilityDistribution.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_DISTRIBUTIONCOMPLETE)
        {

            (this.FV_FacilityDistribution.FindControl("btnDelete")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnSave")).Visible = true;
            (this.FV_FacilityDistribution.FindControl("btnPurchaseComplete")).Visible = true;
            (this.FV_FacilityDistribution.FindControl("btnDistributionComplete")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnClose")).Visible = true;
        }
        else if (facilityDistribution.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CLOSE)
        {

            (this.FV_FacilityDistribution.FindControl("btnDelete")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnSave")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnPurchaseComplete")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnDistributionComplete")).Visible = false;
            (this.FV_FacilityDistribution.FindControl("btnClose")).Visible = false;
        }
    }

    public void InitPageParameter(Int32 id)
    {
        this.Id = id;
        this.ODS_FacilityDistribution.SelectParameters["id"].DefaultValue = this.Id.ToString();
        this.ODS_FacilityDistribution.DataBind();

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_FacilityDistribution_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("Facility.FacilityDistribution.UpdateFacilityDistribution.Successfully");
    }

    protected void ODS_FacilityDistribution_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        FacilityDistribution facilityDistribution = (FacilityDistribution)e.InputParameters[0];
        // facilityDistribution.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE;


        facilityDistribution.DistributionContractCode = ((TextBox)(this.FV_FacilityDistribution.FindControl("tbDistributionContractCode"))).Text;

        facilityDistribution.LastModifyDate = DateTime.Now;
        facilityDistribution.LastModifyUser = this.CurrentUser.Code;
    }
    protected void ODS_FacilityDistribution_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteItem = (Item)e.InputParameters[0];
    }

    protected void ODS_FacilityDistribution_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Facility.FacilityDistribution.DeleteFacilityDistribution.Successfully");
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("Facility.FacilityDistribution.DeleteFacilityDistribution.Failed");
            e.ExceptionHandled = true;
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            TheFacilityDistributionMgr.DeleteFacilityDistribution(this.Id);
            ShowSuccessMessage("Facility.FacilityDistribution.DeleteFacilityDistribution.Successfully");
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Facility.FacilityDistribution.DeleteFacilityDistribution.Fail");
        }


    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        try
        {
            FacilityDistribution facilityDistribution = TheFacilityDistributionMgr.LoadFacilityDistribution(this.Id);
            facilityDistribution.Status = FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CLOSE;
            facilityDistribution.LastModifyDate = DateTime.Now;
            facilityDistribution.LastModifyUser = this.CurrentUser.Code;
            TheFacilityDistributionMgr.UpdateFacilityDistribution(facilityDistribution);
            ShowSuccessMessage("Facility.FacilityDistribution.CloseFacilityDistribution.Successfully");
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Facility.FacilityDistribution.CloseFacilityDistribution.Fail");
        }

    }


    protected void btnCopy_Click(object sender, EventArgs e)
    {
        try
        {
            TextBox tbDistributionContractCode =  (TextBox)(this.FV_FacilityDistribution.FindControl("tbCode"));
           
            FacilityDistribution facilityDistribution = TheFacilityDistributionMgr.LoadFacilityDistribution(this.Id);

            TheFacilityDistributionMgr.CopyFacilityDistribution(facilityDistribution, this.CurrentUser);
            ShowSuccessMessage("Facility.FacilityDistribution.CopyFacilityDistribution.Successfully");
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Facility.FacilityDistribution.CloseFacilityDistribution.Fail");
        }

    }

    protected void btnPurchaseComplete_Click(object sender, EventArgs e)
    {
        try
        {
            FacilityDistribution facilityDistribution = TheFacilityDistributionMgr.LoadFacilityDistribution(this.Id);
            if (facilityDistribution.Status != FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_INPROCESS
                && facilityDistribution.Status != FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_DISTRIBUTIONCOMPLETE)
            {
                throw new BusinessException("Facility.FacilityDistribution.PurchaseCompleteFacilityDistribution.Fail", facilityDistribution.Status);
            }
            if (facilityDistribution.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_DISTRIBUTIONCOMPLETE)
            {
                facilityDistribution.Status = FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CLOSE;
            }
            else
            {
                facilityDistribution.Status = FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_PURCHASECOMPLETE;
            }
            facilityDistribution.LastModifyDate = DateTime.Now;
            facilityDistribution.LastModifyUser = this.CurrentUser.Code;
            TheFacilityDistributionMgr.UpdateFacilityDistribution(facilityDistribution);
            ShowSuccessMessage("Facility.FacilityDistribution.PurchaseCompleteFacilityDistribution.Successfully");
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }

    }

    protected void btnDistributionComplete_Click(object sender, EventArgs e)
    {
        try
        {
            FacilityDistribution facilityDistribution = TheFacilityDistributionMgr.LoadFacilityDistribution(this.Id);
            if (facilityDistribution.Status != FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_INPROCESS
                && facilityDistribution.Status != FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_PURCHASECOMPLETE)
            {
                throw new BusinessException("Facility.FacilityDistribution.DistributionCompleteFacilityDistribution.Fail", facilityDistribution.Status);
            }
            if (facilityDistribution.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_PURCHASECOMPLETE)
            {
                facilityDistribution.Status = FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CLOSE;
            }
            else
            {
                facilityDistribution.Status = FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_DISTRIBUTIONCOMPLETE;
            }
            facilityDistribution.LastModifyDate = DateTime.Now;
            facilityDistribution.LastModifyUser = this.CurrentUser.Code;
            TheFacilityDistributionMgr.UpdateFacilityDistribution(facilityDistribution);
            ShowSuccessMessage("Facility.FacilityDistribution.DistributionCompleteFacilityDistribution.Successfully");
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
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

    public override void UpdateView()
    {
        this.FV_FacilityDistribution.DataBind();
    }

    protected void checkDistributionContractExists(object source, ServerValidateEventArgs args)
    {
        string distributionContractCode = ((TextBox)(this.FV_FacilityDistribution.FindControl("tbDistributionContractCode"))).Text;

        IList<FacilityDistribution> FacilityDistributionList = TheFacilityDistributionMgr.GetFacilityDistributionListByDistribution(distributionContractCode);
        if (FacilityDistributionList != null && FacilityDistributionList.Count > 0)
        {
            args.IsValid = false;
        }
    }
}
