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
using System.Data.SqlClient;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;
using com.Sconit.Entity.Exception;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;

public partial class Cost_InvAdjust_Main : MainModuleBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        //this.lbn3.Enabled = false;
        if (!IsPostBack)
        {
            this.div_1.Visible = true;
            this.div_3.Visible = false;
            this.tbStartDate.Text = DateTime.Today.AddDays(-7).ToString("yyyy-MM-dd");
            this.tbEndDate.Text = DateTime.Today.ToString("yyyy-MM-dd");
            this.fld_Gv_List.Visible = false;
            this.fld_Adj.Visible = false;
        }
    }

    private Dictionary<string, decimal> itemValue
    {
        get
        {
            return (Dictionary<string, decimal>)ViewState["itemValue"];
        }
        set
        {
            ViewState["itemValue"] = value;
        }
    }

    protected void lbn1_Click(object sender, EventArgs e)
    {
        this.tab_1.Attributes["class"] = "ajax__tab_active";
        this.tab_3.Attributes["class"] = "ajax__tab_inactive";
        this.div_1.Visible = true;
        this.div_3.Visible = false;
    }

    protected void lbn3_Click(object sender, EventArgs e)
    {
        this.tab_1.Attributes["class"] = "ajax__tab_inactive";
        this.tab_3.Attributes["class"] = "ajax__tab_active";
        this.div_1.Visible = false;
        this.div_3.Visible = true;
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string sql = @"select top 10000 m.orderno as 订单号,m.location as 库位,m.reason as 原因,m.remark as 备注,
                            case when m.type='Gr' then  '计划外入库' else '计划外出库' end as 类型,m.location as 库位,
                            CONVERT(varchar(10), m.effdate, 23) as 生效时间,
                            d.item as 物料号,item.desc1 as 描述1,item.desc2 as 描述2, 
                            case when m.type='Gr' then  cast(d.qty as numeric(12,2)) else cast(-d.qty as numeric(12,2)) end as 数量,
                            d.cost as 价值
                            from miscorderdet d
                            left join miscordermstr m on d.orderno = m.orderno
                            left join item on item.code = d.item
                            where m.effdate>=@p1 and m.effdate<=@p2";

            if (this.rblListType.SelectedIndex == 1)
            {
                sql = @"select top 10000 m.code as 订单号,m.location as 库位,d.diffreason as 原因,d.memo as 备注,
                        '盘点' as 类型,m.location as 库位,CONVERT(varchar(10), m.effdate, 23) as 生效时间,
                        d.item as 物料号,item.desc1 as 描述1,item.desc2 as 描述2, 
                        cast(d.diffQty as numeric(12,2)) as 数量,0 as 价值
                        from cyclecountresult d
                        left join cyclecountmstr m on d.orderno = m.code
                        left join item on item.code = d.item
                        where m.effdate>=@p1 and m.effdate<=@p2";
            }

            DateTime startDate = DateTime.Now.AddMonths(-1);
            DateTime endDate = DateTime.Now.AddMonths(1);

            if (this.tbStartDate.Text.Trim() != string.Empty)
            {
                try
                {
                    startDate = DateTime.Parse(this.tbStartDate.Text.Trim());
                }
                catch (Exception ex)
                { }
            }
            if (this.tbEndDate.Text.Trim() != string.Empty)
            {
                try
                {
                    endDate = DateTime.Parse(this.tbEndDate.Text.Trim());
                }
                catch (Exception ex)
                { }
            }

            SqlParameter[] sqlParam = new SqlParameter[5];
            sqlParam[0] = new SqlParameter("@p0", this.tbMiscOrderCode.Text.Trim());
            sqlParam[1] = new SqlParameter("@p1", startDate);
            sqlParam[2] = new SqlParameter("@p2", endDate);
            sqlParam[3] = new SqlParameter("@p3", this.tbLocation.Text.Trim());
            sqlParam[4] = new SqlParameter("@p4", this.tbItem.Text.Trim());

            if (this.tbMiscOrderCode.Text.Trim() != string.Empty)
            {
                if (this.rblListType.SelectedIndex == 1)
                {
                    sql += " and m.Code =@p0 ";
                }
                else
                {
                    sql += " and m.Orderno =@p0 ";
                }
                sqlParam[1] = new SqlParameter("@p0", this.tbMiscOrderCode.Text.Trim());
            }
            if (this.tbLocation.Text.Trim() != string.Empty)
            {
                sql += " and m.Location =@p3 ";
                sqlParam[3] = new SqlParameter("@p3", this.tbLocation.Text.Trim());
            }
            if (this.tbItem.Text.Trim() != string.Empty)
            {
                sql += " and d.Item =@p4  ";
                sqlParam[4] = new SqlParameter("@p4", this.tbItem.Text.Trim());
            }


            if (this.rblListType.SelectedIndex == 1)
            {
                sql += " and d.diffQty<>0 order by m.Code desc ";
            }
            else
            {
                sql += " order by m.orderno desc ";
            }

            DataSet dataSet = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);

            this.GV_List.DataSource = dataSet;
            this.GV_List.DataBind();
            this.fld_Gv_List.Visible = true;
            if ((Button)sender == this.btnExport)
            {
                this.ExportXLS(this.GV_List);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    protected void btnImport_Click(object sender, EventArgs e)
    {
        try
        {
            string costGroup = this.tbCostGroup.Text.Trim();
            if (costGroup == string.Empty)
            {
                ShowErrorMessage("Cost.CostGroup.Code.Empty");
                return;
            }

            this.itemValue = new Dictionary<string, decimal>();

            DataTable table = new DataTable("Items");

            DataColumn dataRolumn = new DataColumn();
            dataRolumn = new DataColumn();
            dataRolumn.DataType = System.Type.GetType("System.String");
            dataRolumn.ColumnName = "物料代码";
            table.Columns.Add(dataRolumn);

            dataRolumn = new DataColumn();
            dataRolumn.DataType = System.Type.GetType("System.String");
            dataRolumn.ColumnName = "物料描述";
            table.Columns.Add(dataRolumn);

            dataRolumn = new DataColumn();
            dataRolumn.DataType = System.Type.GetType("System.String");
            dataRolumn.ColumnName = "调整金额";
            table.Columns.Add(dataRolumn);


            Stream inputStream = fileUpload.PostedFile.InputStream;

            if (inputStream.Length == 0)
            {
                throw new BusinessErrorException("Import.Stream.Empty");
            }

            IWorkbook workbook = new HSSFWorkbook(inputStream);

            ISheet sheet = workbook.GetSheetAt(0);

            IEnumerator rows = sheet.GetRowEnumerator();

            #region 列定义
            int colItemCode = 0;//物料代码或参考物料号
            int colItemDescription = 1;//物料描述
            int colValue = 2;//金额
            #endregion
            rows.MoveNext();

            while (rows.MoveNext())
            {
                IRow row = (IRow)rows.Current;
                string rowIndex = (row.RowNum + 1).ToString();

                #region 读取物料代码和调整金额
                try
                {
                    string itemCode = row.GetCell(colItemCode).StringCellValue;
                    double cellValue = row.GetCell(colValue).NumericCellValue;
                    itemValue.Add(itemCode, (decimal)cellValue);
                    Item item = TheItemMgr.CheckAndLoadItem(itemCode);

                    DataRow dataRow = table.NewRow();
                    dataRow["物料代码"] = item.Code;
                    dataRow["物料描述"] = item.Description;
                    dataRow["调整金额"] = cellValue.ToString("0.########");
                    table.Rows.Add(dataRow);
                }
                catch
                {
                    throw new BusinessErrorException("Import.ReadInvAdj.CommonError", rowIndex.ToString());
                }
                #endregion
            }

            this.GV_Adj.DataSource = table;
            this.GV_Adj.DataBind();
            this.fld_Adj.Visible = true;
            ShowSuccessMessage("Import.Result.Successfully");
            this.btnAdj.Visible = true;
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage(ex);
        }
    }

    protected void btnAdj_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime effTime = DateTime.Now;
            if (this.tbStartDate.Text.Trim() != string.Empty)
            {
                try
                {
                    effTime = DateTime.Parse(this.tbStartDate.Text.Trim());
                }
                catch (Exception ex)
                {
                    ShowErrorMessage("生效时间无效");
                    return;
                }
            }
            else
            {
                ShowErrorMessage("生效时间不能为空");
                return;
            }
            //TheCostMgr.ChangeCostBalance(itemValue, this.tbCostGroup.Text.Trim(), this.CurrentUser);
            TheCostTransactionMgr.RecordChangeCostBalance(itemValue, this.CurrentUser, effTime, this.tbCostGroup.Text.Trim());
            ShowSuccessMessage("Cost.ChangeCostBalance.Successfully");
            this.itemValue = new Dictionary<string, decimal>();
            this.fld_Adj.Visible = false;
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    protected void GV_Adj_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[8].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            DateTime effDate = DateTime.Parse(e.Row.Cells[7].Text);
            string itemCode = e.Row.Cells[8].Text;
            decimal qty = decimal.Parse(e.Row.Cells[11].Text);

            e.Row.Cells[12].Text = "0";
            decimal? cost = this.TheCostDetailMgr.CalculateItemUnitCost(itemCode, "CG1", effDate.Year, effDate.Month - 1);
            if (cost.HasValue)
            {
                e.Row.Cells[12].Text = (cost.Value * qty).ToString("0.##");
            }

            e.Row.Cells[3].Text = GetCodeMaster(e.Row.Cells[3].Text);
        }
    }

    protected void btnSearch1_Click(object sender, EventArgs e)
    {
        try
        {
            this.btnAdj.Visible = false;
            string sql = @"select top 10000 costtrans.item as 物料,item.desc1 as 描述1,item.desc2 as 描述2,
                            itemcategory.desc1 as 产品类,costtrans.costcenter as 工作中心,
                            costtrans.actualamount as 金额,costtrans.effdate as 生效日期
                            from costtrans 
                            left join item on item.code = costtrans.item
                            left join itemcategory on itemcategory.code = costtrans.itemcategory
                            where costtrans.effdate>@p0 and costtrans.effdate<@p1 and costtrans.adjtype='adj'";// --and costtrans.adjtype='adj'

            DateTime startDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now.AddDays(1);

            if (this.tbStartDate1.Text.Trim() != string.Empty)
            {
                try
                {
                    startDate = DateTime.Parse(this.tbStartDate1.Text.Trim());
                }
                catch (Exception ex)
                { }
            }
            if (this.tbEndDate1.Text.Trim() != string.Empty)
            {
                try
                {
                    endDate = DateTime.Parse(this.tbEndDate1.Text.Trim());
                }
                catch (Exception ex)
                { }
            }

            SqlParameter[] sqlParam = new SqlParameter[2];
            sqlParam[0] = new SqlParameter("@p0", startDate);
            sqlParam[1] = new SqlParameter("@p1", endDate);

            DataSet dataSet = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);

            this.GV_Adj.DataSource = dataSet;
            this.GV_Adj.DataBind();
            this.fld_Adj.Visible = true;
            if ((Button)sender == this.btnExport1)
            {
                this.ExportXLS(this.GV_List);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    private string GetCodeMaster(string codeValue)
    {
        if (codeValue == string.Empty)
        {
            return string.Empty;
        }
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(CodeMaster));
        selectCriteria.Add(Expression.Eq("Value", codeValue));

        IList<CodeMaster> codemstrs = TheCriteriaMgr.FindAll<CodeMaster>(selectCriteria);
        if (codemstrs != null && codemstrs.Count > 0)
        {
            return codemstrs[0].Description;
        }
        return string.Empty;
    }
}
