using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;
using NHibernate.Expression;
using com.Sconit.Entity.View;
using com.Sconit.Entity;
using System.Data.SqlClient;
using System.Data;
using System.Drawing;
public partial class Report_InvKanBan_Main : MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            fld_Gv_List.Visible = false;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string sql = @"--库存物料明细
                        select item into #items from LocationDet where location = @p0
                        --在途
                        select olt.Item as item, olt.Uom as uom,
                        sum(od.shipQty - (case when od.RecQty is null then 0 else od.RecQty end)) as recqty
                        into #odlts
                        from OrderLocTrans olt 
                        left join orderdet od on olt.orderdetid = od.id
                        left join ordermstr oh on oh.orderno = od.orderno
                        left join #items items on items.item = olt.Item
                        where oh.Status = 'In-Process' and oh.SubType = 'Nml' and not oh.Type = 'Distribution'
                        and olt.IOType = 'In' and od.shipQty is not null --and olt.Loc =@p0
                        and items.item is not null 
                        group by olt.Item,olt.Uom
                        --安全库存
                        select flowdet.item,isnull( flowdet.locto, flowmstr.locto) as location,
                        case when safestock is null then 0 else safestock end as safestock,
                        case when maxstock  is null then 0 else maxstock end as maxstock
                        into #safestock from flowdet 
                        left join flowmstr on flowdet.flow = flowmstr.code
                        left join #items items on items.item = flowdet.Item
                        where (flowdet.locto  = @p0 or flowmstr.locto  =@p0)
						and FlowMstr.IsActive=1
                        and items.item is not null 
                        --总库存
                        select locationdet.item,sum(qty) as qty
                        into #stock
                        from locationdet 
                        left join #items items on items.item = locationdet.item
                        left join location on location.code = locationdet.location
                        where items.item is not null and location.type='NML' and location.IsMrp=1
                        group by locationdet.item

                        select safestock.location as 库位,stock.item as 物料,item.desc1 as 描述1,item.desc2 as 描述2, Item.Uom as 单位,
                        cast(stock.qty as numeric(12,2)) as 总库存,
                        cast(case when safestock.safestock is null then 0 else safestock.safestock end as numeric(12,2)) as 安全库存,
                        cast(case when safestock.maxstock  is null then 0 else safestock.maxstock end as numeric(12,2)) as 最大库存,
                        cast(case when odlts.recqty is null then 0 else odlts.recqty end as numeric(12,2)) as 在途,
                        cast(stock.qty-safestock.safestock + case when odlts.recqty is null then 0 else odlts.recqty end as numeric(12,2)) as 安全差额,
                        cast(stock.qty-safestock.maxstock + case when odlts.recqty is null then 0 else odlts.recqty end as numeric(12,2)) as 最大差额,
                        case when odlts.uom is null then Item.Uom else odlts.uom end as 订单单位,
                        case when stock.qty-safestock.safestock+case when odlts.recqty is null then 0 else odlts.recqty end>0 then 0 else stock.qty-safestock.safestock+
                        case when odlts.recqty is null then 0 else odlts.recqty end end as 百分比
                        from #stock stock 
                        left join #safestock safestock on stock.item =safestock.item
                        left join #odlts odlts on odlts.item = stock.item
                        left join #items items on items.item = stock.Item
                        left join item on item.code = stock.item
                        where safestock.safestock is not null 
                        --and (stock.qty>0 or case when odlts.recqty is null then 0 else odlts.recqty end>0)
                        and items.item is not null 
                        order by 百分比,最大差额 ";
            if (false)
            {
                sql = @"with odlts as
                    (
                    select  olt.Item as item, olt.Uom as uom,
                    sum(od.OrderQty - (case when od.RecQty is null then 0 else od.RecQty end)) as recqty
                    from OrderLocTrans olt left join orderdet od on olt.orderdetid = od.id
                    left join ordermstr oh on oh.orderno = od.orderno
                    where oh.Status in ('Submit', 'In-Process') and oh.SubType = 'Nml' and not oh.Type = 'Distribution'
                    and olt.IOType = 'In' and olt.Loc = @p0
                    group by olt.Item,olt.Uom
                    ),
                    stock as (
                    select flowdet.item,isnull( flowdet.locto, flowmstr.locto) as location,safestock,maxstock from flowdet 
                    left join flowmstr on flowdet.flow = flowmstr.code
                    where (flowdet.locto is not null or flowmstr.locto is not null) and FlowMstr.IsActive=1
                    )
                    select stock.location as 库位,LocationDet.item as 物料,item.desc1 as 描述1,item.desc2 as 描述2, Item.Uom as 单位,
                    cast(LocationDet.qty as numeric(12,2)) as 当前库存,
                    cast(case when stock.safestock is null then 0 else stock.safestock end as numeric(12,2)) as 安全库存,
                    cast(case when stock.maxstock  is null then 0 else stock.maxstock end as numeric(12,2)) as 最大库存,
                    cast(case when odlts.recqty is null then 0 else odlts.recqty end as numeric(12,2)) as 待收,
                    cast(LocationDet.qty-stock.safestock + 
                    case when odlts.recqty is null then 0 else odlts.recqty end as numeric(12,2)) as 安全差额,
                    cast(LocationDet.qty-stock.maxstock + 
                    case when odlts.recqty is null then 0 else odlts.recqty end as numeric(12,2)) as 最大差额,
                    case when odlts.uom is null then Item.Uom else odlts.uom end as 订单单位,
                    case when LocationDet.qty-stock.safestock+
                    case when odlts.recqty is null then 0 else odlts.recqty end>0 then 0 else LocationDet.qty-stock.safestock+
                    case when odlts.recqty is null then 0 else odlts.recqty end end as 百分比
                    from LocationDet left join stock
                    on LocationDet.location =stock.location
                    left join item on item.code = LocationDet.item
                    left join odlts on odlts.item = LocationDet.item
                    where locationdet.item = stock.item and
                    stock.safestock is not null and locationdet.location = @p0
                    and (LocationDet.qty>0 or case when odlts.recqty is null then 0 else odlts.recqty end>0)
                    order by 百分比,最大差额";
            }

            SqlParameter[] sqlParam = new SqlParameter[1];

            if (this.tbLocation.Text.Trim() == string.Empty && this.tbLocation.Text.Trim() == string.Empty)
            {
                ShowErrorMessage("MasterData.MiscOrder.Location.Empty");
                return;
            }
            sqlParam[0] = new SqlParameter("@p0", tbLocation.Text.Trim());
            DataSet dataSet = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);

            this.GV_List.DataSource = dataSet;
            this.GV_List.DataBind();
            this.fld_Gv_List.Visible = true;
            if ((Button)sender == this.btnExport)
            {
                this.ExportXLS(this.GV_List);
            }

            //
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        e.Row.Cells[12].Visible = false;
        //e.Row.Cells[13].Visible = false;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[2].Attributes.Add("style", "vnd.ms-excel.numberformat:@");

            string itemCode = e.Row.Cells[2].Text;
            string uomCode = e.Row.Cells[5].Text;
            decimal diff1 = decimal.Parse(e.Row.Cells[10].Text);
            decimal diff2 = decimal.Parse(e.Row.Cells[11].Text);
            string orderUomCode = e.Row.Cells[12].Text;
            decimal maxStock = decimal.Parse(e.Row.Cells[8].Text);

            if (uomCode != orderUomCode)
            {
                decimal stock = decimal.Parse(e.Row.Cells[6].Text);
                decimal safeStock = decimal.Parse(e.Row.Cells[7].Text);
                decimal receive = decimal.Parse(e.Row.Cells[9].Text);
                receive = TheUomConversionMgr.ConvertUomQty(itemCode, orderUomCode, receive, uomCode);
                e.Row.Cells[9].Text = receive.ToString("0.########");
                diff1 = stock - safeStock + receive;
                diff2 = stock - maxStock + receive;
                e.Row.Cells[10].Text = diff1.ToString("0.########");
                e.Row.Cells[11].Text = diff2.ToString("0.########");
            }
            if (maxStock != 0)
            {
                e.Row.Cells[13].Text = (diff2 / maxStock * 100).ToString("0.##") + "%";
            }
            else
            {
                e.Row.Cells[13].Text = string.Empty;
            }

            if (diff1 < 0)
            {
                e.Row.ForeColor = Color.Red;
                e.Row.BackColor = Color.Yellow;
            }
            else if (diff2 < 0)
            {
                e.Row.ForeColor = Color.OrangeRed;
                //e.Row.BackColor = Color.Wheat;
            }
        }
    }

}
