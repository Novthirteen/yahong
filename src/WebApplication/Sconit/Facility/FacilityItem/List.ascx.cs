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

public partial class Facility_FacilityItem_List : ListModuleBase
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
            this.ExportXLS(this.GV_List, "FacilityItem" + dateTime + ".xls");
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
            TheFacilityItemMgr.DeleteFacilityItem(Convert.ToInt32(id));
            ShowSuccessMessage("Facility.FacilityItem.DeleteFacilityItem.Successfully");
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Facility.FacilityItem.DeleteFacilityItem.Fail");
        }

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[3].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[12].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            FacilityItem facilityItem = (FacilityItem)e.Row.DataItem;
            Label lblAllocateType = (Label)(e.Row.FindControl("lblAllocateType"));
            lblAllocateType.Text = this.TheLanguageMgr.TranslateMessage(facilityItem.AllocateType, this.CurrentUser);

            Label lblAllocateRate = (Label)(e.Row.FindControl("lblAllocateRate"));
            lblAllocateRate.ForeColor = GetAllocateRateColor(facilityItem.AllocateRate, facilityItem.WarnRate);
            lblAllocateRate.Text = facilityItem.AllocateRate.ToString("0.###") + "%";

            LinkButton lbtnDelete = (LinkButton)(e.Row.FindControl("lbtnDelete"));
            if (facilityItem.AllocatedQty > 0)
            {
                lbtnDelete.Visible = false;
            }
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
