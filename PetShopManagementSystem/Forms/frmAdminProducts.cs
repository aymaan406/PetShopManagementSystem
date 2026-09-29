using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmAdminProducts : Form
    {
        private readonly ProductRepository productRepository = new ProductRepository();

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmAdminProducts() : this(null) { }

        public frmAdminProducts(LoginResult user)
        {
            InitializeComponent();

            // Skip all real startup logic (validation, database calls) when the
            // Designer is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            if (user == null || !string.Equals(user.Role, "Admin", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Only an Admin can view all products.");

            LoadProducts();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e) { LoadProducts(); }

        private void btnRefresh_Click(object sender, EventArgs e) { txtAdminSearch.Clear(); LoadProducts(); }

        private void btnClose_Click(object sender, EventArgs e) { Close(); }

        private void LoadProducts()
        {
            try
            {
                string search = txtAdminSearch.Text.Trim();
                List<Product> products = string.IsNullOrWhiteSpace(search)
                    ? productRepository.GetAll()
                    : productRepository.Search(search);

                dgvProducts.Rows.Clear();

                foreach (Product product in products)
                {
                    dgvProducts.Rows.Add(
                        product.ProductID,
                        product.ManagerID,
                        product.FoodTypeName,
                        product.ProductName,
                        product.Description,
                        product.Price.ToString("0.00"),
                        product.StockQuantity);
                }

                lblTotal.Text = "Total Products: " + products.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load products.\n\n" + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
