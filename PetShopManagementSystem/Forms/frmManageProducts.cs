using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmManageProducts : Form
    {
        private readonly ProductRepository productRepository;
        private readonly FoodTypeRepository foodTypeRepository;
        private readonly LoginResult currentUser;

        private int selectedProductID = 0;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmManageProducts() : this(null) { }

        public frmManageProducts(LoginResult user)
        {
            InitializeComponent();

            // Skip all real startup logic (database calls) when the Designer
            // is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            currentUser = user;

            productRepository = new ProductRepository();
            foodTypeRepository = new FoodTypeRepository();

            LoadFoodTypes();
            LoadProducts();

            ClearFields();
        }

        private void LoadFoodTypes()
        {
            try
            {
                List<FoodType> foodTypes = foodTypeRepository.GetAll();

                cmbFoodType.Items.Clear();

                foreach (FoodType foodType in foodTypes)
                {
                    cmbFoodType.Items.Add(
                        new FoodTypeItem
                        {
                            ID = foodType.FoodTypeID,
                            Name = foodType.TypeName
                        });
                }

                if (cmbFoodType.Items.Count > 0)
                {
                    cmbFoodType.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load food types.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadProducts()
        {
            try
            {
                int managerID = currentUser.UserID;

                List<Product> products =
                    productRepository.GetByManager(managerID);

                dgvProducts.Rows.Clear();

                foreach (Product product in products)
                {
                    int rowIndex = dgvProducts.Rows.Add();

                    dgvProducts.Rows[rowIndex].Cells["colProductID"].Value =
                        product.ProductID;

                    dgvProducts.Rows[rowIndex].Cells["colFoodType"].Value =
                        product.FoodTypeName;

                    dgvProducts.Rows[rowIndex].Cells["colProductName"].Value =
                        product.ProductName;

                    dgvProducts.Rows[rowIndex].Cells["colDescription"].Value =
                        product.Description;

                    dgvProducts.Rows[rowIndex].Cells["colPrice"].Value =
                        product.Price;

                    dgvProducts.Rows[rowIndex].Cells["colStock"].Value =
                        product.StockQuantity;
                }

                lblTotalProducts.Text =
                    "Total Products: " + products.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load products.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (!ValidateFields())
                {
                    return;
                }

                FoodTypeItem selectedFoodType =
                    cmbFoodType.SelectedItem as FoodTypeItem;

                Product product = new Product();

                product.ManagerID = currentUser.UserID;
                product.FoodTypeID = selectedFoodType.ID;
                product.ProductName = txtProductName.Text.Trim();
                product.Description = txtDescription.Text.Trim();
                product.Price = Convert.ToDecimal(txtPrice.Text.Trim());
                product.StockQuantity =
                    Convert.ToInt32(txtStock.Text.Trim());

                bool success =
                    productRepository.Add(product);

                if (success)
                {
                    MessageBox.Show(
                        "Product added successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadProducts();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show(
                        "Unable to add the product.",
                        "Add Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show(
                    "Please enter valid numeric values for price and stock.",
                    "Invalid Input",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while adding the product.\n\n"
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedProductID == 0)
                {
                    MessageBox.Show(
                        "Please select a product from the table first.",
                        "Selection Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (!ValidateFields())
                {
                    return;
                }

                FoodTypeItem selectedFoodType =
                    cmbFoodType.SelectedItem as FoodTypeItem;

                Product product = new Product();

                product.ProductID = selectedProductID;
                product.ManagerID = currentUser.UserID;
                product.FoodTypeID = selectedFoodType.ID;
                product.ProductName = txtProductName.Text.Trim();
                product.Description = txtDescription.Text.Trim();
                product.Price = Convert.ToDecimal(txtPrice.Text.Trim());
                product.StockQuantity =
                    Convert.ToInt32(txtStock.Text.Trim());

                bool success =
                    productRepository.Update(product);

                if (success)
                {
                    MessageBox.Show(
                        "Product updated successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadProducts();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show(
                        "Unable to update the product.",
                        "Update Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (FormatException)
            {
                MessageBox.Show(
                    "Please enter valid numeric values for price and stock.",
                    "Invalid Input",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while updating the product.\n\n"
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedProductID == 0)
                {
                    MessageBox.Show(
                        "Please select a product from the table first.",
                        "Selection Required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                DialogResult result = MessageBox.Show(
                    "Are you sure you want to delete this product?",
                    "Confirm Delete",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                bool success =
                    productRepository.Delete(selectedProductID);

                if (success)
                {
                    MessageBox.Show(
                        "Product deleted successfully.",
                        "Success",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadProducts();
                    ClearFields();
                }
                else
                {
                    MessageBox.Show(
                        "Unable to delete the product.",
                        "Delete Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while deleting the product.\n\n"
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string searchText =
                    txtSearch.Text.Trim();

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    LoadProducts();
                    return;
                }

                List<Product> products =
                    productRepository.Search(searchText);

                dgvProducts.Rows.Clear();

                foreach (Product product in products)
                {
                    if (product.ManagerID != currentUser.UserID)
                    {
                        continue;
                    }

                    int rowIndex =
                        dgvProducts.Rows.Add();

                    dgvProducts.Rows[rowIndex].Cells["colProductID"].Value =
                        product.ProductID;

                    dgvProducts.Rows[rowIndex].Cells["colFoodType"].Value =
                        product.FoodTypeName;

                    dgvProducts.Rows[rowIndex].Cells["colProductName"].Value =
                        product.ProductName;

                    dgvProducts.Rows[rowIndex].Cells["colDescription"].Value =
                        product.Description;

                    dgvProducts.Rows[rowIndex].Cells["colPrice"].Value =
                        product.Price;

                    dgvProducts.Rows[rowIndex].Cells["colStock"].Value =
                        product.StockQuantity;
                }

                lblTotalProducts.Text =
                    "Search Results: " + dgvProducts.Rows.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to search products.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();

            LoadFoodTypes();
            LoadProducts();

            ClearFields();
        }

        private void dgvProducts_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.RowIndex < 0)
                {
                    return;
                }

                DataGridViewRow row =
                    dgvProducts.Rows[e.RowIndex];

                selectedProductID =
                    Convert.ToInt32(
                        row.Cells["colProductID"].Value);

                txtProductName.Text =
                    row.Cells["colProductName"].Value.ToString();

                txtDescription.Text =
                    row.Cells["colDescription"].Value.ToString();

                txtPrice.Text =
                    row.Cells["colPrice"].Value.ToString();

                txtStock.Text =
                    row.Cells["colStock"].Value.ToString();

                string foodTypeName =
                    row.Cells["colFoodType"].Value.ToString();

                for (int i = 0; i < cmbFoodType.Items.Count; i++)
                {
                    FoodTypeItem item =
                        cmbFoodType.Items[i] as FoodTypeItem;

                    if (item != null &&
                        item.Name == foodTypeName)
                    {
                        cmbFoodType.SelectedIndex = i;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load the selected product.\n\n"
                    + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private bool ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text))
            {
                MessageBox.Show(
                    "Please enter the product name.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtProductName.Focus();
                return false;
            }

            if (cmbFoodType.SelectedItem == null)
            {
                MessageBox.Show(
                    "Please select a food type.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                cmbFoodType.Focus();
                return false;
            }

            decimal price;

            if (!decimal.TryParse(
                txtPrice.Text.Trim(),
                out price) || price <= 0)
            {
                MessageBox.Show(
                    "Please enter a valid price greater than 0.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtPrice.Focus();
                return false;
            }

            int stock;

            if (!int.TryParse(
                txtStock.Text.Trim(),
                out stock) || stock < 0)
            {
                MessageBox.Show(
                    "Stock quantity must be a valid number and cannot be negative.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                txtStock.Focus();
                return false;
            }

            return true;
        }

        private void ClearFields()
        {
            selectedProductID = 0;

            txtProductName.Clear();
            txtDescription.Clear();
            txtPrice.Clear();
            txtStock.Clear();

            if (cmbFoodType.Items.Count > 0)
            {
                cmbFoodType.SelectedIndex = 0;
            }

            txtProductName.Focus();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private class FoodTypeItem
        {
            public int ID { get; set; }

            public string Name { get; set; }

            public override string ToString()
            {
                return Name;
            }
        }

        private void frmManageProducts_Load(object sender, EventArgs e)
        {

        }

        private void lblSearch_Click(object sender, EventArgs e)
        {

        }
    }
}
