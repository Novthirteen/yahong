using System;
using System.Collections;
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
using com.Sconit.Entity.MasterData;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.Utility;
using com.Sconit.Control;
using NHibernate.Expression;

public partial class Order_OrderHead_Production_Scrap : MainModuleBase
{

    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:false,bool:false,bool:false,bool:true,bool:false,bool:false,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_BOTH;

        this.lblReason.Text = GetCodeMaster(BusinessConstants.CODE_MASTER_STOCK_OUT_REASON_VALUE_REASON1);

        if (!IsPostBack)
        {
            this.tbFlow.Text = string.Empty;
            this.tbRefOrderNo.Text = string.Empty;

            this.tbMiscOrderEffectDate.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
        }
    }

    protected void btnRefOrderNo_Click(object sender, EventArgs e)
    {
        string refOrderNo = this.tbRefOrderNo.Text.Trim();
        if (refOrderNo.StartsWith(BusinessConstants.CODE_PREFIX_INSPECTION_REJECT)
          || refOrderNo.StartsWith(BusinessConstants.CODE_PREFIX_INSPECTION))
        {

            DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(MiscOrder));
            selectCriteria.Add(Expression.Eq("ReferenceOrderNo", refOrderNo));
            IList<MiscOrder> list = TheCriteriaMgr.FindAll<MiscOrder>(selectCriteria);
            if (list != null && list.Count > 0)
            {
                ShowErrorMessage("MasterData.MiscOrder.inspection.reject.isprocess");
                return;
            }

            IList<InspectResult> inspectResultList = TheInspectResultMgr.GetInspectResults(refOrderNo);

            if (inspectResultList == null || inspectResultList.Count == 0)
            {
                ShowWarningMessage("MasterData.MiscOrder.Detail.IsEmpty", this.tbRefOrderNo.Text.Trim());
                return;
            }
            tbLocation.Text = inspectResultList[0].InspectOrderDetail.InspectOrder.RejectLocation;

            this.GV_List.DataSource = inspectResultList;
            this.GV_List.DataBind();
        }
    }

    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        try
        {
            MiscOrder miscOrder = new MiscOrder();
            miscOrder.Remark = this.tbRemark.Text;
            miscOrder.Type = BusinessConstants.CODE_MASTER_MISC_ORDER_TYPE_VALUE_GI;
            miscOrder.Location = this.TheLocationMgr.LoadLocation(this.tbLocation.Text);
            miscOrder.EffectiveDate = DateTime.Parse(this.tbMiscOrderEffectDate.Text);
            miscOrder.ReferenceOrderNo = this.tbRefOrderNo.Text.Trim();
            miscOrder.Reason = BusinessConstants.CODE_MASTER_STOCK_OUT_REASON_VALUE_REASON1;
            miscOrder.ProjectCode = this.tbFlow.Text.Trim();

            if (string.IsNullOrEmpty( this.tbFlow.Text))
            {
                ShowErrorMessage("MasterData.Flow.IsEmpty");
                return;
            }

            foreach (GridViewRow row in GV_List.Rows)
            {
                MiscOrderDetail miscOrderDetail = new MiscOrderDetail();
                Label lblItemCode = row.FindControl("lblItemCode") as Label;
                miscOrderDetail.Item = TheItemMgr.LoadItem(lblItemCode.Text.Trim());
                TextBox tbQty = row.FindControl("tbQty") as TextBox;
                if (!string.IsNullOrEmpty(tbQty.Text))
                {
                    miscOrderDetail.Qty = decimal.Parse(tbQty.Text.Trim());
                }
                if (miscOrderDetail.Qty != 0)
                {
                    miscOrder.AddMiscOrderDetail(miscOrderDetail);
                }
            }

            if (miscOrder.MiscOrderDetails == null || miscOrder.MiscOrderDetails.Count == 0)
            {
                ShowErrorMessage("MasterData.MiscOrder.Detail.IsEmpty");
                return;
            }

            TheMiscOrderMgr.SaveMiscOrder(miscOrder, this.CurrentUser);

            ShowSuccessMessage("MasterData.MiscOrder.GISubmit.Successfully", miscOrder.OrderNo);

            this.GV_List.DataSource = new List<InspectResult>();
            this.GV_List.DataBind();
        }
        catch (BusinessErrorException ex)
        {
            ShowErrorMessage(ex);
        }
    }

    private string GetCodeMaster(string codeValue)
    {
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(CodeMaster));
        selectCriteria.Add(Expression.Eq("Value", codeValue));

        IList<CodeMaster> codemstrs = TheCriteriaMgr.FindAll<CodeMaster>(selectCriteria);
        if (codemstrs != null && codemstrs.Count > 0)
        {
            return codemstrs[0].Description;
        }
        return string.Empty;
    }
}
