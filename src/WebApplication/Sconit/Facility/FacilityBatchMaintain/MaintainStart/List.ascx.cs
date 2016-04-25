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
using com.Sconit.Utility;
using com.Sconit.Entity;
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;
using NHibernate.Expression;
using com.Sconit.Facility.Entity;

public partial class Facility_FacilityMaintain_StartList : ModuleBase
{
    public EventHandler StartEvent;



    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void InitPageParameter(string maintainGroup)
    {

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(FacilityMaster));
        selectCriteria.Add(Expression.Eq("MaintainGroup", maintainGroup)).AddOrder(Order.Asc("FCID"));
        selectCriteria.Add(Expression.Eq("Status", FacilityConstants.CODE_MASTER_FACILITY_STATUS_AVAILABLE));
        IList<FacilityMaster> facilityMasterList = TheCriteriaMgr.FindAll<FacilityMaster>(selectCriteria);
        this.GV_List.DataSource = facilityMasterList;
        this.GV_List.DataBind();
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {

    }

    protected void btnMaintainStart_Click(object sender, EventArgs e)
    {
        if (StartEvent != null)
        {
            List<string> fcidList = this.CollectFCIDList();
            if (fcidList.Count > 0)
            {

                TheFacilityMasterMgr.BatchMaintainStart(fcidList, this.CurrentUser);
                ShowSuccessMessage("Facility.FacilityMaster.FacilityMasterBatchMaintainStart.Successfully");

                StartEvent(sender, e);
            }
            else
            {
                ShowWarningMessage("Common.Business.Warn.DetailEmpty");
            }
        }
    }

    private List<string> CollectFCIDList()
    {
        List<string> fcidList = new List<string>();
        foreach (GridViewRow gvr in GV_List.Rows)
        {
            CheckBox cbCheckBoxGroup = (CheckBox)gvr.FindControl("CheckBoxGroup");
            if (cbCheckBoxGroup.Checked)
            {
                string orderNo = ((Literal)gvr.FindControl("ltlFCID")).Text;
                fcidList.Add(orderNo);
            }
        }
        return fcidList;
    }
}
