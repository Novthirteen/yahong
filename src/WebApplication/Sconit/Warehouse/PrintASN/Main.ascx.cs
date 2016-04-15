using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;
using com.Sconit.Entity.Distribution;
using com.Sconit.Entity.MasterData;
using com.Sconit.Utility;
using com.Sconit.Web;
using NHibernate.Expression;
using System.Web.UI.WebControls;
using System.Linq;

public partial class Warehouse_PrintASN_Main : MainModuleBase
{
    public string ModuleType
    {
        get { return BusinessConstants.CODE_MASTER_ORDER_TYPE_VALUE_DISTRIBUTION; }
    }

    private IDictionary<string, string> dicParam;

    protected void Page_Load(object sender, EventArgs e)
    {
        this.tbPartyFrom.ServiceParameter = "string:" + this.ModuleType + ",string:" + this.CurrentUser.Code;
        this.tbPartyTo.ServiceParameter = "string:" + this.ModuleType + ",string:" + this.CurrentUser.Code;

        this.tbFlow.ServiceParameter = "string:" + this.CurrentUser.Code + ",bool:false,bool:true,bool:true,bool:false,bool:false,bool:false,string:" + BusinessConstants.PARTY_AUTHRIZE_OPTION_FROM;

        if (!IsPostBack)
        {
            this.tbStartDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
            this.tbEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        FillParameter();
        DetachedCriteria selectCriteria = DetachedCriteria.For(typeof(InProcessLocation));
        selectCriteria.CreateAlias("PartyFrom", "pf");
        selectCriteria.CreateAlias("PartyTo", "pt");

        if (this.dicParam["Flow"] != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("Flow", this.dicParam["Flow"]));
        }
        if (this.dicParam["IpNo"] != string.Empty)
        {
            selectCriteria.Add(Expression.Like("IpNo", this.dicParam["IpNo"], MatchMode.Anywhere));
        }
        #region date
        if (this.dicParam["StartDate"] != string.Empty)
        {
            selectCriteria.Add(Expression.Ge("CreateDate", DateTime.Parse(this.dicParam["StartDate"])));
        }
        if (this.dicParam["EndDate"] != string.Empty)
        {
            selectCriteria.Add(Expression.Lt("CreateDate", DateTime.Parse(this.dicParam["EndDate"]).AddDays(1)));
        }
        #endregion

        #region partyFrom
        if (this.dicParam["PartyFrom"] != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("pf.Code", this.dicParam["PartyFrom"]));
        }
        else
        {
            //发货路线
            DetachedCriteria[] pfCrieteria = SecurityHelper.GetPartyPermissionCriteria(this.CurrentUser.Code,
                BusinessConstants.CODE_MASTER_PARTY_TYPE_VALUE_REGION, BusinessConstants.CODE_MASTER_PARTY_TYPE_VALUE_SUPPLIER);

            selectCriteria.Add(
                Expression.Or(
                    Subqueries.PropertyIn("pf.Code", pfCrieteria[0]),
                    Subqueries.PropertyIn("pf.Code", pfCrieteria[1])
            ));
        }
        #endregion

        #region OrderType过滤
        List<string> typeList = new List<string>();
        typeList.Add(this.ModuleType);
        selectCriteria.Add(Expression.In("OrderType", typeList));
        #endregion

        #region partyTo
        if (this.dicParam["PartyTo"] != string.Empty)
        {
            selectCriteria.Add(Expression.Eq("pt.Code", this.dicParam["PartyTo"]));
        }
        #endregion
        selectCriteria.Add(Expression.Eq("Type", BusinessConstants.CODE_MASTER_INPROCESS_LOCATION_TYPE_VALUE_NORMAL));
        selectCriteria.Add(Expression.Not(Expression.Eq("Status", BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE)));
        selectCriteria.AddOrder(Order.Desc("IpNo"));

        IList<InProcessLocation> ips = TheCriteriaMgr.FindAll<InProcessLocation>(selectCriteria, 0, 100);

