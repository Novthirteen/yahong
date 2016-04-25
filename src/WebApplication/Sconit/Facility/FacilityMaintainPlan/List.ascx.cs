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

public partial class Facility_MaintainPlan_List : ListModuleBase
{
    public EventHandler EditEvent;


    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
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
            TheFacilityMaintainPlanMgr.DeleteFacilityMaintainPlan(Convert.ToInt32(id));
            ShowSuccessMessage("Facility.Facility.DeleteMaintainPlan.Successfully");
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Facility.Facility.DeleteMaintainPlan.Failed");
        }

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FacilityMaintainPlan facilityMaintainPlan = (FacilityMaintainPlan)e.Row.DataItem;
            if (!string.IsNullOrEmpty(facilityMaintainPlan.MaintainPlan.Type))
            {
                Label lblType = (Label)(e.Row.FindControl("lblType"));
                lblType.Text = this.TheLanguageMgr.TranslateMessage(facilityMaintainPlan.MaintainPlan.Type, this.CurrentUser);
            }

        }
    }
}
