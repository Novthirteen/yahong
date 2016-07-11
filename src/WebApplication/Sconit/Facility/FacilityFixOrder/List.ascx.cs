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
using com.Sconit.Utility;
using com.Sconit.Web;
using NHibernate.Expression;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using com.Sconit.Entity.Distribution;
using com.Sconit.Facility.Entity;

public partial class Facility_FacilityFixOrder_List : ListModuleBase
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
        
            this.GV_List.Visible = true;
            this.gp.Visible = true;
         
            if (!isExport)
            {
                this.GV_List.Execute();
            }
            else
            {
               
                string dateTime = DateTime.Now.ToString("ddhhmmss");
                this.ExportXLS(this.GV_List, "FacilityFixOrder" + dateTime + ".xls");
            }
     
    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FacilityFixOrder facilityFixOrder = (FacilityFixOrder)e.Row.DataItem;
          
            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(facilityFixOrder.Status, this.CurrentUser);
        }
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string inspectNo = ((LinkButton)sender).CommandArgument;
            EditEvent(inspectNo, e);
        }
    }

}
