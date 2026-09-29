using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmManageCustomers : Form
    {
        private readonly UserRepository userRepository = new UserRepository();

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmManageCustomers() : this(null) { }

        public frmManageCustomers(LoginResult user)
        {
            InitializeComponent();

            // Skip all real startup logic (validation, database calls) when the
            // Designer is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only an Admin can manage customers.");

            LoadCustomers();
        }

        private void btnActive_Click(object sender, EventArgs e) { SetStatus(1, "ACTIVE"); }
        private void btnInactive_Click(object sender, EventArgs e) { SetStatus(0, "INACTIVE"); }
        private void btnBlock_Click(object sender, EventArgs e) { SetStatus(2, "BLOCKED"); }
        private void btnRefresh_Click(object sender, EventArgs e) { LoadCustomers(); }
        private void btnClose_Click(object sender, EventArgs e) { Close(); }

        private void LoadCustomers()
        {
            try
            {
                List<Customer> customers = userRepository.GetCustomers();
                dgvCustomers.Rows.Clear();

                foreach (Customer customer in customers)
                {
                    dgvCustomers.Rows.Add(
                        customer.CustomerID,
                        customer.Name,
                        customer.Username,
                        customer.Email,
                        customer.Phone,
                        customer.Address,
                        StatusName(customer.Status));
                }

                lblTotal.Text = "Total Customers: " + customers.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load customers.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetStatus(int status, string name)
        {
            if (dgvCustomers.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a customer first.",
                    "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int id = Convert.ToInt32(dgvCustomers.SelectedRows[0].Cells["CustomerID"].Value);

            DialogResult result = MessageBox.Show(
                "Set this customer account to " + name + "?",
                "Confirm Status", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                if (userRepository.UpdateCustomerStatus(id, status))
                {
                    MessageBox.Show(
                        status == 1
                            ? "Customer approved. The customer can now log in."
                            : "Customer status changed to " + name + ".",
                        "Status Updated",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    LoadCustomers();
                }
                else
                {
                    MessageBox.Show("Customer status could not be updated.",
                        "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to update customer status.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string StatusName(int status)
        {
            if (status == 1) return "ACTIVE";
            if (status == 2) return "BLOCKED";
            return "INACTIVE";
        }
    }
}