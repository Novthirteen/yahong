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

public partial class Facility_FacilityCategory_New : NewModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    private FacilityCategory facilityCategory;

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

        ((TextBox)(this.FV_FacilityCategory.FindControl("tbCode"))).Text = string.Empty;
        ((TextBox)(this.FV_FacilityCategory.FindControl("tbDescription"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_FacilityCategory.FindControl("tbChargePerson"))).Text = string.Empty;
        ((Controls_TextBox)(this.FV_FacilityCategory.FindControl("tbParentCategory"))).Text = string.Empty;
    }

    protected void ODS_FacilityCategory_Inserting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        facilityCategory = (FacilityCategory)e.InputParameters[0];

        facilityCategory.ParentCategory = ((Controls_TextBox)(this.FV_FacilityCategory.FindControl("tbParentCategory"))).Text.Trim();
        facilityCategory.ChargePerson = ((Controls_TextBox)(this.FV_FacilityCategory.FindControl("tbChargePerson"))).Text.Trim();
        if (TheUserMgr.LoadUser(facilityCategory.ChargePerson) != null)
        {
            facilityCategory.ChargePersonName = TheUserMgr.LoadUser(facilityCategory.ChargePerson).Name;
        }
    }

    protected void ODS_FacilityCategory_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CreateEvent != null)
        {
            CreateEvent(facilityCategory.Code, e);
            ShowSuccessMessage("Facility.FacilityCategory.AddFacilityCategory.Successfully", facilityCategory.Code);
        }
    }

    protected void checkFacilityCategoryExists(object source, ServerValidateEventArgs args)
    {
        string code = ((TextBox)(this.FV_FacilityCategory.FindControl("tbCode"))).Text;

        if (TheFacilityCategoryMgr.LoadFacilityCategory(code) != null)
        {
            args.IsValid = false;
        }
    }

}
