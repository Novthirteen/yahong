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
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;

public partial class MasterData_Uom_Uom : ModuleBase
{

    private string uomCode = string.Empty;

    public void QuickSearch()
    {
        this.GV_Uom.DataBind();
        this.fldSearch.Visible = true;
        this.fldInsert.Visible = false;
        this.fldGV.Visible = true;
    }

    protected void GV_Uom_OnDataBind(object sender, EventArgs e)
    {
        this.fldGV.Visible = true;
        if (((GridView)(sender)).Rows.Count == 0)
        {
            this.lblResult.Visible = true;
        }
        else
        {
            this.lblResult.Visible = false;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        QuickSearch();
    }

    protected void btnInsert_Click(object sender, EventArgs e)
    {
        //this.fldSearch.Visible = false;
        //this.fldGV.Visible = false;
        this.fldInsert.Visible = true;
        ((TextBox)(this.FV_Uom.FindControl("tbCode"))).Text = string.Empty;
        ((TextBox)(this.FV_Uom.FindControl("tbName"))).Text = string.Empty;
        ((TextBox)(this.FV_Uom.FindControl("tbDescription"))).Text = string.Empty;
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        this.fldSearch.Visible = true;
        this.fldGV.Visible = true;
        this.fldInsert.Visible = false;
        // this.GV_Uom.DataBind();
    }

    protected void ODS_GV_Uom_OnUpdating(object source, ObjectDataSourceMethodEventArgs e)
    {
        Uom uom = (Uom)e.InputParameters[0];
        if (uom!=null)
        {
            uom.Description = uom.Description.Trim();
            uom.Name = uom.Name.Trim();
        }
    }
    protected void ODS_Uom_Inserting(object source, ObjectDataSourceMethodEventArgs e)
    {
        Uom uom = (Uom)e.InputParameters[0];

        uom.Description = uom.Description.Trim();
        uom.Name = uom.Name.Trim();

        if (uom.Code == null || uom.Code.Trim() == string.Empty)
        {
            ShowWarningMessage("MasterData.Uom.Code.Empty", "");
            e.Cancel = true;
            return;
        }
        else
        {
            uom.Code = uom.Code.Trim();
        }

        if (TheUomMgr.LoadUom(uom.Code) == null)
        {
            ShowSuccessMessage("MasterData.Uom.AddUom.Successfully", uom.Code);
        }
        else
        {
            e.Cancel = true;
            ShowErrorMessage("MasterData.Uom.AddUom.Error", uom.Code);
        }
    }

    protected void ODS_Uom_Inserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        QuickSearch();
    }

