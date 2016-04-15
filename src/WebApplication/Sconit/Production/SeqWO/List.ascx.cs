using System;
using System.Collections;
using System.Collections.Generic;
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
using com.Sconit.Service.Ext.MasterData;
using com.Sconit.Entity.Exception;
using NHibernate.Expression;
using com.Sconit.Service.Ext.Criteria;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity;
using com.Sconit.Control;
public partial class Production_SeqWO_List : ModuleBase
{

    public string ModuleType
    {
        get
        {
            return (string)ViewState["ModuleType"];
        }
        set
        {
            ViewState["ModuleType"] = value;
        }
    }
    public string ModuleSubType
    {
        get
        {
            return (string)ViewState["ModuleSubType"];
        }
        set
        {
            ViewState["ModuleSubType"] = value;
        }
    }


    public string FlowCode
    {
        get
        {
            return (string)ViewState["FlowCode"];
        }
        set
        {
            ViewState["FlowCode"] = value;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public void InitPageParameter(string flowCode)
    {
        this.FlowCode = flowCode;
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(OrderHead));

        selectCriteria.Add(Expression.Eq("Flow", flowCode))
            .Add(Expression.Eq("Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT));
        selectCriteria.AddOrder(Order.Asc("StartTime"));
        selectCriteria.AddOrder(Order.Asc("Sequence"));
        selectCriteria.AddOrder(Order.Asc("OrderNo"));
        IList<OrderHead> orderHeadList = TheCriteriaMgr.FindAll<OrderHead>(selectCriteria);
        this.GV_List.DataSource = orderHeadList;
        this.GV_List.DataBind();

    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        try
        {
            int j = 0;
            DateTime now = DateTime.Now;
            for (int i = 0; i < this.GV_List.Rows.Count; i++)
            {
                //HiddenField hfIsChanged = (HiddenField)this.GV_List.Rows[i].FindControl("hfIsChanged");
                //if (hfIsChanged.Value == "Y")
                {
                    string orderNo = this.GV_List.Rows[i].Cells[2].Text;
                    OrderHead orderHead = TheOrderMgr.LoadOrder(orderNo, this.CurrentUser.Code);
                    if (orderHead.Status == BusinessConstants.CODE_MASTER_STATUS_VALUE_SUBMIT)
                    {
                        TextBox tbSequence = (TextBox)this.GV_List.Rows[i].FindControl("tbSequence");
                        TextBox tbWindowTime = (TextBox)this.GV_List.Rows[i].FindControl("tbWindowTime");
                        orderHead.Sequence = Int32.Parse(tbSequence.Text.Trim());
                        orderHead.WindowTime = DateTime.Parse(tbWindowTime.Text.Trim());

                        orderHead.LastModifyDate = now;
                        orderHead.LastModifyUser = CurrentUser;

                        TheOrderHeadMgr.UpdateOrderHead(orderHead);
                        j++;
                    }
                }
            }
            if (j == 0)
            {
                ShowErrorMessage("SeqWO.PleaseSelectOrder");
            }
            else
            {
                InitPageParameter(this.FlowCode);
                ShowSuccessMessage("SeqWO.Successfully");
            }
        }
        catch (BusinessErrorException ex)
        {
            this.ShowErrorMessage(ex);
        }
    }

    /*
    private IList<string> GetSelectOrder()
    {
        IList<string> orderNoList = new List<string>();
        for (int i = 0; i < this.GV_List.Rows.Count; i++)
        {
            CheckBox cbOrderNo = (CheckBox)this.GV_List.Rows[i].FindControl("cbOrderNo");

            if (cbOrderNo.Checked == true)
            {
                orderNoList.Add(this.GV_List.DataKeys[i].Value.ToString());
            }
        }
        return orderNoList;
    }
    */
    private GridViewRow[] GetSelectOrder()
    {
        IList<GridViewRow> orderNoList = new List<GridViewRow>();
        for (int i = 0; i < this.GV_List.Rows.Count; i++)
        {
            CheckBox cbOrderNo = (CheckBox)this.GV_List.Rows[i].FindControl("cbOrderNo");

            if (cbOrderNo.Checked == true)
            {
                orderNoList.Add(this.GV_List.Rows[i]);
            }
        }
        return orderNoList.ToArray<GridViewRow>();
    }

    private void exchangeRow(GridViewRow dataRowSrc, GridViewRow dataRowTarge)
    {
        //
        string temp = string.Empty;

        bool tempB = false;
        CheckBox cbOrderNoSrc = (CheckBox)dataRowSrc.FindControl("cbOrderNo");
        CheckBox cbOrderNoTarge = (CheckBox)dataRowTarge.FindControl("cbOrderNo");
        tempB = cbOrderNoTarge.Checked;
        cbOrderNoTarge.Checked = cbOrderNoSrc.Checked;
        cbOrderNoSrc.Checked = tempB;

        TextBox tbSequenceSrc = (TextBox)dataRowSrc.FindControl("tbSequence");
        TextBox tbSequenceTarge = (TextBox)dataRowTarge.FindControl("tbSequence");
        temp = tbSequenceTarge.Text;
        tbSequenceTarge.Text = tbSequenceSrc.Text;
        tbSequenceSrc.Text = temp;

        /*
        HiddenField hfIsChangedSrc = (HiddenField)dataRowSrc.FindControl("hfIsChanged");
        HiddenField hfIsChangedTarge = (HiddenField)dataRowTarge.FindControl("hfIsChanged");
        temp = hfIsChangedTarge.Value;
        hfIsChangedTarge.Value = hfIsChangedSrc.Value;
        hfIsChangedSrc.Value = temp;
        */

        temp = dataRowSrc.Cells[2].Text;
        dataRowSrc.Cells[2].Text = dataRowTarge.Cells[2].Text;
        dataRowTarge.Cells[2].Text = temp;

        temp = dataRowTarge.Cells[2].Attributes["title"];
        dataRowTarge.Cells[2].Attributes["title"] = dataRowSrc.Attributes["title"];
        dataRowSrc.Cells[2].Attributes["title"] = temp;

        Label lblItemCodeSrc = (Label)dataRowSrc.FindControl("lblItemCode");
        Label lblItemCodeTarge = (Label)dataRowTarge.FindControl("lblItemCode");
        temp = lblItemCodeTarge.Text;
        lblItemCodeTarge.Text = lblItemCodeSrc.Text;
        lblItemCodeSrc.Text = temp;

        Label lblItemDescriptionSrc = (Label)dataRowSrc.FindControl("lblItemDescription");
        Label lblItemDescriptionTarge = (Label)dataRowTarge.FindControl("lblItemDescription");
        temp = lblItemDescriptionTarge.Text;
        lblItemDescriptionTarge.Text = lblItemDescriptionSrc.Text;
        lblItemDescriptionSrc.Text = temp;

        CodeMstrLabel lblPrioritySrc = (CodeMstrLabel)dataRowSrc.FindControl("lblPriority");
        CodeMstrLabel lblPriorityTarge = (CodeMstrLabel)dataRowTarge.FindControl("lblPriority");
        temp = lblPriorityTarge.Text;
        lblPriorityTarge.Text = lblPrioritySrc.Text;
        lblPrioritySrc.Text = temp;

        TextBox tbWindowTimeSrc = (TextBox)dataRowSrc.FindControl("tbWindowTime");
        TextBox tbWindowTimeTarge = (TextBox)dataRowTarge.FindControl("tbWindowTime");
        temp = tbWindowTimeTarge.Text;
        tbWindowTimeTarge.Text = tbWindowTimeSrc.Text;
        tbWindowTimeSrc.Text = temp;
    }

    protected void ToFirst(object sender, EventArgs e)
    {
        int j = 0;
        GridViewRow[] rows = GetSelectOrder();
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].RowIndex != 0)
            {
                exchangeRow(this.GV_List.Rows[0 + (j++)], rows[i]);
            }
        }

        if (j > 0)
        {
            sortSeq();
        }
    }
    protected void ToEnd(object sender, EventArgs e)
    {
        int j = 0;
        GridViewRow[] rows = GetSelectOrder();

        for (int i = rows.Length - 1; i >= 0; i--)
        {
            if (rows[i].RowIndex != GV_List.Rows.Count - 1)
            {
                exchangeRow(this.GV_List.Rows[GV_List.Rows.Count - 1 - (j++)], rows[i]);
            }
        }

        if (j > 0)
        {
            sortSeq();
        }
    }
    protected void Previous(object sender, EventArgs e)
    {
        int j = 0;
        GridViewRow[] rows = GetSelectOrder();
        for (int i = 0; i < rows.Length; i++)
        {
            if (rows[i].RowIndex != 0)
            {
                exchangeRow(this.GV_List.Rows[rows[i].RowIndex - 1], rows[i]);
                j++;
            }
        }

        if (j > 0)
        {
            sortSeq();
        }
    }

