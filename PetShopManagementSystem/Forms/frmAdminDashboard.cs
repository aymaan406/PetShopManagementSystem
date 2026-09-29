using System;
using System.ComponentModel;
using System.Windows.Forms;
using PetShopManagementSystem.Models;

namespace PetShopManagementSystem.Forms
{
    public partial class frmAdminDashboard : Form
    {
        private readonly LoginResult currentUser;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmAdminDashboard() : this(null) { }

        public frmAdminDashboard(LoginResult user)
        {
            InitializeComponent();

            // Skip all real startup logic (which needs a real logged-in user)
            // when the Designer is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            currentUser = user;

            lblWelcome.Text = "Welcome, " + currentUser.Name;
            //lblRole.Text = "Role: " + currentUser.Role;
        }

        // =========================================================
        // ADMIN
        // =========================================================

        private void btnManageUsers_Click(object sender, EventArgs e)
        {
            try
            {
                frmManageManagers form =
                    new frmManageManagers(currentUser);

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open Manage Managers.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnAdminViewProducts_Click(object sender, EventArgs e)
        {
            try
            {
                frmAdminProducts form = new frmAdminProducts(currentUser);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open Admin Product View.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnAdminManageCustomers_Click(object sender, EventArgs e)
        {
            try
            {
                frmManageCustomers form = new frmManageCustomers(currentUser);
                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open Manage Customers.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnAdminReports_Click(object sender, EventArgs e)
        {
            try
            {
                frmAdminReports form =
                    new frmAdminReports(currentUser);

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open Admin Reports.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        // =========================================================
        // LOGOUT
        // =========================================================

        private void btnLogout_Click(object sender, EventArgs e)
        {
            DialogResult result =
                MessageBox.Show(
                    "Are you sure you want to logout?",
                    "Logout",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                frmLogin loginForm =
                    new frmLogin();

                loginForm.Show();

                this.Hide();
            }
        }

        // =========================================================
        // FORM CLOSING
        // =========================================================

        private void frmAdminDashboard_FormClosing(
            object sender,
            FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void lblRole_Click(object sender, EventArgs e)
        {

        }

        private void frmAdminDashboard_Load(object sender, EventArgs e)
        {

        }
    }
}
