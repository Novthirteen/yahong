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

public partial class Facility_FacilityFix_Start : EditModuleBase
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
            FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(code);
            this.tbFCID.Text = facilityMaster.FCID;
            this.tbFacilityName.Text = facilityMaster.Name;
            this.tbReferenceNo.Text = facilityMaster.RefenceCode;
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {

            FacilityFixOrder facilityFixOrder = new FacilityFixOrder();

            facilityFixOrder.FixNo = TheNumberControlMgr.GenerateNumber(FacilityConstants.CODE_PREFIX_FACILITYFIXORDER);
            facilityFixOrder.Customer = this.tbCustomer.Text.Trim();
            facilityFixOrder.Description = this.tbDescription.Text.Trim();
            facilityFixOrder.EffectiveDate = Convert.ToDateTime(this.tbEffectiveDate.Text.Trim());
            facilityFixOrder.FCID = this.tbFCID.Text.Trim();
            facilityFixOrder.FacilityName = this.tbFacilityName.Text.Trim();
            facilityFixOrder.IsSample = this.cbIsSample.Checked;
            facilityFixOrder.RefenceCode = this.tbReferenceNo.Text.Trim();
            facilityFixOrder.Shift = this.tbShift.Text.Trim();

            facilityFixOrder.Status = FacilityConstants.CODE_MASTER_FIX_ORDER_CREATE;
            facilityFixOrder.CreateDate = DateTime.Now;
            facilityFixOrder.CreateUser = this.CurrentUser.Code;
            facilityFixOrder.LastModifyDate = DateTime.Now;
            facilityFixOrder.LastModifyUser = this.CurrentUser.Code;

            string description = this.tbDescription.Text.Trim();
            string fcid = this.tbFCID.Text.Trim();

         

            TheFacilityMasterMgr.CreateFacilityFixOrder(facilityFixOrder);


            ShowSuccessMessage("Facility.FacilityMaster.FacilityMasterMaintainStart.Successfully", facilityFixOrder.FixNo);


            StartBackEvent(facilityFixOrder.FixNo, e);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }


}
