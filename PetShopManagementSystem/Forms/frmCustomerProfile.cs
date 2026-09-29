using System;
using System.ComponentModel;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmCustomerProfile : Form
    {
        private readonly LoginResult currentUser;
        private readonly UserRepository userRepository;
        private readonly LoginRepository loginRepository;

        private Customer loadedCustomer;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmCustomerProfile() : this(null) { }

        public frmCustomerProfile(LoginResult user)
        {
            InitializeComponent();

            // Skip all real startup logic (database calls) when the Designer
            // is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            currentUser = user;
            userRepository = new UserRepository();
            loginRepository = new LoginRepository();

            LoadProfile();
        }

        private void LoadProfile()
        {
            try
            {
                Customer customer =
                    userRepository.GetCustomerById(currentUser.UserID);

                if (customer == null)
                {
                    MessageBox.Show(
                        "Customer information could not be found.",
                        "Profile",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                loadedCustomer = customer;

                dgvProfile.Rows.Clear();

                dgvProfile.Rows.Add(
                    customer.CustomerID,
                    customer.Name,
                    customer.Username,
                    customer.Email,
                    customer.Phone,
                    customer.Address);

                dgvProfile.ClearSelection();
                dgvProfile.Rows[0].Selected = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load your profile.\n\n" + ex.Message,
                    "Profile Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        /*
         * Selecting the row copies its values into the editable
         * text boxes below, ready to be changed and saved.
         */
        private void dgvProfile_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvProfile.SelectedRows.Count == 0)
            {
                return;
            }

            DataGridViewRow row = dgvProfile.SelectedRows[0];

            txtName.Text = Convert.ToString(row.Cells["colName"].Value);
            txtUsername.Text = Convert.ToString(row.Cells["colUsername"].Value);
            txtEmail.Text = Convert.ToString(row.Cells["colEmail"].Value);
            txtPhone.Text = Convert.ToString(row.Cells["colPhone"].Value);
            txtAddress.Text = Convert.ToString(row.Cells["colAddress"].Value);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtName.Clear();
            txtUsername.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();

            txtName.Focus();
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (loadedCustomer == null)
                {
                    LoadProfile();
                    return;
                }

                string name = txtName.Text.Trim();
                string username = txtUsername.Text.Trim();
                string email = txtEmail.Text.Trim();
                string phone = txtPhone.Text.Trim();
                string address = txtAddress.Text.Trim();

                if (string.IsNullOrWhiteSpace(name) ||
                    string.IsNullOrWhiteSpace(username))
                {
                    MessageBox.Show(
                        "Name and username are required.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (userRepository.UsernameExistsForAnotherCustomer(
                    username,
                    currentUser.UserID))
                {
                    MessageBox.Show(
                        "This username is already in use.",
                        "Update Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                Customer customer = new Customer
                {
                    CustomerID = currentUser.UserID,
                    Name = name,
                    Username = username,
                    Email = email,
                    Phone = phone,
                    Address = address
                };

                if (userRepository.UpdateCustomerProfile(customer))
                {
                    MessageBox.Show(
                        "Profile updated successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadProfile();
                }
                else
                {
                    MessageBox.Show(
                        "No profile information was updated.",
                        "Update",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to update your profile.\n\n" + ex.Message,
                    "Update Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnChangePassword_Click(object sender, EventArgs e)
        {
            try
            {
                if (loadedCustomer == null)
                {
                    LoadProfile();
                    return;
                }

                string currentPassword = txtCurrentPassword.Text;
                string newPassword = txtNewPassword.Text;
                string confirmPassword = txtConfirmPassword.Text;

                if (string.IsNullOrWhiteSpace(currentPassword) ||
                    string.IsNullOrWhiteSpace(newPassword) ||
                    string.IsNullOrWhiteSpace(confirmPassword))
                {
                    MessageBox.Show(
                        "Please fill in all three password fields.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (newPassword.Length < 6)
                {
                    MessageBox.Show(
                        "Your new password must be at least 6 characters long.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (newPassword != confirmPassword)
                {
                    MessageBox.Show(
                        "New password and confirmation do not match.",
                        "Validation",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                LoginResult verify =
                    loginRepository.Login(loadedCustomer.Username, currentPassword);

                if (verify == null || !verify.IsSuccessful)
                {
                    MessageBox.Show(
                        "Your current password is incorrect.",
                        "Change Password Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                string hashedPassword = LoginRepository.HashPassword(newPassword);

                if (userRepository.UpdateCustomerPassword(
                    currentUser.UserID,
                    hashedPassword))
                {
                    MessageBox.Show(
                        "Your password has been changed successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    ClearPasswordFields();
                }
                else
                {
                    MessageBox.Show(
                        "Your password could not be changed.",
                        "Change Password Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to change your password.\n\n" + ex.Message,
                    "Change Password Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ClearPasswordFields()
        {
            txtCurrentPassword.Clear();
            txtNewPassword.Clear();
            txtConfirmPassword.Clear();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
