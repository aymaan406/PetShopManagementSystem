using System;
using System.ComponentModel;
using System.Windows.Forms;
using PetShopManagementSystem.Models;

namespace PetShopManagementSystem.Forms
{
    public partial class frmCustomerDashboard : Form
    {
        private readonly LoginResult currentUser;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmCustomerDashboard() : this(null) { }

        public frmCustomerDashboard(LoginResult user)
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
        // CUSTOMER
        // =========================================================

        private void btnBrowseProducts_Click(object sender, EventArgs e)
        {
            try
            {
                frmBrowseProducts form =
                    new frmBrowseProducts(currentUser);

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open Browse Products.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnMyOrders_Click(object sender, EventArgs e)
        {
            try
            {
                frmMyOrders form =
                    new frmMyOrders(currentUser);

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open My Orders.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnProfile_Click(object sender, EventArgs e)
        {
            try
            {
                frmCustomerProfile form =
                    new frmCustomerProfile(currentUser);

                form.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open Profile.\n\n" + ex.Message,
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

        private void frmCustomerDashboard_FormClosing(
            object sender,
            FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void lblFooter_Click(object sender, EventArgs e)
        {

        }
    }
}
