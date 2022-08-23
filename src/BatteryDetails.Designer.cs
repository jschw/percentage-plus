namespace percentage_plus
{
    partial class BatteryDetails
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DgvBatteryInfo = new System.Windows.Forms.DataGridView();
            this.dgvColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dgvColValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.DgvBatteryInfo)).BeginInit();
            this.SuspendLayout();
            // 
            // DgvBatteryInfo
            // 
            this.DgvBatteryInfo.AllowUserToAddRows = false;
            this.DgvBatteryInfo.AllowUserToDeleteRows = false;
            this.DgvBatteryInfo.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DgvBatteryInfo.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvColName,
            this.dgvColValue});
            this.DgvBatteryInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DgvBatteryInfo.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DgvBatteryInfo.Location = new System.Drawing.Point(0, 0);
            this.DgvBatteryInfo.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.DgvBatteryInfo.Name = "DgvBatteryInfo";
            this.DgvBatteryInfo.ReadOnly = true;
            this.DgvBatteryInfo.RowHeadersWidth = 62;
            this.DgvBatteryInfo.RowTemplate.Height = 25;
            this.DgvBatteryInfo.ShowEditingIcon = false;
            this.DgvBatteryInfo.Size = new System.Drawing.Size(653, 830);
            this.DgvBatteryInfo.TabIndex = 0;
            // 
            // dgvColName
            // 
            this.dgvColName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvColName.FillWeight = 50F;
            this.dgvColName.HeaderText = "Name";
            this.dgvColName.MinimumWidth = 8;
            this.dgvColName.Name = "dgvColName";
            this.dgvColName.ReadOnly = true;
            // 
            // dgvColValue
            // 
            this.dgvColValue.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dgvColValue.FillWeight = 50F;
            this.dgvColValue.HeaderText = "Value";
            this.dgvColValue.MinimumWidth = 8;
            this.dgvColValue.Name = "dgvColValue";
            this.dgvColValue.ReadOnly = true;
            // 
            // BatteryDetails
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(653, 830);
            this.Controls.Add(this.DgvBatteryInfo);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MinimizeBox = false;
            this.Name = "BatteryDetails";
            this.Text = "Battery Details";
            ((System.ComponentModel.ISupportInitialize)(this.DgvBatteryInfo)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView DgvBatteryInfo;
        private DataGridViewTextBoxColumn dgvColName;
        private DataGridViewTextBoxColumn dgvColValue;
    }
}