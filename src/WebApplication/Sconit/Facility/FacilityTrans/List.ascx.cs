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

public partial class Facility_FacilityTrans_List : ListModuleBase
{
    public EventHandler EditEvent;

    public bool isExport
    {
        get { return ViewState["isExport"] == null ? false : (bool)ViewState["isExport"]; }
        set { ViewState["isExport"] = value; }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
      //  this.GV_List.DataBind();
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
            this.ExportXLS(this.GV_List, "FacilityTrans" + dateTime + ".xls");
        }
    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FacilityTrans facilityTrans = (FacilityTrans)e.Row.DataItem;

            Label lblTransType = (Label)(e.Row.FindControl("lblTransType"));
            lblTransType.Text = this.TheLanguageMgr.TranslateMessage(facilityTrans.TransType, this.CurrentUser);

            e.Row.Cells[6].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
            e.Row.Cells[7].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
            
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
}
