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
using NHibernate.Expression;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;

public partial class ISI_BillIO_List : ListModuleBase
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
            //GridViewHelper.GV_MergeTableCell(GV_List, new int[] { 0, 1, 2, 3 });
        }
        else
        {
            string dateTime = DateTime.Now.ToString("ddhhmmss");
            this.ExportXLS(GV_List, "BillIO" + dateTime + ".xls");
        }
    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            BillIODet billIODet = (BillIODet)e.Row.DataItem;
            Label lblType = (Label)(e.Row.FindControl("lblType"));
            lblType.Text = "${" + billIODet.OrgType + "}";
            //e.Row.Cells[e.Row.Cells.Count - 1].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
        }
    }

    public void InitPageParameter()
    {



    }


}
