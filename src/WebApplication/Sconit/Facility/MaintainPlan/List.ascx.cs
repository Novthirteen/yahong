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
using com.Sconit.ISI.Service.Util;

public partial class Facility_MaintainPlan_List : ListModuleBase
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
            this.ExportXLS(this.GV_List, "MaintainPlan" + dateTime + ".xls");
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

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code = ((LinkButton)sender).CommandArgument;
        try
        {
            TheMaintainPlanMgr.DeleteMaintainPlan(code);
            ShowSuccessMessage("MasterData.MaintainPlan.DeleteMaintainPlan.Successfully", code);
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("MasterData.MaintainPlan.DeleteMaintainPlan.Fail", code);
        }

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            MaintainPlan maintainPlan = (MaintainPlan)e.Row.DataItem;
            if (!string.IsNullOrEmpty(maintainPlan.Type))
            {
                Label lblType = (Label)(e.Row.FindControl("lblType"));
                lblType.Text = this.TheLanguageMgr.TranslateMessage(maintainPlan.Type, this.CurrentUser);
            }
            if (!string.IsNullOrEmpty(maintainPlan.FacilityCategory))
            {
                Label lblCategory = (Label)(e.Row.FindControl("lblCategory"));
                lblCategory.Text = this.TheFacilityCategoryMgr.LoadFacilityCategory(maintainPlan.FacilityCategory).Description;
            }

            //if (!string.IsNullOrEmpty(maintainPlan.StartUpUser))
            //{
            //    Label lblStartUpUser = (Label)(e.Row.FindControl("lblStartUpUser"));
                

            //    string userNames = this.TheUserSubscriptionMgr.GetUserName(maintainPlan.StartUpUser);

            //    lblStartUpUser.Text = ISIUtil.GetUserMerge(maintainPlan.StartUpUser, userNames);

            //}
        }
    }
}
