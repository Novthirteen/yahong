namespace Sconit_CS
{
    partial class UCPrintMonitor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.OrderNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreateDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CreateUser = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Status = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.PrintUrl = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.PurchaseCheckBox = new System.Windows.Forms.CheckBox();
            this.ProductionCheckBox = new System.Windows.Forms.CheckBox();
            this.InspectCheckBox = new System.Windows.Forms.CheckBox();
            this.PickListCheckBox = new System.Windows.Forms.CheckBox();
            this.ASNCheckBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OrderNo,
            this.CreateDate,
            this.CreateUser,
            this.Status,
            this.PrintUrl});
            this.dataGridView1.Location = new System.Drawing.Point(12, 22);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 23;
            this.dataGridView1.Size = new System.Drawing.Size(857, 425);
            this.dataGridView1.TabIndex = 0;
            // 
            // OrderNo
            // 
            this.OrderNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.OrderNo.DataPropertyName = "OrderNo";
            this.OrderNo.HeaderText = "订单号";
            this.OrderNo.Name = "OrderNo";
            this.OrderNo.ReadOnly = true;
            // 
            // CreateDate
            // 
            this.CreateDate.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CreateDate.DataPropertyName = "CreateDate";
            this.CreateDate.HeaderText = "创建日期";
            this.CreateDate.Name = "CreateDate";
            this.CreateDate.ReadOnly = true;
            // 
            // CreateUser
            // 
            this.CreateUser.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.CreateUser.DataPropertyName = "CreateUser";
            this.CreateUser.HeaderText = "创建人";
            this.CreateUser.Name = "CreateUser";
            this.CreateUser.ReadOnly = true;
            // 
            // Status
            // 
            this.Status.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Status.DataPropertyName = "Status";
            this.Status.HeaderText = "状态";
            this.Status.Name = "Status";
            this.Status.ReadOnly = true;
            // 
            // PrintUrl
            // 
            this.PrintUrl.HeaderText = "PrintUrl";
            this.PrintUrl.Name = "PrintUrl";
            this.PrintUrl.ReadOnly = true;
            this.PrintUrl.Visible = false;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 10000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // PurchaseCheckBox
            // 
            this.PurchaseCheckBox.AutoSize = true;
            this.PurchaseCheckBox.Location = new System.Drawing.Point(12, 3);
            this.PurchaseCheckBox.Name = "PurchaseCheckBox";
            this.PurchaseCheckBox.Size = new System.Drawing.Size(60, 16);
            this.PurchaseCheckBox.TabIndex = 1;
            this.PurchaseCheckBox.Text = "要货单";
            this.PurchaseCheckBox.UseVisualStyleBackColor = true;
            this.PurchaseCheckBox.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // ProductionCheckBox
            // 
            this.ProductionCheckBox.AutoSize = true;
            this.ProductionCheckBox.Location = new System.Drawing.Point(96, 3);
            this.ProductionCheckBox.Name = "ProductionCheckBox";
            this.ProductionCheckBox.Size = new System.Drawing.Size(60, 16);
            this.ProductionCheckBox.TabIndex = 1;
            this.ProductionCheckBox.Text = "生产单";
            this.ProductionCheckBox.UseVisualStyleBackColor = true;
            // 
            // InspectCheckBox
            // 
            this.InspectCheckBox.AutoSize = true;
            this.InspectCheckBox.Location = new System.Drawing.Point(180, 3);
            this.InspectCheckBox.Name = "InspectCheckBox";
            this.InspectCheckBox.Size = new System.Drawing.Size(60, 16);
            this.InspectCheckBox.TabIndex = 1;
            this.InspectCheckBox.Text = "检验单";
            this.InspectCheckBox.UseVisualStyleBackColor = true;
            // 
            // PickListCheckBox
            // 
            this.PickListCheckBox.AutoSize = true;
            this.PickListCheckBox.Location = new System.Drawing.Point(264, 3);
            this.PickListCheckBox.Name = "PickListCheckBox";
            this.PickListCheckBox.Size = new System.Drawing.Size(60, 16);
            this.PickListCheckBox.TabIndex = 1;
            this.PickListCheckBox.Text = "拣货单";
            this.PickListCheckBox.UseVisualStyleBackColor = true;
            // 
            // ASNCheckBox
            // 
            this.ASNCheckBox.AutoSize = true;
            this.ASNCheckBox.Location = new System.Drawing.Point(351, 3);
            this.ASNCheckBox.Name = "ASNCheckBox";
            this.ASNCheckBox.Size = new System.Drawing.Size(42, 16);
            this.ASNCheckBox.TabIndex = 1;
            this.ASNCheckBox.Text = "ASN";
            this.ASNCheckBox.UseVisualStyleBackColor = true;
            // 
            // UCPrintMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ASNCheckBox);
            this.Controls.Add(this.PickListCheckBox);
            this.Controls.Add(this.InspectCheckBox);
            this.Controls.Add(this.ProductionCheckBox);
            this.Controls.Add(this.PurchaseCheckBox);
            this.Controls.Add(this.dataGridView1);
            this.Name = "UCPrintMonitor";
            this.Size = new System.Drawing.Size(881, 456);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox PurchaseCheckBox;
        private System.Windows.Forms.CheckBox ProductionCheckBox;
        private System.Windows.Forms.CheckBox InspectCheckBox;
        private System.Windows.Forms.CheckBox PickListCheckBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreateDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn CreateUser;
        private System.Windows.Forms.DataGridViewTextBoxColumn Status;
        private System.Windows.Forms.DataGridViewTextBoxColumn PrintUrl;
        private System.Windows.Forms.CheckBox ASNCheckBox;
    }
}
