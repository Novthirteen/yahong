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
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Control;
using System.Data.SqlClient;
using com.Sconit.Utility;
using com.Sconit.ISI.Entity.ISI;

public partial class ISI_TSK_CostDet : ListModuleBase
{
    public string TaskCode
    {
        get
        {
            return (string)ViewState["TaskCode"];
        }
        set
        {
            ViewState["TaskCode"] = value;
        }
    }
    public string ModuleType
    {
        get
        {
            return (string)ViewState["ModuleType"];
        }
        set
        {
            ViewState["ModuleType"] = value;
        }
    }
    public void PageCleanup()
    {
        TaskCode = string.Empty;
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CostDet costDet = (CostDet)e.Row.DataItem;
            if (string.IsNullOrEmpty(costDet.CostCenter))
            {
                //e.Row.BackColor = System.Drawing.Color.Red;
                e.Row.Cells[1].BackColor = System.Drawing.Color.Yellow;
            }
            if (string.IsNullOrEmpty(costDet.Account1))
            {
                //e.Row.BackColor = System.Drawing.Color.Red;
                e.Row.Cells[2].BackColor = System.Drawing.Color.Yellow;
            }
            if (string.IsNullOrEmpty(costDet.Account2))
            {
                //e.Row.BackColor = System.Drawing.Color.Red;
                e.Row.Cells[3].BackColor = System.Drawing.Color.Yellow;
            }
            if (!costDet.AmountY4.HasValue || costDet.AmountY4.Value < 0)
            {
                //e.Row.BackColor = System.Drawing.Color.Red;
                e.Row.Cells[12].BackColor = System.Drawing.Color.Yellow;
            }

            e.Row.Cells[2].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[3].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
        }
    }

    protected void GV_ListMonth_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            /*
            CostDet costDet = (CostDet)e.Row.DataItem;
            if (string.IsNullOrEmpty(costDet.CostCenter))
            {
                e.Row.BackColor = System.Drawing.Color.Red;
                e.Row.Cells[1].BackColor = System.Drawing.Color.Yellow;
            }
            if (string.IsNullOrEmpty(costDet.Account1))
            {
                e.Row.BackColor = System.Drawing.Color.Red;
                e.Row.Cells[2].BackColor = System.Drawing.Color.Yellow;
            }
            if (string.IsNullOrEmpty(costDet.Account2))
            {
                e.Row.BackColor = System.Drawing.Color.Red;
                e.Row.Cells[3].BackColor = System.Drawing.Color.Yellow;
            }
            if (!costDet.AmountM4.HasValue || costDet.AmountM4.Value < 0)
            {
                e.Row.BackColor = System.Drawing.Color.Red;
                e.Row.Cells[11].BackColor = System.Drawing.Color.Yellow;
            }
            */
            e.Row.Cells[2].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
            e.Row.Cells[3].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: normal;");
        }
    }

    public void InitPageParameter()
    {
        InitPageParameter(this.TaskCode);
    }
    public void InitPageParameter(string taskCode)
    {
        if (taskCode != null)
        {
            this.TaskCode = taskCode;
            IList<SqlParameter> sqlParam1 = new List<SqlParameter>();
            sqlParam1.Add(new SqlParameter("@TaskCode", taskCode));
            sqlParam1.Add(new SqlParameter("@CostCenter", null));
            sqlParam1.Add(new SqlParameter("@Account1", null));
            sqlParam1.Add(new SqlParameter("@Account2", null));
            sqlParam1.Add(new SqlParameter("@StartDate", null));
            sqlParam1.Add(new SqlParameter("@EndDate", null));
            sqlParam1.Add(new SqlParameter("@CreateUser", null));
            sqlParam1.Add(new SqlParameter("@User", null));
            DataSet costDetUpDS = this.TheSqlHelperMgr.GetDatasetByStoredProcedure("USP_Req_BudgetDet", sqlParam1.ToArray<SqlParameter>());
            var costDetList = IListHelper.DataTableToList<CostDet>(costDetUpDS.Tables[0]);
            this.GV_List.DataSource = costDetList;
            this.GV_List.DataBind();

            this.GV_ListMonth.DataSource = costDetList;
            this.GV_ListMonth.DataBind();
        }
    }

}
