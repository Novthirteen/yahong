using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sconit_CS.SconitWS;
using System.Configuration;

namespace Sconit_CS
{
    public partial class UCTransfer : UserControl
    {
        private ClientMgrWSSoapClient TheClientMgr;
        private Resolver resolver;
        private int currentRowIndex;

        public UCTransfer(User user, string moduleType)
        {
            InitializeComponent();
            this.resolver = new Resolver();
            this.resolver.UserCode = user.Code;
            this.resolver.ModuleType = moduleType;
            this.TheClientMgr = new ClientMgrWSSoapClient();
            this.dgvOrderDetail.AutoGenerateColumns = false;
            this.InitialAll();
        }

        protected override bool ProcessCmdKey(ref   Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Enter))
            {
                this.OrderConfirm();
                return true;
            }
            else if (keyData == (Keys.Control | Keys.P))
            {
                //if (orderCode != string.Empty)
                //{
                //    this.PrintReceiver();
                //}
                return true;
            }

            if (this.tbFlowCode.Focused)
            {
                if (keyData == Keys.Enter)
                {
                    this.FlowCodeScan();
                    return true;
                }
                if (keyData == Keys.Escape)
                {
                    this.InitialAll();
                }
            }

            if (this.cbItemCode.Focused)
            {
                if (keyData == Keys.Enter)
                {
                    this.ItemCodeScan();
                    return true;
                }
                if (keyData == Keys.Escape)
                {
                    this.resolver.Input = BusinessConstants.BARCODE_SPECIAL_MARK + BusinessConstants.BARCODE_HEAD_CANCEL;
                    this.resolver = TheClientMgr.ScanBarcode(this.resolver);
                    this.currentRowIndex = -1;
                    this.dgvOrderDetailDataBind();
                }
            }

