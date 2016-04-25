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

public partial class Facility_FacilityItem_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    private FacilityItem facilityItem;

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
        ((TextBox)(this.FV_FacilityItem.FindControl("tbFCID"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_FacilityItem.FindControl("tbItemCode"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityItem.FindControl("tbWarnRate"))).Text = "90";
       // ((TextBox)(this.FV_FacilityItem.FindControl("tbPassRate"))).Text = "100";
        ((TextBox)(this.FV_FacilityItem.FindControl("tbQty"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityItem.FindControl("tbInitQty"))).Text = string.Empty; 
        ((TextBox)(this.FV_FacilityItem.FindControl("tbAmount"))).Text = string.Empty;
        ((CheckBox)(this.FV_FacilityItem.FindControl("cbIsActive"))).Checked = true;
        ((TextBox)(this.FV_FacilityItem.FindControl("tbRemark"))).Text = string.Empty;
    }

    protected void ODS_FacilityItem_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        facilityItem = (FacilityItem)e.InputParameters[0];
        string fcId = ((TextBox)(this.FV_FacilityItem.FindControl("tbFCID"))).Text;
        string itemCode = ((Controls_TextBox)(this.FV_FacilityItem.FindControl("tbItemCode"))).Text;
        facilityItem.FCID = fcId;
        facilityItem.Item = TheItemMgr.LoadItem(itemCode);
        facilityItem.CreateDate = DateTime.Now;
        facilityItem.LastModifyDate = DateTime.Now;
        facilityItem.CreateUser = this.CurrentUser.Code;
        facilityItem.LastModifyUser = this.CurrentUser.Code;
        facilityItem.IsAllocate = true;
        facilityItem.PassRate = 100;
        facilityItem.AllocateType = ((CodeMstrDropDownList)(this.FV_FacilityItem.FindControl("ddlAllocateType"))).SelectedValue;
    }

    protected void ODS_FacilityItem_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(facilityItem.Id, e);
            ShowSuccessMessage("Facility.FacilityItem.AddFacilityItem.Successfully");
        }
    }

    protected void checkFacilityItemExists(object source, ServerValidateEventArgs args)
    {
        string fcId = ((TextBox)(this.FV_FacilityItem.FindControl("tbFCId"))).Text;
        string itemCode = ((Controls_TextBox)(this.FV_FacilityItem.FindControl("tbItemCode"))).Text;
        string allocateType = ((CodeMstrDropDownList)(this.FV_FacilityItem.FindControl("ddlAllocateType"))).Text;

        IList<FacilityItem> facilityItemList = TheFacilityItemMgr.GetFacilityItemList(fcId, itemCode, allocateType);
        if (facilityItemList != null && facilityItemList.Count > 0)
        {
            args.IsValid = false;
        }
    }

}
