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
using com.Sconit.Entity;
using com.Sconit.Service.MasterData;
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
using com.Sconit.Facility.Entity;
using Geekees.Common.Controls;

public partial class Inventory_Stocktaking_Edit : EditModuleBase
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

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void InitPageParameter(string code)
    {
        this.StNo = code;
        FacilityStockMaster facilityStockMaster = TheFacilityStockMasterMgr.LoadFacilityStockMaster(code);
        InitPageParameter(facilityStockMaster);
        UpdateView();
    }

    public void InitPageParameter(FacilityStockMaster facilityStockMaster)
    {
        this.tbStNo.Text = facilityStockMaster.StNo;
        this.tbEffDate.Text = facilityStockMaster.EffDate.ToString("yyyy-MM-dd");
        this.tbChargeOrganization.Text = facilityStockMaster.ChargeOrg;
        this.tbChargePerson.Text = facilityStockMaster.ChargePersonName;
        this.tbChargeSite.Text = facilityStockMaster.ChargeSite;
        this.tbCategory.Text = facilityStockMaster.FacilityCategory;
        this.tbStatus.Text = this.TheLanguageMgr.TranslateMessage(facilityStockMaster.Status, this.CurrentUser);

        #region 显示按钮
        if (facilityStockMaster.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE)
        {
            this.btnSubmit.Visible = true;
            this.btnStart.Visible = false;
            this.btnComplete.Visible = false;
            this.btnClose.Visible = false;
        }
        if (facilityStockMaster.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT)
        {
            this.btnSubmit.Visible = false;
            this.btnStart.Visible = true;
            this.btnComplete.Visible = false;
            this.btnClose.Visible = false;
        }
        if (facilityStockMaster.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS)
        {
            this.btnSubmit.Visible = false;
            this.btnStart.Visible = false;
            this.btnComplete.Visible = true;
            this.btnClose.Visible = false;
        }
        if (facilityStockMaster.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE)
        {
            this.btnSubmit.Visible = false;
            this.btnStart.Visible = false;
            this.btnComplete.Visible = false;
            this.btnClose.Visible = true;
        }
        #endregion
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            FacilityStockMaster facilityStockMaster = TheFacilityStockMasterMgr.LoadFacilityStockMaster(this.StNo);
            facilityStockMaster.ReleaseDate = DateTime.Now;
            facilityStockMaster.ReleaseUser = this.CurrentUser.Code;
            facilityStockMaster.LastModifyDate = DateTime.Now;
            facilityStockMaster.LastModifyUser = this.CurrentUser.Code;
            facilityStockMaster.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT;
            TheFacilityStockMasterMgr.UpdateFacilityStockMaster(facilityStockMaster);
            ShowSuccessMessage("Facility.FacilityStock.SubmitFacilityStockMaster.Successfully", this.StNo);
            InitPageParameter(this.StNo);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnStart_Click(object sender, EventArgs e)
    {
        try
        {
            FacilityStockMaster facilityStockMaster = TheFacilityStockMasterMgr.LoadFacilityStockMaster(this.StNo);
            facilityStockMaster.StartDate = DateTime.Now;
            facilityStockMaster.StartUser = this.CurrentUser.Code;
            facilityStockMaster.LastModifyDate = DateTime.Now;
            facilityStockMaster.LastModifyUser = this.CurrentUser.Code;
            facilityStockMaster.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS;
            TheFacilityStockMasterMgr.UpdateFacilityStockMaster(facilityStockMaster);
            ShowSuccessMessage("Facility.FacilityStock.StartFacilityStockMaster.Successfully", this.StNo);
            InitPageParameter(this.StNo);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }


    protected void btnComplete_Click(object sender, EventArgs e)
    {
        try
        {
            FacilityStockMaster facilityStockMaster = TheFacilityStockMasterMgr.LoadFacilityStockMaster(this.StNo);
            facilityStockMaster.CompleteDate = DateTime.Now;
            facilityStockMaster.CompleteUser = this.CurrentUser.Code;
            facilityStockMaster.LastModifyDate = DateTime.Now;
            facilityStockMaster.LastModifyUser = this.CurrentUser.Code;
            facilityStockMaster.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_COMPLETE;
            TheFacilityStockMasterMgr.UpdateFacilityStockMaster(facilityStockMaster);
            ShowSuccessMessage("Facility.FacilityStock.CompleteFacilityStockMaster.Successfully", this.StNo);
            InitPageParameter(this.StNo);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }


    protected void btnClose_Click(object sender, EventArgs e)
    {
        try
        {
            FacilityStockMaster facilityStockMaster = TheFacilityStockMasterMgr.LoadFacilityStockMaster(this.StNo);
            facilityStockMaster.CloseDate = DateTime.Now;
            facilityStockMaster.CloseUser = this.CurrentUser.Code;
            facilityStockMaster.LastModifyDate = DateTime.Now;
            facilityStockMaster.LastModifyUser = this.CurrentUser.Code;
            facilityStockMaster.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;
            TheFacilityStockMasterMgr.UpdateFacilityStockMaster(facilityStockMaster);
            ShowSuccessMessage("Facility.FacilityStock.CloseFacilityStockMaster.Successfully", this.StNo);
            InitPageParameter(this.StNo);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        FacilityStockMaster facilityStockMaster = TheFacilityStockMasterMgr.LoadFacilityStockMaster(this.StNo);
        IList<FacilityStockDetail> facilityStockDetails = TheFacilityStockDetailMgr.LoadFacilityStockDetails(this.StNo);
        IList<object> list = new List<object>();
        list.Add(facilityStockMaster);
        list.Add(facilityStockDetails);
        //FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.StNo);
        string template = "FacilityStocktake.xls";
        string printUrl = TheReportMgr.WriteToFile(template, list);
        Page.ClientScript.RegisterStartupScript(GetType(), "method", " <script language='javascript' type='text/javascript'>PrintOrder('" + printUrl + "'); </script>");
    }


    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (this.BackEvent != null)
        {
            this.BackEvent(this, e);
        }
    }
    public void UpdateView()
    {

    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
           
            TheFacilityStockMasterMgr.DeleteFacilityStockMaster(this.StNo);
            ShowSuccessMessage("Facility.FacilityStock.DeleteFacilityStockMaster.Successfully", this.StNo);
            BackEvent(sender, e);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }
}
