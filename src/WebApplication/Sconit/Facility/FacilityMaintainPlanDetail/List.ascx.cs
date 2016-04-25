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
using NHibernate.Expression;

public partial class Facility_FacilityMaintainPlanDetail_List : ListModuleBase
{
    public bool isExport
    {
        get { return ViewState["isExport"] == null ? false : (bool)ViewState["isExport"]; }
        set { ViewState["isExport"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
       // this.GV_List.DataBind();
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

            #region 处理附件
            int attachmentCount = this.TheFacilityMasterMgr.GetMaintainPlanAttachmentCount(facilityMaintainPlan.MaintainPlan.Code);
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

        FacilityMaintainPlan p = TheFacilityMaintainPlanMgr.LoadFacilityMaintainPlan(Convert.ToInt32(code));
        this.ucTransAttachment.InitPageParameter(p.MaintainPlan.Code);
        this.ucTransAttachment.Visible = true;
    }

    public void InitPageParameter()
    {

     

    }

 
}
