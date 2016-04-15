using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Web.UI.WebControls;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;

public partial class MasterData_Bom_BomView_List : ListModuleBase
{
    private Item item;
    private Bom bom;

    protected void Page_Load(object sender, EventArgs e)
    {
    }
    public override void UpdateView()
    {
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Attributes.Add("title", item.Description);
            e.Row.Cells[2].Attributes.Add("title", item.Description);

            e.Row.Cells[7].Text = TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_BOM_DETAIL_TYPE, e.Row.Cells[7].Text.Trim()).Description;

            if (item != null)
            {
                if (e.Row.Cells[0].Text == item.Code)
                {
                    e.Row.Cells[0].ForeColor = Color.Green;
                }
                if (e.Row.Cells[2].Text == item.Code)
                {
                    e.Row.Cells[2].ForeColor = Color.Green;
                }
            }
        }
    }

    public void ListView(object sender)
    {
        string itemCode = ((object[])sender)[0].ToString();
        string date = ((object[])sender)[1].ToString();
        string viewType = ((object[])sender)[2].ToString();
        DateTime effDate = DateTime.Now;

        item = TheItemMgr.LoadItem(itemCode);
        if (item == null)
        {
            ShowWarningMessage("MasterData.Bom.WarningMessage.CodeItem", itemCode);
            return;
        }
        try
        {
            effDate = Convert.ToDateTime(date);
        }
        catch (Exception)
        {
            ShowWarningMessage("MasterData.BomView.WarningMessage.DateInvalid");
            return;
        }

        IList<BomDetail> bomDetailList = new List<BomDetail>();
        if (viewType.ToLower() == "normal")
        {
            bomDetailList = TheBomDetailMgr.GetBomView_Nml(item, effDate);
        }
        else
        {
            bomDetailList = TheBomDetailMgr.GetBomView_Cost(itemCode, effDate);
        }
        this.GV_List.DataSource = this.ConvertListToDatatable(bomDetailList);
        this.GV_List.DataBind();

        if (GV_List.Rows.Count > 0)
        {
            IDictionary<int, int[]> dicIndex = new Dictionary<int, int[]>();
            dicIndex.Add(0, new int[] { 0, 1 });
            GridViewHelper.GV_MergeTableCell(GV_List, dicIndex);
        }
    }

    private DataTable ConvertListToDatatable(IList<BomDetail> bomDetailList)
    {
        DataTable dt = new DataTable();
        dt.Columns.Add("ParCode", typeof(string));
        dt.Columns.Add("ParDesc", typeof(string));
        dt.Columns.Add("ParUom", typeof(string));
        dt.Columns.Add("CompCode", typeof(string));
        dt.Columns.Add("CompDesc", typeof(string));
        dt.Columns.Add("CompUom", typeof(string));
        dt.Columns.Add("CalculatedQty", typeof(decimal));
        dt.Columns.Add("StructureType", typeof(string));
        dt.Columns.Add("LeadTime", typeof(decimal));
        if (bomDetailList != null && bomDetailList.Count > 0)
        {
            foreach (BomDetail bomDetail in bomDetailList)
            {
                DataRow dr = dt.NewRow();
                dr["ParCode"] = bomDetail.Bom.Code;
                dr["ParDesc"] = bomDetail.Bom.Description;
                dr["ParUom"] = bomDetail.Bom.Uom.Code;
                dr["CompCode"] = bomDetail.Item.Code;
                dr["CompDesc"] = bomDetail.Item.Description;
                dr["CompUom"] = bomDetail.Uom.Code;
                dr["CalculatedQty"] = bomDetail.CalculatedQty;
                dr["StructureType"] = bomDetail.StructureType;
                dt.Rows.Add(dr);
            }
        }
        return dt;
    }

}
