using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using PetShopManagementSystem.Models;
using PetShopManagementSystem.Repositories;

namespace PetShopManagementSystem.Forms
{
    public partial class frmAdminReports : Form
    {
        private readonly ReportRepository reportRepository;

        // Parameterless constructor used only by the Visual Studio Form Designer
        // so it can open this form in Design view. Never call this at runtime.
        public frmAdminReports() : this(null) { }

        public frmAdminReports(LoginResult user)
        {
            InitializeComponent();

            // Skip all real startup logic (database calls) when the Designer
            // is the one creating this form for the design surface.
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
                return;

            reportRepository = new ReportRepository();

            LoadReport();
            LoadOrderSummary();
        }

        private void LoadReport()
        {
            try
            {
                ReportSummary summary =
                    reportRepository.GetSummary();

                lblTotalRevenue.Text =
                    summary.TotalRevenue.ToString("0.00");

                lblTotalOrders.Text =
                    summary.TotalOrders.ToString();

                lblTotalCustomers.Text =
                    summary.TotalCustomers.ToString();

                lblTotalManagers.Text =
                    summary.TotalManagers.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load report summary.\n\n" +
                    ex.Message,
                    "Report Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void LoadOrderSummary()
        {
            try
            {
                List<Order> orders =
                    reportRepository.GetOrderSummary();

                dgvOrderSummary.Rows.Clear();

                foreach (Order order in orders)
                {
                    dgvOrderSummary.Rows.Add(
                        order.OrderID,
                        order.CustomerName,
                        order.ManagerName,
                        order.OrderDate.ToString(
                            "dd-MMM-yyyy hh:mm tt"),
                        order.TotalAmount.ToString("0.00"),
                        order.OrderStatus);
                }

                lblSummaryCount.Text =
                    "Orders Shown: " + orders.Count;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to load order summary.\n\n" +
                    ex.Message,
                    "Report Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadReport();
            LoadOrderSummary();
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
