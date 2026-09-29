using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmMyOrders : Form
    {
        private readonly LoginResult currentUser;
        private readonly OrderRepository orderRepository;
        private readonly PaymentRepository paymentRepository;

        private int selectedOrderID = -1;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmMyOrders() : this(null) { }

        public frmMyOrders(LoginResult user)
        {
            InitializeComponent();

            // Skip all real startup logic (database calls) when the Designer
            // is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            currentUser = user;
            orderRepository = new OrderRepository();
            paymentRepository = new PaymentRepository();

            cmbStatusFilter.SelectedIndex = 0;

            LoadOrders();
        }

        private void LoadOrders()
        {
            try
            {
                List<Order> orders =
                    orderRepository.GetOrdersByCustomer(currentUser.UserID);

                string searchText = txtSearch.Text.Trim().ToLower();
                string statusFilter = cmbStatusFilter.Text;

                if (!string.IsNullOrWhiteSpace(searchText))
                {
                    orders = orders
                        .Where(o => o.OrderID.ToString().Contains(searchText))
                        .ToList();
                }

                if (statusFilter != "All")
                {
                    orders = orders
                        .Where(o => o.OrderStatus == statusFilter)
                        .ToList();
                }

                dgvOrders.Rows.Clear();

                foreach (Order order in orders)
                {
                    string paymentStatus = "-";

                    try
                    {
                        paymentStatus =
                            paymentRepository.GetPaymentStatus(order.OrderID);
                    }
                    catch
                    {
                        paymentStatus = "-";
                    }

                    dgvOrders.Rows.Add(
                        order.OrderID,
                        order.OrderDate.ToString("dd-MMM-yyyy hh:mm tt"),
                        order.TotalAmount.ToString("0.00"),
                        order.OrderStatus,
                        paymentStatus);
                }

                lblTotalOrders.Text =
                    "Total Orders: " + orders.Count;

                ClearOrderDetails();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load your orders.\n\n" + ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void dgvOrders_CellClick(
            object sender,
            DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            try
            {
                DataGridViewRow row =
                    dgvOrders.Rows[e.RowIndex];

                selectedOrderID =
                    Convert.ToInt32(
                        row.Cells["OrderID"].Value);

                string orderDate =
                    Convert.ToString(
                        row.Cells["OrderDate"].Value);

                string totalAmount =
                    Convert.ToString(
                        row.Cells["TotalAmount"].Value);

                string orderStatus =
                    Convert.ToString(
                        row.Cells["OrderStatus"].Value);

                string paymentStatus =
                    Convert.ToString(
                        row.Cells["PaymentStatus"].Value);

                lblSelectedOrder.Text =
                    "Selected Order: #" + selectedOrderID;

                lblOrderDate.Text =
                    "Order Date: " + orderDate;

                lblOrderTotal.Text =
                    "Total Amount: " + totalAmount;

                lblOrderStatus.Text =
                    "Order Status: " + orderStatus;

                lblPaymentStatus.Text =
                    "Payment Status: " + paymentStatus;

                btnPayment.Enabled =
                    orderStatus.Equals("CONFIRMED", StringComparison.OrdinalIgnoreCase) &&
                    paymentStatus.Equals("UNPAID", StringComparison.OrdinalIgnoreCase);

                LoadOrderItems(selectedOrderID);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to select the order.\n\n" + ex.Message,
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
                        item.SubTotal.ToString("0.00"));
                }

                lblTotalItems.Text =
                    "Items: " + items.Count;
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

        private void ClearOrderDetails()
        {
            selectedOrderID = -1;

            lblSelectedOrder.Text =
                "Selected Order: -";

            lblOrderDate.Text =
                "Order Date: -";

            lblOrderTotal.Text =
                "Total Amount: -";

            lblOrderStatus.Text =
                "Order Status: -";

            lblPaymentStatus.Text =
                "Payment Status: -";

            btnPayment.Enabled = false;

            lblTotalItems.Text =
                "Items: 0";

            dgvOrderItems.Rows.Clear();
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

            LoadOrders();
        }

        private void btnPayment_Click(object sender, EventArgs e)
        {
            try
            {
                if (selectedOrderID == -1)
                {
                    MessageBox.Show(
                        "Please select an order first.",
                        "Payment",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);

                    return;
                }

                string paymentStatus =
                    paymentRepository.GetPaymentStatus(selectedOrderID);

                string orderStatus =
                    orderRepository.GetOrdersByCustomer(currentUser.UserID)
                        .Find(o => o.OrderID == selectedOrderID)?.OrderStatus;

                if (!string.Equals(
                    orderStatus,
                    "CONFIRMED",
                    StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Payment is available only after the manager confirms your order.",
                        "Payment Not Available",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                if (paymentStatus == "PAID")
                {
                    MessageBox.Show(
                        "This order has already been paid.",
                        "Payment",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    return;
                }

                frmPayment paymentForm =
                    new frmPayment(
                        currentUser,
                        selectedOrderID);

                paymentForm.ShowDialog();

                LoadOrders();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to open payment form.\n\n" +
                    ex.Message,
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
