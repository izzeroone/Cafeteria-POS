using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.User;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.AdminWorkSpace
{
    
    /// <summary>
    /// Interaction logic for EmployeeListPage.xaml
    /// </summary>
    public partial class EmployeeListPage : Page
    {
        private BusinessModuleLocator _businessModuleLocator;
        private AdminRe admin;
        private Employee emp;
        private List<Employee> empwithad;
        internal EmployeeAddOrUpdateDialog empAddUptDialog;

        public EmployeeListPage(BusinessModuleLocator businessModuleLocator, AdminRe ad)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            admin = ad;

            this.Loaded += EmployeeListPage_Loaded;
        }

        private void EmployeeListPage_Loaded(object sender, RoutedEventArgs e)
        {
            //            empwithad = _unitofork.EmployeeRepository.Get(x => x.Manager.Equals(admin.AdId) && x.Deleted.Equals(0)).ToList();
            refreshData();

            txtBirth.DisplayDateEnd = new DateTime((DateTime.Now.Year - 16), 12, 31);
            txtStart.DisplayDateStart = DateTime.Now;
        }

        private void lvData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            emp = lvDataEmployee.SelectedItem as Employee;
            if (emp == null)
            {
                txtID.Text = "";
                txtName.Text = "";
                txtBirth.SelectedDate = new DateTime(1990, 1, 1);
                txtAddress.Text = "";
                txtHour_wage.Text = "";
                txtMail.Text = "";
                txtPhone.Text = "";
                txtStart.SelectedDate = DateTime.Now;
                txtAcount.Text = "";
                txtPass.Password = "";
                return;
            }
            txtID.Text = emp.EmpId;
            txtName.Text = emp.Name;
            txtBirth.SelectedDate = emp.Birth;
            txtAddress.Text = emp.Addr;
            txtHour_wage.Text = emp.HourWage.ToString();
            txtMail.Text = emp.Email;
            txtPhone.Text = emp.Phone;
            txtStart.SelectedDate = emp.Startday;
            txtAcount.Text = emp.Username;
            txtPass.Password = emp.DecryptedPass;
            switch (emp.EmpRole)
            {
                case (int) EmployeeRole.Counter:
                {
                    txtRole.Text = EmployeeRole.Counter.ToString();
                    break;
                }
                case (int)EmployeeRole.Stock:
                {
                    txtRole.Text = EmployeeRole.Stock.ToString();
                    break;
                }
            }
        }

        private void bntAddnew_Click(object sender, RoutedEventArgs e)
        {
            empAddUptDialog = new EmployeeAddOrUpdateDialog(_businessModuleLocator);
            empAddUptDialog.ShowDialog();

            refreshData();
        }

        private void bntUpdate_Click(object sender, RoutedEventArgs e)
        {
            if(lvDataEmployee.SelectedItem == null)
            {
                MessageBox.Show("Employee must be selected to update! Choose again!");
                return;
            }

            empAddUptDialog = new EmployeeAddOrUpdateDialog(_businessModuleLocator, emp);
            empAddUptDialog.ShowDialog();
            lvDataEmployee.UnselectAll();
            lvDataEmployee.Items.Refresh();
        }

        private void bntDel_Click(object sender, RoutedEventArgs e)
        {
            if(lvDataEmployee.SelectedItem == null)
            {
                MessageBox.Show("Employee must be selected to delete! Choose again!");
                return;
            }

            var delEmp = lvDataEmployee.SelectedItem as Employee;
            if (delEmp != null)
            {
                MessageBoxResult delMess = MessageBox.Show("Do you want to delete " + delEmp.Name + "(" + delEmp.Username + ")?", "Warning! Are you sure?", MessageBoxButton.YesNo);
                if(delMess == MessageBoxResult.Yes)
                {
                    _businessModuleLocator.EmployeeModule.deleteEmployee(delEmp);
                    refreshData();
                }
            }
            else
            {
                MessageBox.Show("Please choose employee you want to delete and try again!");
            }
        }

        private void refreshData()
        {
            empwithad = _businessModuleLocator.EmployeeModule.getEmployees().ToList();
            lvDataEmployee.ItemsSource = empwithad;
            lvDataEmployee.UnselectAll();
            lvDataEmployee.Items.Refresh();
        }
    }
}
