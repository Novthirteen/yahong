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

public partial class Facility_FacilityTransfer_List : ListModuleBase
{
    public EventHandler EditEvent;
    public EventHandler TransferEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }


    protected void lbtnTransfer_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string code = ((LinkButton)sender).CommandArgument;
            EditEvent(code, e);
        }
    }

    

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        FacilityMaster facilityMaster = (FacilityMaster)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(facilityMaster.Status, this.CurrentUser);

            Label lblOwner = (Label)(e.Row.FindControl("lblOwner"));
            lblOwner.Text = this.TheLanguageMgr.TranslateMessage(facilityMaster.Owner, this.CurrentUser);

            Label lblCategory = (Label)(e.Row.FindControl("lblCategory"));
            lblCategory.Text = this.TheFacilityCategoryMgr.LoadFacilityCategory(facilityMaster.Category).Description;
        }
    }
}
