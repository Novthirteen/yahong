namespace Sconit_CS
{
    partial class UCTransfer
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblMessage = new System.Windows.Forms.Label();
            this.cbItemCode = new System.Windows.Forms.ComboBox();
            this.lbItemCode = new System.Windows.Forms.Label();
            this.gbItemGroup = new System.Windows.Forms.GroupBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.tbQty = new System.Windows.Forms.TextBox();
            this.lbQty = new System.Windows.Forms.Label();
            this.lblFlowCode = new System.Windows.Forms.Label();
            this.tbFlowCode = new System.Windows.Forms.TextBox();
            this.lbHelp = new System.Windows.Forms.Label();
            this.gpOrderDetail = new System.Windows.Forms.GroupBox();
            this.dgvOrderDetail = new System.Windows.Forms.DataGridView();
            this.colSeq = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colItemCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUnit = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbItemGroup.SuspendLayout();
            this.gpOrderDetail.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderDetail)).BeginInit();
            this.SuspendLayout();
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMessage.Location = new System.Drawing.Point(348, 29);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(49, 20);
            this.lblMessage.TabIndex = 20;
            this.lblMessage.Text = "Desc";
            // 
            // cbItemCode
            // 
            this.cbItemCode.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cbItemCode.FormattingEnabled = true;
            this.cbItemCode.Location = new System.Drawing.Point(102, 72);
            this.cbItemCode.Name = "cbItemCode";
            this.cbItemCode.Size = new System.Drawing.Size(240, 40);
            this.cbItemCode.TabIndex = 18;
            // 
            // lbItemCode
            // 
            this.lbItemCode.AutoSize = true;
            this.lbItemCode.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbItemCode.Location = new System.Drawing.Point(20, 79);
            this.lbItemCode.Name = "lbItemCode";
            this.lbItemCode.Size = new System.Drawing.Size(80, 28);
            this.lbItemCode.TabIndex = 15;
            this.lbItemCode.Text = "物料号:";
            // 
            // gbItemGroup
            // 
            this.gbItemGroup.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbItemGroup.Controls.Add(this.btnConfirm);
            this.gbItemGroup.Controls.Add(this.lblMessage);
            this.gbItemGroup.Controls.Add(this.cbItemCode);
            this.gbItemGroup.Controls.Add(this.lbItemCode);
            this.gbItemGroup.Controls.Add(this.tbQty);
            this.gbItemGroup.Controls.Add(this.lbQty);
            this.gbItemGroup.Controls.Add(this.lblFlowCode);
            this.gbItemGroup.Controls.Add(this.tbFlowCode);
            this.gbItemGroup.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gbItemGroup.Location = new System.Drawing.Point(126, 12);
            this.gbItemGroup.Name = "gbItemGroup";
            this.gbItemGroup.Size = new System.Drawing.Size(850, 120);
            this.gbItemGroup.TabIndex = 26;
            this.gbItemGroup.TabStop = false;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConfirm.Location = new System.Drawing.Point(743, 69);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(101, 45);
            this.btnConfirm.TabIndex = 25;
            this.btnConfirm.Text = "移  库";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // tbQty
            // 
            this.tbQty.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbQty.Location = new System.Drawing.Point(415, 72);
            this.tbQty.Name = "tbQty";
            this.tbQty.Size = new System.Drawing.Size(200, 39);
            this.tbQty.TabIndex = 19;
            this.tbQty.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbQty_KeyPress);
            // 
            // lbQty
            // 
            this.lbQty.AutoSize = true;
            this.lbQty.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbQty.Location = new System.Drawing.Point(354, 79);
            this.lbQty.Name = "lbQty";
            this.lbQty.Size = new System.Drawing.Size(59, 28);
            this.lbQty.TabIndex = 14;
            this.lbQty.Text = "数量:";
            // 
            // lblFlowCode
            // 
            this.lblFlowCode.AutoSize = true;
            this.lblFlowCode.Font = new System.Drawing.Font("微软雅黑", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblFlowCode.Location = new System.Drawing.Point(22, 24);
            this.lblFlowCode.Name = "lblFlowCode";
            this.lblFlowCode.Size = new System.Drawing.Size(77, 28);
            this.lblFlowCode.TabIndex = 0;
            this.lblFlowCode.Text = "路   线:";
            // 
            // tbFlowCode
            // 
            this.tbFlowCode.Font = new System.Drawing.Font("Arial", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tbFlowCode.Location = new System.Drawing.Point(102, 17);
            this.tbFlowCode.Name = "tbFlowCode";
            this.tbFlowCode.Size = new System.Drawing.Size(240, 39);
            this.tbFlowCode.TabIndex = 1;
            this.tbFlowCode.Click += new System.EventHandler(this.tbFlowCode_Click);
            // 
            // lbHelp
            // 
            this.lbHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbHelp.AutoSize = true;
            this.lbHelp.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbHelp.ForeColor = System.Drawing.Color.Blue;
            this.lbHelp.Location = new System.Drawing.Point(28, 642);
            this.lbHelp.Name = "lbHelp";
            this.lbHelp.Size = new System.Drawing.Size(477, 16);
            this.lbHelp.TabIndex = 28;
            this.lbHelp.Text = "CTRL+ENTER 确认, CTRL+P 重新打印, CTRL+BACKSPACE 取消";
            // 
            // gpOrderDetail
            // 
            this.gpOrderDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gpOrderDetail.Controls.Add(this.dgvOrderDetail);
            this.gpOrderDetail.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.gpOrderDetail.Location = new System.Drawing.Point(20, 138);
            this.gpOrderDetail.Name = "gpOrderDetail";
            this.gpOrderDetail.Size = new System.Drawing.Size(956, 501);
            this.gpOrderDetail.TabIndex = 27;
            this.gpOrderDetail.TabStop = false;
            // 
            // dgvOrderDetail
            // 
            this.dgvOrderDetail.AllowUserToAddRows = false;
            this.dgvOrderDetail.AllowUserToDeleteRows = false;
            this.dgvOrderDetail.AllowUserToResizeColumns = false;
            this.dgvOrderDetail.AllowUserToResizeRows = false;
            this.dgvOrderDetail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvOrderDetail.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("宋体", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvOrderDetail.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvOrderDetail.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrderDetail.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colSeq,
            this.colItemCode,
            this.colDescription,
            this.colUC,
            this.colUnit,
            this.colQty});
            this.dgvOrderDetail.Location = new System.Drawing.Point(11, 25);
            this.dgvOrderDetail.Name = "dgvOrderDetail";
            this.dgvOrderDetail.ReadOnly = true;
            this.dgvOrderDetail.RowHeadersVisible = false;
            this.dgvOrderDetail.RowTemplate.DefaultCellStyle.Font = new System.Drawing.Font("宋体", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.dgvOrderDetail.RowTemplate.Height = 30;
            this.dgvOrderDetail.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOrderDetail.Size = new System.Drawing.Size(930, 461);
            this.dgvOrderDetail.TabIndex = 25;
            this.dgvOrderDetail.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvOrderDetail_CellClick);
            // 
            // colSeq
            // 
            this.colSeq.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colSeq.DataPropertyName = "Sequence";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("宋体", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.colSeq.DefaultCellStyle = dataGridViewCellStyle2;
            this.colSeq.HeaderText = "序号";
            this.colSeq.Name = "colSeq";
            this.colSeq.ReadOnly = true;
            this.colSeq.Width = 65;
            // 
            // colItemCode
            // 
            this.colItemCode.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colItemCode.DataPropertyName = "ItemCode";
            dataGridViewCellStyle3.Font = new System.Drawing.Font("宋体", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.colItemCode.DefaultCellStyle = dataGridViewCellStyle3;
            this.colItemCode.HeaderText = "物料号";
            this.colItemCode.Name = "colItemCode";
            this.colItemCode.ReadOnly = true;
            this.colItemCode.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colItemCode.Width = 81;
            // 
            // colDescription
            // 
            this.colDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.colDescription.DataPropertyName = "ItemDescription";
            dataGridViewCellStyle4.Font = new System.Drawing.Font("宋体", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.colDescription.DefaultCellStyle = dataGridViewCellStyle4;
            this.colDescription.HeaderText = "物料描述";
            this.colDescription.Name = "colDescription";
            this.colDescription.ReadOnly = true;
            // 
            // colUC
            // 
            this.colUC.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colUC.DataPropertyName = "UnitCount";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("宋体", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle5.Format = "0.########";
            this.colUC.DefaultCellStyle = dataGridViewCellStyle5;
            this.colUC.HeaderText = "单包装";
            this.colUC.Name = "colUC";
            this.colUC.ReadOnly = true;
            this.colUC.Width = 81;
            // 
            // colUnit
            // 
            this.colUnit.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colUnit.DataPropertyName = "UomCode";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("宋体", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.colUnit.DefaultCellStyle = dataGridViewCellStyle6;
            this.colUnit.HeaderText = "单位";
            this.colUnit.Name = "colUnit";
            this.colUnit.ReadOnly = true;
            this.colUnit.Width = 65;
            // 
            // colQty
            // 
            this.colQty.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.colQty.DataPropertyName = "CurrentQty";
            dataGridViewCellStyle7.Font = new System.Drawing.Font("宋体", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle7.Format = "0.########";
            dataGridViewCellStyle7.NullValue = null;
            this.colQty.DefaultCellStyle = dataGridViewCellStyle7;
            this.colQty.HeaderText = "数量";
            this.colQty.Name = "colQty";
            this.colQty.ReadOnly = true;
            this.colQty.Width = 65;
            // 
            // UCTransfer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbItemGroup);
            this.Controls.Add(this.lbHelp);
            this.Controls.Add(this.gpOrderDetail);
            this.Name = "UCTransfer";
            this.Size = new System.Drawing.Size(995, 678);
            this.gbItemGroup.ResumeLayout(false);
            this.gbItemGroup.PerformLayout();
            this.gpOrderDetail.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderDetail)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ComboBox cbItemCode;
        private System.Windows.Forms.Label lbItemCode;
        private System.Windows.Forms.GroupBox gbItemGroup;
        private System.Windows.Forms.TextBox tbQty;
        private System.Windows.Forms.Label lbQty;
        private System.Windows.Forms.Label lblFlowCode;
        private System.Windows.Forms.TextBox tbFlowCode;
        private System.Windows.Forms.Label lbHelp;
        private System.Windows.Forms.GroupBox gpOrderDetail;
        private System.Windows.Forms.DataGridView dgvOrderDetail;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSeq;
        private System.Windows.Forms.DataGridViewTextBoxColumn colItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUC;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUnit;
        private System.Windows.Forms.DataGridViewTextBoxColumn colQty;
    }
}
