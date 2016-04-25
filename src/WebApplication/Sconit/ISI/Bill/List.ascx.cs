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
using com.Sconit.ISI.Entity;
using System.Drawing;

public partial class ISI_Bill_List : ListModuleBase
{
    public EventHandler EditEvent;
    public string PSIType
    {
        get
        {
            return (string)ViewState["PSIType"];
        }
        set
        {
            ViewState["PSIType"] = value;
        }
    }
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
            this.ExportXLS(this.GV_List, "PSIBill" + dateTime + ".xls");
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
        string code = ((LinkButton)sender).CommandArgument;
        try
        {
            TheMouldMgr.DeleteMould(code);
            ShowSuccessMessage("PSI.Bill.Delete" + this.PSIType + ".Successfully", code);
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("PSI.Bill.Delete" + this.PSIType + ".Failed", code);
        }

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Mould mould = (Mould)e.Row.DataItem;

            for (int i = 1; i < e.Row.Cells.Count - 11; i++)
            {
                e.Row.Cells[i].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            }

            LinkButton lbtnDelete = (LinkButton)(e.Row.FindControl("lbtnDelete"));
            if (mould.SOBilledAmount.HasValue && mould.SOBilledAmount.Value > 0
                                || mould.SOPayAmount.HasValue && mould.SOPayAmount.Value > 0
                                || mould.POBilledAmount.HasValue && mould.POBilledAmount.Value > 0
                                || mould.POPayAmount.HasValue && mould.POPayAmount.Value > 0
                                || mould.SupplierBilledAmount.HasValue && mould.SupplierBilledAmount.Value > 0
                                || mould.SupplierPayAmount.HasValue && mould.SupplierPayAmount.Value > 0)
            {
                lbtnDelete.Visible = false;
            }


            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage("ISI.Status." + mould.Status, this.CurrentUser);

            #region 处理附件
            int attachmentCount = this.TheAttachmentDetailMgr.GetAttachmentCount(mould.Code, typeof(Mould).FullName);
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
