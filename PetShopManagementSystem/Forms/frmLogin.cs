using System;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmLogin : Form
    {
        private readonly LoginRepository loginRepository;

        public frmLogin()
        {
            InitializeComponent();
            loginRepository = new LoginRepository();
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;

                // Username validation
                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show(
                        "Please enter your username.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtUsername.Focus();
                    return;
                }

                // Password validation
                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show(
                        "Please enter your password.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtPassword.Focus();
                    return;
                }

                // Login through repository
                LoginResult result = loginRepository.Login(username, password);

                if (result == null)
                {
                    string customerStatus =
                        loginRepository.GetCustomerStatus(username);

                    if (customerStatus == "INACTIVE")
                    {
                        MessageBox.Show(
                            "Your customer account is waiting for Admin approval.",
                            "Login Not Available",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);
                    }
                    else if (customerStatus == "BLOCKED")
                    {
                        MessageBox.Show(
                            "Your customer account has been blocked by the Admin.",
                            "Login Not Available",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show(
                            "Invalid username or password.",
                            "Login Failed",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                    }

                    txtPassword.Clear();
                    txtPassword.Focus();
                    return;
                }

                MessageBox.Show(
                    "Login successful!",
                    "Welcome",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                // Open the dashboard that matches this user's role
                Form dashboard;

                if (string.Equals(result.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                {
                    dashboard = new frmAdminDashboard(result);
                }
                else if (string.Equals(result.Role, "Manager", StringComparison.OrdinalIgnoreCase))
                {
                    dashboard = new frmManagerDashboard(result);
                }
                else
                {
                    dashboard = new frmCustomerDashboard(result);
                }

                dashboard.Show();

                // Hide login form
                this.Hide();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while logging in.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                frmRegisterCustomer registerForm =
                    new frmRegisterCustomer();

                registerForm.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open registration form.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar =
                !chkShowPassword.Checked;
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                txtPassword.Focus();
                e.SuppressKeyPress = true;
            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnLogin.PerformClick();
                e.SuppressKeyPress = true;
            }
        }
    }
}
