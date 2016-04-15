using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Sconit_CS.SconitWS;

namespace Sconit_CS
{
    public partial class UCPrintMonitor : UserControl
    {
        private ClientMgrWSSoapClient TheClientMgr;
        private Resolver resolver;
        private List<ReceiptNote> cacheReceiptNotes;

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            try
            {
                if (keyData == (Keys.Control | Keys.P))
                {
                    if (this.dataGridView1 != null && this.dataGridView1.CurrentCell != null)
                    {
                        int currentRowIndex = this.dataGridView1.CurrentCell.RowIndex;
                        if (currentRowIndex > -1)
                        {
                            DoPrint(this.dataGridView1["PrintUrl", currentRowIndex].Value.ToString());
                        }
                    }
                    return true;
                }

            }
            catch (Exception)
            {

            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public UCPrintMonitor(User user, string moduleType)
        {
            InitializeComponent();
            this.resolver = new Resolver();
            this.resolver.UserCode = user.Code;
            this.resolver.ModuleType = moduleType;
            this.TheClientMgr = new ClientMgrWSSoapClient();
            this.dataGridView1.AutoGenerateColumns = false;

            this.resolver.Transformers = null;
            this.resolver.Result = string.Empty;
            this.resolver.BinCode = string.Empty;
            this.resolver.Code = string.Empty;
            this.resolver.CodePrefix = string.Empty;
            this.cacheReceiptNotes = new List<ReceiptNote>();

            timer1_Tick(this, null);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            try
            {
                this.resolver.Input = BusinessConstants.BARCODE_SPECIAL_MARK + BusinessConstants.BARCODE_HEAD_NOTE;

                if (this.PurchaseCheckBox.Checked) {
                    this.resolver.Input += BusinessConstants.PRINT_ORDER_TYPE_PURCHASE;
                }

                if (this.ProductionCheckBox.Checked)
                {
                    if (this.resolver.Input.Length > 2)
                    {
                        this.resolver.Input += "|" + BusinessConstants.PRINT_ORDER_TYPE_PRODUCTION;
                    } 
                    else
                    {
                        this.resolver.Input += BusinessConstants.PRINT_ORDER_TYPE_PRODUCTION;
                    }
                }

                if (this.InspectCheckBox.Checked)
                {
                    if (this.resolver.Input.Length > 2)
                    {
                        this.resolver.Input += "|" + BusinessConstants.PRINT_ORDER_TYPE_INSPECT;
                    }
                    else
                    {
                        this.resolver.Input += BusinessConstants.PRINT_ORDER_TYPE_INSPECT;
                    }
                }

                if (this.PickListCheckBox.Checked)
                {
                    if (this.resolver.Input.Length > 2)
                    {
                        this.resolver.Input += "|" + BusinessConstants.PRINT_ORDER_TYPE_PICKLIST;
                    }
                    else
                    {
                        this.resolver.Input += BusinessConstants.PRINT_ORDER_TYPE_PICKLIST;
                    }
                }

                if (this.ASNCheckBox.Checked)
                {
                    if (this.resolver.Input.Length > 2)
                    {
                        this.resolver.Input += "|" + BusinessConstants.PRINT_ORDER_TYPE_ASN;
                    }
                    else
                    {
                        this.resolver.Input += BusinessConstants.PRINT_ORDER_TYPE_ASN;
                    }
                }

                if (this.resolver.Input.Length > 2)
                {
                    resolver = TheClientMgr.ScanBarcode(resolver);
                    if (resolver.ReceiptNotes != null)
                    {
                        foreach (ReceiptNote receiptNote in resolver.ReceiptNotes)
                        {
                            DoPrint(receiptNote.PrintUrl);
                        }
                    }
                    Databind();
                }
            }
            catch (Exception ex)
            {
                //Utility.Log("Error:UCPrintMonitor:" + ex.Message);
            }
        }

        private void Databind()
        {
            cacheReceiptNotes.AddRange(resolver.ReceiptNotes);

            var query = (from t in this.cacheReceiptNotes orderby t.CreateDate descending select t).Take(100);
            List<ReceiptNote> SelectReceiptNote = query.ToList();
            this.dataGridView1.DataSource = new BindingList<ReceiptNote>(SelectReceiptNote);
        }

        private void DoPrint(string printUrl)
        {
            Utility.PrintOrder(printUrl, this);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
