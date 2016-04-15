using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.ServiceModel;
using Sconit_CS.SconitWS;

namespace Sconit_CS
{
    public partial class UCSequenceOnline : UserControl
    {
        private Resolver resolver;
        private ClientMgrWSSoapClient TheClientMgr = new ClientMgrWSSoapClient();
        private string currentFlowCode;
        public UCSequenceOnline(User user, string moduleType)
        {
            InitializeComponent();
            this.gvSubmitWO.AutoGenerateColumns = false;
            this.gvInProcessWO.AutoGenerateColumns = false;
            this.resolver = new Resolver();
            this.resolver.UserCode = user.Code;
            this.resolver.ModuleType = BusinessConstants.TRANSFORMER_MODULE_TYPE_ONLINE;
            this.lblmessage.Text = string.Empty;
        }

        private void UCSequenceOnline_Load(object sender, EventArgs e)
        { }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Enter))
            {
                if (this.gvSubmitWO.Focused)
                {
                    ExecuteOnline();
                    return true;
                }
            }

            if (keyData == Keys.Enter)
            {
                if (this.tbFlow.Focused)
                {
                    this.lblmessage.Text = string.Empty;
                    currentFlowCode = this.tbFlow.Text.Trim();
                    this.GetDataSource(currentFlowCode);
                    return true;
                }
                else if (this.gvSubmitWO.Focused)
                {
                    //this.SelectDataGridView(gvInProcessWO);
                    if (this.gvSubmitWO.CurrentRow.Index == this.gvSubmitWO.Rows.Count - 1)
                    {
                        this.gvSubmitWO.ClearSelection();
                        this.gvInProcessWO.Focus();
                        this.gvInProcessWO.CurrentCell = this.gvInProcessWO[0, 0];
                    }
                }
                else if (this.gvInProcessWO.Focused)
                {
                    //this.SelectDataGridView(gvSubmitWO);
                    if (this.gvInProcessWO.CurrentRow.Index == this.gvInProcessWO.Rows.Count - 1)
                    {
                        this.gvInProcessWO.ClearSelection();
                        this.gvSubmitWO.Focus();
                        this.gvSubmitWO.CurrentCell = this.gvSubmitWO[0, 0];
                    }
                }
            }

            if (keyData == (Keys.Control | Keys.F))
            {
                this.GetDataSource(currentFlowCode);
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }


        private void GVDataBind()
        {
            if (this.resolver.Transformers == null)
            {
                this.resolver.Transformers = new Transformer[] { };
            }
            this.gvSubmitWO.DataSource = resolver.Transformers.Where(t => t.Status == "Submit").ToArray();
            this.gvInProcessWO.DataSource = resolver.Transformers.Where(t => t.Status == "In-Process").ToArray();
            this.gvSubmitWO.Focus();
            if (gvSubmitWO.Rows.Count > 0)
                this.gvSubmitWO.Rows[0].Selected = true;
            this.tbFlow.Text = string.Empty;
        }

        private void ExecuteOnline()
        {
            string woNo = this.gvSubmitWO.SelectedRows[0].Cells["WONo1"].Value.ToString();
            resolver.Input = woNo;
            TheClientMgr.ScanBarcode(resolver);
            this.lblmessage.Text = "工单:" + woNo + "上线成功！";
            this.lblmessage.ForeColor = System.Drawing.Color.Red;
            //刷新DataGrid
            this.GetDataSource(currentFlowCode);
        }

        private void GV_SelectionChanged(object sender, EventArgs e)
        {
            //try
            //{
            //    DataGridView OrderGV = (DataGridView)sender;
            //    OrderGV.Rows[OrderGV.CurrentCell.RowIndex].Selected = true;
            //    if (OrderGV.SelectedRows.Count != 0)
            //        ExtraSelectBySameOrderNo(OrderGV);
            //}
            //catch
            //{

            //}
        }

        private void ExtraSelectBySameOrderNo(DataGridView gv)
        {
            if (gv.SelectedRows.Count > 0)
            {
                string strSameSeq = gv.CurrentRow.Cells[0].Value.ToString();
                foreach (DataGridViewRow gvRow in gv.Rows)
                {
                    if (strSameSeq.Equals(gvRow.Cells[0].Value.ToString()))
                    {
                        gvRow.Selected = true;
                    }
                }

            }

        }

        private void GetDataSource(string flowCode)
        {
            try
            {
                this.resolver.Input = flowCode;
                this.resolver = TheClientMgr.ScanBarcode(resolver);
                this.GVDataBind();
            }
            catch (Exception ex)
            {
                lblmessage.Text = Utility.FormatExMessage(ex.Message);
            }
        }

        //private void SelectDataGridView(DataGridView gv)
        //{
        //    gv.Select();
        //    if (gv.Rows.Count != 0)
        //    {
        //        gv.Rows[0].Selected = true;
        //        ExtraSelectBySameOrderNo(gv);
        //    }
        //}
        //todo 添加空缸产品
        private void InsertEmptyContainer()
        { }


        //todo 终端上支持排序
        private void SortingOrder()
        { }
    }
}
