namespace PetShopManagementSystem.Forms
{
    partial class frmAdminReports
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;

        private System.Windows.Forms.GroupBox grpStatistics;

        private System.Windows.Forms.Label lblRevenueCaption;
        private System.Windows.Forms.Label lblTotalRevenue;

        private System.Windows.Forms.Label lblOrdersCaption;
        private System.Windows.Forms.Label lblTotalOrders;

        private System.Windows.Forms.Label lblCustomersCaption;
        private System.Windows.Forms.Label lblTotalCustomers;

        private System.Windows.Forms.Label lblManagersCaption;
        private System.Windows.Forms.Label lblTotalManagers;

        private System.Windows.Forms.GroupBox grpOrderSummary;
        private System.Windows.Forms.DataGridView dgvOrderSummary;
        private System.Windows.Forms.Label lblSummaryCount;

        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnClose;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.grpOrderSummary = new System.Windows.Forms.GroupBox();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvOrderSummary = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.lblSummaryCount = new System.Windows.Forms.Label();
            this.grpStatistics = new System.Windows.Forms.GroupBox();
            this.lblRevenueCaption = new System.Windows.Forms.Label();
            this.lblTotalRevenue = new System.Windows.Forms.Label();
            this.lblOrdersCaption = new System.Windows.Forms.Label();
            this.lblTotalOrders = new System.Windows.Forms.Label();
            this.lblCustomersCaption = new System.Windows.Forms.Label();
            this.lblTotalCustomers = new System.Windows.Forms.Label();
            this.lblManagersCaption = new System.Windows.Forms.Label();
            this.lblTotalManagers = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.grpOrderSummary.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderSummary)).BeginInit();
            this.grpStatistics.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpOrderSummary
            // 
            this.grpOrderSummary.BackgroundImage = global::PetShopManagementSystem.Properties.Resources._38;
            this.grpOrderSummary.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grpOrderSummary.Controls.Add(this.lblTitle);
            this.grpOrderSummary.Controls.Add(this.dgvOrderSummary);
            this.grpOrderSummary.Controls.Add(this.lblSubtitle);
            this.grpOrderSummary.Controls.Add(this.lblSummaryCount);
            this.grpOrderSummary.Controls.Add(this.grpStatistics);
            this.grpOrderSummary.Controls.Add(this.btnClose);
            this.grpOrderSummary.Controls.Add(this.btnRefresh);
            this.grpOrderSummary.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpOrderSummary.Location = new System.Drawing.Point(0, 0);
            this.grpOrderSummary.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.grpOrderSummary.Name = "grpOrderSummary";
            this.grpOrderSummary.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.grpOrderSummary.Size = new System.Drawing.Size(1832, 842);
            this.grpOrderSummary.TabIndex = 3;
            this.grpOrderSummary.TabStop = false;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitle.Location = new System.Drawing.Point(841, 148);
            this.lblTitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(210, 37);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Admin Reports";
            // 
            // dgvOrderSummary
            // 
            this.dgvOrderSummary.AllowUserToAddRows = false;
            this.dgvOrderSummary.AllowUserToDeleteRows = false;
            this.dgvOrderSummary.AllowUserToResizeRows = false;
            this.dgvOrderSummary.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvOrderSummary.BackgroundColor = System.Drawing.Color.AliceBlue;
            this.dgvOrderSummary.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvOrderSummary.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2,
            this.dataGridViewTextBoxColumn3,
            this.dataGridViewTextBoxColumn4,
            this.dataGridViewTextBoxColumn5,
            this.dataGridViewTextBoxColumn6});
            this.dgvOrderSummary.GridColor = System.Drawing.SystemColors.Control;
            this.dgvOrderSummary.Location = new System.Drawing.Point(640, 321);
            this.dgvOrderSummary.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.dgvOrderSummary.MultiSelect = false;
            this.dgvOrderSummary.Name = "dgvOrderSummary";
            this.dgvOrderSummary.ReadOnly = true;
            this.dgvOrderSummary.RowHeadersVisible = false;
            this.dgvOrderSummary.RowHeadersWidth = 62;
            this.dgvOrderSummary.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvOrderSummary.Size = new System.Drawing.Size(703, 252);
            this.dgvOrderSummary.TabIndex = 0;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Order ID";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "Customer";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "Manager";
            this.dataGridViewTextBoxColumn3.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "Order Date";
            this.dataGridViewTextBoxColumn4.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "Total Amount";
            this.dataGridViewTextBoxColumn5.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.ReadOnly = true;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "Status";
            this.dataGridViewTextBoxColumn6.MinimumWidth = 8;
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.ReadOnly = true;
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblSubtitle.Location = new System.Drawing.Point(866, 204);
            this.lblSubtitle.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(164, 19);
            this.lblSubtitle.TabIndex = 1;
            this.lblSubtitle.Text = "Sales and order overview.";
            this.lblSubtitle.Click += new System.EventHandler(this.lblSubtitle_Click);
            // 
            // lblSummaryCount
            // 
            this.lblSummaryCount.AutoSize = true;
            this.lblSummaryCount.Location = new System.Drawing.Point(637, 576);
            this.lblSummaryCount.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblSummaryCount.Name = "lblSummaryCount";
            this.lblSummaryCount.Size = new System.Drawing.Size(86, 13);
            this.lblSummaryCount.TabIndex = 1;
            this.lblSummaryCount.Text = "Orders Shown: 0";
            // 
            // grpStatistics
            // 
            this.grpStatistics.BackColor = System.Drawing.SystemColors.ControlLightLight;
            this.grpStatistics.Controls.Add(this.lblRevenueCaption);
            this.grpStatistics.Controls.Add(this.lblTotalRevenue);
            this.grpStatistics.Controls.Add(this.lblOrdersCaption);
            this.grpStatistics.Controls.Add(this.lblTotalOrders);
            this.grpStatistics.Controls.Add(this.lblCustomersCaption);
            this.grpStatistics.Controls.Add(this.lblTotalCustomers);
            this.grpStatistics.Controls.Add(this.lblManagersCaption);
            this.grpStatistics.Controls.Add(this.lblTotalManagers);
            this.grpStatistics.ForeColor = System.Drawing.Color.IndianRed;
            this.grpStatistics.Location = new System.Drawing.Point(517, 321);
            this.grpStatistics.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.grpStatistics.Name = "grpStatistics";
            this.grpStatistics.Padding = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.grpStatistics.Size = new System.Drawing.Size(119, 252);
            this.grpStatistics.TabIndex = 2;
            this.grpStatistics.TabStop = false;
            // 
            // lblRevenueCaption
            // 
            this.lblRevenueCaption.AutoSize = true;
            this.lblRevenueCaption.Location = new System.Drawing.Point(19, 9);
            this.lblRevenueCaption.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblRevenueCaption.Name = "lblRevenueCaption";
            this.lblRevenueCaption.Size = new System.Drawing.Size(78, 13);
            this.lblRevenueCaption.TabIndex = 0;
            this.lblRevenueCaption.Text = "Total Revenue";
            // 
            // lblTotalRevenue
            // 
            this.lblTotalRevenue.AutoSize = true;
            this.lblTotalRevenue.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTotalRevenue.Location = new System.Drawing.Point(19, 29);
            this.lblTotalRevenue.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTotalRevenue.Name = "lblTotalRevenue";
            this.lblTotalRevenue.Size = new System.Drawing.Size(53, 28);
            this.lblTotalRevenue.TabIndex = 1;
            this.lblTotalRevenue.Text = "0.00";
            // 
            // lblOrdersCaption
            // 
            this.lblOrdersCaption.AutoSize = true;
            this.lblOrdersCaption.Location = new System.Drawing.Point(21, 68);
            this.lblOrdersCaption.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblOrdersCaption.Name = "lblOrdersCaption";
            this.lblOrdersCaption.Size = new System.Drawing.Size(65, 13);
            this.lblOrdersCaption.TabIndex = 2;
            this.lblOrdersCaption.Text = "Total Orders";
            // 
            // lblTotalOrders
            // 
            this.lblTotalOrders.AutoSize = true;
            this.lblTotalOrders.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTotalOrders.Location = new System.Drawing.Point(21, 88);
            this.lblTotalOrders.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTotalOrders.Name = "lblTotalOrders";
            this.lblTotalOrders.Size = new System.Drawing.Size(24, 28);
            this.lblTotalOrders.TabIndex = 3;
            this.lblTotalOrders.Text = "0";
            // 
            // lblCustomersCaption
            // 
            this.lblCustomersCaption.AutoSize = true;
            this.lblCustomersCaption.Location = new System.Drawing.Point(21, 125);
            this.lblCustomersCaption.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblCustomersCaption.Name = "lblCustomersCaption";
            this.lblCustomersCaption.Size = new System.Drawing.Size(83, 13);
            this.lblCustomersCaption.TabIndex = 4;
            this.lblCustomersCaption.Text = "Total Customers";
            // 
            // lblTotalCustomers
            // 
            this.lblTotalCustomers.AutoSize = true;
            this.lblTotalCustomers.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTotalCustomers.Location = new System.Drawing.Point(21, 145);
            this.lblTotalCustomers.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTotalCustomers.Name = "lblTotalCustomers";
            this.lblTotalCustomers.Size = new System.Drawing.Size(24, 28);
            this.lblTotalCustomers.TabIndex = 5;
            this.lblTotalCustomers.Text = "0";
            // 
            // lblManagersCaption
            // 
            this.lblManagersCaption.AutoSize = true;
            this.lblManagersCaption.Location = new System.Drawing.Point(21, 184);
            this.lblManagersCaption.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblManagersCaption.Name = "lblManagersCaption";
            this.lblManagersCaption.Size = new System.Drawing.Size(81, 13);
            this.lblManagersCaption.TabIndex = 6;
            this.lblManagersCaption.Text = "Total Managers";
            // 
            // lblTotalManagers
            // 
            this.lblTotalManagers.AutoSize = true;
            this.lblTotalManagers.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            this.lblTotalManagers.Location = new System.Drawing.Point(21, 204);
            this.lblTotalManagers.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblTotalManagers.Name = "lblTotalManagers";
            this.lblTotalManagers.Size = new System.Drawing.Size(24, 28);
            this.lblTotalManagers.TabIndex = 7;
            this.lblTotalManagers.Text = "0";
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.RosyBrown;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Location = new System.Drawing.Point(988, 645);
            this.btnClose.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(129, 32);
            this.btnClose.TabIndex = 5;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.BackColor = System.Drawing.Color.RosyBrown;
            this.btnRefresh.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.Location = new System.Drawing.Point(809, 645);
            this.btnRefresh.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(129, 32);
            this.btnRefresh.TabIndex = 4;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // frmAdminReports
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1832, 842);
            this.Controls.Add(this.grpOrderSummary);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAdminReports";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Admin Reports";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.grpOrderSummary.ResumeLayout(false);
            this.grpOrderSummary.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvOrderSummary)).EndInit();
            this.grpStatistics.ResumeLayout(false);
            this.grpStatistics.PerformLayout();
            this.ResumeLayout(false);

        }

        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    }
}
