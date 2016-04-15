using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ServiceModel;
using Sconit_CS.SconitWS;

namespace Sconit_CS
{
    public partial class UCInspection : UserControl
    {
        protected ClientMgrWSSoapClient TheClientMgr;
        protected Resolver resolver;

        public UCInspection(User user, string moduleType)
        {
            InitializeComponent();
            this.resolver = new Resolver();
            this.resolver.UserCode = user.Code;
            this.resolver.ModuleType = moduleType;
            this.gvList.AutoGenerateColumns = false;
            this.TheClientMgr = new ClientMgrWSSoapClient();
            this.lblMessage.Text = string.Empty;
            this.lblResult.Text = string.Empty;
            this.InitialAll();
        }

        protected virtual void InitialAll()
        {
            this.gvList.Visible = true;
            this.resolver.Transformers = null;
            this.resolver.Result = null;
            this.resolver.BinCode = null;
            this.resolver.Code = null;
            this.resolver.CodePrefix = null;
            this.resolver.PrintUrl = null;
            this.resolver.FlowCode = null;
            this.tbBarCode.Text = string.Empty;
            this.tbBarCode.Focus();
            this.DataBind();
        }

        protected virtual void DataBind()
        {
            if (this.resolver.Transformers == null)
            {
                this.resolver.Transformers = new Transformer[] { };
            }
            this.gvList.DataSource = new BindingList<Transformer>(this.resolver.Transformers);
            if (this.resolver != null)
            {
                if (this.gvList.CurrentCell != null)
                {
                    int index = this.gvList.CurrentCell.RowIndex;
                    this.gvList.Columns["CurrentQty"].Visible = true;
                    this.gvList.CurrentCell = this.gvList.Rows[index].Cells["CurrentQty"];
                }
                this.gvList.Columns["CurrentQty"].ReadOnly = false;
            }
            this.gvList = Utility.RenderDataGridViewBackColor(this.gvList);
            this.gvList_CellValueChanged(null, null);
        }

        private void gvList_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            foreach (DataGridViewRow row in this.gvList.Rows)
            {
                decimal CurrentQty = Convert.ToDecimal(row.Cells["CurrentQty"].Value.ToString());
                decimal CurrentRejectQty = 0;
                try
                {
                    CurrentRejectQty = Convert.ToDecimal(row.Cells["CurrentRejectQty"].Value.ToString());
                }
                catch (Exception) { }

                decimal Qty = Convert.ToDecimal(row.Cells["Qty"].Value.ToString());
                if (CurrentQty + CurrentRejectQty == Qty)
                {
                    row.DefaultCellStyle.ForeColor = Color.Black;
                }
                else if (CurrentQty + CurrentRejectQty > Qty)
                {
                    row.DefaultCellStyle.ForeColor = Color.OrangeRed;
                }
                else if (CurrentQty + CurrentRejectQty < Qty)
                {
                    row.DefaultCellStyle.ForeColor = Color.Green;
                }
            }
        }

        private void gvList_CurrentCellChanged(object sender, EventArgs e)
        {

        }

        private void gvList_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            DataGridViewTextBoxEditingControl EditingControl = (DataGridViewTextBoxEditingControl)e.Control;
            EditingControl.KeyPress += new KeyPressEventHandler(Utility.DataGridViewDecimalFilter);
        }

        private void gvList_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

        }

        private void gvList_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void gvList_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show(this, "请输入数字");
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.Enter))
                {
                    this.OrderConfirm();
                    return true;
                }
                else if (keyData == Keys.Escape)
                {
                    this.InitialAll();
                    return true;
                }
                if (this.tbBarCode.Focused)
                {
                    if (keyData == Keys.Enter)
                    {
                        this.BarCodeScan();
                        return true;
                    }
                }
                else if (this.btnConfirm.Focused)
                {
                    if (keyData == Keys.Enter)
                    {
                        if (this.tbBarCode.Text.Trim() != string.Empty)
                        {
                            this.BarCodeScan();
                        }
                        else
                        {
                            this.OrderConfirm();
                        }
                        return true;
                    }
                }
                else if (this.gvList.Focused)
                {
                    if (keyData == Keys.Enter)
                    {
                        this.btnConfirm.Focus();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                this.InitialAll();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        protected void BarCodeScan()
        {
            try
            {
                this.tbBarCode.Text = this.tbBarCode.Text.Trim();
                #region 当输入框为空时,按回车焦点跳转
                if (this.tbBarCode.Text.Trim() == string.Empty && this.gvList.Rows.Count > 0)
                {
                    this.gvList.Focus();
                    this.gvList.BeginEdit(true);
                    return;
                }
                if (this.tbBarCode.Text.Trim() == string.Empty)
                {
                    return;
                }
                #endregion

                this.resolver.Input = this.tbBarCode.Text;
                this.resolver = TheClientMgr.ScanBarcode(this.resolver);
                this.gvList.Focus();
                this.DataBind();
            }
            catch (FaultException ex)
            {
                string messageText = Utility.FormatExMessage(ex.Message);
                this.lblMessage.Text = messageText;
                MessageBox.Show(this, messageText);
                this.DataBind();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "程序内部错误,请与管理员联系", MessageBoxButtons.OK, MessageBoxIcon.Error);
                InitialAll();
            }
        }

        protected virtual void OrderConfirm()
        {
            try
            {
                bool isFullfill = true;

                if (resolver != null && resolver.Transformers != null)
                {
                    foreach (Transformer transformer in resolver.Transformers)
                    {
                        if (transformer != null)
                        {
                            if (transformer.CurrentQty + transformer.CurrentRejectQty != transformer.Qty)
                            {
                                isFullfill = false;
                            }
                        }
                    }
                }

                if (!isFullfill)
                {
                    this.DataBind();
                    this.lblMessage.Text = "检验明细中的合格数加上不合格数不等于待验数.";
                    MessageBox.Show(this, "检验明细中的合格数加上不合格数不等于待验数.", "检验明细不完全.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.tbBarCode.Focus();
                    return;
                }

                Utility.RemoveLot(resolver);
                this.resolver.Input = BusinessConstants.BARCODE_SPECIAL_MARK + BusinessConstants.BARCODE_HEAD_OK;
                if (resolver.ModuleType == BusinessConstants.TRANSFORMER_MODULE_TYPE_TRANSFER)
                    resolver.IOType = BusinessConstants.CODE_MASTER_ORDER_SUB_TYPE_VALUE_NML;
                this.resolver = TheClientMgr.ScanBarcode(this.resolver);
                this.lblMessage.Text = this.resolver.Result;
                if (this.resolver.PrintUrl != null && this.resolver.PrintUrl != string.Empty)
                {
                    string[] printUrlArray = this.resolver.PrintUrl.Split('|');
                    foreach (string printUrl in printUrlArray)
                    {
                        Utility.PrintOrder(printUrl, this);
                    }
                }
                InitialAll();
            }
            catch (FaultException ex)
            {
                this.DataBind();
                this.lblMessage.Text = Utility.FormatExMessage(ex.Message);
                MessageBox.Show(this, Utility.FormatExMessage(ex.Message));
                this.tbBarCode.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, "程序内部错误,请与管理员联系", MessageBoxButtons.OK, MessageBoxIcon.Error);
                InitialAll();
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            this.OrderConfirm();
        }
    }
}
