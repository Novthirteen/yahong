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
using NHibernate.Expression;

public partial class Facility_FacilityAllocate_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    private FacilityAllocate facilityAllocate;

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
        ((Controls_TextBox)(this.FV_FacilityAllocate.FindControl("tbFCID"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_FacilityAllocate.FindControl("tbItemCode"))).Text = string.Empty;
      
        ((CheckBox)(this.FV_FacilityAllocate.FindControl("cbIsActive"))).Checked = true;
        ((TextBox)(this.FV_FacilityAllocate.FindControl("tbMouldCount"))).Text = "1";
        ((TextBox)(this.FV_FacilityAllocate.FindControl("tbGroupName"))).Text = string.Empty;
    }

    protected void ODS_FacilityAllocate_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        facilityAllocate = (FacilityAllocate)e.InputParameters[0];
        string fcId = ((Controls_TextBox)(this.FV_FacilityAllocate.FindControl("tbFCID"))).Text;
        string itemCode = ((Controls_TextBox)(this.FV_FacilityAllocate.FindControl("tbItemCode"))).Text;
        facilityAllocate.FacilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(fcId);
        facilityAllocate.Item = TheItemMgr.LoadItem(itemCode);
     
        facilityAllocate.CreateDate = DateTime.Now;
        facilityAllocate.LastModifyDate = DateTime.Now;
        facilityAllocate.CreateUser = this.CurrentUser.Code;
        facilityAllocate.LastModifyUser = this.CurrentUser.Code;
        facilityAllocate.AllocateType = ((CodeMstrDropDownList)(this.FV_FacilityAllocate.FindControl("ddlAllocateType"))).SelectedValue;
    }

    protected void ODS_FacilityAllocate_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(facilityAllocate.Id, e);
            ShowSuccessMessage("Facility.FacilityAllocate.AddFacilityAllocate.Successfully");
        }
    }

    protected void checkFacilityAllocateExists(object source, ServerValidateEventArgs args)
    {
        string fcId = ((Controls_TextBox)(this.FV_FacilityAllocate.FindControl("tbFCId"))).Text;
        string itemCode = ((Controls_TextBox)(this.FV_FacilityAllocate.FindControl("tbItemCode"))).Text;

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityAllocate));
        selectCriteria.CreateAlias("Item", "i");
        selectCriteria.CreateAlias("FacilityMaster", "f");

        selectCriteria.Add(Expression.Eq("f.FCID",fcId));
        selectCriteria.Add(Expression.Eq("i.Code", itemCode));

        IList<FacilityAllocate> facilityAllocateList = TheCriteriaMgr.FindAll<FacilityAllocate>(selectCriteria);
        if (facilityAllocateList != null && facilityAllocateList.Count > 0)
        {
            args.IsValid = false;
        }
    }

}
