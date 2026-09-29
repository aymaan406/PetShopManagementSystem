using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmManageOrders : Form
    {
        private readonly OrderRepository orderRepository;
        private readonly LoginResult currentUser;

        private int selectedOrderID = 0;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmManageOrders() : this(null) { }

        public frmManageOrders(LoginResult user)
        {
            InitializeComponent();

            // Skip all real startup logic (database calls) when the Designer
            // is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            currentUser = user;
            orderRepository = new OrderRepository();

            cmbStatusFilter.SelectedIndex = 0;

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                List<Order> orders = orderRepository.GetOrdersByManager(currentUser.UserID);

                dgvOrders.Rows.Clear();

                string searchText = txtSearch.Text.Trim().ToLower();
                string selectedStatus = cmbStatusFilter.SelectedItem == null
                    ? "PENDING"
                    : cmbStatusFilter.SelectedItem.ToString();

                int count = 0;

                foreach (Order order in orders)
                {
                    bool matchesSearch =
                        searchText == "" ||
                        order.OrderID.ToString().Contains(searchText) ||
                        order.CustomerID.ToString().Contains(searchText);

                    bool matchesStatus =
                        order.OrderStatus.Equals(
                            selectedStatus,
                            StringComparison.OrdinalIgnoreCase);

                    if (matchesSearch && matchesStatus)
                    {
                        dgvOrders.Rows.Add(
                            order.OrderID,
                            order.CustomerID,
                            order.OrderDate.ToString("dd-MM-yyyy HH:mm"),
                            order.TotalAmount.ToString("0.00"),
                            order.OrderStatus
                        );

                        count++;
                    }
                }

                lblTotalOrders.Text = "Total Orders: " + count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "An error occurred while loading orders.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvOrders_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            try
            {
                selectedOrderID =
                    Convert.ToInt32(dgvOrders.Rows[e.RowIndex].Cells["OrderID"].Value);

                LoadOrderItems(selectedOrderID);

                string status =
                    dgvOrders.Rows[e.RowIndex].Cells["OrderStatus"].Value.ToString();

                lblSelectedOrder.Text =
                    "Selected Order: #" + selectedOrderID +
                    " | Status: " + status;

                btnConfirm.Enabled =
                    status.Equals("PENDING", StringComparison.OrdinalIgnoreCase);

                btnReject.Enabled =
                    status.Equals("PENDING", StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load order details.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadOrderItems(int orderID)
        {
            try
            {
                List<OrderItem> items =
                    orderRepository.GetOrderItems(orderID);

                dgvOrderItems.Rows.Clear();

                foreach (OrderItem item in items)
                {
                    dgvOrderItems.Rows.Add(
                        item.ProductID,
                        item.Quantity,
                        item.UnitPrice.ToString("0.00"),
                        item.SubTotal.ToString("0.00")
                    );
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load order items.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (selectedOrderID == 0)
            {
                MessageBox.Show(
                    "Please select an order first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to confirm Order #" +
                selectedOrderID + "?",
                "Confirm Order",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (result != DialogResult.Yes)
                return;

            try
            {
                bool success =
                    orderRepository.ConfirmOrder(selectedOrderID, currentUser.UserID);

                if (success)
                {
                    MessageBox.Show(
                        "Order #" + selectedOrderID +
                        " has been confirmed successfully.",
                        "Order Confirmed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    selectedOrderID = 0;
                    dgvOrderItems.Rows.Clear();
                    lblSelectedOrder.Text = "Selected Order: None";

                    btnConfirm.Enabled = false;
                    btnReject.Enabled = false;

                    LoadOrders();
                }
                else
                {
                    MessageBox.Show(
                        "The order could not be confirmed.",
                        "Operation Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to confirm the order.\n\n" + ex.Message,
                    "Order Confirmation Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnReject_Click(object sender, EventArgs e)
        {
            if (selectedOrderID == 0)
            {
                MessageBox.Show(
                    "Please select an order first.",
                    "Validation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to reject Order #" +
                selectedOrderID + "?",
                "Reject Order",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result != DialogResult.Yes)
                return;

            try
            {
                bool success =
                    orderRepository.RejectOrder(selectedOrderID, currentUser.UserID);

                if (success)
                {
                    MessageBox.Show(
                        "Order #" + selectedOrderID +
                        " has been rejected.",
                        "Order Rejected",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    selectedOrderID = 0;
                    dgvOrderItems.Rows.Clear();
                    lblSelectedOrder.Text = "Selected Order: None";

                    btnConfirm.Enabled = false;
                    btnReject.Enabled = false;

                    LoadOrders();
                }
                else
                {
                    MessageBox.Show(
                        "The order could not be rejected.",
                        "Operation Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to reject the order.\n\n" + ex.Message,
                    "Order Rejection Failed",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            LoadOrders();
        }

        private void cmbStatusFilter_SelectedIndexChanged(
            object sender,
            EventArgs e)
        {
            LoadOrders();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            txtSearch.Clear();
            cmbStatusFilter.SelectedIndex = 0;

            selectedOrderID = 0;

            dgvOrderItems.Rows.Clear();

            lblSelectedOrder.Text = "Selected Order: None";

            btnConfirm.Enabled = false;
            btnReject.Enabled = false;

            LoadOrders();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
