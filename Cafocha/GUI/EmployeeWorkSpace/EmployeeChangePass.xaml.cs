using System.Windows;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for EmployeeChangePass.xaml
    /// </summary>
    public partial class EmployeeChangePass : Window
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        private readonly Employee _emp;

        public EmployeeChangePass(BusinessModuleLocator businessModuleLocator, Employee emp)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            _emp = emp;
            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.NoResize;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var oldPass = txtPass.Password.Trim();
            if (!oldPass.Equals(_emp.Pass))
            {
                MessageBox.Show("Your old password is incorrect!");
                txtPass.Focus();
                return;
            }

            var newPass = txtNewPass.Password.Trim();
            if (newPass.Length == 0 || newPass.Length > 50)
            {
                MessageBox.Show("New password is not valid!");
                txtNewPass.Focus();
                return;
            }

            var passcon = txtConNew.Password.Trim();
            if (!passcon.Equals(newPass))
            {
                MessageBox.Show("Confirm new password is not match!");
                txtConNew.Focus();
                return;
            }

            _businessModuleLocator.EmployeeModule.updateEmployeePassword(_emp, newPass);

            MessageBox.Show("Your password was changed!");
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}