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

//TODO: Add other using statements here.liqiuyun

public partial class Modules_ISI_ResSop_Main : MainModuleBase
{
    public Modules_ISI_ResSop_Main()
    { }
    //Get the logger
    private static ILog log = LogManager.GetLogger("ISI");

    protected void Page_Load(object sender, EventArgs e)
    {
        //TODO: Add code for Page_Load here.
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
            ResSop resSop = (ResSop)(e.Row.DataItem);
            if (!string.IsNullOrEmpty(resSop.WorkShop))
            {
                e.Row.Cells[1].Text = this.TheResWokShopMgr.LoadResWokShop(resSop.WorkShop).CodeName;
            }
            e.Row.Cells[3].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
        }
    }

    //The event handler when user click button "Back" button of ucNew
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
        this.fldSearch.Visible = true;
        this.fldList.Visible = true;
        this.UpdateView();
        this.ucEdit.Visible = false;
        this.ucNew.Visible = false;
    }

    //The event handler when user click button "Back" button of ucEdit
    void EditBack_Render(object sender, EventArgs e)
    {
        this.fldSearch.Visible = true;
        this.fldList.Visible = true;
        this.UpdateView();
        this.ucEdit.Visible = false;
    }

    //The event handler when user button "Search".
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
        //TODO: Add other event handler code here.
    }

    //Do data query and binding.
    private void DoSearch()
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(ResSop));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(ResSop))
        .SetProjection(Projections.ProjectionList()
        .Add(Projections.Count("WorkShop")));
        string workshop = this.tbWorkShop.Text.Trim();
        if (workshop != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("WorkShop", workshop));
            selectCountCriteria.Add(Expression.Eq("WorkShop", workshop));
        }
        string operate = this.tbOperate.Text.Trim();
        if (operate != string.Empty)
        {
            int op = 0;
            int.TryParse(operate, out op);
            selectCriteria.Add(Expression.Eq("Operate", op));
            selectCountCriteria.Add(Expression.Eq("Operate", op));
        }
        selectCriteria.AddOrder(Order.Asc("WorkShop"));
        selectCriteria.AddOrder(Order.Asc("Operate"));
        this.SetSearchCriteria(selectCriteria, selectCountCriteria);
        this.fldList.Visible = true;
        this.UpdateView();
        this.ucEdit.Visible = false;
        //TODO: Add your code to do data query and binding here.
    }

    protected void btnNew_Click(object sender, EventArgs e)
    {
        this.fldSearch.Visible = false;
        this.fldList.Visible = false;
        this.ucNew.Visible = true;
        this.ucNew.PageCleanup();
        //TODO: Add othere code here.
    }

    protected void lbtnEdit_Click(object sender, EventArgs e)
    {
        string code_ = ((LinkButton)sender).CommandArgument;

        this.fldSearch.Visible = false;
        this.fldList.Visible = false;
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter(code_);
    }

    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string code_ = ((LinkButton)sender).CommandArgument;
        try
        {
            //TheResSopMgr.DeleteResSop(code_);
            ShowSuccessMessage("ISI.ResSop.DeleteResSop.Successfully");
            UpdateView();
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.ResSop.DeleteResSop.Failed");
        }
    }
}
