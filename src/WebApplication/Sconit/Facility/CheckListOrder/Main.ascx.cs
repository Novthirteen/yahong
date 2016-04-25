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

public partial class Facility_CheckListOrder_Main : MainModuleBase
{
    public Facility_CheckListOrder_Main()
    { }

    private static ILog log = LogManager.GetLogger("ISI");

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
            CheckListOrderMaster checkListOrderMaster = (CheckListOrderMaster)e.Row.DataItem;
            e.Row.Cells[9].Text = TheLanguageMgr.TranslateContent(checkListOrderMaster.Status, this.CurrentUser.UserLanguage);

            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                e.Row.Cells[5].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
                e.Row.Cells[6].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
            }
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
            this.GV_List.ExportXLS("CheckListOrder" + dateTime + ".xls");
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

        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(CheckListOrderMaster));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(CheckListOrderMaster))
        .SetProjection(Projections.ProjectionList()
        .Add(Projections.Count("Code")));
        string code = this.tbCode.Text.Trim();
        if (code != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Code", code, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Code", code));
        }
        string name = this.tbName.Text.Trim();
        if (name != string.Empty)
        {
            selectCriteria.Add(Expression.Like("CheckListName", name, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("CheckListName", name, MatchMode.Anywhere));
        }
        if (this.tbStartDate .Text .Trim()!= string.Empty)
        {
            selectCriteria.Add(Expression.Ge("CheckDate", DateTime.Parse(this.tbStartDate.Text.Trim())));
            selectCountCriteria.Add(Expression.Ge("CheckDate", DateTime.Parse(this.tbStartDate.Text.Trim())));
        }
        if (this.tbEndDate.Text.Trim() != string.Empty)
        {
            selectCriteria.Add(Expression.Lt("CheckDate", DateTime.Parse(this.tbEndDate.Text.Trim())));
            selectCountCriteria.Add(Expression.Lt("CheckDate", DateTime.Parse(this.tbEndDate.Text.Trim())));
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
            selectCriteria.Add(Expression.Eq("Region", region));
            selectCountCriteria.Add(Expression.Eq("Region", region));
        }
        string status = this.ddlStatus.SelectedValue;
        if (status != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Status", status));
            selectCountCriteria.Add(Expression.Eq("Status", status));
        }
        selectCriteria.AddOrder(Order.Desc("Code"));

        return new object[] { selectCriteria, selectCountCriteria, isExport, true };
    }


    protected void DoSearch()
    {
        object criteriaParam = this.CollectParam(false);

        this.SetSearchCriteria((DetachedCriteria)((object[])criteriaParam)[0], (DetachedCriteria)((object[])criteriaParam)[1]);
        this.fldList.Visible = true;
        this.UpdateView();
        this.ucEdit.Visible = false;
        this.ucNew.Visible = false;

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
