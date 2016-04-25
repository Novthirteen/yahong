using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity.View;
using NHibernate.Expression;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using System.Collections;
using com.Sconit.Entity.Exception;
using com.Sconit.Facility.Entity;

public partial class Facility_FacilityStock_Detail : ListModuleBase
{
    public event EventHandler BackEvent;

    protected string StNo
    {
        get
        {
            return (string)ViewState["StNo"];
        }
        set
        {
            ViewState["StNo"] = value;
        }
    }

    protected string Status
    {
        get
        {
            return (string)ViewState["Status"];
        }
        set
        {
            ViewState["Status"] = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    public void UpdateView(IList<FacilityStockDetail> facilityStockDetailList)
    {
        this.GV_List.DataSource = facilityStockDetailList;
        this.GV_List.DataBind();

    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (this.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                FacilityStockDetail facilityStockDetail = (FacilityStockDetail)e.Row.DataItem;

                LinkButton lbtnConfirm = (LinkButton)(e.Row.FindControl("lbtnConfirm"));
                lbtnConfirm.Visible = facilityStockDetail.Qty != facilityStockDetail.InvQty;
            }
        }
    }

    public void InitPageParameter(string code)
    {
        this.StNo = code;
        FacilityStockMaster m = TheFacilityStockMasterMgr.LoadFacilityStockMaster(code);
        this.Status = m.Status;

        DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityStockDetail));

        criteria.Add(Expression.Eq("StNo", code));
        IList<FacilityStockDetail> facilityStockDetailList = TheCriteriaMgr.FindAll<FacilityStockDetail>(criteria);

       UpdateView(facilityStockDetailList);
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (this.BackEvent != null)
        {
            this.BackEvent(this, e);
        }
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        string dateTime = DateTime.Now.ToString("ddhhmmss");
        this.ExportXLS(this.GV_List, "FacilityStockDetail" + dateTime + ".xls");
    }


    protected void lbtnConfirm_Click(object sender, EventArgs e)
    {
          string id = ((LinkButton)sender).CommandArgument;
          FacilityStockDetail d = TheFacilityStockDetailMgr.LoadFacilityStockDetail(Convert.ToInt32(id));
          d.Qty = d.InvQty;
          d.DiffQty = 0;
          d.LastModifyDate = DateTime.Now;
          d.LastModifyUser = this.CurrentUser.Code;
          TheFacilityStockDetailMgr.UpdateFacilityStockDetail(d);
          ShowSuccessMessage("Facility.FacilityStock.ConfirmFacilityStockDetailQty.Successfully");  
           InitPageParameter(this.StNo);
    }
}