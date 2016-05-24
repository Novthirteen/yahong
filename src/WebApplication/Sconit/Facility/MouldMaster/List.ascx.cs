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
using com.Sconit.Web;
using com.Sconit.Service.Ext.MasterData;
using System.IO;
using com.Sconit.Entity;
using com.Sconit.Facility.Entity;
using System.Collections.Generic;

public partial class Facility_MouldMaster_List : ListModuleBase
{
    public EventHandler EditEvent;

    public bool isExport
    {
        get { return ViewState["isExport"] == null ? false : (bool)ViewState["isExport"]; }
        set { ViewState["isExport"] = value; }
    }

    protected IList<FacilityCategory> FacilityCategoryList
    {
        get
        {
            return (IList<FacilityCategory>)ViewState["FacilityCategoryList"];
        }
        set
        {
            ViewState["FacilityCategoryList"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (FacilityCategoryList == null || FacilityCategoryList.Count == 0)
        {
            FacilityCategoryList = TheFacilityCategoryMgr.GetAllFacilityCategory();
        }
    }

    public override void UpdateView()
    {
        if (!isExport)
        {
            this.GV_List.Execute();
        }
        else
        {
            string dateTime = DateTime.Now.ToString("ddhhmmss");
            this.ExportXLS(this.GV_List, "Facility" + dateTime + ".xls");
        }
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            EditEvent(code, e);
        }
    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FacilityMaster facilityMaster = (FacilityMaster)e.Row.DataItem;

            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(facilityMaster.Status, this.CurrentUser);

            Label lblOwner = (Label)(e.Row.FindControl("lblOwner"));
            lblOwner.Text = this.TheLanguageMgr.TranslateMessage(facilityMaster.Owner, this.CurrentUser);

            Label lblCategory = (Label)(e.Row.FindControl("lblCategory"));
            FacilityCategory facilityCategory = FacilityCategoryList.Where(p => p.Code == facilityMaster.Category).FirstOrDefault();
            if (facilityCategory != null)
            {
                lblCategory.Text = facilityCategory.Description;
            }
        }
    }
}
