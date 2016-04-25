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

public partial class Facility_FacilityScrap_List : ListModuleBase
{
    public EventHandler ScrapEvent;
    public EventHandler FinishEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void lbtnScrap_Click(object sender, EventArgs e)
    {
        if (ScrapEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            ScrapEvent(code, e);
        }
    }

    protected void lbtnFinish_Click(object sender, EventArgs e)
    {
        if (FinishEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            FinishEvent(code, e);
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        FacilityMaster facilityMaster = (FacilityMaster)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            LinkButton lbtnScrap = (LinkButton)e.Row.FindControl("lbtnScrap");
            LinkButton lbtnFinish = (LinkButton)e.Row.FindControl("lbtnFinish");

            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(facilityMaster.Status, this.CurrentUser);

            Label lblOwner = (Label)(e.Row.FindControl("lblOwner"));
            lblOwner.Text = this.TheLanguageMgr.TranslateMessage(facilityMaster.Owner, this.CurrentUser);
            Label lblCategory = (Label)(e.Row.FindControl("lblCategory"));
            FacilityCategory facilityCategory = this.TheFacilityCategoryMgr.LoadFacilityCategory(facilityMaster.Category);
            if (facilityCategory != null)
            {
                lblCategory.Text = facilityCategory.Description;
            }

        
        }
    }
}
