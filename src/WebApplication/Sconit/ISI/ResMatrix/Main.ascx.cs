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

//TODO: Add other using statements here.liqiuyun

public partial class Modules_ISI_ResMatrix_Main : MainModuleBase
{
    public Modules_ISI_ResMatrix_Main()
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
            e.Row.Cells[4].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");

            ResMatrix resMatrix = (ResMatrix)(e.Row.DataItem);
            if (!string.IsNullOrEmpty(resMatrix.WorkShop))
            {
                e.Row.Cells[1].Text = this.TheResWokShopMgr.LoadResWokShop(resMatrix.WorkShop).CodeName;
            }
            if (resMatrix.Operate != null)
            {
                var sop = this.TheResSopMgr.LoadResSop(resMatrix.WorkShop, resMatrix.Operate.Value);
                if (sop != null)
                {
                    e.Row.Cells[2].Text = sop.Operate + "[" + sop.OperateDesc + "]";
                }
            }

            if (!string.IsNullOrEmpty(resMatrix.Role))
            {
                e.Row.Cells[3].Text = this.TheResRoleMgr.LoadResRole(resMatrix.Role).CodeName;
            }
            //e.Row.Cells[4].Style.Add("min-width", "45px");
            //e.Row.Cells[5].Style.Add("min-width", "55px");
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
        RadioButtonList rblType = this.rblType;
        this.fs.Visible = false;
        if (rblType.SelectedIndex == 0)
        {
            this.single.Visible = true;
            this.batch.Visible = false;

            this.fldSearch.Visible = true;
            this.fldList.Visible = true;
            this.UpdateView();
        }
        else
        {
            this.single.Visible = false;
            this.batch.Visible = true;
            if (!string.IsNullOrEmpty(this.tbDirector.Text.Trim()))
            {
                btnBatchSearch_Click(this.btnBatchSearch, e);
            }
        }
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
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(ResMatrix));
        DetachedCriteria selectCountCriteria = DetachedCriteria.For(typeof(ResMatrix))
        .SetProjection(Projections.ProjectionList()
        .Add(Projections.Count("Id")));
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
            bool r = int.TryParse(operate, out op);
            if (r)
            {
                selectCriteria.Add(Expression.Eq("Operate", op));
                selectCountCriteria.Add(Expression.Eq("Operate", op));
            }
            else
            {
                selectCriteria.Add(Expression.IsNotNull("Operate"));
                selectCountCriteria.Add(Expression.IsNotNull("Operate"));
            }
        }
        string role = this.tbRole.Text.Trim();
        if (role != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Role", role));
            selectCountCriteria.Add(Expression.Eq("Role", role));
        }
        string responsibility = this.tbResponsibility.Text.Trim();
        if (responsibility != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Responsibility", responsibility));
            selectCountCriteria.Add(Expression.Eq("Responsibility", responsibility));
        }
        selectCriteria.AddOrder(Order.Asc("WorkShop"));
        selectCriteria.AddOrder(Order.Asc("Operate"));
        selectCriteria.AddOrder(Order.Asc("Seq"));
        selectCriteria.AddOrder(Order.Asc("Responsibility"));
        this.SetSearchCriteria(selectCriteria, selectCountCriteria);
        this.fldList.Visible = true;
        this.UpdateView();
        this.ucEdit.Visible = false;

        //TODO: Add your code to do data query and binding here.
    }

    protected void btnRun_Click(object sender, EventArgs e)
    {
        //TheResMatrixMgr.CreateTask(this.CurrentUser);
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
        this.batch.Visible = false;
        this.ucEdit.Visible = true;
        this.ucEdit.InitPageParameter(code_);
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


    protected void btnBatchSearch_Click(object sender, EventArgs e)
    {
        string resUser = this.tbDirector.Text.Trim();
        if (string.IsNullOrEmpty(resUser))
        {
            this.fs.Visible = false;
            ShowErrorMessage("ISI.ResMatrix.Director.NotEmpty");
            return;
        }

        this.lblEndDateInfo.Text = string.Format("批量设置人员{0}职责的结束时间", resUser);
        this.lblNewUserInfo.Text = string.Format("将人员{0}的职责克隆到新人员上", resUser);

        string sql = string.Empty;
        SqlParameter[] sqlParam = new SqlParameter[2];
        sqlParam[0] = new SqlParameter("@UserCode", resUser);
        sqlParam[1] = new SqlParameter("@HistoryDate", null);

        DataSet dataSetInv = TheSqlHelperMgr.GetDatasetByStoredProcedure("usp_Query_Res_GetResponsibility", sqlParam);
        List<Res> responsibilityList = IListHelper.DataTableToList<Res>(dataSetInv.Tables[0])
            .GroupBy(p => p.ResMatrixId).Select(p => p.First()).ToList();
        this.Gv_Batch.DataSource = responsibilityList;
        this.Gv_Batch.DataBind();
        this.fs.Visible = true;
        if ((Button)sender == this.btnExport)
        {
            this.Export(this.GV_List, "application/ms-excel", DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss") + ".xls");
        }
    }

    protected void GV_Batch_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        Res responsibility = (Res)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[4].Style.Add("style", "word-break:break-all;word-wrap:break-word;white-space: pre-wrap;");
            if (!string.IsNullOrEmpty(responsibility.WorkShopName))
            {
                e.Row.Cells[1].Text = responsibility.WorkShop + "[" + responsibility.WorkShopName + "]";
            }
            if (responsibility.Operate != null)
            {
                e.Row.Cells[2].Text = responsibility.Operate + "[" + responsibility.OperateDesc + "]";
            }
            if (!string.IsNullOrEmpty(responsibility.RoleName))
            {
                e.Row.Cells[3].Text = responsibility.Role + "[" + responsibility.RoleName + "]";
            }
        }
        else
        {
            //e.Row.Cells[4].Style.Add("min-width", "45px");
            //e.Row.Cells[5].Style.Add("min-width", "30px");
        }
    }

    protected void rblType_SelectedIndexChanged(object sender, EventArgs e)
    {
        EditBack_Render(sender, e);
        return;
    }

    protected void btnBatchSet_Click(object sender, EventArgs e)
    {
        try
        {
            var endDate = DateTime.Parse(this.tbEndDate.Text.Trim());
            TheResUserMgr.BatchUpdateEndDate(this.tbDirector.Text.Trim(), endDate, this.CurrentUser.Code);
            ShowSuccessMessage("ISI.ResMatrix.BatchUpdate.Successfully");
            btnBatchSearch_Click(sender, e);
        }
        catch (Exception)
        {
            ShowErrorMessage("ISI.ResMatrix.BatchUpdate.Failed");
        }
    }

    protected void btnCloneUser_Click(object sender, EventArgs e)
    {
        try
        {
            TheResUserMgr.CloneUser(this.tbDirector.Text.Trim(), this.tbNewUser.Text.Trim(), this.CurrentUser.Code);
            ShowSuccessMessage("ISI.ResMatrix.CloneUser.Successfully");
        }
        catch (Exception)
        {
            ShowErrorMessage("ISI.ResMatrix.CloneUser.Failed");
        }
    }

    class Res
    {
        public Int32 ResMatrixId { get; set; }
        public string WorkShop { get; set; }
        public string WorkShopName { get; set; }
        public int? Operate { get; set; }
        public string OperateDesc { get; set; }
        public string Role { get; set; }
        public string RoleName { get; set; }
        public string Priority { get; set; }
        public string SkillLevel { get; set; }
        public int SopId { get; set; }
        public string AttachmentIds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Responsibility { get; set; }
    }
}
