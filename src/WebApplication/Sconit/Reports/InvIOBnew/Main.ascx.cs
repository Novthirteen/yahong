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


public partial class Reports_InvIOBnew_Main : MainModuleBase
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
            string itemCode = this.tbItem.Text.Trim();
            string locationCode = this.tbLocation.Text.Trim();
            string itemCategory = this.tbItemCategory.Text.Trim();

            SqlParameter[] sqlParamInv = new SqlParameter[3];
            SqlParameter[] sqlParamStart = new SqlParameter[4];
            SqlParameter[] sqlParamEnd = new SqlParameter[4];

            sqlParamStart[3] = new SqlParameter("@p3", startDate);
            sqlParamEnd[3] = new SqlParameter("@p3", endDate);

            string sqlInv = @"select locationdet.item as 物料,item.desc1+'['+isnull(item.desc2,'')+']' as 物料描述,item.uom as 单位,item.category as 产品类,locationdet.location as 库位,locationdet.qty as 数量
                            from locationdet left join item on item.code = locationdet.item where 1=1 ";

            string sqlStart = @"select loctrans.item as 物料,loctrans.transtype as 类型,loctrans.loc as 库位,sum(loctrans.qty) as 数量 ,loctrans.IsSubcontract as IsSubcontract
                                from loctrans 
                                left join item on item.code = loctrans.item where loctrans.CreateDate>=@p3 ";
            string sqlEnd = @"select loctrans.item as 物料,loctrans.transtype as 类型,loctrans.loc as 库位,sum(loctrans.qty) as 数量 ,loctrans.IsSubcontract as IsSubcontract
                                from loctrans 
                                left join item on item.code = loctrans.item where loctrans.CreateDate>=@p3 ";

            if (itemCategory != string.Empty)
            {
                sqlInv += " and item.Category =@p2 ";
                sqlParamInv[2] = new SqlParameter("@p2", itemCategory);

                sqlStart += " and item.Category =@p2 ";
                sqlParamStart[2] = new SqlParameter("@p2", itemCategory);

                sqlEnd += " and item.Category =@p2 ";
                sqlParamEnd[2] = new SqlParameter("@p2", itemCategory);
            }

            if (itemCode != string.Empty)
            {
                sqlInv += " and locationdet.item =@p0 ";
                sqlParamInv[0] = new SqlParameter("@p0", itemCode);

                sqlStart += " and loctrans.item =@p0 ";
                sqlParamStart[0] = new SqlParameter("@p0", itemCode);

                sqlEnd += " and loctrans.item =@p0 ";
                sqlParamEnd[0] = new SqlParameter("@p0", itemCode);
            }

            if (locationCode != string.Empty)
            {
                sqlInv += " and locationdet.location =@p1 ";
                sqlParamInv[1] = new SqlParameter("@p1", locationCode);

                sqlStart += " and loctrans.loc =@p1 ";
                sqlParamStart[1] = new SqlParameter("@p1", locationCode);

                sqlEnd += " and loctrans.loc =@p1 ";
                sqlParamEnd[1] = new SqlParameter("@p1", locationCode);

            }

            sqlInv += " order by locationdet.location,locationdet.item ";
            sqlStart += " group by loctrans.item,loctrans.transtype,loctrans.loc,loctrans.IsSubcontract ";
            sqlEnd += " group by loctrans.item,loctrans.transtype,loctrans.loc,loctrans.IsSubcontract ";

            DataSet dataSetInv = TheSqlHelperMgr.GetDatasetBySql(sqlInv, sqlParamInv);
            DataSet dataSetStart = TheSqlHelperMgr.GetDatasetBySql(sqlStart, sqlParamStart);
            DataSet dataSetEnd = TheSqlHelperMgr.GetDatasetBySql(sqlEnd, sqlParamEnd);

            List<Inv> invs = IListHelper.DataTableToList<Inv>(dataSetInv.Tables[0]);
            List<Trans> starts = IListHelper.DataTableToList<Trans>(dataSetStart.Tables[0]);
            List<Trans> ends = IListHelper.DataTableToList<Trans>(dataSetEnd.Tables[0]);

            List<Iob> iobs = new List<Iob>();
            foreach (var inv in invs)
            {
                Iob iob = new Iob();
                //Item item = TheItemMgr.LoadItem(inv.物料);
                //Location location = TheLocationMgr.LoadLocation(inv.库位);
                iob.物料 = inv.物料;
                iob.物料描述 = inv.物料描述;
                iob.产品类 = inv.产品类;
                iob.单位 = inv.单位;
                iob.库位 = inv.库位;
                //iob.库位描述 = location.Name;

                iob.期初 = inv.数量;
                foreach (var s in starts)
                {
                    if (s.库位.Trim().ToLower() == inv.库位.Trim().ToLower() && s.物料.Trim().ToLower() == inv.物料.Trim().ToLower())
                    {
                        switch (s.类型)
                        {
                            case "RCT-PO":
                                iob.采购 += s.数量;
                                break;
                            case "RCT-TR":
                                iob.移库入 += s.数量;
                                break;
                            case "RCT-INP":
                                iob.检验入 += s.数量;
                                break;
                            case "RCT-WO":
                                if (!s.IsSubcontract)
                                {
                                    iob.生产入 += s.数量;
                                }
                                else
                                {
                                    iob.委外入 += s.数量;
                                }
                                break;
                            case "ISS-SO":
                                iob.销售 += s.数量;
                                break;
                            case "ISS-TR":
                                iob.移库出 += s.数量;
                                break;
                            case "ISS-INP":
                                iob.检验出 += s.数量;
                                break;
                            case "ISS-WO":
                            case "ISS-MIN":
                                if (!s.IsSubcontract)
                                {
                                    iob.生产出 += s.数量;
                                }
                                else
                                {
                                    iob.委外出 += s.数量;
                                }
                                break;
                            case "CYC-CNT":
                                iob.盘差 += s.数量;
                                break;
                            case "RCT-UNP":
                            case "ISS-UNP":
                                iob.计划外 += s.数量;
                                break;
                        }
                        iob.期初 -= s.数量;
                    }
                }

                iob.期末 = inv.数量;
                foreach (var s in ends)
                {
                    if (s.库位.Trim().ToLower() == inv.库位.Trim().ToLower() && s.物料.Trim().ToLower() == inv.物料.Trim().ToLower())
                    {
                        switch (s.类型)
                        {
                            case "RCT-PO":
                                iob.采购 -= s.数量;
                                break;
                            case "RCT-TR":
                                iob.移库入 -= s.数量;
                                break;
                            case "RCT-INP":
                                iob.检验入 -= s.数量;
                                break;
                            case "RCT-WO":
                                if (!s.IsSubcontract)
                                {
                                    iob.生产入 -= s.数量;
                                }
                                else
                                {
                                    iob.委外入 -= s.数量;
                                }
                                break;
                            case "ISS-SO":
                                iob.销售 -= s.数量;
                                break;
                            case "ISS-TR":
                                iob.移库出 -= s.数量;
                                break;
                            case "ISS-INP":
                                iob.检验出 -= s.数量;
                                break;
                            case "ISS-WO":
                            case "ISS-MIN":
                                if (!s.IsSubcontract)
                                {
                                    iob.生产出 -= s.数量;
                                }
                                else
                                {
                                    iob.委外出 -= s.数量;
                                }
                                break;
                            case "CYC-CNT":
                                iob.盘差 -= s.数量;
                                break;
                            case "RCT-UNP":
                            case "ISS-UNP":
                                iob.计划外 -= s.数量;
                                break;
                        }
                        iob.期末 -= s.数量;
                    }
                }

                //var si = starts.Where(s => s.库位.Trim().ToLower() == inv.库位.Trim().ToLower() && s.物料.Trim().ToLower() == inv.物料.Trim().ToLower());
                //var ei = ends.Where(s => s.库位.Trim().ToLower() == inv.库位.Trim().ToLower() && s.物料.Trim().ToLower() == inv.物料.Trim().ToLower());

                //iob.期初 = (inv.数量 - si.Sum(s => s.数量));
                //iob.采购 = (si.Where(s => s.类型 == "RCT-PO").Sum(s => s.数量) - ei.Where(s => s.类型 == "RCT-PO").Sum(s => s.数量));
                //iob.移库入 = (si.Where(s => s.类型 == "RCT-TR").Sum(s => s.数量) - ei.Where(s => s.类型 == "RCT-TR").Sum(s => s.数量));
                //iob.检验入 = (si.Where(s => s.类型 == "RCT-INP").Sum(s => s.数量) - ei.Where(s => s.类型 == "RCT-INP").Sum(s => s.数量));
                //iob.生产入 = (si.Where(s => (s.类型 == "RCT-WO" && !s.IsSubcontract)).Sum(s => s.数量) - ei.Where(s => (s.类型 == "RCT-WO" && !s.IsSubcontract)).Sum(s => s.数量));
                //iob.委外入 = (si.Where(s => (s.类型 == "RCT-WO" && s.IsSubcontract)).Sum(s => s.数量) - ei.Where(s => (s.类型 == "RCT-WO" && s.IsSubcontract)).Sum(s => s.数量));

                //iob.销售 = (si.Where(s => s.类型 == "ISS-SO").Sum(s => s.数量) - ei.Where(s => s.类型 == "ISS-SO").Sum(s => s.数量));
                //iob.移库出 = (si.Where(s => s.类型 == "ISS-TR").Sum(s => s.数量) - ei.Where(s => s.类型 == "ISS-TR").Sum(s => s.数量));
                //iob.检验出 = (si.Where(s => s.类型 == "ISS-INP").Sum(s => s.数量) - ei.Where(s => s.类型 == "ISS-INP").Sum(s => s.数量));
                //iob.生产出 = (si.Where(s => ((s.类型 == "ISS-WO" || s.类型 == "ISS-MIN") && !s.IsSubcontract)).Sum(s => s.数量) - ei.Where(s => ((s.类型 == "ISS-WO" || s.类型 == "ISS-MIN") && !s.IsSubcontract)).Sum(s => s.数量));
                //iob.委外出 = (si.Where(s => ((s.类型 == "ISS-WO" || s.类型 == "ISS-MIN") && s.IsSubcontract)).Sum(s => s.数量) - ei.Where(s => ((s.类型 == "ISS-WO" || s.类型 == "ISS-MIN") && s.IsSubcontract)).Sum(s => s.数量));

                //iob.盘差 = (si.Where(s => s.类型 == "CYC-CNT").Sum(s => s.数量) - ei.Where(s => s.类型 == "CYC-CNT").Sum(s => s.数量));
                //iob.计划外 = ((si.Where(s => (s.类型 == "RCT-UNP" || s.类型 == "ISS-UNP"))).Sum(s => s.数量) - (ei.Where(s => (s.类型 == "RCT-UNP" || s.类型 == "ISS-UNP"))).Sum(s => s.数量));


                //iob.期末 = (inv.数量 - ei.Sum(s => s.数量));

                iob.其它 = (iob.期末 - iob.期初 - iob.采购 - iob.移库入 - iob.检验入 - iob.生产入 - iob.委外入 - iob.销售 - iob.移库出 - iob.检验出 - iob.生产出 - iob.委外出 - iob.盘差 - iob.计划外);
                iobs.Add(iob);
            }

            string exMessage = "此收发存报表时间段:从" + startDate.ToString("yyyy-MM-dd HH:mm") + " 到 " + endDate.ToString("yyyy-MM-dd HH:mm");
            this.GV_List.DataSource = iobs;
            this.GV_List.DataBind();
            this.fld_Gv_List.Visible = true;
            if ((Button)sender == this.btnExport)
            {
                //this.ExportXLS(this.GV_List);
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
            e.Row.Cells[1].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[5].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            for (int i = 6; i < e.Row.Cells.Count; i++)
            {
                e.Row.Cells[i].Text = (double.Parse(e.Row.Cells[i].Text)).ToString("0.####");
            }
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

    class Iob
    {
        public string 物料 { get; set; }
        public string 物料描述 { get; set; }
        public string 库位 { get; set; }
        //public string 库位描述 { get; set; }
        public string 单位 { get; set; }
        public string 产品类 { get; set; }
        public decimal 期初 { get; set; }
        //入库
        public decimal 采购 { get; set; }
        public decimal 移库入 { get; set; }
        public decimal 检验入 { get; set; }
        public decimal 生产入 { get; set; }
        public decimal 委外入 { get; set; }
        //出库
        public decimal 销售 { get; set; }
        public decimal 移库出 { get; set; }
        public decimal 检验出 { get; set; }
        public decimal 生产出 { get; set; }
        public decimal 委外出 { get; set; }
        //有出有入
        public decimal 计划外 { get; set; }
        public decimal 盘差 { get; set; }
        public decimal 其它 { get; set; }
        public decimal 期末 { get; set; }
    }

    class Inv
    {
        public string 物料 { get; set; }
        public string 物料描述 { get; set; }
        public string 库位 { get; set; }
        public decimal 数量 { get; set; }
        public string 单位 { get; set; }
        public string 产品类 { get; set; }
    }

    class Trans
    {
        public string 物料 { get; set; }
        public string 物料描述 { get; set; }
        public string 库位 { get; set; }
        public string 类型 { get; set; }
        public decimal 数量 { get; set; }
        public bool IsSubcontract { get; set; }
    }

    private decimal GetPurchaseAmount(string itemCode, string location)
    {
        DateTime endDate = DateTime.Now.Date;
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            endDate = DateTime.Parse(this.tbEndDate.Text.Trim());
        }

        DateTime startDate = DateTime.Parse(this.tbStartDate.Text.Trim());

        DetachedCriteria criteria = DetachedCriteria.For(typeof(ActingBill));
        criteria.Add(Expression.Eq("Item.Code", itemCode));
        criteria.Add(Expression.Eq("LocationFrom", location));
        criteria.Add(Expression.Ge("EffectiveDate", startDate));
        criteria.Add(Expression.Le("EffectiveDate", endDate));

        criteria.SetProjection(Projections.ProjectionList()
            .Add(Projections.GroupProperty("Item.Code").As("Item"))
            .Add(Projections.Sum("BillAmount").As("BillAmount"))
            .Add(Projections.Sum("BillQty").As("BillQty")));

        IList<object[]> objs = TheCriteriaMgr.FindAll<object[]>(criteria);
        if (objs != null && objs.Count() > 0)
        {
            return Convert.ToDecimal(objs[0][1]);
        }
        return 0M;
    }
}
