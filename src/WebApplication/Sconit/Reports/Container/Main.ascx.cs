using System;
using System.Data;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;
using System.Linq;

public partial class Reports_Container_Main : MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if(!IsPostBack)
        {
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DetachedCriteria criteria = DetachedCriteria.For(typeof(Item));

        criteria.SetProjection(Projections.ProjectionList()
                    .Add(Projections.GroupProperty("Container").As("Container")));

        IList<object> obj = TheCriteriaMgr.FindAll<object>(criteria);
        List<string> containers = new List<string>();
        foreach(var ctn in obj)
        {
            string container = (string)ctn;
            if(!string.IsNullOrEmpty(container))
            {
                containers.Add(container);
            }
        }
        containers = containers.OrderBy(c => c).ToList();
        try
        {
            string sql = @"with container as(
                            (select locationdet.location as 库位,item.Container as Container,
                            sum(locationdet.qty/item.uc) as Qty
                            from locationdet 
                            left join item on item.code = locationdet.item
                            where item.container is not null and locationdet.qty>0
                            group by item.container,locationdet.location)
                            UNION
                            (select '计划' as 库位,item.Container,
                            sum(case when (orderdet.orderqty-isnull(orderdet.recqty,0))/item.uc>0 then (orderdet.orderqty-isnull(orderdet.recqty,0))/item.uc else 0 end) as Qty
                            from ordermstr
                            left join orderdet on ordermstr.orderno = orderdet.orderno
                            left join item on item.code = orderdet.item
                            where ordermstr.type='production' and item.container is not null
                            and starttime>'2011-07-20' and status in ('In-Process','Submit')
                            group by item.container)
                            )
                            SELECT * FROM container PIVOT ( SUM(qty)
                            FOR Container IN ( 
                            ";
            foreach(var container in containers)
            {
                sql += (string)container + ",";
            }
            sql = sql.Substring(0, sql.Length - 1);
            sql += ")) as pvt";

            //投入的成品

            DataSet dataSet = TheSqlHelperMgr.GetDatasetBySql(sql, null);

            DataTable dt = dataSet.Tables[0];
            DataRow dr = dt.NewRow();
            DataColumn dc = dt.Columns.Add("合计", Type.GetType("System.Decimal"));

            dr["库位"] = "总计";

            foreach(var container in containers)
            {
                dr[container] = dt.Compute("Sum(" + container + ") ", " ");
            }
            dt.Rows.Add(dr);

            this.GV_List.DataSource = dataSet;
            this.GV_List.DataBind();
            this.fld_Gv_List.Visible = true;
            if((Button)sender == this.btnExport)
            {
                this.ExportXLS(this.GV_List);
            }
        }
        catch(Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.btnSearch_Click(sender, e);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if(e.Row.RowType == DataControlRowType.DataRow)
        {
            decimal total = 0;
            int count = e.Row.Cells.Count - 1;
            for(int i = 2; i < count; i++)
            {
                decimal cell = e.Row.Cells[i].Text == "&nbsp;" ? 0 : decimal.Parse(e.Row.Cells[i].Text);
                e.Row.Cells[i].Text = cell.ToString("#,##0.##");
                total += cell;
            }

            e.Row.Cells[count].Text = total.ToString("#,##0.##");
        }
    }
}
