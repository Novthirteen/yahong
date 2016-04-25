using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;
using NHibernate.Expression;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Facility.Entity;
using com.Sconit.Entity;
using com.Sconit.Entity.MasterData;

public partial class Facility_RepairOrder_Main : MainModuleBase
{
    public Facility_RepairOrder_Main()
    { }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            DateTime now = DateTime.Now;
            this.tbStartDate.Text = now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");
            this.tbEndDate.Text = now.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        this.ucNew.Back += new System.EventHandler(this.NewBack_Render);
        this.ucNew.Create += new System.EventHandler(this.CreateBack_Render);
        this.ucEdit.Back += new System.EventHandler(this.EditBack_Render);
        //TODO: Add other init code here.
    }

    public override void UpdateView()
    {
        this.GV_List.Execute();
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            RepairOrder checkListOrderMaster = (RepairOrder)e.Row.DataItem;
            e.Row.Cells[9].Text = TheLanguageMgr.TranslateContent(checkListOrderMaster.Status, this.CurrentUser.UserLanguage);
        }
    }

    void NewBack_Render(object sender, EventArgs e)
    {
        this.fldSearch.Visible = true;
        this.fldList.Visible = true;
        this.UpdateView();
        this.ucNew.Visible = false;
    }

    //The event handler when user click button "Save" button of ucNew
    void CreateBack_Render(object sender, EventArgs e)
    {
        if (sender.ToString() == string.Empty)
        {
            this.fldSearch.Visible = true;
            this.fldList.Visible = true;
            this.UpdateView();
            this.ucEdit.Visible = false;
            this.ucNew.Visible = false;
        }
        else
        {
            this.fldSearch.Visible = false;
            this.fldList.Visible = false;
            this.ucNew.Visible = false;
            this.ucEdit.Visible = true;
            this.ucEdit.InitPageParameter(sender.ToString());
        }
    }

    //The event handler when user click button "Back" button of ucEdit
    void EditBack_Render(object sender, EventArgs e)
    {
        this.fldSearch.Visible = true;
        this.fldList.Visible = true;
        this.UpdateView();
        this.ucEdit.Visible = false;
    }

    void ViewBack_Render(object sender, EventArgs e)
    {

        this.fldSearch.Visible = true;
        this.fldList.Visible = true;
        this.UpdateView();
        this.ucEdit.Visible = false;
    }
    void TraceBack_Render(object sender, EventArgs e)
    {

        this.fldSearch.Visible = true;
        this.fldList.Visible = true;
        this.UpdateView();
        this.ucEdit.Visible = false;
    }


    protected void btnSearch_Click(object sender, EventArgs e)
    {
        Button btn = (Button)sender;

        if (btn == this.btnExport)
        {
            object criteriaParam = this.CollectParam(true);
            string dateTime = DateTime.Now.ToString("ddhhmmss");
            this.GV_List.ExportXLS("RepairOrder" + dateTime + ".xls");
        }
        else
        {
            object criteriaParam = this.CollectParam(false);
            this.SetSearchCriteria((DetachedCriteria)((object[])criteriaParam)[0], (DetachedCriteria)((object[])criteriaParam)[1]);
            this.fldList.Visible = true;
            this.UpdateView();
            this.ucEdit.Visible = false;
            this.ucNew.Visible = false;
        }

    }

    private object CollectParam(bool isExport)
    {

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(RepairOrder));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(RepairOrder))
        .SetProjection(Projections.ProjectionList()
        .Add(Projections.Count("OrderNo")));
        string OrderNo = this.tbOrderNo.Text.Trim();
        if (OrderNo != string.Empty)
        {
            selectCriteria.Add(Expression.Like("OrderNo", OrderNo, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("OrderNo", OrderNo));
        }
        string FCID = this.tbFCID.Text.Trim();
        if (FCID != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FCID", FCID, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FCID", FCID, MatchMode.Anywhere));
        }
        if (this.tbStartDate .Text .Trim()!= string.Empty)
        {
            selectCriteria.Add(Expression.Ge("SubmitTime", DateTime.Parse(this.tbStartDate.Text.Trim())));
            selectCountCriteria.Add(Expression.Ge("SubmitTime", DateTime.Parse(this.tbStartDate.Text.Trim())));
        }
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Lt("SubmitTime", DateTime.Parse(this.tbEndDate.Text.Trim())));
            selectCountCriteria.Add(Expression.Lt("SubmitTime", DateTime.Parse(this.tbEndDate.Text.Trim())));
        }

        string AssetNo = this.tbAssetNo.Text.Trim();
        if (AssetNo != string.Empty)
        {
            selectCriteria.Add(Expression.Like("AssetNo", AssetNo, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("AssetNo", AssetNo, MatchMode.Anywhere));
        }

        string FCName = this.tbFCName.Text.Trim();
        if (FCName != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FCName", FCName,MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FCName", FCName,MatchMode.Anywhere));
        }
        string status = this.ddlStatus.SelectedValue;
        if (status != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Status", status));
            selectCountCriteria.Add(Expression.Eq("Status", status));
        }
        selectCriteria.AddOrder(Order.Desc("OrderNo"));

        return new object[] { selectCriteria, selectCountCriteria, isExport, true };
    }


    protected void btnNew_Click(object sender, EventArgs e)
    {
        this.fldSearch.Visible = false;
        this.fldList.Visible = false;
        this.ucNew.Visible = true;
        this.ucEdit.Visible = false;
        this.ucNew.PageCleanup();
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        string code_ = ((LinkButton)sender).CommandArgument;

        this.fldSearch.Visible = false;
        this.fldList.Visible = false;
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter(code_);
    }



}
