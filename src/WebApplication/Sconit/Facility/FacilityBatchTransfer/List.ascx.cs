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
using System.IO;
using com.Sconit.Entity;
using com.Sconit.Facility.Entity;
using System.Collections.Generic;
using NHibernate.Expression;
public partial class Facility_FacilityBatchTransfer_List : ListModuleBase
{
    public EventHandler EditEvent;
    public EventHandler TransferEvent;

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void InitPageParameter(object category)
    {

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityMaster));
        selectCriteria.Add(Expression.Like("Category", category)).AddOrder(Order.Asc("FCID"));  
        selectCriteria.Add(Expression.Eq("Status", FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE));

        IList<FacilityMaster> facilityMasterList = TheCriteriaMgr.FindAll<FacilityMaster>(selectCriteria);
        this.GV_List.DataSource = facilityMasterList;
        this.GV_List.DataBind();
    }
    protected void lbtnTransfer_Click(object sender, EventArgs e)
    {
         try
        {
            this.CollectFCIDList();
            ShowSuccessMessage("Facility.FacilityMaster.FacilityMasterBatchTransfer.Successfully");
        }
        catch(Exception ex) 
        {
            ShowErrorMessage("Common.Business.Warn.DetailEmpty");
        }

    }

    private void CollectFCIDList()
    {
        List<string> fcidList = new List<string>();
        foreach (GridViewRow gvr in GV_List.Rows)
        {
            CheckBox cbCheckBoxGroup = (CheckBox)gvr.FindControl("CheckBoxGroup");
            if (cbCheckBoxGroup.Checked)
            {
                string FCID = ((Literal)gvr.FindControl("ltlFCID")).Text;
                FacilityMaster facilityMaster = TheFacilityMasterMgr.LoadFacilityMaster(FCID);
                string transferPerson = ((Controls_TextBox)(this.FindControl("tbTransferPerson"))).Text;
                string transferSite = ((Controls_TextBox)(this.FindControl("tbTransferSite"))).Text;
                string transferOrganization = ((Controls_TextBox)(this.FindControl("tbTransferOrg"))).Text;
                //string transferPerson = ((Literal)gvr.FindControl("ltlFCID")).Text;
                //string transferSite = ((Literal)gvr.FindControl("ltlFCID")).Text;
                //string transferOrganization = ((Literal)gvr.FindControl("ltlFCID")).Text;

                FacilityTrans facilityTrans = new FacilityTrans();
                facilityTrans.CreateDate = DateTime.Now;
                facilityTrans.CreateUser = this.CurrentUser.Code;
                facilityTrans.EffDate = DateTime.Now.Date;
                facilityTrans.FCID = facilityMaster.FCID;
                facilityTrans.FromChargePerson = facilityMaster.CurrChargePerson;
                facilityTrans.FromChargePersonName = facilityMaster.CurrChargePersonName;
                facilityTrans.FromChargeSite = facilityMaster.ChargeSite;
                facilityTrans.FromOrganization = facilityMaster.ChargeOrganization;
                facilityTrans.ToChargePerson = transferPerson;
                if (TheUserMgr.LoadUser(transferPerson) != null)
                {
                    facilityTrans.ToChargePersonName = TheUserMgr.LoadUser(transferPerson).Name;
                }
                facilityTrans.ToChargeSite = transferSite;
                facilityTrans.ToOrganization = transferOrganization;
                facilityTrans.TransType = FacilityConstants.CODE_MASTER_FACILITY_TRANSTYPE_TRANSFER;

                facilityMaster.IsInStore = false;
                facilityMaster.ChargeDate = DateTime.Now;
                facilityMaster.OldChargePerson = facilityMaster.CurrChargePerson;
                facilityMaster.OldChargePersonName = facilityMaster.CurrChargePersonName;
                facilityMaster.CurrChargePerson = transferPerson;
                if (TheUserMgr.LoadUser(transferPerson) != null)
                {
                    facilityMaster.CurrChargePersonName = TheUserMgr.LoadUser(transferPerson).Name;
                }
                else
                {
                    facilityMaster.CurrChargePersonName = string.Empty;
                }
                facilityMaster.ChargeSite = transferSite;
                facilityMaster.ChargeOrganization = transferOrganization;

                TheFacilityMasterMgr.UpdateFacilityMasterAndCreateFacilityTrans(facilityMaster, facilityTrans, FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE, this.CurrentUser.Code);
            }
        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        FacilityMaster facilityMaster = (FacilityMaster)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Label lblStatus = (Label)(e.Row.FindControl("lblStatus"));
            lblStatus.Text = this.TheLanguageMgr.TranslateMessage(facilityMaster.Status, this.CurrentUser);

            Label lblOwner = (Label)(e.Row.FindControl("lblOwner"));
            lblOwner.Text = this.TheLanguageMgr.TranslateMessage(facilityMaster.Owner, this.CurrentUser);

            Label lblCategory = (Label)(e.Row.FindControl("lblCategory"));
            lblCategory.Text = this.TheFacilityCategoryMgr.LoadFacilityCategory(facilityMaster.Category).Description;
        }
    }
}