            if (tbQty.Focused)
            {
                if (keyData == Keys.Enter)
                {
                    this.InputQty();
                    return true;
                }
                if (keyData == Keys.Escape)
                {
                    this.resolver.Input = BusinessConstants.BARCODE_SPECIAL_MARK + BusinessConstants.BARCODE_HEAD_CANCEL;
                    this.resolver = TheClientMgr.ScanBarcode(this.resolver);
                    this.currentRowIndex = -1;
                    this.dgvOrderDetailDataBind();
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void OrderConfirm()
        {
            try
            {
                if (Utility.IsHasTransformer(this.resolver))
                {
                    this.resolver.IOType = GetOrderSubType();
                    this.resolver.Input = BusinessConstants.BARCODE_SPECIAL_MARK + BusinessConstants.BARCODE_HEAD_OK;
                    this.resolver = TheClientMgr.ScanBarcode(this.resolver);
                    if (this.resolver.PrintUrl != null && this.resolver.PrintUrl != string.Empty && resolver.NeedPrintReceipt)
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            Utility.PrintOrder(this.resolver.PrintUrl, this);
                        }
                    }
                    string message = this.resolver.Result;
                    this.InitialAll();
                    this.lblMessage.Text = message;
                }
                else
                {
                    MessageBox.Show("没有移库的明细!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.tbFlowCode.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, Utility.FormatExMessage(ex.Message));
            }
        }

        private void InitialAll()
        {
            this.resolver.Transformers = null;
            this.resolver.Result = null;
            this.resolver.BinCode = null;
            this.resolver.Code = null;
            this.resolver.CodePrefix = null;
            this.resolver.PrintUrl = null;
            this.resolver.FlowCode = null;

            this.tbFlowCode.Text = string.Empty;
            this.lblMessage.Text = string.Empty;
            this.cbItemCode.Text = string.Empty;
            this.tbQty.Text = string.Empty;
            this.tbFlowCode.Focus();
            this.currentRowIndex = -1;

            this.dgvOrderDetailDataBind();
        }


        private void FlowCodeScan()
        {
            try
            {
                string flowCode = this.tbFlowCode.Text.Trim().ToUpper();
                this.InitialAll();
                if (flowCode == string.Empty)
                {
                    return;
                }
                this.tbFlowCode.Text = flowCode;

                this.resolver.Input = flowCode;
                this.resolver = TheClientMgr.ScanBarcode(this.resolver);

                this.lblMessage.Text = this.resolver.Description;
                this.cbItemCode.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, Utility.FormatExMessage(ex.Message));
                this.tbFlowCode.Text = "";
            }
        }

        private void ItemCodeScan()
        {
            try
            {
                if (this.tbFlowCode.Text.Trim() == string.Empty)
                {
                    this.tbFlowCode.Focus();
                    this.cbItemCode.Text = string.Empty;
                }
                else if (this.cbItemCode.Text.Trim() == string.Empty)
                {
                    this.btnConfirm.Focus();
                }
                else
                {
                    bool isExist = false;
                    string itemCode = this.cbItemCode.Text.Trim();
                    if (itemCode == string.Empty)
                    {
                        return;
                    }
                    this.cbItemCode.Text = itemCode;

                    if (this.resolver.Transformers != null && this.resolver.Transformers.Length > 0)
                    {
                        int maxRow = int.Parse(ConfigurationManager.AppSettings["InvTransMaxRows"]);
                        if (this.resolver.Transformers.Length >= maxRow)
                        {
                            MessageBox.Show("最大行数超出限制!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            this.cbItemCode.Text = "";
                            return;
                        }

                        foreach (Transformer td in this.resolver.Transformers)
                        {
                            if (td.ItemCode.Trim().ToLower() == itemCode.ToLower())
                            {
                                isExist = true;
                                break;
                            }
                        }
                    }
                    if (!isExist)
                    {
                        this.resolver.Input = itemCode;
                        this.resolver = TheClientMgr.ScanBarcode(this.resolver);
                    }
                    else
                    {
                        MessageBox.Show("该零件在订单中已存在,继续操作数量将修改原有数量!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    foreach (Transformer td in this.resolver.Transformers)
                    {
                        if (td.ItemCode.ToLower() == this.cbItemCode.Text.ToLower())
                        {
                            this.currentRowIndex = td.Sequence - 1;
                        }
                    }
                    this.dgvOrderDetailDataBind();
                    this.tbQty.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, Utility.FormatExMessage(ex.Message));
                this.cbItemCode.Text = "";
            }
        }

        private void InputQty()
        {
            if (this.tbFlowCode.Text.Trim() == string.Empty)
            {
                this.tbFlowCode.Focus();
                this.cbItemCode.Text = string.Empty;
                this.tbQty.Text = string.Empty;
            }
            else if (this.cbItemCode.Text.Trim() == string.Empty)
            {
                this.cbItemCode.Focus();
                this.tbQty.Text = string.Empty;
            }
            else if (this.tbQty.Text.Trim() == string.Empty)
            {
                //
            }
            else
            {
                foreach (Transformer td in this.resolver.Transformers)
                {
                    if (td.ItemCode.ToLower() == this.cbItemCode.Text.ToLower())
                    {
                        td.CurrentQty = decimal.Parse(this.tbQty.Text);
                        this.currentRowIndex = td.Sequence - 1;
                    }
                }
                this.cbItemCode.Focus();
                this.cbItemCode.Text = string.Empty;
                this.tbQty.Text = string.Empty;
                this.dgvOrderDetailDataBind();
            }
        }

        private void dgvOrderDetailDataBind()
        {
            List<Transformer> transformers = new List<Transformer>();
            if (this.resolver.Transformers == null)
            {
                this.resolver.Transformers = transformers.ToArray();
            }

            this.dgvOrderDetail.DataSource = new BindingList<Transformer>(this.resolver.Transformers);
            if (currentRowIndex > -1)
            {
                this.dgvOrderDetail.ClearSelection();
                dgvOrderDetail.Rows[currentRowIndex].Selected = true;
            }
        }

        private string GetOrderSubType()
        {
            bool isNml = true;
            bool isRtn = true;
            string orderSubType = string.Empty;
            foreach (Transformer td in this.resolver.Transformers)
            {
                if (td.CurrentQty < 0)
                {
                    isNml = false;
                }

                if (td.CurrentQty > 0)
                {
                    isRtn = false;
                }
            }
            if (isNml)
            {
                orderSubType = BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML;
            }
            else if (isRtn)
            {
                orderSubType = BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_RTN;
            }
            else
            {
                orderSubType = BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_ADJ;
            }
            return orderSubType;
        }

        //只能输入数字 最大值为10000
        private void tbQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            string str;
            if (e.KeyChar.ToString() == "\b")
            {
                e.Handled = false;
                return;
            }
            else
            {
                str = ((TextBox)sender).Text + e.KeyChar.ToString();
            }

            if (Utility.IsDecimal(str) || str == "-")
            {
                if (str != "-" && decimal.Parse(str) > 10000)
                {
                    e.Handled = true;
                }
                else
                {
                    e.Handled = false;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (this.tbQty.Text.Trim() != string.Empty)
            {
                this.InputQty();
            }
            else if (this.cbItemCode.Text.Trim() != string.Empty)
            {
                this.ItemCodeScan();
            }
            else if (this.tbFlowCode.Text.Trim() != string.Empty &&
                (this.resolver.Transformers == null || this.resolver.Transformers.Count() == 0))
            {
                this.FlowCodeScan();
            }
            else
            {
                this.OrderConfirm();
            }
        }

        private void tbFlowCode_Click(object sender, EventArgs e)
        {
            //this.tbFlowCode.Text = string.Empty;
        }

        private void dgvOrderDetail_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex > -1)
            {
                this.cbItemCode.Text = this.dgvOrderDetail.Rows[e.RowIndex].Cells[1].Value.ToString();
                this.tbQty.Focus();
                string qty = this.dgvOrderDetail.Rows[e.RowIndex].Cells[5].Value.ToString();
                if (qty != "0")
                {
                    this.tbQty.Text = qty;
                    this.tbQty.SelectAll();
                }
            }
        }

    }
}
