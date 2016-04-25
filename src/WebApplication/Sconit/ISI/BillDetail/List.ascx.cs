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

public partial class ISI_BillDetail_List : ListModuleBase
{
    public bool isGroup
    {
        get { return ViewState["isGroup"] == null ? true : (bool)ViewState["isGroup"]; }
        set { ViewState["isGroup"] = value; }
    }
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
            if (isGroup)
            {
                this.GridViewGroup.Execute();
                this.GridViewGroup.Visible = true;
                this.GridPagerGroup.Visible = true;
                this.GV_List.Visible = false;
                this.gp.Visible = false;
                GridViewHelper.GV_MergeTableCell(GridViewGroup, new int[] { 0, 1, 2, 3 });
            }
            else
            {
                this.GV_List.Execute();
                this.GridViewGroup.Visible = false;
                this.GV_List.Visible = true;
                this.GridPagerGroup.Visible = false;
                this.gp.Visible = true;
            }
        }
        else
        {
            string dateTime = DateTime.Now.ToString("ddhhmmss");
            if (isGroup)
            {
                this.ExportXLS(GridViewGroup, "PSIBillGroup" + dateTime + ".xls");
            }
            else
            {
                this.ExportXLS(this.GV_List, "PSIBillDetail" + dateTime + ".xls");
            }
        }
    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            MouldDetail mouldDetail = (MouldDetail)e.Row.DataItem;
            Label lblType = (Label)(e.Row.FindControl("lblType"));
            lblType.Text = this.TheLanguageMgr.TranslateMessage(mouldDetail.Type, this.CurrentUser);
            e.Row.Cells[e.Row.Cells.Count - 1].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
        }
    }

    protected void GVGroup_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            BillView billView = (BillView)e.Row.DataItem;
            e.Row.Cells[4].Text = this.TheLanguageMgr.TranslateMessage("PSI.Bill.Type." + billView.Type, this.CurrentUser);
            e.Row.Cells[0].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[1].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
        }
    }

    public void InitPageParameter()
    {



    }


}