    protected void ODS_GV_Uom_OnDeleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        Uom uom = (Uom)e.InputParameters[0];
        uomCode = uom.Code;
    }

    protected void ODS_GV_Uom_OnDeleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        try
        {
            ShowSuccessMessage("MasterData.Uom.DeleteUom.Successfully", uomCode);
        }
        catch (Exception ex)
        {
            ShowErrorMessage("MasterData.Uom.DeleteUom.Fail", uomCode);
        }

    }

    protected void GV_Uom_RowUpdating(object sender, GridViewUpdateEventArgs e)
    {
        //校验
    }


    protected void btnUom_Click(object sender, EventArgs e)
    {
        IList<Item> items = TheItemMgr.GetAllItem();
        IList<BomDetail> bomDetails = TheBomDetailMgr.GetAllBomDetail();
        IList<UomConversion> uomConversions = TheUomConversionMgr.GetAllUomConversion();
        IList<PriceListDetail> priceListDetails = ThePriceListDetailMgr.GetAllPriceListDetail();
        IList<FlowDetail> flowDetails = TheFlowDetailMgr.GetAllFlowDetail();

        //BomDetail
        var q_bd = bomDetails.Join(items, d => d.Item.Code, i => i.Code, (d, i) => new { d, i }).Where(q1 => q1.d.Uom.Code != q1.i.Uom.Code &&
                 (uomConversions.Where(u => u.AlterUom.Code == q1.d.Uom.Code && u.BaseUom.Code == q1.i.Uom.Code && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0
               && uomConversions.Where(u => u.BaseUom.Code == q1.d.Uom.Code && u.AlterUom.Code == q1.i.Uom.Code && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0))
               .Select(q2 => new UomCheckView { Code = q2.d.Bom.Code, Description = q2.d.Bom.Description, Type = "BomDetail", Item = q2.i, AlterUom = q2.d.Uom })
               .ToList();

        //FlowDetail
        var q_fd = flowDetails.Join(items, d => d.Item.Code, i => i.Code, (d, i) => new { d, i }).Where(q1 => q1.d.Uom.Code != q1.i.Uom.Code &&
                 (uomConversions.Where(u => u.AlterUom.Code == q1.d.Uom.Code && u.BaseUom.Code == q1.i.Uom.Code && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0
               && uomConversions.Where(u => u.BaseUom.Code == q1.d.Uom.Code && u.AlterUom.Code == q1.i.Uom.Code && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0))
               .Select(q2 => new UomCheckView { Code = q2.d.Flow.Code, Description = q2.d.Flow.Description, SubType = q2.d.Flow.Type, Type = "FlowDetail", Item = q2.i, AlterUom = q2.d.Uom })
               .ToList();

        //PriceListDetail
        var q_pd = priceListDetails.Join(items, d => d.Item.Code, i => i.Code, (d, i) => new { d, i }).Where(q1 => q1.d.Uom.Code != q1.i.Uom.Code &&
                 (uomConversions.Where(u => u.AlterUom.Code == q1.d.Uom.Code && u.BaseUom.Code == q1.i.Uom.Code && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0
               && uomConversions.Where(u => u.BaseUom.Code == q1.d.Uom.Code && u.AlterUom.Code == q1.i.Uom.Code && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0))
               .Select(q2 => new UomCheckView { Code = q2.d.PriceList.Code, Description = q2.d.PriceList.Party.Name, SubType = q2.d.PriceList.Party.Type, Type = "PriceListDetail", Item = q2.i, AlterUom = q2.d.Uom })
               .ToList();


        //BomMstr 取值item  
        var q_bm1 = bomDetails.Select(b => b.Bom).Distinct().Join(items, d => d.Code, i => i.DefaultBomCode, (d, i) => new { d, i }).Where(q1 => q1.d.Uom.Code != q1.i.Uom.Code &&
                 (uomConversions.Where(u => u.AlterUom.Code == q1.d.Uom.Code && u.BaseUom.Code == q1.i.Uom.Code && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0
               && uomConversions.Where(u => u.BaseUom.Code == q1.d.Uom.Code && u.AlterUom.Code == q1.i.Uom.Code && (u.Item == null || u.Item.Code == q1.i.Code)).Count() == 0))
               .Select(q2 => new UomCheckView { Code = q2.d.Code, Description = q2.d.Description, Type = "BomMaster", Item = q2.i, AlterUom = q2.d.Uom })
               .ToList();


        //BomMstr 取值 flowdet 
        var q_bm2 = bomDetails.Select(b => b.Bom).Distinct().Join(flowDetails.Where(f => f.Bom != null), d => d.Code, i => i.Bom.Code, (d, i) => new { d, i }).Where(q1 => q1.d.Uom.Code != q1.i.Uom.Code &&
                 (uomConversions.Where(u => u.AlterUom.Code == q1.d.Uom.Code && u.BaseUom.Code == q1.i.Uom.Code && (u.Item == null || u.Item.Code == q1.i.Item.Code)).Count() == 0
               && uomConversions.Where(u => u.BaseUom.Code == q1.d.Uom.Code && u.AlterUom.Code == q1.i.Uom.Code && (u.Item == null || u.Item.Code == q1.i.Item.Code)).Count() == 0))
               .Select(q2 => new UomCheckView { Code = q2.d.Code, Description = q2.d.Description, Type = "BomMaster", Item = q2.i.Item, AlterUom = q2.d.Uom })
               .ToList();

        q_bd.AddRange(q_fd);
        q_bd.AddRange(q_pd);
        q_bd.AddRange(q_bm1);
        q_bd.AddRange(q_bm2);

        this.GV_List.DataSource = q_bd.Take(1000);
        this.GV_List.DataBind();
        this.fldList.Visible = q_bd.Count > 0;
    }
    

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        { }
    }
    class UomCheckView
    {
        public string Code { get; set; }
        public string Description { get; set; }
        public string SubType { get; set; }
        public string Type { get; set; }
        public Item Item { get; set; }
        public Uom AlterUom { get; set; }
    }

}
