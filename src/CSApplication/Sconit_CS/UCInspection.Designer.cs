namespace Sconit_CS
{
    partial class UCInspection
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.gbList = new System.Windows.Forms.GroupBox();
            this.gvList = new System.Windows.Forms.DataGridView();
            this.gbPickUp = new System.Windows.Forms.GroupBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.tbBarCode = new System.Windows.Forms.TextBox();
            this.lblResult = new System.Windows.Forms.Label();
            this.lblBarCode = new System.Windows.Forms.Label();
            this.lblMessage = new System.Windows.Forms.Label();
            this.CheckColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ItemCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ItemDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UomCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UnitCount = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LocationToCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Qty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CurrentRejectQty = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.gbList.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gvList)).BeginInit();
            this.gbPickUp.SuspendLayout();
            this.SuspendLayout();
            // 
            // gbList
            // 
            this.gbList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbList.Controls.Add(this.gvList);
            this.gbList.Location = new System.Drawing.Point(12, 116);
            this.gbList.Name = "gbList";
            this.gbList.Size = new System.Drawing.Size(849, 376);
            this.gbList.TabIndex = 8;
            this.gbList.TabStop = false;
            // 
            // gvList
            // 
            this.gvList.AllowUserToAddRows = false;
            this.gvList.AllowUserToDeleteRows = false;
            this.gvList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gvList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.gvList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckColumn,
            this.ItemCode,
            this.ItemDescription,
            this.UomCode,
            this.UnitCount,
            this.LocationToCode,
            this.Qty,
            this.CurrentQty,
            this.CurrentRejectQty});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.Format = "0.########";
            dataGridViewCellStyle8.NullValue = null;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gvList.DefaultCellStyle = dataGridViewCellStyle8;
            this.gvList.Location = new System.Drawing.Point(18, 17);
            this.gvList.MultiSelect = false;
            this.gvList.Name = "gvList";
            this.gvList.RowHeadersVisible = false;
            this.gvList.RowTemplate.Height = 23;
            this.gvList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gvList.Size = new System.Drawing.Size(820, 342);
            this.gvList.TabIndex = 1;
            this.gvList.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvList_CellValueChanged);
            this.gvList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.gvList_CellMouseClick);
            this.gvList.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvList_CellEndEdit);
            this.gvList.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.gvList_EditingControlShowing);
            this.gvList.CurrentCellChanged += new System.EventHandler(this.gvList_CurrentCellChanged);
            this.gvList.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.gvList_DataError);
            // 
            // gbPickUp
            // 
            this.gbPickUp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.gbPickUp.Controls.Add(this.btnConfirm);
            this.gbPickUp.Controls.Add(this.tbBarCode);
            this.gbPickUp.Controls.Add(this.lblResult);
            this.gbPickUp.Controls.Add(this.lblBarCode);
            this.gbPickUp.Controls.Add(this.lblMessage);
            this.gbPickUp.Location = new System.Drawing.Point(131, 9);
            this.gbPickUp.Name = "gbPickUp";
            this.gbPickUp.Size = new System.Drawing.Size(730, 101);
            this.gbPickUp.TabIndex = 7;
            this.gbPickUp.TabStop = false;
            // 
            // btnConfirm
            // 
            this.btnConfirm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnConfirm.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.btnConfirm.Location = new System.Drawing.Point(601, 17);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(123, 45);
            this.btnConfirm.TabIndex = 2;
            this.btnConfirm.Text = "确  定";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // tbBarCode
            // 
            this.tbBarCode.Font = new System.Drawing.Font("Arial", 20F);
            this.tbBarCode.Location = new System.Drawing.Point(80, 21);
            this.tbBarCode.MaxLength = 50;
            this.tbBarCode.Name = "tbBarCode";
            this.tbBarCode.Size = new System.Drawing.Size(380, 38);
            this.tbBarCode.TabIndex = 1;
            // 
            // lblResult
            // 
            this.lblResult.AutoSize = true;
            this.lblResult.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblResult.Location = new System.Drawing.Point(460, 28);
            this.lblResult.Name = "lblResult";
            this.lblResult.Size = new System.Drawing.Size(66, 25);
            this.lblResult.TabIndex = 0;
            this.lblResult.Text = "Result";
            // 
            // lblBarCode
            // 
            this.lblBarCode.AutoSize = true;
            this.lblBarCode.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblBarCode.Location = new System.Drawing.Point(17, 24);
            this.lblBarCode.Name = "lblBarCode";
            this.lblBarCode.Size = new System.Drawing.Size(60, 25);
            this.lblBarCode.TabIndex = 0;
            this.lblBarCode.Text = "条码:";
            // 
            // lblMessage
            // 
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lblMessage.ForeColor = System.Drawing.Color.Red;
            this.lblMessage.Location = new System.Drawing.Point(17, 67);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(93, 25);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "Message";
            // 
            // CheckColumn
            // 
            this.CheckColumn.FillWeight = 50F;
            this.CheckColumn.HeaderText = "";
            this.CheckColumn.Name = "CheckColumn";
            this.CheckColumn.Visible = false;
            // 
            // ItemCode
            // 
            this.ItemCode.DataPropertyName = "ItemCode";
            this.ItemCode.FillWeight = 180F;
            this.ItemCode.HeaderText = "物料号";
            this.ItemCode.Name = "ItemCode";
            this.ItemCode.ReadOnly = true;
            // 
            // ItemDescription
            // 
            this.ItemDescription.DataPropertyName = "ItemDescription";
            this.ItemDescription.FillWeight = 200F;
            this.ItemDescription.HeaderText = "物料描述";
            this.ItemDescription.Name = "ItemDescription";
            this.ItemDescription.ReadOnly = true;
            // 
            // UomCode
            // 
            this.UomCode.DataPropertyName = "UomCode";
            this.UomCode.FillWeight = 60F;
            this.UomCode.HeaderText = "单位";
            this.UomCode.Name = "UomCode";
            this.UomCode.ReadOnly = true;
            // 
            // UnitCount
            // 
            this.UnitCount.DataPropertyName = "UnitCount";
            this.UnitCount.FillWeight = 80F;
            this.UnitCount.HeaderText = "单包装";
            this.UnitCount.Name = "UnitCount";
            this.UnitCount.ReadOnly = true;
            // 
            // LocationToCode
            // 
            this.LocationToCode.DataPropertyName = "LocationToCode";
            this.LocationToCode.HeaderText = "库位";
            this.LocationToCode.Name = "LocationToCode";
            this.LocationToCode.ReadOnly = true;
            // 
            // Qty
            // 
            this.Qty.DataPropertyName = "Qty";
            this.Qty.FillWeight = 80F;
            this.Qty.HeaderText = "待验数";
            this.Qty.Name = "Qty";
            this.Qty.ReadOnly = true;
            // 
            // CurrentQty
            // 
            this.CurrentQty.DataPropertyName = "CurrentQty";
            this.CurrentQty.FillWeight = 80F;
            this.CurrentQty.HeaderText = "合格数";
            this.CurrentQty.Name = "CurrentQty";
            // 
            // CurrentRejectQty
            // 
            this.CurrentRejectQty.DataPropertyName = "CurrentRejectQty";
            this.CurrentRejectQty.FillWeight = 80F;
            this.CurrentRejectQty.HeaderText = "不合格";
            this.CurrentRejectQty.Name = "CurrentRejectQty";
            // 
            // UCInspection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.gbList);
            this.Controls.Add(this.gbPickUp);
            this.Name = "UCInspection";
            this.Size = new System.Drawing.Size(873, 500);
            this.gbList.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.gvList)).EndInit();
            this.gbPickUp.ResumeLayout(false);
            this.gbPickUp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbList;
        protected System.Windows.Forms.DataGridView gvList;
        protected System.Windows.Forms.GroupBox gbPickUp;
        protected System.Windows.Forms.Button btnConfirm;
        protected System.Windows.Forms.TextBox tbBarCode;
        protected System.Windows.Forms.Label lblResult;
        private System.Windows.Forms.Label lblBarCode;
        protected System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CheckColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemDescription;
        private System.Windows.Forms.DataGridViewTextBoxColumn UomCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn UnitCount;
        private System.Windows.Forms.DataGridViewTextBoxColumn LocationToCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn Qty;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentQty;
        private System.Windows.Forms.DataGridViewTextBoxColumn CurrentRejectQty;

    }
}
