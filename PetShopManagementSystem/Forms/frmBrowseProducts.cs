using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmBrowseProducts : Form
    {
        private readonly LoginResult currentUser;
        private readonly ProductRepository productRepository;
        private readonly OrderRepository orderRepository;

        private readonly List<CartItem> cartItems;

        private Product selectedProduct = null;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmBrowseProducts() : this(null) { }

        public frmBrowseProducts(LoginResult user)
        {
            InitializeComponent();

            // Skip all real startup logic (database calls) when the Designer
            // is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            currentUser = user;

            productRepository = new ProductRepository();
            orderRepository = new OrderRepository();

            cartItems = new List<CartItem>();

            LoadProducts();
            RefreshCart();
        }

        private void LoadProducts()
        {
            try
            {
                string searchText = txtSearch.Text.Trim();

                List<Product> products;

                if (string.IsNullOrWhiteSpace(searchText))
                {
                    products = productRepository.GetAll();
                }
                else
                {
                    products = productRepository.Search(searchText);
                }

                dgvProducts.Rows.Clear();

                foreach (Product product in products)
                {
                    int rowIndex = dgvProducts.Rows.Add(
                        product.ProductID,
                        product.ManagerID,
                        product.FoodTypeName,
                        product.ProductName,
                        product.Description,
                        product.Price.ToString("0.00"),
                        product.StockQuantity);

                    // Keep the full Product object with the row so it
                    // can be recovered without re-querying the database.
                    dgvProducts.Rows[rowIndex].Tag = product;
                }

                lblTotalProducts.Text =
                    "Available Products: " + products.Count;

                ClearSelection();
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

        private void dgvProducts_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            try
            {
                DataGridViewRow row = dgvProducts.Rows[e.RowIndex];

                selectedProduct = row.Tag as Product;

                if (selectedProduct == null)
                {
                    ClearSelection();
                    return;
                }

                lblSelectedProduct.Text =
                    "Selected: " + selectedProduct.ProductName +
                    "  |  Price: " + selectedProduct.Price.ToString("0.00") +
                    "  |  Stock: " + selectedProduct.StockQuantity;

                if (selectedProduct.StockQuantity > 0)
                {
                    nudQuantity.Minimum = 1;

                    nudQuantity.Maximum =
                        selectedProduct.StockQuantity < 1000
                            ? selectedProduct.StockQuantity
                            : 1000;

                    nudQuantity.Value = 1;

                    btnAddToCart.Enabled = true;
                }
                else
                {
                    nudQuantity.Minimum = 1;
                    nudQuantity.Maximum = 1;
                    nudQuantity.Value = 1;

                    btnAddToCart.Enabled = false;

                    lblSelectedProduct.Text += "  (Out of Stock)";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to select the product.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void ClearSelection()
        {
            selectedProduct = null;

            lblSelectedProduct.Text = "Selected Product: None";

            nudQuantity.Minimum = 1;
            nudQuantity.Maximum = 1000;
            nudQuantity.Value = 1;

            btnAddToCart.Enabled = false;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadProducts();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            LoadProducts();
        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedProduct == null)
                {
                    MessageBox.Show(
                        "Please select a product first.",
                        "Add to Cart",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                if (selectedProduct.StockQuantity <= 0)
                {
                    MessageBox.Show(
                        "This product is currently out of stock.",
                        "Out of Stock",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                int quantity = (int)nudQuantity.Value;

                if (quantity > selectedProduct.StockQuantity)
                {
                    MessageBox.Show(
                        "Requested quantity is greater than available stock.",
                        "Stock Warning",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                /*
                 * sp_PlaceOrder accepts one ManagerID for an order.
                 * Therefore all products in one cart must belong
                 * to the same manager.
                 */
                if (cartItems.Count > 0)
                {
                    int existingManagerID = cartItems[0].Product.ManagerID;

                    if (existingManagerID != selectedProduct.ManagerID)
                    {
                        MessageBox.Show(
                            "You cannot add products from different managers " +
                            "to the same order.\n\n" +
                            "Please place a separate order for another manager.",
                            "Different Manager",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }
                }

                CartItem existingItem = cartItems
                    .FirstOrDefault(x => x.Product.ProductID == selectedProduct.ProductID);

                if (existingItem != null)
                {
                    int newQuantity = existingItem.Quantity + quantity;

                    if (newQuantity > selectedProduct.StockQuantity)
                    {
                        MessageBox.Show(
                            "The total quantity exceeds available stock.",
                            "Stock Warning",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);

                        return;
                    }

                    existingItem.Quantity = newQuantity;
                }
                else
                {
                    CartItem item = new CartItem
                    {
                        Product = selectedProduct,
                        Quantity = quantity
                    };

                    cartItems.Add(item);
                }

                RefreshCart();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to add product to cart.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void RefreshCart()
        {
            dgvCart.Rows.Clear();

            decimal total = 0;

            foreach (CartItem item in cartItems)
            {
                dgvCart.Rows.Add(
                    item.Product.ProductName,
                    item.Quantity,
                    item.Product.Price.ToString("0.00"),
                    item.SubTotal.ToString("0.00"));

                total += item.SubTotal;
            }

            lblTotal.Text = "Total: " + total.ToString("0.00");
            lblCartCount.Text = "Items in Cart: " + cartItems.Count;

            btnRemove.Enabled = dgvCart.Rows.Count > 0;
            btnPlaceOrder.Enabled = cartItems.Count > 0;
            btnClearCart.Enabled = cartItems.Count > 0;
        }

        private void dgvCart_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            dgvCart.Rows[e.RowIndex].Selected = true;
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvCart.SelectedRows.Count == 0)
                {
                    MessageBox.Show(
                        "Please select an item to remove.",
                        "Cart",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                int rowIndex = dgvCart.SelectedRows[0].Index;

                if (rowIndex >= 0 && rowIndex < cartItems.Count)
                {
                    cartItems.RemoveAt(rowIndex);
                    RefreshCart();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to remove the item.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnClearCart_Click(object sender, EventArgs e)
        {
            if (cartItems.Count == 0)
            {
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to clear the cart?",
                "Clear Cart",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                cartItems.Clear();
                RefreshCart();
            }
        }

        private void btnPlaceOrder_Click(object sender, EventArgs e)
        {
            try
            {
                if (cartItems.Count == 0)
                {
                    MessageBox.Show(
                        "Your cart is empty.",
                        "Place Order",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                decimal total = cartItems.Sum(x => x.SubTotal);

                DialogResult result = MessageBox.Show(
                    "Confirm your order?\n\n" +
                    "Total Amount: " + total.ToString("0.00") +
                    "\nPayment Method: COD",
                    "Confirm Order",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                int managerID = cartItems[0].Product.ManagerID;

                decimal totalAmount;
                string orderStatus;
                int newOrderId = orderRepository.PlaceOrder(
                    currentUser.UserID,
                    managerID,
                    cartItems,
                    out totalAmount,
                    out orderStatus);

                if (newOrderId > 0)
                {
                    MessageBox.Show(
                        "Order placed successfully!\n\n" +
                        "Your order is now pending manager confirmation.\n" +
                        "You can pay once the manager confirms it.",
                        "Order Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    cartItems.Clear();
                    RefreshCart();

                    LoadProducts();
                }
                else
                {
                    MessageBox.Show(
                        "The order could not be placed.",
                        "Order Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to place the order.\n\n" + ex.Message,
                    "Order Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void frmBrowseProducts_Load(object sender, EventArgs e)
        {

        }
    }
}
