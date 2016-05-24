using System;
using System.IO;
using System.Web.UI.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using com.Sconit.Facility.Entity;
using com.Sconit.Entity;
using com.Sconit.Entity.Exception;
using System.Collections.Generic;
using NHibernate.Expression;

public partial class Facility_MouldMaster_Edit : EditModuleBase
{
    public event EventHandler BackEvent;

    protected string FCID
    {
        get
        {
            return (string)ViewState["FCID"];
        }
        set
        {
            ViewState["FCID"] = value;
        }
    }

    protected string AssetNo
    {
        get
        {
            return (string)ViewState["AssetNo"];
        }
        set
        {
            ViewState["AssetNo"] = value;
        }
    }


    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void FV_FacilityMaster_DataBound(object sender, EventArgs e)
    {


        FacilityMaster facilityMaster = (FacilityMaster)(((FormView)(sender)).DataItem);
        if (facilityMaster != null)
        {
            ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbCategory"))).Text = (facilityMaster.Category == null) ? string.Empty : facilityMaster.Category;
            ((CodeMstrDropDownList)(this.FV_FacilityMaster.FindControl("ddlOwner"))).SelectedValue = facilityMaster.Owner;
            ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbMaintainGroup"))).Text = (facilityMaster.MaintainGroup == null) ? string.Empty : facilityMaster.MaintainGroup;
            ((Controls_TextBox)(this.FV_FacilityMaster.FindControl("tbMaintainType"))).Text = (facilityMaster.MaintainType == null) ? string.Empty : facilityMaster.MaintainType;
            UpdateView();
        }
    }

    public void InitPageParameter(string code)
    {
        this.FCID = code;
        this.ODS_FacilityMaster.SelectParameters["fcId"].DefaultValue = this.FCID;

        this.ODS_FacilityMaster.DataBind();
        UpdateView();


    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);
        string template = "Facility2DBarCode.xls";


