using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.ISI.Entity;
using NHibernate.Expression;
using com.Sconit.Control;
using com.Sconit.ISI.Service.Util;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Entity.Exception;
using com.Sconit.Utility;
public partial class ISI_Evaluation_Main : com.Sconit.Web.MainModuleBase
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            doSearch();
        }
    }

    private void PageCleanup()
    {

    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            Evaluation evaluation = (Evaluation)e.Row.DataItem;
            if (evaluation.IsSeparator)
            {
                for (int i = 0; i < e.Row.Cells.Count - 1; i++)
                {
                    TableCell cell = e.Row.Cells[i];
                    cell.Text = string.Empty;
                }

                e.Row.FindControl("lbtnDelete").Visible = false;
                e.Row.Height = 39;
            }
            else
            {
                e.Row.FindControl("lbtnDelete").Visible = !evaluation.IsBlankDetail;
                Controls_TextBox tbUserCode = (Controls_TextBox)e.Row.FindControl("tbUserCode");
                tbUserCode.SuggestTextBox.Attributes.Add("onchange", "GenerateUserName(this);");

                if (evaluation.IsBlankDetail)
                {
                    e.Row.Cells[7].Text = string.Empty;
                    e.Row.Cells[9].Text = string.Empty;
                }
                else
                {
                    //e.Row.Cells[0].Text = evaluation.UserCode;
                    //((Label)e.Row.FindControl("lblUserName")).Text = evaluation.UserName;
                    tbUserCode.Text = evaluation.UserCode;
                    //((TextBox)e.Row.FindControl("tbStandardQty")).Text = evaluation.StandardQty.ToString();
                    //((TextBox)e.Row.FindControl("tbAmount")).Text = evaluation.Amount.ToString("0.########");
                }
            }
        }
    }


    protected void lbtnDelete_Click(object sender, EventArgs e)
    {
        string userCode = ((System.Web.UI.WebControls.LinkButton)sender).CommandArgument;
        try
        {
            this.TheEvaluationMgr.DeleteEvaluation(userCode);
            this.ShowSuccessMessage("ISI.Evaluation.DeleteEvaluation.Successfully", userCode);
            this.doSearch();
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        doSearch();
    }

    private void doSearch()
    {
        var evaluationList = this.TheEvaluationMgr.GetAllEvaluation();
        if (evaluationList == null)
        {
            evaluationList = new List<Evaluation>();
        }
        Evaluation evaluation = new Evaluation();
        evaluation.IsBlankDetail = true;
        evaluation.IsCheckup = true;
        evaluation.IsActive = true;
        evaluation.StandardQty = 10;
        evaluation.Amount = 50;
        if (evaluationList.Count != 0)
        {
            Evaluation blank = new Evaluation();
            blank.IsSeparator = true;
            evaluationList.Add(blank);
        }
        for (int i = 0; i < 10; i++)
        {
            evaluationList.Add(evaluation);
        }

        this.GV_List.DataSource = evaluationList;
        this.GV_List.DataBind();
    }

    protected void lbtnAdd_Click(object sender, EventArgs e)
    {
    }

    private Evaluation GetEvaluation(GridViewRow row, DateTime now)
    {
        string userCode = ((Controls_TextBox)row.FindControl("tbUserCode")).Text.Trim();
        if (string.IsNullOrEmpty(userCode)) return null;
        var user = this.TheUserMgr.LoadUser(userCode, false, false);
        if (user == null) return null;
        string oldUserCode = ((HiddenField)row.FindControl("hfUserCode")).Value;

        Evaluation evaluation = null;

        if (string.IsNullOrEmpty(oldUserCode))
        {
            evaluation = new Evaluation();
            evaluation.UserCode = userCode;
            evaluation.IsBlankDetail = true;
            evaluation.CreateDate = now;
            evaluation.CreateUser = this.CurrentUser.Code;
            evaluation.CreateUserNm = this.CurrentUser.Name;
        }
        else
        {
            evaluation = this.TheEvaluationMgr.LoadEvaluation(oldUserCode);
            evaluation.OldUserCode = oldUserCode;
            evaluation.UserCode = userCode;
        }

        var amount = ((TextBox)row.FindControl("tbAmount")).Text.Trim();
        if (amount != string.Empty)
        {
            evaluation.Amount = int.Parse(amount);
        }
        else
        {
            evaluation.Amount = 0;
        }
        var standardQty = ((TextBox)row.FindControl("tbStandardQty")).Text.Trim();
        if (standardQty != string.Empty)
        {
            evaluation.StandardQty = int.Parse(standardQty);
        }
        else
        {
            evaluation.StandardQty = 0;
        }

        evaluation.UserName = user.Name;
        evaluation.IsActive = ((CheckBox)row.FindControl("cbIsActive")).Checked;
        evaluation.IsCheckup = ((CheckBox)row.FindControl("cbIsCheckup")).Checked;
        evaluation.LastModifyDate = now;
        evaluation.LastModifyUser = this.CurrentUser.Code;
        evaluation.LastModifyUserNm = this.CurrentUser.Name;
        return evaluation;
    }
    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            DateTime now = DateTime.Now;
            IList<Evaluation> evaluationList = new List<Evaluation>();
            foreach (GridViewRow row in this.GV_List.Rows)
            {
                var rfvUserCode = ((RequiredFieldValidator)row.FindControl("rfvUserCode"));
                string userCode = ((Controls_TextBox)row.FindControl("tbUserCode")).Text.Trim();
                if (rfvUserCode.IsValid && !string.IsNullOrEmpty(userCode))
                {
                    Evaluation evaluation = GetEvaluation(row, now);
                    if (evaluation != null && evaluationList.Where(ee => ee.UserCode == evaluation.UserCode).Count() == 0)
                    {
                        evaluationList.Add(evaluation);
                    }
                }
            }

            this.TheEvaluationMgr.UpdateEvaluation(evaluationList);

            this.ShowSuccessMessage("ISI.Evaluation.UpdateEvaluation.Successfully");
            doSearch();
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

}