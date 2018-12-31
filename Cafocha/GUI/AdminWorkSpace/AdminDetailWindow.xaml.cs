using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.User;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    /// Interaction logic for AdminDetailWindow.xaml
    /// </summary>
    
    public partial class AdminDetailWindow : Window
    {
        internal BusinessModuleLocator _businessModuleLocator;
        private AdminRe admin;
        private List<Employee> empwithad;
        public AdminDetailWindow(BusinessModuleLocator businessModuleLocator, AdminRe ad)
        {
            InitializeComponent();
            _businessModuleLocator = businessModuleLocator;
            admin = ad;
            empwithad = _businessModuleLocator.EmployeeModule.getEmployeeWithAd(admin.AdId).ToList();
            lvDataEmployee.ItemsSource = empwithad;
            loadAdData();
        }

        private void loadAdData()
        {
            this.AdminInfo.DataContext = admin;
            //txtName.IsEnabled = false;
        }

        private void bntUpdate_Click(object sender, RoutedEventArgs e)
        {
            //check name
            string namee = txtName.Text.Trim();
            if (namee.Length == 0 || namee.Length > 50)
            {
                MessageBox.Show("Name is not valid!");
                txtName.Focus();
                return;
            }

            admin.Name = namee;
            //admin.Employees.Clear();
            _businessModuleLocator.AdminModule.updateAdmin(admin);
        }

        private void btnChangePass_Click(object sender, RoutedEventArgs e)
        {
            AdminChangePass adPass = new AdminChangePass(_businessModuleLocator, admin);
            adPass.ShowDialog();
        }
    }
}
