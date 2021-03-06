﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using System.Text;

public partial class Facility_Reports_FacilityTransReport_Main : com.Sconit.Web.MainModuleBase
{
   
    protected void Page_Load(object sender, EventArgs e)
    {
        

        if (!IsPostBack)
        {
            DateTime now = DateTime.Now;
            this.tbStartDate.Text = now.AddDays(-7).ToString("yyyy-MM-dd");
            this.tbEndDate.Text = now.ToString("yyyy-MM-dd");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {

            DataSet dataSet;
            GetDateSet(out dataSet, string.Empty);

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


    private void GetDateSet(out DataSet dataSet, string sortdirection)
    {
       
        string startDate = this.tbStartDate.Text != string.Empty ? this.tbStartDate.Text.Trim() : string.Empty;
        string endDate = this.tbEndDate.Text != string.Empty ? this.tbEndDate.Text.Trim() : string.Empty;

        IList<SqlParameter> sqlParam = new List<SqlParameter>();
       
        sqlParam.Add(new SqlParameter("@StartDate", DateTime.Parse(startDate)));
        sqlParam.Add(new SqlParameter("@EndDate", DateTime.Parse(endDate).AddDays(1).AddMilliseconds(-1)));
        sqlParam.Add(new SqlParameter("@ChargeOrg", this.tbChargeOrganization.Text));
        sqlParam.Add(new SqlParameter("@CategoryCode", this.tbCategoryCode.Text));
        dataSet = TheSqlHelperMgr.GetDatasetByStoredProcedure("USP_Rep_FacilityTrans", sqlParam.ToArray<SqlParameter>());

    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        this.IsExport = true;
        this.btnSearch_Click(sender, e);
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
       
    }

   

}