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

public partial class Modules_ISI_ResRole_Main : MainModuleBase
{
    public Modules_ISI_ResRole_Main()
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
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(ResRole));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(ResRole))
        .SetProjection(Projections.ProjectionList()
        .Add(Projections.Count("Code")));
        string code = this.tbCode.Text.Trim();
        if (code != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Code", code, MatchMode.Start));
            selectCountCriteria.Add(Expression.Like("Code", code, MatchMode.Start));
        }
        string name = this.tbName.Text.Trim();
        if (name != string.Empty)
        {
            selectCriteria.Add(Expression.Like("Name", name, MatchMode.Anywhere));
            selectCountCriteria.Add(Expression.Like("Name", name, MatchMode.Anywhere));
        }
        string roletype = this.ddlRoleType.SelectedValue;
        if (roletype != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("RoleType", roletype));
            selectCountCriteria.Add(Expression.Eq("RoleType", roletype));
        }
        selectCriteria.Add(Expression.Eq("IsActive", this.cbIsActive.Checked));
        selectCountCriteria.Add(Expression.Eq("IsActive", this.cbIsActive.Checked));
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
            //TheResRoleMgr.DeleteResRole(code_);
            ShowSuccessMessage("ISI.ResRole.DeleteResRole.Successfully");
            UpdateView();
        }
        catch (Castle.Facilities.NHibernateIntegration.DataException ex)
        {
            ShowErrorMessage("ISI.ResRole.DeleteResRole.Failed");
        }
    }
}