        this.GV_List.DataSource = ips;
        this.GV_List.DataBind();
    }

    private void FillParameter()
    {
        this.dicParam = new Dictionary<string, string>();
        this.dicParam["IpNo"] = this.tbIpNo.Text.Trim();
        this.dicParam["PartyFrom"] = this.tbPartyFrom.Text.Trim();
        this.dicParam["PartyTo"] = this.tbPartyTo.Text.Trim();
        //this.dicParam["AsnType"] = this.AsnType;
        //this.dicParam["Status"] = this.ddlStatus.SelectedValue;
        //this.dicParam["OrderNo"] = this.tbOrderNo.Text.Trim();
        //this.dicParam["Item"] = this.tbItem.Text.Trim();
        this.dicParam["StartDate"] = this.tbStartDate.Text.Trim();
        this.dicParam["EndDate"] = this.tbEndDate.Text.Trim();
        this.dicParam["Flow"] = this.tbFlow.Text.Trim();
    }

    private IList<CodeMaster> GetStatusGroup()
    {
        IList<CodeMaster> statusGroup = new List<CodeMaster>();

        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_CREATE));
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_INPROCESS));
        statusGroup.Add(GetStatus(BusinessConstants.CODE_MASTER_STATUS_VALUE_CLOSE));

        return statusGroup;
    }

    private CodeMaster GetStatus(string statusValue)
    {
        return TheCodeMasterMgr.GetCachedCodeMaster(BusinessConstants.CODE_MASTER_STATUS, statusValue);
    }

    protected void lbtnPrint_Click(object sender, EventArgs e)
    {
        string ipNo = ((LinkButton)sender).CommandArgument;
        InProcessLocation inProcessLocation = TheInProcessLocationMgr.LoadInProcessLocation(ipNo, true);
        if (inProcessLocation.AsnTemplate == null || inProcessLocation.AsnTemplate == string.Empty)
        {
            ShowErrorMessage("ASN.PrintError.NoASNTemplate");
            return;
        }

        IList<object> list = new List<object>();
        list.Add(inProcessLocation);
        list.Add(inProcessLocation.InProcessLocationDetails);

        //报表url

        string template = inProcessLocation.AsnTemplate;
        if (e == null)
        {
            template = inProcessLocation.InProcessLocationDetails[0].OrderLocationTransaction.OrderDetail.OrderHead.HuTemplate;
        }
        string asnUrl = TheReportMgr.WriteToFile(template, list);

        //客户端打印
        //如果在UpdatePanel中调用JavaScript需要使用 ScriptManager.RegisterClientScriptBlock
        //ScriptManager.RegisterClientScriptBlock(this, GetType(), "method", " <script language='javascript' type='text/javascript'>PrintOrder('" + asnUrl + "'); </script>", false);
        Page.ClientScript.RegisterStartupScript(GetType(), "method", " <script language='javascript' type='text/javascript'>PrintOrder('" + asnUrl + "'); </script>");
        this.btnSearch_Click(null, null);
    }

    protected void lbtnPrintBarCode_Click(object sender, EventArgs e)
    {
        lbtnPrint_Click(sender, null);
    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        InProcessLocation ip = (InProcessLocation)e.Row.DataItem;
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[0].Attributes.Add("title", GetDetail(ip));

            LinkButton lbtnPrint = (LinkButton)e.Row.FindControl("lbtnPrint");
            if (ip.IsPrinted)
            {
                lbtnPrint.ForeColor = System.Drawing.Color.Red;
            }
        }
    }

    private string GetDetail(InProcessLocation ip)
    {
        string detail = "";

        if (ip.InProcessLocationDetails.Count == 0)
        {
            return detail;
        }
        OrderDetail orderDetail = ip.InProcessLocationDetails[0].OrderLocationTransaction.OrderDetail;
        OrderHead orderHead = orderDetail.OrderHead;
        //string itemCategory = orderDetail.DefaultItemCategory;

        detail += "cssbody=[obbd] cssheader=[obhd] header=[" + orderHead.OrderNo + " | " + (ip.Flow == null ? string.Empty : ip.Flow) +
             "] body=[<table width=100%>";
        var ods = ip.InProcessLocationDetails.Take(20);
        foreach (InProcessLocationDetail od in ods)
        {
            string Carton = od.UnitCount.ToString("0.########") + "/" + Math.Ceiling((double)(od.Qty / od.UnitCount)).ToString("0");
            string ItemCode = od.OrderLocationTransaction.Item.Code;
            //string ItemDesc = od.OrderLocationTransaction.Item.Description.Replace("[", "&#91;").Replace("]", "&#93;");
            string ItemDesc = od.ReferenceItemCode;
            string OrderQty = od.Qty.ToString("0.########");
            string Uom = od.OrderLocationTransaction.Uom.Code;
            detail += "<tr><td>" + ItemCode + "</td><td>" + ItemDesc + "</td><td>" + OrderQty + "</td><td>" + Uom + "</td><td>" + Carton + "</td></tr>";
        }
        detail += "</table>]";
        return detail;
    }
}
