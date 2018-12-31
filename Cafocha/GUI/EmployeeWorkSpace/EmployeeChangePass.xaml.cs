using System.Linq;
using System.Windows;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.User;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    /// Interaction logic for EmployeeChangePass.xaml
    /// </summary>
    public partial class EmployeeChangePass : Window
    {
        private BusinessModuleLocator _businessModuleLocator;
        private Employee _emp;

        public EmployeeChangePass(BusinessModuleLocator businessModuleLocator, Employee emp)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            _emp = emp;
            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            string oldPass = txtPass.Password.Trim();
            if (!oldPass.Equals(_emp.Pass))
            {
                MessageBox.Show("Your old password is incorrect!");
                txtPass.Focus();
                return;
            }

            string newPass = txtNewPass.Password.Trim();
            if (newPass.Length == 0 || newPass.Length > 50)
            {
                MessageBox.Show("New password is not valid!");
                txtNewPass.Focus();
                return;
            }

            string passcon = txtConNew.Password.Trim();
            if (!passcon.Equals(newPass))
            {
                MessageBox.Show("Confirm new password is not match!");
                txtConNew.Focus();
                return;
            }

            _businessModuleLocator.EmployeeModule.updateEmployeePassword(_emp, newPass);

            MessageBox.Show("Your password was changed!");
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
