﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.User;
using Cafocha.Entities;
using Cafocha.GUI.AdminWorkSpace;
using Cafocha.GUI.CafowareWorkSpace;
using Cafocha.GUI.EmployeeWorkSpace;
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
//            var config = ReadWriteData.ReadDBConfig();

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
        }

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
                btnLogin_Click(sender, e);
            }
        }


        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text;
            var pass = txtPass.Password;

            try
            {
                btnLogin.IsEnabled = false;
                PgbLoginProcess.Visibility = Visibility.Visible;
                await Task.Run(async () => { await _businessModuleLocator.EmployeeModule.login(username, pass, null); });
                if (_businessModuleLocator.EmployeeModule.Emploglist.Count != 0)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (_businessModuleLocator.EmployeeModule.Emploglist[0].Emp.EmpRole == (int)EmployeeRole.Stock)
                        {
                            _businessModuleLocator.EmployeeModule.startWorkingRecord(_businessModuleLocator.EmployeeModule.Emploglist[0]);
                            var main = new CafowareWindow();
                            main.Show();
                        }
                        else
                        {
                            var main = new MainWindow();
                            main.Show();
                        }

                        this.Close();
                        return;

                    });

                }
                else
                {
                    if (await _businessModuleLocator.AdminModule.login(username, pass))
                    {
                        this.Dispatcher.Invoke(() =>
                        {
                            var adminWindow = new AdminWindow();
                            adminWindow.Show();
                            this.Close();
                        });

                    }
                    else
                    {
                        MessageBox.Show("Tên hoặc mật khẩu không chính xác");

                    }
                }
                btnLogin.IsEnabled = true;
                PgbLoginProcess.Visibility = Visibility.Collapsed;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
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
                MessageBox.Show("Không hợp lệ!");
                return;
            }

            try
            {
                KbEmpCodeLoginForm.ButtonGoAbleState(false);
                await Task.Run(async () => { await _businessModuleLocator.EmployeeModule.login(null, null, code); });

                if (_businessModuleLocator.EmployeeModule.Emploglist.Count != 0)
                {
                    this.Dispatcher.Invoke(() =>
                    {
                        if (EmployeeModule.WorkingEmployee.Emp.EmpRole == (int)EmployeeRole.Stock)
                        {
                            var main = new CafowareWindow();
                            main.Show();
                        }
                        else
                        {
                            var main = new MainWindow();
                            main.Show();
                        }

                        this.Close();
                        return;

                    });

                }
                KbEmpCodeLoginForm.ButtonGoAbleState(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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