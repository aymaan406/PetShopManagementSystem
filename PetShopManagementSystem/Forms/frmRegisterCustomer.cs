using PetShopManagementSystem.Repositories;
using PetShopManagementSystem.Models;
using System;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PetShopManagementSystem.Forms
{
    public partial class frmRegisterCustomer : Form
    {
        private readonly UserRepository userRepository;

        public frmRegisterCustomer()
        {
            InitializeComponent();
            userRepository = new UserRepository();
        }

        private void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                string name = txtName.Text.Trim();
                string username = txtUsername.Text.Trim();
                string password = txtPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;
                string email = txtEmail.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string address = txtAddress.Text.Trim();

                // Required field validation
                if (string.IsNullOrWhiteSpace(name))
                {
                    MessageBox.Show(
                        "Please enter your name.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtName.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show(
                        "Please enter a username.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtUsername.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(password))
                {
                    MessageBox.Show(
                        "Please enter a password.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtPassword.Focus();
                    return;
                }

                if (password.Length < 6)
                {
                    MessageBox.Show(
                        "Password must contain at least 6 characters.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtPassword.Focus();
                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show(
                        "Password and confirm password do not match.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtConfirmPassword.Clear();
                    txtConfirmPassword.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    MessageBox.Show(
                        "Please enter your email address.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtEmail.Focus();
                    return;
                }

                if (!IsValidEmail(email))
                {
                    MessageBox.Show(
                        "Please enter a valid email address.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtEmail.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(phone))
                {
                    MessageBox.Show(
                        "Please enter your phone number.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtPhone.Focus();
                    return;
                }

                if (string.IsNullOrWhiteSpace(address))
                {
                    MessageBox.Show(
                        "Please enter your address.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtAddress.Focus();
                    return;
                }

                // Check username before inserting
                if (userRepository.UsernameExists(username))
                {
                    MessageBox.Show(
                        "This username is already in use. Please choose another username.",
                        "Registration Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    txtUsername.Focus();
                    return;
                }

                var customer = new Customer
                {
                    Name = name,
                    Username = username,
                    Password = LoginRepository.HashPassword(password),
                    Email = email,
                    Phone = phone,
                    Address = address
                };

                bool success = userRepository.AddCustomer(customer);

                if (success)
                {
                    MessageBox.Show(
                        "Customer registration submitted successfully!\n\nYour account is INACTIVE by default and must be approved by an Admin before you can login.",
                        "Registration Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearFields();

                    // Return to the existing login form after submission.
                    this.Close();
                }
                else
                {
                    MessageBox.Show(
                        "Registration failed. Please try again.",
                        "Registration Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred during registration.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(email);
                return address.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private void ClearFields()
        {
            txtName.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            txtConfirmPassword.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();

            txtName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            frmLogin login = new frmLogin();
            login.Show();

            this.Close();
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            bool showPassword = chkShowPassword.Checked;

            txtPassword.UseSystemPasswordChar = !showPassword;
            txtConfirmPassword.UseSystemPasswordChar = !showPassword;
        }
    }
}
