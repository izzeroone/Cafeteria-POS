using System.Windows;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for AdminChangePass.xaml
    /// </summary>
    public partial class AdminChangePass : Window
    {
        private readonly AdminRe _admin;
        private readonly BusinessModuleLocator _businessModuleLocator;

        public AdminChangePass(BusinessModuleLocator businessModuleLocator, AdminRe admin)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            _admin = admin;
            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.NoResize;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            var oldPass = txtPass.Password.Trim();
            if (!oldPass.Equals(_admin.DecryptedPass))
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

            _admin.Pass = newPass;
            _businessModuleLocator.AdminModule.updateAdmin(_admin);
            MessageBox.Show("Your password was changed!");
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}