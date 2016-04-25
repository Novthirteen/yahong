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

public partial class Facility_FacilityItem_Edit : EditModuleBase
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

    protected void FV_FacilityItem_DataBound(object sender, EventArgs e)
    {
        FacilityItem facilityItem = (FacilityItem)(((FormView)(sender)).DataItem);
        ((Label)(this.FV_FacilityItem.FindControl("lblFCID"))).Text = facilityItem.FCID;
        ((Label)(this.FV_FacilityItem.FindControl("lblItemCode"))).Text = facilityItem.Item.Code;
        ((Label)(this.FV_FacilityItem.FindControl("ddlAllocateType"))).Text = this.TheLanguageMgr.TranslateMessage(facilityItem.AllocateType, this.CurrentUser);
    }

    public void InitPageParameter(Int32 id)
    {
        this.Id = id;
        this.ODS_FacilityItem.SelectParameters["id"].DefaultValue = this.Id.ToString();
        this.ODS_FacilityItem.DataBind();

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_FacilityItem_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("Facility.FacilityItem.UpdateFacilityItem.Successfully");
    }

    protected void ODS_FacilityItem_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        FacilityItem facilityItem = (FacilityItem)e.InputParameters[0];
        //  facilityItem =  TheFacilityItemMgr.LoadFacilityItem(this.Id);
        string fcId = ((Label)(this.FV_FacilityItem.FindControl("lblFCID"))).Text;
        string itemCode = ((Label)(this.FV_FacilityItem.FindControl("lblItemCode"))).Text;
        facilityItem.FCID = fcId;
        facilityItem.Item = TheItemMgr.LoadItem(itemCode);
        //  facilityItem.AllocateType = ((CodeMstrDropDownList)(this.FV_FacilityItem.FindControl("ddlAllocateType"))).SelectedValue;
        facilityItem.Qty = Convert.ToDecimal(((TextBox)(this.FV_FacilityItem.FindControl("tbQty"))).Text);
        facilityItem.Amount = Convert.ToDecimal(((TextBox)(this.FV_FacilityItem.FindControl("tbAmount"))).Text);
        facilityItem.AllocatedQty = Convert.ToDecimal(((TextBox)(this.FV_FacilityItem.FindControl("tbAllocatedQty"))).Text);

        facilityItem.InitQty = Convert.ToDecimal(((TextBox)(this.FV_FacilityItem.FindControl("tbInitQty"))).Text);
        facilityItem.SingleQty = Convert.ToDecimal(((TextBox)(this.FV_FacilityItem.FindControl("tbSingleQty"))).Text);

        facilityItem.AllocatedAmount = (facilityItem.InitQty + facilityItem.SingleQty) * facilityItem.Amount / facilityItem.Qty;
        facilityItem.LastModifyDate = DateTime.Now;
        facilityItem.LastModifyUser = this.CurrentUser.Code;
    }
    protected void ODS_FacilityItem_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteItem = (Item)e.InputParameters[0];
    }

    protected void ODS_FacilityItem_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Facility.FacilityItem.DeleteFacilityItem.Successfully");
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("Facility.FacilityItem.DeleteFacilityItem.Failed");
            e.ExceptionHandled = true;
        }
    }

    //protected void checkFacilityItemExists(object source, ServerValidateEventArgs args)
    //{
    //    string fcId = ((Controls_TextBox)(this.FV_FacilityItem.FindControl("tbFCId"))).Text;
    //    string itemCode = ((Controls_TextBox)(this.FV_FacilityItem.FindControl("tbItemCode"))).Text;

    //    IList<FacilityItem> facilityItemList = TheFacilityItemMgr.GetFacilityItemList(fcId, itemCode);
    //    if (facilityItemList != null && facilityItemList.Where(p=>p.Id != Convert.ToInt32(this.Id)).Count() > 0)
    //    {
    //        args.IsValid = false;
    //    }
    //}
}
