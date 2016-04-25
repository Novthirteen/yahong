using System;
using System.Collections;
using System.Web.UI.WebControls;
using System.Linq;
using log4net;
using com.Sconit.Web;
using com.Sconit.ISI.Service.Ext;
using com.Sconit.ISI.Entity;
using com.Sconit.Utility;
using NHibernate.Expression;
using System.Collections.Generic;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using com.Sconit.ISI.Entity.Util;
using com.Sconit.Facility.Entity;
using System.Drawing;
using com.Sconit.ISI.Service.Util;
using LeanEngine.Entity;
using com.Sconit.Facility.Service;

//TODO: Add other using statements here.by liqiuyun
public partial class Facility_CheckListOrder_Edit : EditModuleBase
{
    private CheckListOrderMaster checkListOrder
    {
        get { return (CheckListOrderMaster)ViewState["CheckListOrderMaster"]; }
        set { ViewState["CheckListOrderMaster"] = value; }
    }

    private String Code
    {
        get { return (string)ViewState["Code"]; }
        set { ViewState["Code"] = value; }
    }

    public event EventHandler Back;
    //Get the logger
    private static ILog log = LogManager.GetLogger("ISI");

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {

        }
    }

    public void InitPageParameter(string Code)
    {
        checkListOrder = this.TheGenericMgr.FindById<CheckListOrderMaster>(Code);
        this.Code = Code;
        this.lblCode.Text = checkListOrder.Code;
        this.lblRegion.Text = checkListOrder.Region;
        this.lblCheckListCode.Text = checkListOrder.CheckListCode;
        this.lblCheckListName.Text = checkListOrder.CheckListName;
        this.lblDescription.Text = checkListOrder.Description;
        this.lblFacilityID.Text = checkListOrder.FacilityID;
        this.lblFacilityName.Text = checkListOrder.FacilityName;
        this.tbCheckUser.Text = checkListOrder.CheckUser;
        this.tbCheckDate.Text = checkListOrder.CheckDate.ToString();
        this.tbRemark.Text = checkListOrder.Remark;
        if (checkListOrder.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE)
        {
            this.tbCheckUser.Enabled = false;
            this.tbCheckDate.Enabled = false;
            this.tbRemark.Enabled = false;
            this.btnClose.Visible = false;
            this.btnUpdate.Visible = false;
            this.btnDelete.Visible = false;
        }
        else
        {
            this.tbCheckUser.Enabled = true;
            this.tbCheckDate.Enabled = true;
            this.tbRemark.Enabled = true;
            this.btnClose.Visible = true;
            this.btnUpdate.Visible = true;
            this.btnDelete.Visible = true;
        }
        DetailDataBind(checkListOrder);
    }

    private void DetailDataBind(CheckListOrderMaster checkListOrder)
    {
        var details = this.TheGenericMgr.FindAll<CheckListOrderDetail>
            ("from CheckListOrderDetail where OrderNo = ? order by Seq desc ", checkListOrder.Code, 0, 25) ?? new List<CheckListOrderDetail>();
        details = details.OrderBy(d => d.Seq).ToList();

        this.GV_List.DataSource = details;
        this.GV_List.DataBind();
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        if (Back != null)
        {
            this.Visible = false;
            Back(sender, e);
        }
    }

    protected void btnUpdate_Click(object sender, EventArgs e)
    {
        CheckListOrderMaster checkListOrder = TheGenericMgr.FindById<CheckListOrderMaster>(this.Code);
        checkListOrder.CheckDate = Convert.ToDateTime(this.tbCheckDate.Text);
        checkListOrder.CheckUser = this.tbCheckUser.Text.Trim();
        checkListOrder.Remark = this.tbRemark.Text.Trim();
        checkListOrder.CheckListOrderDetailList = this.GetCheckListOrderDetailList();

        #region �ж���ϸ�в���������Ҫ��д��ע
        foreach (CheckListOrderDetail d in checkListOrder.CheckListOrderDetailList)
        {
            if (!d.IsNormal && string.IsNullOrEmpty(d.Remark))
            {
                ShowErrorMessage(string.Format("���{0}����ĿѲ�����쳣������д���", d.Seq.ToString()));
                return;
            }
            //�жϱ���
            if (d.IsRequired && string.IsNullOrEmpty(d.Remark))
            {
                ShowErrorMessage(string.Format("���{0}����ĿѲ����Ϊ������", d.Seq.ToString()));
                return;
            }

            //�ж��Ƿ�������ֵ��Χ��
            if (d.MaxValue == 0 && d.MinValue == 0)
            {
                //nothing todo
            }
            else if (!string.IsNullOrEmpty(d.Remark))
            {
                double thisValue = 0;
                if (!double.TryParse(d.Remark, out thisValue))
                {
                    ShowErrorMessage(string.Format("���{0}�Ľ��������������", d.Seq.ToString()));
                    return;
                }
                if (thisValue <= d.MaxValue && thisValue >= d.MinValue)
                {
                    d.IsNormal = true;
                }
                else
                {
                    d.IsNormal = false;
                }
            }
        }
        #endregion

        TheCheckListMgr.UpdateCheckListOrder(checkListOrder);
        ShowSuccessMessage("���³ɹ�");
        InitPageParameter(Code);
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            TheCheckListMgr.DeleteCheckListOrder(this.Code);
            ShowSuccessMessage("ɾ���ɹ�");
            Back(sender, e);
        }
        catch (BusinessException ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        //CheckListOrderMaster master = TheGenericMgr.FindById<CheckListOrderMaster>(this.Code);
        //master.LastModifyDate = DateTime.Now;
        //master.LastModifyUser = this.CurrentUser.Code;
        //master.Status = BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE;

        ////todo ����ISI����

        //TheGenericMgr.Update(master);
        TheCheckListMgr.CloseCheckListMaster(this.Code, this.CurrentUser);
        ShowSuccessMessage("�رճɹ�");
        InitPageParameter(this.Code);
    }

    protected void lbtnUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            LinkButton lb = (LinkButton)sender;
            //��ȡ��������commentID
            int comId = int.Parse(lb.CommandArgument);

            GridView gv_List = this.GV_List;
            int rowIndex = ((GridViewRow)(((DataControlFieldCell)(((LinkButton)(sender)).Parent)).Parent)).RowIndex;
            GridViewRow newRow = gv_List.Rows[rowIndex];

            CheckBox tbIsNormal = (CheckBox)newRow.FindControl("tbIsNormal");
            TextBox tbRemark = (TextBox)newRow.FindControl("tbRemark");

            var checkListDetail = this.TheGenericMgr.FindById<CheckListOrderDetail>(comId);

            //checkListDetail. = tbDetailCode.Text.Trim();
            checkListDetail.Remark = tbRemark.Text.Trim();
            checkListDetail.IsNormal = tbIsNormal.Checked;

            checkListDetail.LastModifyDate = DateTime.Now;
            checkListDetail.LastModifyUser = this.CurrentUser.Code;

            this.TheGenericMgr.Update(checkListDetail);
            ShowSuccessMessage("���³ɹ�");


            DetailDataBind(checkListOrder);
        }
        catch (Exception ex)
        {
            ShowErrorMessage("Common.Business.Result.Insert.Failed");
        }
    }



    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            CheckListOrderDetail d = (CheckListOrderDetail)e.Row.DataItem;
            Label lblIsRequired = (Label)e.Row.FindControl("lblIsRequired");

            if (d.IsRequired)
            {
                lblIsRequired.Text = "����";
                lblIsRequired.ForeColor = System.Drawing.Color.Red;
                TextBox tbRemark = (TextBox)e.Row.FindControl("tbRemark");
                tbRemark.Attributes.Add("class", "inputRequired");
            }

            RadioButton rbAbnormal = (RadioButton)e.Row.FindControl("rbAbnormal");
            rbAbnormal.Checked = !d.IsNormal;
        }
    }

    private List<CheckListOrderDetail> GetCheckListOrderDetailList()
    {
        List<CheckListOrderDetail> checkListOrderDetailList = new List<CheckListOrderDetail>();
        foreach (GridViewRow row in this.GV_List.Rows)
        {
            HiddenField hfId = (HiddenField)row.FindControl("hfId");
            RadioButton rbNormal = (RadioButton)row.FindControl("rbNormal");
            TextBox tbRemark = (TextBox)row.FindControl("tbRemark");

            var checkListOrderDetail = this.TheGenericMgr.FindById<CheckListOrderDetail>(Convert.ToInt32(hfId.Value));

            checkListOrderDetail.Remark = tbRemark.Text.Trim();
            checkListOrderDetail.IsNormal = rbNormal.Checked;

            checkListOrderDetail.LastModifyDate = DateTime.Now;
            checkListOrderDetail.LastModifyUser = this.CurrentUser.Code;
            checkListOrderDetailList.Add(checkListOrderDetail);
        }
        return checkListOrderDetailList;

    }

    protected void btnPrint_Click(object sender, EventArgs e)
    {
        CheckListOrderMaster master = TheCheckListMgr.GetCheckListOrderMaster(this.Code);
        IList<object> list = new List<object>();
        list.Add(master);
        list.Add(master.CheckListOrderDetailList);
        string template = "CheckListOrder.xls";
        string printUrl = TheReportMgr.WriteToFile(template, list);
        Page.ClientScript.RegisterStartupScript(GetType(), "method", " <script language='javascript' type='text/javascript'>PrintOrder('" + printUrl + "'); </script>");
        ShowSuccessMessage("��ӡ�ɹ�");
    }

    protected void btnExport_Click(object sender, EventArgs e)
    {
        CheckListOrderMaster master = TheCheckListMgr.GetCheckListOrderMaster(this.Code);
        IList<object> list = new List<object>();
        list.Add(master);
        list.Add(master.CheckListOrderDetailList);
        TheReportMgr.WriteToClient("CheckListOrder.xls", list, "CheckListOrder.xls");
    }
}