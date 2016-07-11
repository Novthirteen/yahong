using System;
using System.IO;
using System.Web.UI.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using com.Sconit.Facility.Entity;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;

public partial class Facility_FacilityFixOrder_Edit : EditModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler StartBackEvent;

   
    protected void Page_Load(object sender, EventArgs e)
    {

    }


    public void InitPageParameter(string code)
    {
        if (!string.IsNullOrEmpty(code))
        {
          

            FacilityFixOrder facilityFixOrder = TheGenericMgr.FindById<FacilityFixOrder>(code);
            this.tbFixNo.Text = facilityFixOrder.FixNo;
            this.tbCustomer.Text = facilityFixOrder.Customer;
            this.tbDescription.Text = facilityFixOrder.Description;
            this.tbEffectiveDate.Text = facilityFixOrder.EffectiveDate.ToString("yyyy-MM-dd");
            this.tbFacilityName.Text = facilityFixOrder.FacilityName;
            this.tbFCID.Text = facilityFixOrder.FCID;
            this.tbFixSite.Text = facilityFixOrder.FixSite;
            this.tbReferenceNo.Text = facilityFixOrder.ReferenceCode;
            this.tbResult.Text = facilityFixOrder.Result;
            this.tbShift.Text = facilityFixOrder.Shift;

            UpdateView(code);
        }
    }


    public void UpdateView(string fixNo)
    {
        FacilityFixOrder facilityFixOrder = TheGenericMgr.FindById<FacilityFixOrder>(fixNo);
        if (facilityFixOrder.Status == FacilityConstants.CODE_MASTER_FIX_ORDER_CREATE)
        {
            this.btnSubmit.Visible = true;
            this.btnStart.Visible = false;
            this.btnClose.Visible = false;

            this.tbFixSite.ReadOnly = false;
            this.tbResult.ReadOnly = false;
        }

        if (facilityFixOrder.Status == FacilityConstants.CODE_MASTER_FIX_ORDER_SUBMIT)
        {
            this.btnSubmit.Visible = false;
            this.btnStart.Visible = true;
            this.btnClose.Visible = false;

            this.tbFixSite.ReadOnly = true;
            this.tbResult.ReadOnly = true;

        }

        if (facilityFixOrder.Status == FacilityConstants.CODE_MASTER_FIX_ORDER_INPROCESS)
        {
            this.btnSubmit.Visible = false;
            this.btnStart.Visible = false;
            this.btnClose.Visible = false;

            this.tbFixSite.ReadOnly = true;
            this.tbResult.ReadOnly = true;
        }

        if (facilityFixOrder.Status == FacilityConstants.CODE_MASTER_FIX_ORDER_COMPLETE)
        {
            this.btnSubmit.Visible = false;
            this.btnStart.Visible = false;
            this.btnClose.Visible = true;

            this.tbFixSite.ReadOnly = true;
            this.tbResult.ReadOnly = true;
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            string fixNo = this.tbFixNo.Text.Trim();
            TheFacilityFixOrderMgr.ReleaseFacilityFixOrder(fixNo,this.CurrentUser.Code);
            ShowSuccessMessage("Facility.FacilityFixOrder.FacilityFixOrderSubmitSuccessfully", fixNo);
            UpdateView( fixNo);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        try
        {
            string fixNo = this.tbFixNo.Text.Trim();
            TheFacilityFixOrderMgr.StartFacilityFixOrder(fixNo, this.CurrentUser.Code);
            ShowSuccessMessage("Facility.FacilityFixOrder.FacilityFixOrderStartSuccessfully", fixNo);
            UpdateView(fixNo);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }

    }

 

    protected void btnClose_Click(object sender, EventArgs e)
    {
        try
        {
            string fixNo = this.tbFixNo.Text.Trim();
            TheFacilityFixOrderMgr.CloseFacilityFixOrder(fixNo, this.CurrentUser.Code);
            ShowSuccessMessage("Facility.FacilityFixOrder.FacilityFixOrderCloseSuccessfully", fixNo);
            UpdateView(fixNo);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }

    }

}
