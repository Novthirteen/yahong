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
using System.Drawing;

public partial class Facility_FacilityAllocate_List : ListModuleBase
{
    public EventHandler EditEvent;

    public bool isExport
    {
        get { return ViewState["isExport"] == null ? false : (bool)ViewState["isExport"]; }
        set { ViewState["isExport"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

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
            this.ExportXLS(this.GV_List, "FacilityAllocate" + dateTime + ".xls");
        }
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string id = ((LinkButton)sender).CommandArgument;
            EditEvent(id, e);
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        try
        {
            TheFacilityAllocateMgr.DeleteFacilityAllocate(Convert.ToInt32(id));
            ShowSuccessMessage("Facility.FacilityAllocate.DeleteFacilityAllocate.Successfully");
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Facility.FacilityAllocate.DeleteFacilityAllocate.Fail");
        }

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FacilityAllocate facilityAllocate = (FacilityAllocate)e.Row.DataItem;
            Label lblAllocateType = (Label)(e.Row.FindControl("lblAllocateType"));
            lblAllocateType.Text = this.TheLanguageMgr.TranslateMessage(facilityAllocate.AllocateType, this.CurrentUser);

         
        }
    }

    public static Color GetAllocateRateColor(decimal allocateRate, decimal warnRate)
    {
        Color color = Color.Black;
        if (warnRate <= allocateRate)
        {
            color = Color.Red;
        }
        return color;
    }
}