    protected void Next(object sender, EventArgs e)
    {
        int j = 0;
        GridViewRow[] rows = GetSelectOrder();
        for (int i = rows.Length - 1; i >= 0; i--)
        {
            if (rows[i].RowIndex != this.GV_List.Rows.Count - 1)
            {
                exchangeRow(this.GV_List.Rows[rows[i].RowIndex + 1], rows[i]);
                j++;
            }
        }

        if (j > 0)
        {
            sortSeq();
        }
    }

    private void sortSeq()
    {
        for (int i = 0; i < this.GV_List.Rows.Count; i++)
        {
            TextBox tbSequence = (TextBox)this.GV_List.Rows[i].FindControl("tbSequence");
            if (tbSequence.Text == null || tbSequence.Text.Trim() != ((i + 1) * 10).ToString())
            {
                tbSequence.Text = ((i + 1) * 10).ToString();
                //HiddenField hfIsChanged = (HiddenField)this.GV_List.Rows[i].FindControl("hfIsChanged");
                //hfIsChanged.Value = "Y";
            }

        }
    }

    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        OrderHead orderHead = (OrderHead)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[2].Attributes.Add("onmouseover", "e=this.style.backgroundColor; this.style.backgroundColor=this.style.borderColor");
            e.Row.Cells[2].Attributes.Add("onmouseout", "this.style.backgroundColor=e");
            e.Row.Cells[2].Attributes.Add("title", GetDetail(orderHead));
            if (orderHead.Sequence.HasValue)
            {
                TextBox tbSequence = (TextBox)e.Row.FindControl("tbSequence");
                tbSequence.Text = orderHead.Sequence.Value.ToString();
            }
            if (orderHead.OrderDetails != null && orderHead.OrderDetails.Count > 0)
            {
                OrderDetail orderDetail = orderHead.OrderDetails[0];
                Item item = orderDetail.Item;
                Label lblItemCode = (Label)e.Row.FindControl("lblItemCode");
                Label lblItemDescription = (Label)e.Row.FindControl("lblItemDescription");
                lblItemCode.Text = item.Code;
                lblItemDescription.Text = item.Description;
           }
        }
    }

    private string GetDetail(OrderHead orderHead)
    {
        string detail = string.Empty;
        detail += "cssbody=[obbd] cssheader=[obhd] header=[" + orderHead.OrderNo + " | " + (orderHead.Flow == null ? string.Empty : orderHead.Flow) + "] body=[<table width=100%>";
        System.Collections.Generic.IList<OrderDetail> ods = orderHead.OrderDetails;
        foreach (OrderDetail od in ods)
        {
            string ItemCode = od.Item.Code;
            string ItemDesc = od.Item.Description.Replace("[", "&#91;").Replace("]", "&#93;");
            string OrderQty = od.OrderedQty.ToString("0.########");
            string Uom = od.Uom.Code;
            string RecQty = od.ReceivedQty == null ? "0" : od.ReceivedQty.Value.ToString("0.########");
            detail += "<tr><td>" + ItemCode + "</td><td>" + ItemDesc + "</td><td>" + OrderQty + "</td><td>" + Uom + "</td><td>" + RecQty + "</td></tr>";
        }
        detail += "</table>]";
        return detail;
    }

}
