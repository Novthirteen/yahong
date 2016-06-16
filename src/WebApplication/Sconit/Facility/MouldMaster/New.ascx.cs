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
using com.Sconit.Entity;
using com.Sconit.Control;
using NHibernate.Expression;


public partial class Facility_MouldMaster_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    private FacilityMaster facilityMaster;

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
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbName"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbSpecification"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbCapacity"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbManufactureDate"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbManufacturer"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbSerialNo"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbAssetNo"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbWarrantyInfo"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbTechInfo"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbSupplier"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbSupplierInfo"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbPONo"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbEffDate"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbPrice"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityMaster.FindControl("tbRemark"))).Text = string.Empty;
        ((CheckBox)(this.FV_FacilityMaster.FindControl("cbIsOffBalance"))).Checked = false;
        ((CheckBox)(this.FV_FacilityMaster.FindControl("cbIsAsset"))).Checked = true;
        ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbCurrChargePerson"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbChargeSite"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbChargeOrganization"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbmaintainGroup"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbmaintainType"))).Text = string.Empty;
    }

    protected void ODS_FacilityMaster_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {

        facilityMaster = (FacilityMaster)e.InputParameters[0];
       // facilityMaster.FCID = TheNumberControlMgr.GenerateNumber(FacilityConstants.CODE_PREFIX_FACILITY);
        facilityMaster.CreateDate = DateTime.Now;
        facilityMaster.LastModifyDate = DateTime.Now;
        FacilityCategory facilityCategoty = TheFacilityCategoryMgr.LoadFacilityCategory(facilityMaster.Category);
        facilityMaster.ParentCategory = facilityCategoty.ParentCategory;
        if (string.IsNullOrEmpty(facilityMaster.ChargeOrganization))
        {
            facilityMaster.ChargeOrganization = facilityCategoty.ChargeOrganization;
        }
        if (string.IsNullOrEmpty(facilityMaster.ChargeSite))
        {
            facilityMaster.ChargeSite = facilityCategoty.ChargeSite;
        }
        if (string.IsNullOrEmpty(facilityMaster.CurrChargePerson))
        {
            facilityMaster.CurrChargePerson = facilityCategoty.ChargePerson;
            facilityMaster.CurrChargePersonName = facilityCategoty.ChargePersonName;
        }
        else
        {
            facilityMaster.CurrChargePersonName = TheUserMgr.LoadUser(facilityMaster.CurrChargePerson).Name;
        }


        facilityMaster.CreateUser = this.CurrentUser.Code;
        facilityMaster.LastModifyUser = this.CurrentUser.Code;
        facilityMaster.IsInStore = true;
        facilityMaster.Owner = ((CodeMstrDropDownList)(this.FV_FacilityMaster.FindControl("ddlOwner"))).SelectedValue;

        facilityMaster.Status = FacilityConstants.CODE_MASTER_FACILITY_STATUS_TEST;

    }

    protected void ODS_FacilityMaster_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(facilityMaster.FCID, e);
            if (CheckAssetNoDup(facilityMaster.AssetNo))
            {
                ShowWarningMessage("Facility.FacilityMaster.AddFacilityMaster.DupAssetNo", facilityMaster.FCID);
               // ShowSuccessMessage("Facility.FacilityMaster.AddFacilityMaster.Successfully", facilityMaster.FCID);
            }
            else
            {
                ShowSuccessMessage("Facility.FacilityMaster.AddFacilityMaster.Successfully", facilityMaster.FCID);
            }
        }
    }

    private bool CheckAssetNoDup(string assetNo)
    {
        bool isDup = false;
        if (!string.IsNullOrEmpty(assetNo))
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityMaster));
            criteria.Add(Expression.Eq("AssetNo", assetNo));

            int resultCnt = TheCriteriaMgr.FindAll<FacilityMaster>(criteria).Count;
            if (resultCnt > 1)
            {
                isDup =  true;
            }
        }
        return isDup;
    }

    protected void checkFCIDExists(object source, ServerValidateEventArgs args)
    {
        String fcId = ((TextBox)(this.FV_FacilityMaster.FindControl("tbFCID"))).Text;
        if (TheFacilityMasterMgr.LoadFacilityMaster(fcId) != null)
        {
            args.IsValid = false;
        }

    }

}
