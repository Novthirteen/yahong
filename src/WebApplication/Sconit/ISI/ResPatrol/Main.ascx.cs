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

public partial class Modules_ISI_ResPatrol_Main : MainModuleBase
{
    public Modules_ISI_ResPatrol_Main()
    { }
    //Get the logger
    private static ILog log = LogManager.GetLogger("ISI");

    protected void Page_Load(object sender, EventArgs e)
    {
        //if (this.CurrentUser.IsSupper)
        //{
        //    this.btnRun.Visible = true;
        //}
        //else
        //{
        //    this.btnRun.Visible = false;
        //}
        this.btnRun.Visible = false;
    }

    protected void btnRun_Click(object sender, EventArgs e)
    {
        DateTime dt = DateTimeHelper.GetWeekStart(DateTime.Now).AddHours(38);
        dt = DateTime.Parse("2014-07-01 12:00");
        TheResMatrixMgr.CreateTask(TheUserMgr.LoadUser("monitor"), dt);
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
            e.Row.Cells[1].Text = this.TheResWokShopMgr.LoadResWokShop(e.Row.Cells[1].Text).CodeName;
            ResPatrol resPatrol = (ResPatrol)(e.Row.DataItem);
            if (!string.IsNullOrEmpty(resPatrol.Role))
            {
                e.Row.Cells[2].Text = this.TheResRoleMgr.LoadResRole(resPatrol.Role).CodeName;
            }
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
        if (sender.ToString() == "0")
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
            this.ucEdit.InitPageParameter(sender);
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

    //The event handler when user button "Search".
    protected void btnSearch_Click(object sender, EventArgs e)
    {
        DoSearch();
        //TODO: Add other event handler code here.
    }

    //Do data query and binding.
    private void DoSearch()
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(ResPatrol));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(ResPatrol))
        .SetProjection(Projections.ProjectionList()
        .Add(Projections.Count("Id")));
        string workshop = this.tbWorkShop.Text.Trim();
        if (workshop != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("WorkShop", workshop));
            selectCountCriteria.Add(Expression.Eq("WorkShop", workshop));
        }
        string role = this.tbRole.Text.Trim();
        if (role != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Role", role));
            selectCountCriteria.Add(Expression.Eq("Role", role));
        }
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
            //TheResPatrolMgr.DeleteResPatrol(code_);
            ShowSuccessMessage("ISI.ResPatrol.DeleteResPatrol.Successfully");
            UpdateView();
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.ResPatrol.DeleteResPatrol.Failed");
        }
    }
}
