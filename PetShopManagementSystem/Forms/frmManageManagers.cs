using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmManageManagers : Form
    {
        private readonly UserRepository userRepository;
        private int selectedManagerId = 0;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmManageManagers() : this(null) { }

        public frmManageManagers(LoginResult user)
        {
            InitializeComponent();

            // Skip all real startup logic (validation, database calls) when the
            // Designer is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only an Admin can manage manager accounts.");

            userRepository = new UserRepository();
            LoadManagers();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) { LoadManagers(); }
        private void btnRefresh_Click(object sender, EventArgs e) { txtSearch.Clear(); ClearFields(); LoadManagers(); }
        private void btnClear_Click(object sender, EventArgs e) { ClearFields(); }

        private void LoadManagers()
        {
            try
            {
                List<Manager> managers = userRepository.GetManagers();
                string search = txtSearch.Text.Trim().ToLower();
                dgvManagers.Rows.Clear();

                int count = 0;
                foreach (Manager manager in managers)
                {
                    string combined =
                        (manager.Name + " " + manager.Username + " " + manager.Email + " " + manager.Phone).ToLower();

                    if (search.Length > 0 && !combined.Contains(search))
                        continue;

                    int row = dgvManagers.Rows.Add(
                        manager.ManagerID,
                        manager.Name,
                        manager.Username,
                        manager.Email,
                        manager.Phone,
                        StatusName(manager.Status));
                    dgvManagers.Rows[row].Tag = manager.ManagerID;
                    count++;
                }

                lblTotal.Text = "Total Managers: " + count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load managers.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvManagers_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            DataGridViewRow row = dgvManagers.Rows[e.RowIndex];
            selectedManagerId = Convert.ToInt32(row.Cells["ManagerID"].Value);
            txtName.Text = row.Cells["Name"].Value.ToString();
            txtUsername.Text = row.Cells["Username"].Value.ToString();
            txtEmail.Text = row.Cells["Email"].Value.ToString();
            txtPhone.Text = row.Cells["Phone"].Value.ToString();
            txtPassword.Clear();
            string status = row.Cells["Status"].Value.ToString();
            cmbStatus.SelectedIndex = status == "ACTIVE" ? 0 : (status == "BLOCKED" ? 2 : 1);
        }

        private Manager ReadManagerFromFields(bool requirePassword)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text) ||
                string.IsNullOrWhiteSpace(txtUsername.Text))
                throw new Exception("Name and username are required.");

            if (requirePassword && string.IsNullOrWhiteSpace(txtPassword.Text))
                throw new Exception("Password is required when adding a manager.");

            string password = txtPassword.Text.Trim();
            if (!string.IsNullOrWhiteSpace(password))
                password = LoginRepository.HashPassword(password);

            return new Manager
            {
                ManagerID = selectedManagerId,
                Name = txtName.Text.Trim(),
                Username = txtUsername.Text.Trim(),
                Password = password,
                Email = txtEmail.Text.Trim(),
                Phone = txtPhone.Text.Trim(),
                Status = cmbStatus.SelectedIndex == 0 ? 1 : (cmbStatus.SelectedIndex == 2 ? 2 : 0)
            };
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                Manager manager = ReadManagerFromFields(true);

                if (userRepository.UsernameExists(manager.Username))
                {
                    MessageBox.Show("That username is already in use.", "Duplicate Username",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (userRepository.AddManager(manager))
                {
                    MessageBox.Show("Manager added successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadManagers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to add manager.\n\n" + ex.Message,
                    "Add Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedManagerId == 0)
                {
                    MessageBox.Show("Select a manager first.", "Selection Required",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Manager manager = ReadManagerFromFields(false);

                if (userRepository.UsernameExistsForAnotherManager(manager.Username, manager.ManagerID))
                {
                    MessageBox.Show("That username is already in use.", "Duplicate Username",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                bool updatePassword = !string.IsNullOrWhiteSpace(txtPassword.Text);

                if (userRepository.UpdateManager(manager, updatePassword))
                {
                    MessageBox.Show("Manager updated successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadManagers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to update manager.\n\n" + ex.Message,
                    "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (selectedManagerId == 0)
            {
                MessageBox.Show("Select a manager first.", "Selection Required",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Delete manager #" + selectedManagerId + "?\n\n" +
                "A manager with existing products or orders cannot be physically deleted.",
                "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                if (userRepository.DeleteManager(selectedManagerId))
                {
                    MessageBox.Show("Manager deleted successfully.", "Success",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    ClearFields();
                    LoadManagers();
                }
                else
                {
                    MessageBox.Show(
                        "This manager has products or orders linked to the account, so the account was not deleted.\n\nDeactivate the manager instead if you want to stop future use.",
                        "Cannot Delete Manager", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to delete manager.\n\n" + ex.Message,
                    "Delete Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string StatusName(int status)
        {
            if (status == 1) return "ACTIVE";
            if (status == 2) return "BLOCKED";
            return "INACTIVE";
        }

        private void ClearFields()
        {
            selectedManagerId = 0;
            txtName.Clear();
            txtUsername.Clear();
            txtPassword.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            cmbStatus.SelectedIndex = 0;
            dgvManagers.ClearSelection();
        }
    }
}
