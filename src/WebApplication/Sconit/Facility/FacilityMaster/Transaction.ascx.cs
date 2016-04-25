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

public partial class Facility_FacilityMaster_Trans : ListModuleBase
{
    public event EventHandler BackEvent;
    protected void Page_Load(object sender, EventArgs e)
    {
       // this.GV_List.DataBind();
    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            FacilityTrans facilityTrans = (FacilityTrans)e.Row.DataItem;

            Label lblTransType = (Label)(e.Row.FindControl("lblTransType"));
            lblTransType.Text = this.TheLanguageMgr.TranslateMessage(facilityTrans.TransType, this.CurrentUser);

            int attachmentCount = this.TheFacilityMasterMgr.GetFacilityTransAttachmentCount(facilityTrans.Id.ToString());
            if (attachmentCount > 0)
            {
                LinkButton lbtnAttachment = (LinkButton)(e.Row.FindControl("lbtnAttachment"));
                lbtnAttachment.Text = "${ISI.TSK.Attachment}(<font color='blue'>" + attachmentCount + "</font>)";
                lbtnAttachment.Visible = true;
            }
        }
    }

    public void InitPageParameter(string fcId)
    {


        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityTrans));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(FacilityTrans)).SetProjection(Projections.Count("FCID"));

        selectCriteria.Add(Expression.Eq("FCID", fcId));
        selectCountCriteria.Add(Expression.Like("FCID", fcId));

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

    protected void lbtnAttachment_Click(object sender, EventArgs e)
    {

        string code = ((LinkButton)sender).CommandArgument;
        this.ucTransAttachment.InitPageParameter(code);
        this.ucTransAttachment.Visible = true;
    }
}
