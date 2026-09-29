using System;
using System.ComponentModel;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmPayment : Form
    {
        private readonly LoginResult currentUser;
        private readonly int orderID;

        private readonly PaymentRepository paymentRepository;
        private readonly OrderRepository orderRepository;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmPayment() : this(null, 0) { }

        public frmPayment(
            LoginResult user,
            int selectedOrderID)
        {
            InitializeComponent();

            // Skip all real startup logic (database calls) when the Designer
            // is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            currentUser = user;
            orderID = selectedOrderID;

            paymentRepository = new PaymentRepository();
            orderRepository = new OrderRepository();

            LoadPaymentDetails();
        }

        private void LoadPaymentDetails()
        {
            try
            {
                decimal amount =
                    paymentRepository.GetPaymentAmount(orderID);

                string paymentStatus =
                    paymentRepository.GetPaymentStatus(orderID);

                string orderStatus =
                    orderRepository.GetOrderStatus(orderID);

                lblOrderID.Text =
                    "Order ID: #" + orderID;

                lblAmount.Text =
                    "Amount: " + amount.ToString("0.00");

                lblPaymentMethod.Text =
                    "Payment Method: COD";

                lblPaymentStatus.Text =
                    "Payment Status: " + paymentStatus;

                if (!string.Equals(orderStatus, "CONFIRMED", StringComparison.OrdinalIgnoreCase))
                {
                    btnPay.Enabled = false;
                    //lblMessage.Text = "Payment is unavailable until the manager confirms the order.";
                }
                else if (paymentStatus == "PAID")
                {
                    btnPay.Enabled = false;
                    //lblMessage.Text = "This order has already been paid.";
                }
                else
                {
                    btnPay.Enabled = true;
                    //lblMessage.Text ="Payment will be recorded.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load payment details.\n\n" +
                    ex.Message,
                    "Payment Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            try
            {
                string orderStatus =
                    orderRepository.GetOrderStatus(orderID);

                if (!string.Equals(orderStatus, "CONFIRMED", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show(
                        "Payment is available only after the manager confirms your order.",
                        "Payment Not Available",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    LoadPaymentDetails();
                    return;
                }

                string paymentStatus =
                    paymentRepository.GetPaymentStatus(orderID);

                if (paymentStatus == "PAID")
                {
                    MessageBox.Show(
                        "This payment has already been completed.",
                        "Payment",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    btnPay.Enabled = false;
                    return;
                }

                DialogResult result =
                    MessageBox.Show(
                        "Are you sure you want to mark this order as paid?\n\n" +
                        "Payment Method: Cash on Delivery",
                        "Confirm Payment",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                if (result != DialogResult.Yes)
                {
                    return;
                }

                bool success =
                    paymentRepository.MarkPaymentPaid(orderID);

                if (success)
                {
                    MessageBox.Show(
                        "Payment recorded successfully!",
                        "Payment Successful",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);

                    LoadPaymentDetails();
                }
                else
                {
                    MessageBox.Show(
                        "Payment could not be recorded.",
                        "Payment Failed",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to process payment.\n\n" +
                    ex.Message,
                    "Payment Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadPaymentDetails();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void lblSubtitle_Click(object sender, EventArgs e)
        {

        }
    }
}
