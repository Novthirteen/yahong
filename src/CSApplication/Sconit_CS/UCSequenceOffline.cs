using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using System.Configuration;
using Sconit_CS.SconitWS;

namespace Sconit_CS
{
    public partial class UCSequenceOffline : UserControl
    {
        private User user;
        private int currentRowIndex;
        private string flow;
        private ClientMgrWSSoapClient TheClientMgr;
        private Resolver resolver;
        private DataTable dt;
        private Employee employee;
        private string moduleType;

        public UCSequenceOffline(User user, string moduleType)
        {
            InitializeComponent();
            this.user = user;
            this.gvWODetail.AutoGenerateColumns = false;
            this.gvEmployee.AutoGenerateColumns = false;
            this.moduleType = BusinessConstants.TRANSFORMER_MODULE_TYPE_OFFLINE;
        }

        private void UCWOScanOffline_Load(object sender, EventArgs e)
        {
            this.InitialAll();
            this.TheClientMgr = new ClientMgrWSSoapClient();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Enter))
            {
                ReceiveOrder();
                return true;
            }
            if (keyData == Keys.Tab)
            {
                if (tbEmployee.Focused)
                {
                    this.EmployeeScan();
                    return true;
                }
            }
            if (keyData == Keys.Enter)
            {
                if (tbFlow.Focused)
                {
                    this.lblMessage.Text = string.Empty;
                    this.flow = this.tbFlow.Text.Trim();
                    this.WorkOrderScan();
                    return true;
                }
                else if (tbEmployee.Focused)
                {
                    this.EmployeeScan();
                    return true;
                }
                else if (tbHours.Focused)
                {
                    this.HoursScan();
                    return true;
                }
                else if (this.gvWODetail.Focused)
                {
                    if (this.gvWODetail.CurrentCell.RowIndex + 1 == this.gvWODetail.Rows.Count
                        && this.gvWODetail.CurrentCell.ColumnIndex == 9)
                    {
                        this.btnOffline.Focus();
                        //this.gvWODetail.ClearSelection();
                        return true;
                    }
                    //最后一个单元格如果是处于编辑状态再按回车，则以下代码没有作用，待修改
                    else if (this.gvWODetail.CurrentCell.RowIndex + 1 < this.gvWODetail.Rows.Count
                        && this.gvWODetail.CurrentCell.ColumnIndex == 9)
                    {
                        string orderinCurrentRow = this.gvWODetail[0, this.gvWODetail.CurrentCell.RowIndex].Value.ToString();
                        string orderinNextRow = this.gvWODetail[0, this.gvWODetail.CurrentCell.RowIndex + 1].Value.ToString();
                        if (orderinCurrentRow == orderinNextRow)
                        {
                            this.gvWODetail.CurrentCell = this.gvWODetail[7, this.gvWODetail.CurrentCell.RowIndex + 1];
                            this.gvWODetail.BeginEdit(true);
                        }
                        else
                        {
                            this.btnOffline.Focus();
                            //this.gvWODetail.ClearSelection();
                        }
                        return true;
                    }
                }
                else if (this.btnOffline.Focused)
                {
                    if (this.tbFlow.Text.Trim() != string.Empty)
                    {
                        this.lblMessage.Text = string.Empty;
                        this.flow = this.tbFlow.Text.Trim();
                        this.WorkOrderScan();
                    }
                    else
                    {
                        this.gbDetail.Focus();
                        ReceiveOrder();
                    }
                    return true;
                }
                if (this.gvWODetail.CurrentCell != null && (string)this.gvWODetail.CurrentCell.EditedFormattedValue == string.Empty)
                {
                    SendKeys.Send("0");
                    return true;
                }
            }
            else if (keyData == Keys.Escape)
            {
                if (this.gvWODetail.CurrentCell != null && (string)this.gvWODetail.CurrentCell.EditedFormattedValue == string.Empty)
                {
                    SendKeys.Send("0{ESC}");
                    return true;
                }
                else
                {
                    this.resolver.UserCode = this.user.Code;
                    this.resolver.ModuleType = this.moduleType;
                    //this.resolver.Input = BusinessConstants.BARCODE_SPECIAL_MARK + BusinessConstants.BARCODE_HEAD_CANCEL;
                    //this.resolver = this.TheClientMgr.ScanBarcode(this.resolver);
                    this.resolver.Transformers = null;
                    this.gvOrderDataBind();
                    this.tbFlow.Text = string.Empty;
                    this.tbFlow.Focus();
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void gvEmployeeDataBind()
        {
            this.gvEmployee.ClearSelection();
            if (this.gvEmployee.Rows.Count > 0)
            {
                this.gvEmployee.Rows[0].Selected = true;
            }
        }

        private void WorkOrderScan()
        {
            try
            {
                this.InitialAll();

                this.resolver.Input = this.flow;
                this.resolver.UserCode = this.user.Code;
                this.resolver.ModuleType = this.moduleType;

                this.resolver = TheClientMgr.ScanBarcode(this.resolver);

                this.lblMessage.Text = resolver.Result;
                this.lblMessage.ForeColor = Color.Black;
                //this.resolver = TheClientMgr.GetTransformer(this.resolver);

                this.gvOrderDataBind();
                if (this.gvWODetail.Rows.Count > 0)
                {
                    //this.gvWODetail.Rows[0].Cells["ScrapQty"].Selected = true;
                    this.gvWODetail.CurrentCell = this.gvWODetail.Rows[0].Cells["ScrapQty"];
                    this.ExtraSelectBySameOrderNo(this.gvWODetail);
                    this.gvWODetail.BeginEdit(true);
                }
                //this.gvWODetail.Focus();
                //SendKeys.Send("{Tab}");
                this.tbFlow.Text = string.Empty;
                this.tbFlow.ForeColor = Color.Black;

            }
            catch (FaultException ex)
            {
                MessageBox.Show(this, Utility.FormatExMessage(ex.Message));
                this.tbFlow.Text = string.Empty;
                this.tbFlow.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "程序内部错误,请与管理员联系", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.tbFlow.Text = string.Empty;
                this.tbFlow.Focus();
            }
        }

        private void gvOrderDataBind()
        {
            if (this.resolver.Transformers == null)
            {
                this.resolver.Transformers = new Transformer[] { };
            }
            this.gvWODetail.DataSource = resolver.Transformers.Where(t => t.Status == "In-Process").ToArray();
            this.gvWODetail.ClearSelection();
            this.tbFlow.Text = string.Empty;
        }

        private void EmployeeScan()
        {
            try
            {
                this.tbEmployee.Text = this.tbEmployee.Text.Trim().ToUpper();
                if (this.tbEmployee.Text != string.Empty)
                {
                    this.employee = TheClientMgr.LoadEmployee(this.tbEmployee.Text.Trim());
                    if (employee != null)
                    {
                        this.ShowEmployee(employee);
                    }
                    else
                    {
                        this.tbEmployee.Text = string.Empty;
                        this.lblEmployeeMessage.Text = "此雇员信息不存在!";
                        this.lblEmployeeMessage.ForeColor = Color.Red;
                    }
                }
                else
                {
                    this.btnOffline.Focus();
                }
            }
            catch (FaultException ex)
            {
                MessageBox.Show(this, Utility.FormatExMessage(ex.Message));
                this.tbFlow.Text = string.Empty;
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "获取雇员信息失败!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.tbEmployee.Text = string.Empty;
                this.tbEmployee.Focus();
            }
        }

        private void ShowEmployee(Employee employee)
        {
            string name = employee.Name.Trim() == string.Empty ? string.Empty : "姓名:" + employee.Name;
            string department = employee.Department.Trim() == string.Empty ? string.Empty : "  部门:" + employee.Department;
            string workGroup = employee.WorkGroup.Trim() == string.Empty ? string.Empty : "  班组:" + employee.WorkGroup;
            string post = employee.Post.Trim() == string.Empty ? string.Empty : "  岗位:" + employee.Post;
            this.lblEmployeeMessage.Text = name + department + workGroup + post;
            this.lblEmployeeMessage.ForeColor = Color.Black;
            this.tbHours.Focus();
        }

        private void HoursScan()
        {
            this.tbHours.Text = this.tbHours.Text.Trim() == string.Empty ? "0" : this.tbHours.Text.Trim();
            decimal CurrentWorkingHours = Convert.ToDecimal(this.tbHours.Text);
            bool isNewRow = true;
            foreach (DataRow dr in this.dt.Rows)
            {
                if (dr[0].ToString().Trim().ToLower() == this.tbEmployee.Text.Trim().ToLower())
                {
                    DialogResult dialogResult = MessageBox.Show(this, "该雇员工时已经录入,继续操作数量将累加!", "Warning", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    switch (dialogResult)
                    {
                        case DialogResult.Yes:
                            this.currentRowIndex = this.dt.Rows.IndexOf(dr);
                            this.dt.Rows[this.currentRowIndex][2] = (CurrentWorkingHours + Convert.ToDecimal(this.dt.Rows[currentRowIndex][2].ToString())).ToString("0.########");
                            this.dt.Rows[this.currentRowIndex][3] = DateTime.Now;
                            isNewRow = false;
                            break;
                        case DialogResult.No:
                            this.tbEmployee.Text = string.Empty;
                            this.tbHours.Text = string.Empty;
                            this.tbEmployee.Focus();
                            return;
                        default:
                            break;
                    }
                    break;
                }
            }
            if (isNewRow && CurrentWorkingHours > 0 && this.employee != null)
            {
                DataRow dr = this.dt.NewRow();
                dr[0] = this.employee.Code;
                dr[1] = this.employee.Name;
                dr[2] = this.tbHours.Text;
                dr[3] = DateTime.Now;
                this.dt.Rows.InsertAt(dr, dt.Rows.Count);
                this.lblEmployeeMessage.Text = "新增雇员工时成功!";
                this.lblEmployeeMessage.ForeColor = Color.Green;
            }
            else if (!isNewRow)
            {
                if (Convert.ToDecimal(this.dt.Rows[this.currentRowIndex][2]) <= 0)
                {
                    this.dt.Rows.RemoveAt(this.currentRowIndex);
                    this.lblEmployeeMessage.Text = "删除雇员" + this.employee.Name + "工时成功!";
                    this.lblEmployeeMessage.ForeColor = Color.Black;
                }
                else
                {
                    this.lblEmployeeMessage.Text = "累加雇员工时成功!";
                    this.lblEmployeeMessage.ForeColor = Color.Green;
                }
            }

            this.gvEmployee.ClearSelection();
            //this.gvEmployee.Rows[0].Selected = true;
            this.tbEmployee.Text = string.Empty;
            this.tbHours.Text = string.Empty;
            this.tbEmployee.Focus();
        }

        private void gvWODetail_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (this.gvWODetail.CurrentCell != null
                    && this.gvWODetail.CurrentCell.ColumnIndex + 1 == this.gvWODetail.ColumnCount
                    && this.gvWODetail.CurrentCell.RowIndex + 1 == this.gvWODetail.RowCount)
                {
                    SendKeys.Send("{Tab}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "不可预知的错误!请与管理员联系", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveOrder()
        {
            try
            {
                //支持选择某工单下线，但不支持多张工单同时下线
                string orderNo = this.gvWODetail.Rows[this.gvWODetail.CurrentCell.RowIndex].Cells[0].Value.ToString();
                resolver.Transformers = ((Transformer[])this.gvWODetail.DataSource).Where(t => t.OrderNo == orderNo).ToArray();
                                       
                if (this.resolver.Transformers != null && this.resolver.Transformers.Length > 0)
                {
                    List<string[]> workingHoursList = new List<string[]>();
                    foreach (DataRow dr in this.dt.Rows)
                    {
                        string[] stringArray = new string[2];
                        stringArray[0] = dr[0].ToString();
                        stringArray[1] = dr[2].ToString();
                        workingHoursList.Add(stringArray);
                    }
                    this.resolver.WorkingHours = workingHoursList.ToArray();
                    this.resolver.Input = BusinessConstants.BARCODE_SPECIAL_MARK + BusinessConstants.BARCODE_HEAD_OK;
                    this.resolver = TheClientMgr.ScanBarcode(this.resolver);
                    if ((this.resolver.AutoPrintHu || this.resolver.NeedPrintReceipt || this.resolver.NeedInspection)
                    && this.resolver.PrintUrl != null && this.resolver.PrintUrl != string.Empty)
                    {
                        string[] printUrlArray = this.resolver.PrintUrl.Split('|');
                        foreach (string printUrl in printUrlArray)
                        {
                            Utility.PrintOrder(printUrl, this);
                        }
                    }
                    this.InitialAll();
                    this.lblMessage.Text = "收货成功!";
                    this.lblMessage.ForeColor = Color.Green;
                    this.WorkOrderScan();

                }
                else
                {
                    MessageBox.Show(this, "没有可收的货物!", "收货失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (FaultException ex)
            {
                MessageBox.Show(this, Utility.FormatExMessage(ex.Message));
            }
            catch (Exception ex)
            {
                this.InitialAll();
                MessageBox.Show(this, ex.Message, "收货失败", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitialAll()
        {
            this.dt = new DataTable();
            this.employee = new Employee();
            this.resolver = new Resolver();
            this.dt.Columns.Add(new DataColumn("EmployeeCode", Type.GetType("System.String")));
            this.dt.Columns.Add(new DataColumn("EmployeeName", Type.GetType("System.String")));
            this.dt.Columns.Add(new DataColumn("EmployeeWorkingHours", Type.GetType("System.String")));
            this.dt.Columns.Add(new DataColumn("ScanTime", Type.GetType("System.DateTime")));
            this.gvEmployee.DataSource = this.dt;
            this.dt.DefaultView.Sort = "ScanTime DESC";

            this.lblMessage.Text = string.Empty;
            this.lblEmployeeMessage.Text = string.Empty;
            this.tbFlow.Text = string.Empty;
            this.currentRowIndex = -1;
            this.gvOrderDataBind();
            this.tbFlow.Focus();
        }

        #region 过滤输入
        private void gvWODetail_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl EditingControl = (DataGridViewTextBoxEditingControl)e.Control;
            EditingControl.KeyPress += new KeyPressEventHandler(Utility.DataGridViewDecimalFilter);
        }

        private void tbHours_KeyPress(object sender, KeyPressEventArgs e)
        {
            Utility.TextBoxDecimalFilter(sender, e);
        }
        #endregion

        private void btnOffline_Click(object sender, EventArgs e)
        {
            ReceiveOrder();
        }

        private void gvWODetail_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void gvWODetail_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                DataGridView gvWODetail = (DataGridView)sender;
                if (gvWODetail.SelectedRows.Count != 0)
                    ExtraSelectBySameOrderNo(gvWODetail);
            }
            catch
            {

            }
        }

        private void ExtraSelectBySameOrderNo(DataGridView gv)
        {
            if (gv.SelectedRows.Count > 0)
            {
                string strSameOrder;
                if (this.gvWODetail.CurrentCell.RowIndex + 1 < this.gvWODetail.Rows.Count
                        && this.gvWODetail.CurrentCell.ColumnIndex == 9)
                    strSameOrder = gv.Rows[gv.CurrentRow.Index + 1].Cells[0].Value.ToString();
                else
                    strSameOrder = gv.CurrentRow.Cells[0].Value.ToString();
                foreach (DataGridViewRow gvRow in gv.Rows)
                {
                    if (strSameOrder.Equals(gvRow.Cells[0].Value.ToString()))
                    {
                        gvRow.Selected = true;
                    }
                }

            }

        }
        //默认反算
        private void gvWODetail_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.ColumnIndex < 9)
            {
                decimal orderQty = Convert.ToDecimal(this.gvWODetail.Rows[e.RowIndex].Cells["OrderedQty"].FormattedValue);
                decimal recQty = Convert.ToDecimal(this.gvWODetail.Rows[e.RowIndex].Cells["ReceivedQty"].FormattedValue);
                decimal scrapQty = Convert.ToDecimal(this.gvWODetail.Rows[e.RowIndex].Cells["ScrapQty"].FormattedValue);
                decimal rejectedQty = Convert.ToDecimal(this.gvWODetail.Rows[e.RowIndex].Cells["RejectedQty"].FormattedValue);

                if (e.ColumnIndex == 7)
                    scrapQty = Convert.ToDecimal(e.FormattedValue);
                else if (e.ColumnIndex == 8)
                    rejectedQty = Convert.ToDecimal(e.FormattedValue);

                this.gvWODetail.Rows[e.RowIndex].Cells["CurrentQty"].Value = Convert.ToString(orderQty - recQty - scrapQty - rejectedQty);
            }
        }

    }
}
