using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.Entities;
using MaterialDesignThemes.Wpf;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for PermissionRequired.xaml
    /// </summary>
    public partial class PermissionRequired : Window
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        private readonly Chip _cUser;
        private readonly bool _isPrinted;
        private readonly bool _isTable;

        public PermissionRequired(BusinessModuleLocator BusinessModuleLocator, Chip cUser, bool isPrinted, bool isTable)
        {
            _businessModuleLocator = BusinessModuleLocator;
            _cUser = cUser;
            _isPrinted = isPrinted;
            _isTable = isTable;
            InitializeComponent();

            //txtUsername.Focus();

            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.NoResize;
        }

        private async void btnAcceptLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var pass = txtPass.Password.Trim();
            try
            {
                btnAcceptLogin.IsEnabled = false;
                PgbLoginProcess.Visibility = Visibility.Visible;
                await Async(username, pass, null);

                btnAcceptLogin.IsEnabled = true;
                PgbLoginProcess.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async Task Async(string username, string pass, object p)
        {
            try
            {
                await Task.Run(() =>
                {
                    var AdList = _businessModuleLocator.AdminModule.getAdmins().ToList();

                    var ad = AdList.FirstOrDefault(x => x.Username.Equals(username) && x.DecryptedPass.Equals(pass));
                    //Get Admin
                    var isFoundAd = false;
                    if (ad != null)
                    {
                        Application.Current.Properties["AdLogin"] = ad;
                        isFoundAd = true;
                    }

                    if (!isFoundAd)
                    {
                        MessageBox.Show("incorrect username or password");
                        return;
                    }

                    Dispatcher.Invoke(() =>
                    {
                        _cUser.Content = (Application.Current.Properties["AdLogin"] as AdminRe).Username;
                        if (_isPrinted)
                        {
                            var dcd = new DeleteConfirmDialog(_cUser, _isTable);
                            if (dcd.ShowDialog() == false) DialogResult = false;

                            Close();
                        }

                        Close();
                    });
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) txtPass.Focus();
        }

        private async void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var username = txtUsername.Text.Trim();
                var pass = txtPass.Password.Trim();
                try
                {
                    btnAcceptLogin.IsEnabled = false;
                    PgbLoginProcess.Visibility = Visibility.Visible;
                    await Async(username, pass, null);

                    btnAcceptLogin.IsEnabled = true;
                    PgbLoginProcess.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private void btnAcceptCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}