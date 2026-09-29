using System;
using System.ComponentModel;
using System.Windows.Forms;
using PetShopManagementSystem.Models;

namespace PetShopManagementSystem.Forms
{
    public partial class frmManagerDashboard : Form
    {
        private readonly LoginResult currentUser;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmManagerDashboard() : this(null) { }

        public frmManagerDashboard(LoginResult user)
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
        // MANAGER
        // =========================================================

        private void btnManageProducts_Click(object sender, EventArgs e)
        {
            try
            {
                frmManageProducts form =
                    new frmManageProducts(currentUser);

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open Manage Products.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnManageOrders_Click(object sender, EventArgs e)
        {
            try
            {
                frmManageOrders form =
                    new frmManageOrders(currentUser);

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open Manage Orders.\n\n" +
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

        private void frmManagerDashboard_FormClosing(
            object sender,
            FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void lblFooter_Click(object sender, EventArgs e)
        {

        }

        private void grpManager_Enter(object sender, EventArgs e)
        {

        }

        private void lblRole_Click(object sender, EventArgs e)
        {

        }
    }
}
