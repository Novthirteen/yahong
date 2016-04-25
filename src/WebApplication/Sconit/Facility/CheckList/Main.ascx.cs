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

public partial class Facility_CheckList_Main : MainModuleBase
{
    public Facility_CheckList_Main()
    { }

    private static ILog log = LogManager.GetLogger("ISI");

    protected void Page_Load(object sender, EventArgs e)
    {
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
            e.Row.Cells[6].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
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
        DoSearch();
    }

    private void DoSearch()
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(CheckListMaster));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(CheckListMaster))
        .SetProjection(Projections.ProjectionList()
        .Add(Projections.Count("Code")));
        string code = this.tbCode.Text.Trim();
        if (code != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Code", code,MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Code", code));
        }
        string name = this.tbName.Text.Trim();
        if (name != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Name", name, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Name", name, MatchMode.Anywhere));
        }

        string toolNumber = this.tbToolNumber.Text.Trim();
        if (toolNumber != string.Empty)
        {
            selectCriteria.Add(Expression.Like("FacilityID", toolNumber, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("FacilityID", toolNumber, MatchMode.Anywhere));
        }

        string region = this.tbRegion.Text.Trim();
        if (region != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Region", toolNumber));
            selectCountCriteria.Add(Expression.Eq("Region", toolNumber));
        }

      
        selectCriteria.AddOrder(Order.Desc("Code"));
        this.SetSearchCriteria(selectCriteria, selectCountCriteria);
        this.fldList.Visible = true;
        this.UpdateView();
        this.ucEdit.Visible = false;
        this.ucNew.Visible = false;
        this.ucEdit.Visible = false;
       
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        this.fldSearch.Visible = false;
        this.fldList.Visible = false;
        this.ucNew.Visible = true;
        this.ucEdit.Visible = false;
        //this.ucNew.PageCleanup();
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        string code_ = ((LinkButton)sender).CommandArgument;

        this.fldSearch.Visible = false;
        this.fldList.Visible = false;
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter(code_);
    }

    protected void lbtnView_Click(object sender, EventArgs e)
    {
        string code_ = ((LinkButton)sender).CommandArgument;

        this.fldSearch.Visible = false;
        this.fldList.Visible = false;
        this.ucEdit.Visible = false;
    }

    protected void lbtnTrace_Click(object sender, EventArgs e)
    {
        string code_ = ((LinkButton)sender).CommandArgument;

        this.fldSearch.Visible = false;
        this.fldList.Visible = false;
        this.ucEdit.Visible = false;
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code_ = ((LinkButton)sender).CommandArgument;
        try
        {
            //TheResMatrixMgr.DeleteResMatrix(code_);
            ShowSuccessMessage("ISI.ResMatrix.DeleteResMatrix.Successfully");
            UpdateView();
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.ResMatrix.DeleteResMatrix.Failed");
        }
    }
}
