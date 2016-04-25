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

public partial class Facility_FacilityDistribution_List : ListModuleBase
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
            this.ExportXLS(this.GV_List, "FacilityDistribution" + dateTime + ".xls");
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
            TheFacilityDistributionMgr.DeleteFacilityDistribution(Convert.ToInt32(id));
            ShowSuccessMessage("Facility.FacilityDistribution.DeleteFacilityDistribution.Successfully");
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Facility.FacilityDistribution.DeleteFacilityDistribution.Fail");
        }

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FacilityDistribution facilityDistribution = (FacilityDistribution)e.Row.DataItem;

            LinkButton lbtnDelete = (LinkButton)(e.Row.FindControl("lbtnDelete"));
            if (facilityDistribution.PurchaseBilledAmount > 0 || facilityDistribution.PurchaseContractAmount > 0)
            {
                lbtnDelete.Visible = false;
            }


            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(facilityDistribution.Status, this.CurrentUser);

            #region 处理附件
            int attachmentCount = this.TheFacilityMasterMgr.GetFacilityDistributionAttachmentCount(facilityDistribution.Id.ToString());
            if (attachmentCount > 0)
            {
                LinkButton lbtnAttachment = (LinkButton)(e.Row.FindControl("lbtnAttachment"));
                lbtnAttachment.Text = "(<font color='blue'>" + attachmentCount + "</font>)";
                lbtnAttachment.Visible = true;
            }
            #endregion
        }
    }

    protected void lbtnAttachment_Click(object sender, EventArgs e)
    {

        string code = ((LinkButton)sender).CommandArgument;
        this.ucTransAttachment.InitPageParameter(code);
        this.ucTransAttachment.Visible = true;
    }
}