        //IReportBaseMgr iReportBaseMgr = this.GetIReportBaseMgr(orderTemplate, orderHead);
        //string printUrl = XlsHelper.WriteToFile(iReportBaseMgr.GetWorkbook());
        //orderTemplate = "RequisitionOrderContract.xls";
        string printUrl = TheReportMgr.WriteToFile(template, new List<object> { facilityMaster });
        Page.ClientScript.RegisterStartupScript(GetType(), "method", " <script language='javascript' type='text/javascript'>PrintOrder('" + printUrl + "'); </script>");
    }

    protected void ODS_FacilityMaster_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (CheckAssetNoDup(AssetNo))
        {
            ShowWarningMessage("Facility.FacilityMaster.AddFacilityMaster.DupAssetNo", FCID);
            //ShowSuccessMessage("Facility.FacilityMaster.AddFacilityMaster.Successfully", FCID);
        }
        else
        {
            ShowSuccessMessage("Facility.FacilityMaster.UpdateFacilityMaster.Successfully", FCID);
        }
    }

    protected void ODS_FacilityMaster_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        FacilityMaster facilityMaster = (FacilityMaster)e.InputParameters[0];
        //FacilityCategory facilityCategoty = TheFacilityCategoryMgr.LoadFacilityCategory(facilityMaster.Category);
        //facilityMaster.ChargeOrganization = facilityCategoty.ChargeOrganization;
        //facilityMaster.ChargeSite = facilityCategoty.ChargeSite;
        facilityMaster.Owner = ((CodeMstrDropDownList)(this.FV_FacilityMaster.FindControl("ddlOwner"))).SelectedValue;
        facilityMaster.LastModifyDate = DateTime.Now;
        facilityMaster.LastModifyUser = this.CurrentUser.Code;
        AssetNo = facilityMaster.AssetNo;
        facilityMaster.Status = TheFacilityMasterMgr.LoadFacilityMaster(facilityMaster.FCID).Status;
    }
    protected void ODS_FacilityMaster_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteFacilityMaster = (FacilityMaster)e.InputParameters[0];
    }

    protected void ODS_FacilityMaster_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("Facility.FacilityMaster.DeleteFacilityMaster.Successfully", FCID);
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("Facility.FacilityMaster.DeleteFacilityMaster.Fail", FCID);
            e.ExceptionHandled = true;
        }
    }

    protected void btnAvailable_Click(object sender, EventArgs e)
    {
        try
        {
            FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);
            string effDate = ((TextBox)(this.FV_FacilityMaster.FindControl("tbEffDate"))).Text;
         
                facilityMaster.EffDate = effDate;
           

            FacilityTrans facilityTrans = new FacilityTrans();
            facilityTrans.CreateDate = DateTime.Now;
            facilityTrans.CreateUser = this.CurrentUser.Code;
            facilityTrans.EffDate = DateTime.Now.Date;
            facilityTrans.FCID = facilityMaster.FCID;
            facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
            facilityTrans.ToChargePerson = facilityMaster.CurrChargePerson;
            facilityTrans.ToChargePersonName = facilityMaster.CurrChargePersonName;
            facilityTrans.ToOrganization = facilityMaster.ChargeOrganization;
            facilityTrans.ToChargeSite = facilityMaster.ChargeSite;
            facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_ENABLE;

            TheFacilityMasterMgr.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE, this.CurrentUser.Code);

            InitPageParameter(this.FCID);
            ShowSuccessMessage("Facility.FacilityMaster.ConfirmFacilityMaster.Successfully", this.FCID);
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }


    public void UpdateView()
    {
        FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(this.FCID);
        if (facilityMaster != null)
        {

            string facilityStatus = facilityMaster.Status;
            ((Label)(this.FV_FacilityMaster.FindControl("tbStatus"))).Text = this.TheLanguageMgr.TranslateMessage(facilityStatus, this.CurrentUser);
            //facilityMaster.Status; //不知道为什么状态总更新不了

            #region 根据设施状态显示按钮
            // com.Sconit.Control.Button btnEdit = ((com.Sconit.Control.Button)(this.FV_FacilityMaster.FindControl("btnEdit")));
            com.Sconit.Control.Button btnAvailable = ((com.Sconit.Control.Button)(this.FV_FacilityMaster.FindControl("btnAvailable")));
            com.Sconit.Control.Button btnDelete = ((com.Sconit.Control.Button)(this.FV_FacilityMaster.FindControl("btnDelete")));

            if (facilityStatus == FacilityConstants.CODE_MASTER_FACILITY_STATUS_TEST)
            {
                // btnEdit.Visible = true;
                btnAvailable.Visible = true;
                btnDelete.Visible = true;
            }
            else if (facilityStatus == FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE)
            {
                //  btnEdit.Visible = false;
                btnAvailable.Visible = false;
                btnDelete.Visible = false;
            }
            else if (facilityStatus == FacilityConstants.CODE_MASTER_FACILITY_STATUS_FIX
                || facilityStatus == FacilityConstants.CODE_MASTER_FACILITY_STATUS_ENVELOP
                || facilityStatus == FacilityConstants.CODE_MASTER_FACILITY_STATUS_SCRAP
                || facilityStatus == FacilityConstants.CODE_MASTER_FACILITY_STATUS_MAINTAIN
                || facilityStatus == FacilityConstants.CODE_MASTER_FACILITY_STATUS_INSPECT
                || facilityStatus == FacilityConstants.CODE_MASTER_FACILITY_STATUS_SELL
                || facilityStatus == FacilityConstants.CODE_MASTER_FACILITY_STATUS_LEND
                || facilityStatus == FacilityConstants.CODE_MASTER_FACILITY_STATUS_LOSE)
            {
                // btnEdit.Visible = false;
                btnAvailable.Visible = false;
                btnDelete.Visible = false;
            }

            #endregion

        }
    }

    private bool CheckAssetNoDup(string assetNo)
    {
        bool isDup = false;
        if (!string.IsNullOrEmpty(assetNo))
        {
            DetachedCriteria criteria = DetachedCriteria.For(typeof(FacilityMaster));
            criteria.Add(Expression.Eq("AssetNo", assetNo));
            criteria.Add(Expression.Not(Expression.Eq("FCID", this.FCID)));

            int resultCnt = TheCriteriaMgr.FindAll<FacilityMaster>(criteria).Count;
            if (resultCnt > 0)
            {
                return true;
            }
        }
        return isDup;

    }
}
