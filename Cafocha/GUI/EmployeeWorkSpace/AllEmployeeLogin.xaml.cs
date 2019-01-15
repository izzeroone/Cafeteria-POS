using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.User;
using Cafocha.Entities;
using MaterialDesignThemes.Wpf;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for AllEmployeeLogin.xaml
    /// </summary>
    public partial class AllEmployeeLogin : Window
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        private readonly Chip _cUser;
        private readonly string INVAILD_PASS_PHASE = "Mật khẩu không hợp lệ";
        private readonly string NO_EMPLOYEE = "Bạn chưa chọn nhân viên";
        private EmpLoginList _emplog;
        private readonly Window _main;
        private readonly int _typeshow; //1: login, 2: details, 3: logout, 4: start working, 5: endworking
        private bool IsShow;
        private readonly DispatcherTimer LoadForm;

        public AllEmployeeLogin(Window main, BusinessModuleLocator businessModuleLocator, Chip cUser, int typeshow)
        {
            _businessModuleLocator = businessModuleLocator;
            _main = main;

            _cUser = cUser;
            _typeshow = typeshow;
            InitializeComponent();

            initData();

            Loaded += AllEmployeeLogin_Loaded;

            LoadForm = new DispatcherTimer();
            LoadForm.Tick += LoadForm_Tick;
            LoadForm.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

        private void AllEmployeeLogin_Loaded(object sender, RoutedEventArgs e)
        {
            Width = 500;
            spLoginAnother.Visibility = Visibility.Visible;

            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.NoResize;
        }

        private void LoadForm_Tick(object sender, EventArgs e)
        {
            if (IsShow)
            {
                Width -= 100;
                if (Width < 500) LoadForm.Stop();
            }
            else
            {
                Width += 100;
                if (Width > 900) LoadForm.Stop();
            }
        }

        private void initData()
        {
            //main control
            btnLoginNew.Visibility = Visibility.Collapsed;
//            BtnCodeLogin.Visibility = Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Collapsed;
            btnView.Visibility = Visibility.Collapsed;
            btnStart.Visibility = Visibility.Collapsed;
            btnEnd.Visibility = Visibility.Collapsed;
            //left control
            btnAcceptLogin.Visibility = Visibility.Collapsed;
            btnAcceptLogout.Visibility = Visibility.Collapsed;
            btnAcceptView.Visibility = Visibility.Collapsed;
            btnAcceptStart.Visibility = Visibility.Collapsed;
            btnAcceptCancel.Visibility = Visibility.Collapsed;
            btnAcceptEnd.Visibility = Visibility.Collapsed;

            lvLoginList.ItemsSource = _businessModuleLocator.EmployeeModule.Emploglist;

            if (_typeshow == 1) //login
            {
                btnLoginNew.Visibility = Visibility.Visible;
//                BtnCodeLogin.Visibility = Visibility.Visible;

                btnAcceptLogin.Visibility = Visibility.Visible;
                btnAcceptCancel.Visibility = Visibility.Visible;
            }
            else if (_typeshow == 2) //details
            {
                btnView.Visibility = Visibility.Visible;

                btnAcceptView.Visibility = Visibility.Visible;
                btnAcceptCancel.Visibility = Visibility.Visible;
            }
            else if (_typeshow == 3) //logout
            {
                btnLogout.Visibility = Visibility.Visible;

                btnAcceptLogout.Visibility = Visibility.Visible;
                btnAcceptCancel.Visibility = Visibility.Visible;
            }
            else if (_typeshow == 4) //start
            {
                btnStart.Visibility = Visibility.Visible;

                btnAcceptStart.Visibility = Visibility.Visible;
                btnAcceptCancel.Visibility = Visibility.Visible;
            }
            else if (_typeshow == 5) //end
            {
                btnEnd.Visibility = Visibility.Visible;

                btnAcceptEnd.Visibility = Visibility.Visible;
                btnAcceptCancel.Visibility = Visibility.Visible;
            }
        }


        private void btnLoginNew_Click(object sender, RoutedEventArgs e)
        {
            if (Width <= 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Visible;
            loginNormal.Visibility = Visibility.Visible;
            loginCode.Visibility = Visibility.Collapsed;
            btnLoginNew.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Đăng nhập mới";
            setControl(true);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            var t = lvLoginList.SelectedItem as EmpLoginList;
            if (t == null)
            {
                MessageBox.Show(NO_EMPLOYEE);
                return;
            }

            if (t.IsStartWorking)
            {
                MessageBox.Show("Bạn đang trong phiên làm việc. Không thể bắt đầu làm việc");
                return;
            }

            _emplog = t;


            if (Width <= 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Visible;
            loginNormal.Visibility = Visibility.Visible;
            loginCode.Visibility = Visibility.Collapsed;
            btnStart.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Bắt đầu làm việc";
            setControl(false);
            txtUsername.Text = _emplog.Emp.Username;
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            var t = lvLoginList.SelectedItem as EmpLoginList;
            if (t == null)
            {
                MessageBox.Show(NO_EMPLOYEE);
                return;
            }

            if (t.IsStartWorking == false)
            {
                MessageBox.Show("Bạn chưa bắt đầu làm việc!");
                return;
            }

            _emplog = t;
            if (Width <= 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Visible;
            loginNormal.Visibility = Visibility.Visible;
            loginCode.Visibility = Visibility.Collapsed;
            btnEnd.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Kết thúc làm việc";
            setControl(false);
            txtUsername.Text = _emplog.Emp.Username;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            var t = lvLoginList.SelectedItem as EmpLoginList;
            _emplog = t;
            if (_emplog == null)
            {
                MessageBox.Show(NO_EMPLOYEE);
                return;
            }
            if (Width <= 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Visible;
            loginNormal.Visibility = Visibility.Visible;
            loginCode.Visibility = Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Đăng xuất";
            setControl(false);
            txtUsername.Text = _emplog.Emp.Username;
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            var t = lvLoginList.SelectedItem as EmpLoginList;
            _emplog = t;
            if (_emplog == null)
            {
                MessageBox.Show(NO_EMPLOYEE);
                return;
            }

            if (Width <= 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Visible;
            loginNormal.Visibility = Visibility.Visible;
            loginCode.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Xem chi tiết";
            setControl(false);
            txtUsername.Text = _emplog.Emp.Username;
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter) txtPass.Focus();
        }

        private async void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (_typeshow == 1)
                {
                    btnAcceptLogin_Click(sender, null);
                }
                else if (_typeshow == 2)
                {
                    btnAcceptView_Click(sender, null);
                }
                else if (_typeshow == 3)
                {
                    btnAcceptLogout_Click(sender, null);
                }
                else if (_typeshow == 4)
                {
                    btnAcceptStart_Click(sender, null);
                }
                else if (_typeshow == 5)
                {
                    btnAcceptEnd_Click(sender, null);
                }
            }
        }

        private async void btnAcceptLogin_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var pass = txtPass.Password.Trim();

            try
            {
                btnAcceptLogin.IsEnabled = false;
                PgbLoginProcess.Visibility = Visibility.Visible;
                var t = await Task.Run(async() => await _businessModuleLocator.EmployeeModule.login(username, pass, ""));
                if (t == false)
                {
//                    MessageBox.Show(INVAILD_PASS_PHASE);
                }
                btnAcceptLogin.IsEnabled = true;
                PgbLoginProcess.Visibility = Visibility.Collapsed;

                lvLoginList.ItemsSource = _businessModuleLocator.EmployeeModule.Emploglist;
                lvLoginList.Items.Refresh();

                setControl(true);

                updateStatus();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void btnAcceptLogout_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var pass = txtPass.Password.Trim();

            if (!_emplog.Emp.DecryptedPass.Equals(txtPass.Password.Trim()))
            {
                MessageBox.Show(INVAILD_PASS_PHASE);
                return;
            }

            if (_emplog.IsStartWorking == true)
            {

                var dialogResult = MessageBox.Show("Bạn đang trong phiên làm việc.\n Bạn có muốn kết thúc phiên làm việc và đăng xuất!", "Đăng xuất", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    btnAcceptEnd_Click(sender, null);
                }
                else
                {
                    return;
                }
            }

            try
            {
                     
                btnAcceptLogout.IsEnabled = false;
                PgbLoginProcess.Visibility = Visibility.Visible;
                await _businessModuleLocator.EmployeeModule.logout(username, pass, "");

                btnAcceptLogout.IsEnabled = true;
                PgbLoginProcess.Visibility = Visibility.Collapsed;

                lvLoginList.ItemsSource = _businessModuleLocator.EmployeeModule.Emploglist;
                lvLoginList.Items.Refresh();

                if (_businessModuleLocator.EmployeeModule.Emploglist.Count == 0)
                {
                    Dispatcher.Invoke(() =>
                    {
                        var t = new LoginWindow();
                        t.Show();
                        this.Close();
                        _main.Close();
                    });
                }
                updateStatus();
                setControl(true);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnAcceptStart_Click(object sender, RoutedEventArgs e)
        {
            if (!_emplog.Emp.DecryptedPass.Equals(txtPass.Password.Trim()))
            {
                MessageBox.Show(INVAILD_PASS_PHASE);
                return;
            }

            _businessModuleLocator.EmployeeModule.startWorkingRecord(_emplog);
            updateStatus();

            Close();
        }

        private void btnAcceptEnd_Click(object sender, RoutedEventArgs e)
        {
            if (!_emplog.Emp.DecryptedPass.Equals(txtPass.Password.Trim()))
            {
                MessageBox.Show(INVAILD_PASS_PHASE);
                return;
            }


            _businessModuleLocator.EmployeeModule.endWorkingRecord(_emplog);
            updateStatus();

            Close();
        }

        private void btnAcceptView_Click(object sender, RoutedEventArgs e)
        {
            if (!_emplog.Emp.DecryptedPass.Equals(txtPass.Password.Trim()))
            {
                MessageBox.Show(INVAILD_PASS_PHASE);
                return;
            }

            var ed = new EmployeeDetail(_emplog.Emp.Username, _businessModuleLocator);
            ed.ShowDialog();
            updateStatus();
            setControl(true);
        }

        private void btnAcceptCancel_Click(object sender, RoutedEventArgs e)
        {
            if (Width >= 900)
            {
                IsShow = true;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Collapsed;
            loginNormal.Visibility = Visibility.Collapsed;
            loginCode.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            setControl(true);
        }


        private void setControl(bool b)
        {
            if (b)
            {
                txtUsername.IsEnabled = b;
                txtPass.IsEnabled = true;
                txtUsername.Text = "";
                txtPass.Password = "";
            }
            else
            {
                txtUsername.IsEnabled = b;
                txtPass.IsEnabled = true;
                txtUsername.Text = "";
                txtPass.Password = "";
            }
        }

        private void LoginCode_OnTurnOffKeyboard(object sender, RoutedEventArgs e)
        {
            //do nothing
        }

        private void loginCode_GoClick(object sender, RoutedEventArgs e)
        {
           
        }

        private void updateStatus()
        {

            if (EmployeeModule.WorkingEmployee != null)
            {
                _cUser.Content = "Đang làm việc";
            }
            else
            {
                _cUser.Content = String.Format("Hiện có {0} nhân viên",
                    _businessModuleLocator.EmployeeModule.Emploglist.Count);
                    ;
            }

        }

        private void LvLoginList_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var t = lvLoginList.SelectedItem as EmpLoginList;
            if (t != null && t.Emp != null && !string.IsNullOrEmpty(t.Emp.Username.Trim()) )
            {
                txtUsername.Text = t.Emp.Username.Trim();
            }
            
        }
    }

}