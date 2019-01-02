using System;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Cafocha.BusinessContext;
using Cafocha.GUI.BusinessModel;
using log4net;

namespace Cafocha.GUI
{
    /// <summary>
    ///     Interaction logic for Login.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private static readonly ILog AppLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        internal BusinessModuleLocator _businessModuleLocator;

        private bool isCodeLoginTurnOn;
        private readonly DispatcherTimer LoadCodeLogin;

        public LoginWindow()
        {
            var config = ReadWriteData.ReadDBConfig();

//            try
//            {
//                _businessModuleLocator = new BusinessModuleLocator(config);
//            }
//            catch (Exception e)
//            {
//                _businessModuleLocator = new BusinessModuleLocator();
//            }

            InitializeComponent();
            _businessModuleLocator = new BusinessModuleLocator();
            //            txtUsername.Focus();

            WindowState = WindowState.Normal;
            ResizeMode = ResizeMode.NoResize;

            LoadCodeLogin = new DispatcherTimer();
            LoadCodeLogin.Tick += LoadCodeLogin_Tick;
            ;
            LoadCodeLogin.Interval = new TimeSpan(0, 0, 0, 0, 1);

            Closing += Closing_LoginWindos;
            LoginModule = new LoginModule(this);
        }

        public LoginModule LoginModule { get; }

        private void LoadCodeLogin_Tick(object sender, EventArgs e)
        {
            if (isCodeLoginTurnOn)
            {
                gNormalLoginForm.Width -= 10;
                if (gNormalLoginForm.Width == 0) LoadCodeLogin.Stop();
            }
            else
            {
                gNormalLoginForm.Width += 10;
                if (gNormalLoginForm.Width == 400) LoadCodeLogin.Stop();
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
                var username = txtUsername.Text;
                var pass = txtPass.Password;
                try
                {
                    btnLogin.IsEnabled = false;
                    PgbLoginProcess.Visibility = Visibility.Visible;
                    await LoginModule.LoginAsync(username, pass);

                    btnLogin.IsEnabled = true;
                    PgbLoginProcess.Visibility = Visibility.Collapsed;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var pass = txtPass.Password.Trim();
            try
            {
                btnLogin.IsEnabled = false;
                PgbLoginProcess.Visibility = Visibility.Visible;
                await LoginModule.LoginAsync(username, pass);

                btnLogin.IsEnabled = true;
                PgbLoginProcess.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void btnLoginCode_Click(object sender, RoutedEventArgs e)
        {
            string code;
            try
            {
                code = KbEmpCodeLoginForm.InputValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Incorrect input!");
                return;
            }

            try
            {
                KbEmpCodeLoginForm.ButtonGoAbleState(false);
                await LoginModule.LoginByCodeAsync(code);
                KbEmpCodeLoginForm.ButtonGoAbleState(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        private void btnDatabase_Click(object sender, RoutedEventArgs e)
        {
            var dbConfig = new DatabaseConfigWindow();
            dbConfig.ShowDialog();
            var connectString = (string) Application.Current.Properties["ConnectionString"];
            if (!string.IsNullOrWhiteSpace(connectString)) _businessModuleLocator.ConnectionString = connectString;
        }

        private void Closing_LoginWindos(object sender, EventArgs args)
        {
            _businessModuleLocator.RepositoryLocator.Dispose();
        }

        private void ButtonChangeLoginType_Click(object sender, RoutedEventArgs e)
        {
            isCodeLoginTurnOn = true;
            LoadCodeLogin.Start();
            //gNormalLoginForm.Visibility = Visibility.Collapsed;
        }

        private void KbEmpCodeLoginForm_OnTurnOffKeyboard(object sender, RoutedEventArgs e)
        {
            isCodeLoginTurnOn = false;
            LoadCodeLogin.Start();
        }
    }
}