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
using com.Sconit.Utility;
using com.Sconit.Entity.Cost;


public partial class Reports_ProdIONew_Main : MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            this.tbStartDate.Text = DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm");
            this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
        this.tbLocation.ServiceParameter = "string:" + this.CurrentUser.Code;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.tbStartDate.Text.Trim() == string.Empty)
            {
                ShowErrorMessage("请选择开始日期");
                return;
            }
            DateTime endDate = DateTime.Now.Date;
            if (this.tbEndDate.Text.Trim() != string.Empty)
            {
                endDate = DateTime.Parse(this.tbEndDate.Text.Trim());
            }
            DateTime startDate = DateTime.Parse(this.tbStartDate.Text.Trim());
            string locationCode = this.tbLocation.Text.Trim();

            SqlParameter[] sqlParam = new SqlParameter[3];
            sqlParam[0] = new SqlParameter("@p0", startDate);
            sqlParam[1] = new SqlParameter("@p1", endDate);

            string sql = @"
                Select olt.Item,olt.Loc, sum((isnull(od.RejQty,0)+isnull(od.ScrapQty,0))*olt.UnitQty) as ScrapQty
                into #a1
                from OrderLocTrans olt with(nolock)
                join OrderDet od with(nolock) on od.Id = olt.OrderDetId 
                join OrderMstr oh with(nolock) on oh.OrderNo = od.OrderNo
                where oh.SubType in('Adj','Rwo') and  oh.Type ='Production' and olt.IOType = 'Out' and olt.BackFlushMethod='BatchFeed'
                and StartTime >= @p0 and StartTime<@p1
                group by olt.Loc,olt.Item
                order by olt.Loc,olt.Item

                Select olt.Item,olt.Loc, sum(isnull(od.RecQty,0)*olt.UnitQty) as Qty,sum((isnull(od.RejQty,0)+isnull(od.ScrapQty,0))*olt.UnitQty) as ScrapQty
                into #a
                from OrderLocTrans olt with(nolock)
                join OrderDet od with(nolock) on od.Id = olt.OrderDetId 
                join OrderMstr oh with(nolock) on oh.OrderNo = od.OrderNo
                where oh.SubType = 'Nml' and  oh.Type ='Production' and olt.IOType = 'Out' and olt.BackFlushMethod='BatchFeed'
                and StartTime >= @p0 and StartTime<@p1
                group by olt.Loc,olt.Item
                order by olt.Loc,olt.Item

                select Item,Loc,SUM(-Qty) as Qty 
                into #b
                from LocTrans with(nolock) where TransType='ISS-MIN'
                and  CreateDate >= @p0 and CreateDate<@p1 group by Item,Loc
                order by Item,Loc

                select  T.Loc as 库位,T.Item as 物料号,i.Desc1 + ISNULL(i.Desc2,'') as 物料描述,
                cast(round(T.正常消耗-T.原材料报废,2) as float) as 正常消耗,
                cast(round(T.废品消耗,2) as float) as 废品消耗,
                cast(round(T.原材料报废,2) as float) as 原材料报废,
                cast(round(T.正常消耗+T.废品消耗,2) as float) as 理论消耗,
                cast(round(T.实际消耗,2) as float) as 实际消耗,
                case when (T.正常消耗+T.废品消耗)=0 then '-' else cast(cast(round((T.废品消耗+T.原材料报废)*100/(T.正常消耗+T.废品消耗),2) as float) as varchar)+'%' end as 废品率,
                case when T.实际消耗=0 then '-' else cast(cast(round((T.实际消耗-T.正常消耗-T.废品消耗)*100/T.实际消耗,2) as float) as varchar)+'%' end as 差异率
                from 
                (select ISNULL(ISNULL(a.Item,b.Item),a1.Item) Item,ISNULL(ISNULL(a.Loc,b.Loc),a1.Loc) as Loc,ISNULL(a.Qty,0) as 正常消耗,ISNULL(a.ScrapQty,0) as 废品消耗,
                ISNULL(b.Qty,0) as 实际消耗,ISNULL(a1.ScrapQty,0) as 原材料报废
                from #a a full join #b b on a.Item= b.Item and a.Loc=b.Loc
                full join #a1 a1 on a1.Item= b.Item and a1.Loc=b.Loc
                ) T join Item i on i.Code = T.Item
                where (T.实际消耗<>0 or T.正常消耗<>0 or T.原材料报废<>0)
                ";

            if (locationCode != string.Empty)
            {
                sql += " and T.Loc =@p2 ";
                sqlParam[2] = new SqlParameter("@p2", locationCode);
            }

            sql += " Order by T.Loc,T.Item ";

            DataSet dataSetInv = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);

            string exMessage = "此报表时间段:从" + startDate.ToString("yyyy-MM-dd HH:mm") + " 到 " + endDate.ToString("yyyy-MM-dd HH:mm");
            this.GV_List.DataSource = dataSetInv;
            this.GV_List.DataBind();
            this.fld_Gv_List.Visible = true;
            if ((Button)sender == this.btnExport)
            {
                this.Export(this.GV_List, "application/ms-excel", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".xls", "  " + exMessage);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[2].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
    }

    protected void tbFinanceYear_TextChange(object sender, EventArgs e)
    {
        if (this.tbFinanceYear.Text.Trim() == string.Empty)
        {
            return;
        }
        DateTime f = DateTime.Parse(this.tbFinanceYear.Text.Trim());
        int year = f.Year;
        int month = f.Month;
        FinanceCalendar financeCalendar = TheFinanceCalendarMgr.GetFinanceCalendar(year, month);

        this.tbStartDate.Text = financeCalendar.StartDate.ToString("yyyy-MM-dd HH:mm:ss");
        this.tbEndDate.Text = financeCalendar.EndDate.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
