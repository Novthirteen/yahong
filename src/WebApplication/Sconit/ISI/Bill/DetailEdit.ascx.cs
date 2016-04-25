using System;
using System.IO;
using System.Web.UI.WebControls;
using com.Sconit.Control;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using com.Sconit.ISI.Entity;
using System.Collections.Generic;
using System.Linq;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity.Util;

public partial class ISI_Bill_DetailEdit : EditModuleBase
{
    public event EventHandler BackEvent;
    public string PSIType
    {
        get
        {
            return (string)ViewState["PSIType"];
        }
        set
        {
            ViewState["PSIType"] = value;
        }
    }
    protected Int32 MouldDetailId
    {
        get
        {
            return (Int32)ViewState["MouldDetailId"];
        }
        set
        {
            ViewState["MouldDetailId"] = value;
        }
    }

    protected string MouldCode
    {
        get
        {
            return (string)ViewState["MouldCode"];
        }
        set
        {
            ViewState["MouldCode"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void FV_BillDetail_DataBound(object sender, EventArgs e)
    {
        MouldDetail mouldDetail = (MouldDetail)(((FormView)(sender)).DataItem);
        ((CodeMstrDropDownList)(this.FV_BillDetail.FindControl("ddlType"))).SelectedValue = mouldDetail.Type;
        ((CodeMstrDropDownList)(this.FV_BillDetail.FindControl("ddlPhase"))).SelectedValue = mouldDetail.Phase;
        Mould fd = TheMouldMgr.LoadMould(this.MouldCode);
        if (fd.Status == ISIConstants.CODE_MASTER_PSI_BILL_STATUS_CLOSE)
        {
            ((com.Sconit.Control.Button)(this.FV_BillDetail.FindControl("btnSave"))).Visible = false;
        }
    }

    public void InitPageParameter(int id, string mouldCode)
    {
        this.MouldDetailId = id;
        this.MouldCode = mouldCode;
        this.ODS_MouldDetail.SelectParameters["Id"].DefaultValue = this.MouldDetailId.ToString();
        this.ODS_MouldDetail.DataBind();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void ODS_MouldDetail_Updated(object sender, ObjectDataSourceStatusEventArgs e)
    {
        ShowSuccessMessage("PSI.MouldDetail.UpdateMouldDetail.Successfully");
    }

    protected void ODS_MouldDetail_Updating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        MouldDetail mouldDetail = (MouldDetail)e.InputParameters[0];
        mouldDetail.Code = this.MouldCode;
        mouldDetail.Type = ((CodeMstrDropDownList)(this.FV_BillDetail.FindControl("ddlType"))).SelectedValue;
        mouldDetail.Phase = ((CodeMstrDropDownList)(this.FV_BillDetail.FindControl("ddlPhase"))).SelectedValue;
        var mould = this.TheMouldMgr.LoadMould(this.MouldCode);
        var oldMouldDetail = this.TheMouldDetailMgr.LoadMouldDetail(mouldDetail.Id);
        mouldDetail.Version = oldMouldDetail.Version;
        mouldDetail.CreateDate = oldMouldDetail.CreateDate;
        mouldDetail.CreateUser = oldMouldDetail.CreateUser;
        mouldDetail.CreateUserNm = oldMouldDetail.CreateUserNm;

        if (mould.Status == ISIConstants.CODE_MASTER_PSI_BILL_STATUS_SOCOMPLETE
           && mouldDetail.Type == ISIConstants.CODE_MASTER_PSIBILLDETAIL_TYPE_SO)
        {
            ShowErrorMessage("PSI.MouldDetail.UpdateMouldDetail.Fail", this.TheLanguageMgr.TranslateMessage("ISI.Status." + mould.Status, this.CurrentUser), this.TheLanguageMgr.TranslateMessage("PSI.MouldDetail.Type." + mouldDetail.Type, this.CurrentUser));
            e.Cancel = true;
        }
        if (mould.Status == ISIConstants.CODE_MASTER_PSI_BILL_STATUS_POCOMPLETE
           && mouldDetail.Type == ISIConstants.CODE_MASTER_PSIBILLDETAIL_TYPE_PO)
        {
            ShowErrorMessage("PSI.MouldDetail.UpdateMouldDetail.Fail", this.TheLanguageMgr.TranslateMessage("ISI.Status." + mould.Status, this.CurrentUser), this.TheLanguageMgr.TranslateMessage("PSI.MouldDetail.Type." + mouldDetail.Type, this.CurrentUser));
            e.Cancel = true;
        }

        mouldDetail.LastModifyDate = DateTime.Now;
        mouldDetail.LastModifyUser = this.CurrentUser.Code;
        mouldDetail.LastModifyUserNm = this.CurrentUser.Name;
    }
    protected void ODS_MouldDetail_Deleting(object sender, ObjectDataSourceMethodEventArgs e)
    {
        //DeleteItem = (Item)e.InputParameters[0];
    }

    protected void ODS_MouldDetail_Deleted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (e.Exception == null)
        {
            btnBack_Click(this, e);
            ShowSuccessMessage("PSI.MouldDetail.DeleteMouldDetail.Successfully");
        }
        else if (e.Exception.InnerException is Castle.Facilities.NHibernateIntegration.DataException)
        {
            ShowErrorMessage("PSI.MouldDetail.DeleteMouldDetail.Failed");
            e.ExceptionHandled = true;
        }
    }
}
