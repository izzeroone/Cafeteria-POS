using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for AdminDetailWindow.xaml
    /// </summary>
    public partial class AdminDetailWindow : Window
    {
        internal BusinessModuleLocator _businessModuleLocator;
        private readonly AdminRe admin;
        private readonly List<Employee> empwithad;

        public AdminDetailWindow(BusinessModuleLocator businessModuleLocator, AdminRe ad)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            admin = ad;
            empwithad = _businessModuleLocator.EmployeeModule.getEmployeeWithAd(admin.AdId).ToList();
            lvDataEmployee.ItemsSource = empwithad;
            loadAdData();
        }

        private void loadAdData()
        {
            AdminInfo.DataContext = admin;
            //txtName.IsEnabled = false;
        }

        private void bntUpdate_Click(object sender, RoutedEventArgs e)
        {
            //check name
            var namee = txtName.Text.Trim();
            if (namee.Length == 0 || namee.Length > 50)
            {
                MessageBox.Show("Nhập tên!");
                txtName.Focus();
                return;
            }

            admin.Name = namee;
            //admin.Employees.Clear();
            _businessModuleLocator.AdminModule.updateAdmin(admin);

            MessageBox.Show("Cập nhật thành công!");

            Close();
        }

        private void btnChangePass_Click(object sender, RoutedEventArgs e)
        {
            var adPass = new AdminChangePass(_businessModuleLocator, admin);
            adPass.ShowDialog();
        }
    }
}