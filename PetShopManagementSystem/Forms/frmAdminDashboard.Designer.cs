namespace PetShopManagementSystem.Forms
{
    partial class frmAdminDashboard
    {
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblWelcome;

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
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblWelcome = new System.Windows.Forms.Label();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnAdminViewProducts = new System.Windows.Forms.Button();
            this.btnAdminManageCustomers = new System.Windows.Forms.Button();
            this.btnAdminReports = new System.Windows.Forms.Button();
            this.btnManageUsers = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 22F, System.Drawing.FontStyle.Bold);
            this.lblTitle.Location = new System.Drawing.Point(763, 185);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(273, 41);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Admin Dashboard";
            // 
            // lblWelcome
            // 
            this.lblWelcome.AutoSize = true;
            this.lblWelcome.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWelcome.Location = new System.Drawing.Point(785, 246);
            this.lblWelcome.Name = "lblWelcome";
            this.lblWelcome.Size = new System.Drawing.Size(82, 21);
            this.lblWelcome.TabIndex = 1;
            this.lblWelcome.Text = "Welcome";
            // 
            // btnLogout
            // 
            this.btnLogout.BackColor = System.Drawing.Color.Salmon;
            this.btnLogout.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.btnLogout.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnLogout.Location = new System.Drawing.Point(815, 745);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(158, 48);
            this.btnLogout.TabIndex = 4;
            this.btnLogout.Text = "Logout";
            this.btnLogout.UseVisualStyleBackColor = false;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnAdminViewProducts
            // 
            this.btnAdminViewProducts.BackColor = System.Drawing.Color.Lavender;
            this.btnAdminViewProducts.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdminViewProducts.Location = new System.Drawing.Point(940, 565);
            this.btnAdminViewProducts.Name = "btnAdminViewProducts";
            this.btnAdminViewProducts.Size = new System.Drawing.Size(171, 120);
            this.btnAdminViewProducts.TabIndex = 3;
            this.btnAdminViewProducts.Text = "View Products";
            this.btnAdminViewProducts.UseVisualStyleBackColor = false;
            this.btnAdminViewProducts.Click += new System.EventHandler(this.btnAdminViewProducts_Click);
            // 
            // btnAdminManageCustomers
            // 
            this.btnAdminManageCustomers.BackColor = System.Drawing.Color.Pink;
            this.btnAdminManageCustomers.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdminManageCustomers.Location = new System.Drawing.Point(940, 346);
            this.btnAdminManageCustomers.Name = "btnAdminManageCustomers";
            this.btnAdminManageCustomers.Size = new System.Drawing.Size(171, 120);
            this.btnAdminManageCustomers.TabIndex = 2;
            this.btnAdminManageCustomers.Text = "Manage Customers";
            this.btnAdminManageCustomers.UseVisualStyleBackColor = false;
            this.btnAdminManageCustomers.Click += new System.EventHandler(this.btnAdminManageCustomers_Click);
            // 
            // btnAdminReports
            // 
            this.btnAdminReports.BackColor = System.Drawing.Color.Honeydew;
            this.btnAdminReports.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdminReports.Location = new System.Drawing.Point(696, 565);
            this.btnAdminReports.Name = "btnAdminReports";
            this.btnAdminReports.Size = new System.Drawing.Size(171, 120);
            this.btnAdminReports.TabIndex = 1;
            this.btnAdminReports.Text = "Admin Reports";
            this.btnAdminReports.UseVisualStyleBackColor = false;
            this.btnAdminReports.Click += new System.EventHandler(this.btnAdminReports_Click);
            // 
            // btnManageUsers
            // 
            this.btnManageUsers.BackColor = System.Drawing.Color.Bisque;
            this.btnManageUsers.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnManageUsers.Location = new System.Drawing.Point(696, 346);
            this.btnManageUsers.Name = "btnManageUsers";
            this.btnManageUsers.Size = new System.Drawing.Size(171, 120);
            this.btnManageUsers.TabIndex = 0;
            this.btnManageUsers.Text = "Manage Managers";
            this.btnManageUsers.UseVisualStyleBackColor = false;
            this.btnManageUsers.Click += new System.EventHandler(this.btnManageUsers_Click);
            // 
            // frmAdminDashboard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = global::PetShopManagementSystem.Properties.Resources._29;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1826, 844);
            this.Controls.Add(this.btnLogout);
            this.Controls.Add(this.btnAdminViewProducts);
            this.Controls.Add(this.btnAdminReports);
            this.Controls.Add(this.btnManageUsers);
            this.Controls.Add(this.btnAdminManageCustomers);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.lblWelcome);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmAdminDashboard";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pet Food Management System - Admin";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmAdminDashboard_FormClosing);
            this.Load += new System.EventHandler(this.frmAdminDashboard_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.Button btnAdminViewProducts;
        private System.Windows.Forms.Button btnAdminManageCustomers;
        private System.Windows.Forms.Button btnAdminReports;
        private System.Windows.Forms.Button btnManageUsers;
    }
}
