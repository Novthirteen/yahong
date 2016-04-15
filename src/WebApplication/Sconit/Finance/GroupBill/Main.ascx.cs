using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using com.Sconit.Web;
using com.Sconit.Entity.MasterData;
using com.Sconit.Entity.Exception;
using NHibernate.Expression;
using com.Sconit.Entity.View;
using com.Sconit.Entity;
using System.Data.SqlClient;
using System.Data;
using com.Sconit.Utility;
public partial class Finance_GroupBill_Main : MainModuleBase
{
    public event EventHandler BackEvent;
    public event EventHandler CreateEvent;

    public string ModuleType
    {
        get { return (string)ViewState["ModuleType"]; }
        set { ViewState["ModuleType"] = value; }
    }
    public DateTime startTime { get { return Convert.ToDateTime(this.tbStartTime.Text.Trim()); } }
    public DateTime endTime { get { return Convert.ToDateTime(this.tbEndTime.Text.Trim()); } }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (this.ModuleType == BusinessConstants.BILL_TRANS_TYPE_PO)
        {
            this.tbPartyCode.ServicePath = "SupplierMgr.service";
            this.tbPartyCode.ServiceMethod = "GetAllSupplier";
            this.ltlPartyCode.Text = "供应商代号";
        }
        else
        {
            this.ltlPartyCode.Text = "${MasterData.ActingBill.Customer}:";
            this.tbPartyCode.ServicePath = "CustomerMgr.service";
            this.tbPartyCode.ServiceMethod = "GetAllCustomer";
        }

        if (!Page.IsPostBack)
        {
            fld_Gv_List.Visible = false;
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        try
        {
            string sql = @"select item.code as 物料,item.desc1+isnull(item.desc2,'') 物料描述,
                    cast(sum(BillQty) as numeric(12,2)) as 总数量,
                    cast(sum(BilledQty) as numeric(12,2)) as 已结算数,
                    cast(sum(BillQty)-sum(BilledQty) as numeric(12,2)) as 未结数量
                    from actbill 
                    left join item on item.code = actbill.item
                    left join partyaddr on partyaddr.code = actbill.billaddr
                    left join party on partyaddr.partycode = party.code                         
                    where transtype=@p0 
                    ";
            if (rblTimeType.SelectedIndex == 0)
            {
                sql += " and effdate>@p1 and effdate <@p2 ";
            }
            else
            {
                sql += " and CreateDate>@p1 and CreateDate <@p2 ";
            }

            SqlParameter[] sqlParam = new SqlParameter[4];

            try
            {
                if (DateTime.Compare(startTime, endTime) > 0)
                {
                    ShowErrorMessage("Common.StarDate.EndDate.Compare");
                    return;
                }
            }
            catch (Exception)
            {
                ShowErrorMessage("Common.Business.Error.DateInvalid");
                return;
            }

            sqlParam[1] = new SqlParameter("@p1", startTime);
            sqlParam[2] = new SqlParameter("@p2", endTime);

            sqlParam[0] = new SqlParameter("@p0", this.ModuleType);

            if (this.tbPartyCode.Text.Trim() != string.Empty)
            {
                sql += " and party.code =@p3 ";
                sqlParam[3] = new SqlParameter("@p3", tbPartyCode.Text.Trim());
            }
            else
            {
                ShowErrorMessage("供应商/客户不能为空");
                return;
            }

            sql += @" group by item.code,item.desc1,item.desc2 ";

            DataSet dataSet = TheSqlHelperMgr.GetDatasetBySql(sql, sqlParam);
            List<GroupBill> groupBills = IListHelper.DataTableToList<GroupBill>(dataSet.Tables[0]);

            if ((Button)sender == this.btnImport)
            {
                List<GroupBill> newGroupBills = new List<GroupBill>();
                Dictionary<string, decimal> dicBill = TheImportMgr.ReadBillFromXls(fileUpload.PostedFile.InputStream, this.tbPartyCode.Text.Trim(), this.CurrentUser);

                Dictionary<string, decimal> notMatchBill = new Dictionary<string, decimal>();

                foreach (var actingBill1 in dicBill)
                {
                    bool isMatch = false;
                    foreach (var actingBill2 in groupBills)
                    {
                        if (actingBill1.Value > 0 && actingBill2.未结数量 > 0)
                        {
                            if (actingBill1.Key == actingBill2.物料)
                            {
                                if (actingBill1.Value <= actingBill2.未结数量)
                                {
                                    actingBill2.本次结算数 = actingBill1.Value;
                                }
                                else
                                {
                                    actingBill2.本次结算数 = actingBill1.Value;
                                    actingBill2.信息 = "数量不足本次结算";
                                }
                                isMatch = true;
                                newGroupBills.Add(actingBill2);
                            }
                        }
                    }

                    if (!isMatch)
                    {
                        GroupBill groupbill = new GroupBill();
                        groupbill.物料 = actingBill1.Key;
                        groupbill.本次结算数 = actingBill1.Value;
                        groupbill.信息 = "未匹配";
                        newGroupBills.Add(groupbill);
                    }
                }
                this.GV_List.DataSource = newGroupBills;
            }
            else
            {
                this.GV_List.DataSource = groupBills;
            }

            this.GV_List.DataBind();
            this.fld_Gv_List.Visible = true;
            if ((Button)sender == this.btnExport)
            {
                this.ExportXLS(this.GV_List);
            }
        }
        catch (Exception ex)
        {
            ShowErrorMessage(ex.Message);
        }
    }


    protected void btnConfirm_Click(object sender, EventArgs e)
    {
        try
        {
            if (this.GV_List.Rows != null && this.GV_List.Rows.Count > 0)
            {
                Dictionary<string, decimal> dicBill = new Dictionary<string, decimal>();
                foreach (GridViewRow row in this.GV_List.Rows)
                {
                    string message = row.Cells[7].Text;
                    if (message == "&nbsp;" || message == string.Empty)
                    {
                        string itemCode = row.Cells[1].Text;
                        decimal qty = decimal.Parse(row.Cells[6].Text);
                        dicBill.Add(itemCode, qty);
                    }
                }
                var billList = TheBillMgr.CreateBill(dicBill, this.tbPartyCode.Text.Trim(), this.ModuleType, this.CurrentUser, startTime, endTime, this.rblTimeType.SelectedIndex == 0);
                this.CreateEvent(billList[0].BillNo, e);
                this.ShowSuccessMessage("MasterData.Bill.CreateSuccessfully", billList[0].BillNo);
            }
        }
        catch (Exception ex)
        { ShowErrorMessage(ex.Message); }

    }


    protected void GV_List_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            e.Row.Cells[3].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
            e.Row.Cells[4].Attributes.Add("style", "vnd.ms-excel.numberformat:@");
        }
    }

    protected void btnBack_Click(object sender, EventArgs e)
    {
        this.BackEvent(sender, e);
    }

    class GroupBill
    {
        public string 物料 { get; set; }
        public string 物料描述 { get; set; }
        public string 总数量 { get; set; }
        public decimal 已结算数 { get; set; }
        public decimal 未结数量 { get; set; }
        public decimal 本次结算数 { get; set; }
        public string 信息 { get; set; }
    }

}
