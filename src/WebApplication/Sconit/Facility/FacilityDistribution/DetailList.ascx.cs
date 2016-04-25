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
using NHibernate.Expression;

public partial class Facility_FacilityDistributionDetail_List : ListModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler EditEvent;
    public event EventHandler NewEvent;

    public Int32 FacilityDistributionId
    {
        get
        {
            return (Int32)ViewState["FacilityDistributionId"];
        }
        set
        {
            ViewState["FacilityDistributionId"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
       // this.GV_List.DataBind();
    }

    public override void UpdateView()
    {
        this.GV_List.Execute();

        FacilityDistribution fd = TheFacilityDistributionMgr.LoadFacilityDistribution(this.FacilityDistributionId);
        if (fd.Status != FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CLOSE)
        {
            btnNew.Visible = true;
        }
        else
        {
            btnNew.Visible = false;
        }

    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        FacilityDistribution fd = TheFacilityDistributionMgr.LoadFacilityDistribution(this.FacilityDistributionId);

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FacilityDistributionDetail facilityDistributionDetail = (FacilityDistributionDetail)e.Row.DataItem;
            Label lblType = (Label)(e.Row.FindControl("lblType"));
            lblType.Text = this.TheLanguageMgr.TranslateMessage(facilityDistributionDetail.Type, this.CurrentUser);

            if (fd.Status == FacilityConstants.CODE_MASTER_FACILITY_DISTRIBUTION_STARUS_CLOSE)
            {
                ((com.Sconit.Control.LinkButton)(e.Row.FindControl("lbtnDelete"))).Visible = false;
            }
        }
    }

    public void InitPageParameter(Int32 facilityDistributionId)
    {

        this.FacilityDistributionId = facilityDistributionId;
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityDistributionDetail));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityDistributionDetail)).SetProjection(Projections.Count("Id"));

        selectCriteria.Add(Expression.Eq("FacilityDistribution.Id", facilityDistributionId));
        selectCountCriteria.Add(Expression.Like("FacilityDistribution.Id", facilityDistributionId));

        SetSearchCriteria(selectCriteria, selectCountCriteria);
        UpdateView();

    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (BackEvent != null)
        {
            BackEvent(this, e);
        }
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        NewEvent(sender, e);
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        if (EditEvent != null)
        {
            string id = ((LinkButton)sender).CommandArgument;
            EditEvent(id, e);
        }
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string id = ((LinkButton)sender).CommandArgument;
        try
        {
            TheFacilityDistributionDetailMgr.DeleteFacilityDistributionDetail(Convert.ToInt32(id));
            ShowSuccessMessage("Facility.FacilityDistributionDetail.DeleteFacilityDistributionDetail.Successfully");
            UpdateView();
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Facility.FacilityDistributionDetail.DeleteFacilityDistributionDetail.Fail");
        }

    }
}
