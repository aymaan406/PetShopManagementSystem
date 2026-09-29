using PetShopManagementSystem;
using System;
using System.Windows.Forms;

namespace PetShopManagementSystem
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Application.Run(new Forms.frmLogin());
        }
    }
}
