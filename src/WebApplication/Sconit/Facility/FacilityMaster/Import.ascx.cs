using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity;
using com.Sconit.Facility.Entity;

public partial class Facility_FacilityMaster_Import : ModuleBase
{
    public event EventHandler ImportEvent;
    public event EventHandler BackEvent;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            
        }
    }


    protected void btnImport_Click(object sender, EventArgs e)
    {
        this.Import();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(null, null);
        }
    }

   
    private void Import()
    {
        try
        {

            IList<FacilityMaintainPlan> facilityMaintainPlanList = TheFacilityMasterMgr.ReadFacilityMaintainPlanFromxls(fileUpload.PostedFile.InputStream, this.CurrentUser);

            TheFacilityMaintainPlanMgr.CreateFacilityMaintainPlanList(facilityMaintainPlanList);
            //IList<OrderHead> ohList = TheOrderMgr.ConvertShiftPlanScheduleToOrders(spsList, this.cbCreateOpt.Checked);
            if (ImportEvent != null)
            {
                ImportEvent(null, null);
            }
            ShowSuccessMessage("Import.Result.Successfully");
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage(ex);
        }
    }
}
