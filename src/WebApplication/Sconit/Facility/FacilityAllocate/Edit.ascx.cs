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
using NHibernate.Expression;

public partial class Facility_FacilityAllocate_Edit : EditModuleBase
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

    protected void FV_FacilityAllocate_DataBound(object sender, EventArgs e)
    {
        FacilityAllocate facilityAllocate = (FacilityAllocate)(((FormView)(sender)).DataItem);
        ((Label)(this.FV_FacilityAllocate.FindControl("lblFCID"))).Text = facilityAllocate.FacilityMaster.FCID;
        ((Label)(this.FV_FacilityAllocate.FindControl("lblItemCode"))).Text = facilityAllocate.Item.Code;
        ((Label)(this.FV_FacilityAllocate.FindControl("ddlAllocateType"))).Text = this.TheLanguageMgr.TranslateMessage(facilityAllocate.AllocateType, this.CurrentUser);
    }

    public void InitPageParameter(Int32 id)
    {
        this.Id = id;
        this.ODS_FacilityAllocate.SelectParameters["id"].DefaultValue = this.Id.ToString();
        this.ODS_FacilityAllocate.DataBind();
        this.FV_FacilityAllocate.DataBind();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_FacilityAllocate_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("Facility.FacilityAllocate.UpdateFacilityAllocate.Successfully");
    }

    protected void ODS_FacilityAllocate_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        FacilityAllocate facilityAllocate = (FacilityAllocate)e.InputParameters[0];
        //  facilityAllocate =  TheFacilityAllocateMgr.LoadFacilityAllocate(this.Id);
        string fcId = ((Label)(this.FV_FacilityAllocate.FindControl("lblFCID"))).Text;
        string itemCode = ((Label)(this.FV_FacilityAllocate.FindControl("lblItemCode"))).Text;
        facilityAllocate.FacilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(fcId);
        facilityAllocate.Item = TheItemMgr.LoadItem(itemCode);
        facilityAllocate.AllocatedQty = Convert.ToDecimal(((TextBox)(this.FV_FacilityAllocate.FindControl("tbAllocatedQty"))).Text);
        facilityAllocate.LastModifyDate = DateTime.Now;
        facilityAllocate.LastModifyUser = this.CurrentUser.Code;
    }
    protected void ODS_FacilityAllocate_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteItem = (Item)e.InputParameters[0];
    }

    protected void ODS_FacilityAllocate_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Facility.FacilityAllocate.DeleteFacilityAllocate.Successfully");
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("Facility.FacilityAllocate.DeleteFacilityAllocate.Failed");
            e.ExceptionHandled = true;
        }
    }
}
