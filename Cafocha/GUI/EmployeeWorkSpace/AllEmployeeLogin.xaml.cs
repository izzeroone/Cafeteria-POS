using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Cafocha.BusinessContext.User;
using Cafocha.Entities;
using Cafocha.Repository.DAL;


namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    /// Interaction logic for AllEmployeeLogin.xaml
    /// </summary>
    public partial class AllEmployeeLogin : Window
    {
        internal EmployeeModule _employeeModule;
        
        private EmpLoginList _emplog;
        MaterialDesignThemes.Wpf.Chip _cUser;
        private DispatcherTimer LoadForm;
        private bool IsShow = false;
        private int _typeshow = 0; //1: login, 2: details, 3: logout, 4: start working
        private Window _main;

        public AllEmployeeLogin(Window main, EmployeeModule employeeModule, MaterialDesignThemes.Wpf.Chip cUser, int typeshow)
        {
            _employeeModule = employeeModule;
            _main = main;

            _cUser = cUser;
            _typeshow = typeshow;
            InitializeComponent();

            initData();

            this.Loaded += AllEmployeeLogin_Loaded;

            LoadForm = new DispatcherTimer();
            LoadForm.Tick += LoadForm_Tick;
            LoadForm.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

        private void AllEmployeeLogin_Loaded(object sender, RoutedEventArgs e)
        {
            this.Width = 500;
            spLoginAnother.Visibility = Visibility.Visible;

            this.WindowStyle = WindowStyle.SingleBorderWindow;
            this.ResizeMode = ResizeMode.NoResize;
        }

        private void LoadForm_Tick(object sender, EventArgs e)
        {
            if(IsShow)
            {
                this.Width -= 10;
                if(this.Width == 500)
                {
                    LoadForm.Stop();
                }
            }
            else
            {
                this.Width += 10;
                if (this.Width == 900)
                {
                    LoadForm.Stop();
                }
            }
        }

        private void initData()
        {
            //main control
            btnLoginNew.Visibility = Visibility.Collapsed;
            BtnCodeLogin.Visibility = Visibility.Collapsed;
            btnLogout.Visibility = Visibility.Collapsed;
            btnView.Visibility = Visibility.Collapsed;
            btnStart.Visibility = Visibility.Collapsed;
            //left control
            btnAcceptLogin.Visibility = Visibility.Collapsed;
            btnAcceptLogout.Visibility = Visibility.Collapsed;
            btnAcceptView.Visibility = Visibility.Collapsed;
            btnAcceptStart.Visibility = Visibility.Collapsed;
            btnAcceptCancel.Visibility = Visibility.Collapsed;

            foreach (var e in _employeeModule.Emploglist)
            {
                e.EmpWH.EndTime = DateTime.Now;
                int h = (e.EmpWH.EndTime - e.EmpWH.StartTime).Hours;
                int m = (e.EmpWH.EndTime - e.EmpWH.StartTime).Minutes;
                int s = (e.EmpWH.EndTime - e.EmpWH.StartTime).Seconds;

                e.TimePercent = (int)((((double)h) + (double)m / 60.0 + (double)s / 3600.0) / 24.0 * 100);
            }

            lvLoginList.ItemsSource = _employeeModule.Emploglist;

            if(_typeshow == 1)//login
            {
                btnLoginNew.Visibility = Visibility.Visible;
                BtnCodeLogin.Visibility = Visibility.Visible;

                btnAcceptLogin.Visibility = Visibility.Visible;
                btnAcceptCancel.Visibility = Visibility.Visible;
            }
            else if(_typeshow == 2)//details
            {
                btnView.Visibility = Visibility.Visible;
                
                btnAcceptView.Visibility = Visibility.Visible;
                btnAcceptCancel.Visibility = Visibility.Visible;
            }
            else if(_typeshow == 3)//logout
            {
                btnLogout.Visibility = Visibility.Visible;

                btnAcceptLogout.Visibility = Visibility.Visible;
                btnAcceptCancel.Visibility = Visibility.Visible;
            }
            else if(_typeshow == 4)//start
            {
                btnStart.Visibility = Visibility.Visible;

                btnAcceptStart.Visibility = Visibility.Visible;
                btnAcceptCancel.Visibility = Visibility.Visible;
            }
        }

        private void BtnCodeLogin_Click(object sender, RoutedEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is ListViewItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
            {
                if (this.Width == 500)
                {
                    IsShow = false;
                    LoadForm.Start();
                }

                spLoginAnother.Visibility = Visibility.Visible;
                loginNormal.Visibility = Visibility.Collapsed;
                loginCode.Visibility = Visibility.Visible;
                lvLoginList.UnselectAll();
                txbLabel.Text = "Login Another";
                setControl(true);
            }
            else
            {
                if(_typeshow == 1)
                {
                    return;
                }

                int index = lvLoginList.ItemContainerGenerator.IndexFromContainer(dep);

                EmpLoginList emp = _employeeModule.Emploglist[index];
                if (emp == null)
                {
                    MessageBox.Show("Please choose employee to continue!");
                    return;
                }

                if (this.Width == 500)
                {
                    IsShow = false;
                    LoadForm.Start();
                }

                _emplog = emp;

                spLoginAnother.Visibility = Visibility.Visible;
                loginNormal.Visibility = Visibility.Collapsed;
                loginCode.Visibility = Visibility.Visible;
                lvLoginList.UnselectAll();
                txbLabel.Text = "Login Another";
                setControl(true);
            }
        }

        private async void loginCode_GoClick(object sender, RoutedEventArgs e)
        {
            string code;
            try
            {
                code = loginCode.InputValue;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Incorrect input!");
                return;
            }

            try
            {
                loginCode.ButtonGoAbleState(false);
                if(_typeshow == 1)//login
                {
                    await Async("", "", code, null);
                    setControl(true);
                }
                else if (_typeshow == 2)//view
                {
                    EmployeeDetail ed = new EmployeeDetail(_emplog.Emp.Username, _employeeModule);
                    ed.ShowDialog();
                    setControl(true);
                }
                else if(_typeshow == 3)//logout
                {
                    await Async("", "", code, _emplog);
                    setControl(true);
                }
                else if(_typeshow == 4)//start
                {
                    if (_emplog.Emp.DecryptedCode.Equals(code))
                    {
                        App.Current.Properties["CurrentEmpWorking"] = _emplog;
                        _cUser.Content = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList).Emp.Username;
                        setControl(true);
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Login fail Employee's code is not correct!");
                    }
                }

                loginCode.ButtonGoAbleState(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void btnLoginNew_Click(object sender, RoutedEventArgs e)
        {
            if(this.Width == 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Visible;
            loginNormal.Visibility = Visibility.Visible;
            loginCode.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Login Another";
            setControl(true);
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            _emplog = lvLoginList.SelectedItem as EmpLoginList;
            if (_emplog == null)
            {
                MessageBox.Show("Please choose one employee want to start working!");
                return;
            }

            if (this.Width == 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Visible;
            loginNormal.Visibility = Visibility.Visible;
            loginCode.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Start Working";
            setControl(false);
            txtUsername.Text = _emplog.Emp.Username;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            _emplog = lvLoginList.SelectedItem as EmpLoginList;
            if (_emplog == null)
            {
                MessageBox.Show("Please choose one employee want to logout!");
                return;
            }

            if (this.Width == 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Visible;
            loginNormal.Visibility = Visibility.Visible;
            loginCode.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            txbLabel.Text = "Logout";
            setControl(false);
            txtUsername.Text = _emplog.Emp.Username;
        }

        private void btnView_Click(object sender, RoutedEventArgs e)
        {
            _emplog = lvLoginList.SelectedItem as EmpLoginList;
            if (_emplog == null)
            {
                MessageBox.Show("Please choose one employee want to view details!");
                return;
            }

            if (this.Width == 500)
            {
                IsShow = false;
                LoadForm.Start();
            }

            spLoginAnother.Visibility = Visibility.Visible;
            loginNormal.Visibility = Visibility.Visible;
            loginCode.Visibility = Visibility.Collapsed;
            lvLoginList.UnselectAll();
            txbLabel.Text = "View Details";
            setControl(false);
            txtUsername.Text = _emplog.Emp.Username;
        }

        private void txtUsername_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                txtPass.Focus();
            }
        }

        private async void txtPass_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if(_typeshow == 1)
                {
                    string username = txtUsername.Text.Trim();
                    string pass = txtPass.Password.Trim();
                    try
                    {
                        btnAcceptLogin.IsEnabled = false;
                        PgbLoginProcess.Visibility = Visibility.Visible;
                        await Async(username, pass, "", null);

                        btnAcceptLogin.IsEnabled = true;
                        PgbLoginProcess.Visibility = Visibility.Collapsed;

                        lvLoginList.ItemsSource = _employeeModule.Emploglist;
                        lvLoginList.Items.Refresh();

                        setControl(true);

                        if (App.Current.Properties["CurrentEmpWorking"] != null)
                        {
                            _cUser.Content = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList).Emp.Username;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else if(_typeshow == 2)
                {
                    if (!_emplog.Emp.DecryptedPass.Equals(txtPass.Password.Trim()))
                    {
                        MessageBox.Show("Login fail! Please try again!");
                        return;
                    }

                    EmployeeDetail ed = new EmployeeDetail(_emplog.Emp.Username, _employeeModule);
                    ed.ShowDialog();

                    setControl(true);
                }
                else if(_typeshow == 3)
                {
                    if (!_emplog.Emp.DecryptedPass.Equals(txtPass.Password.Trim()))
                    {
                        MessageBox.Show("Fail! Please try again!");
                        return;
                    }

                    try
                    {
                        btnAcceptLogout.IsEnabled = false;
                        PgbLoginProcess.Visibility = Visibility.Visible;
                        await Async("", "", "", _emplog);

                        btnAcceptLogout.IsEnabled = true;
                        PgbLoginProcess.Visibility = Visibility.Collapsed;

                        lvLoginList.ItemsSource = _employeeModule.Emploglist;
                        lvLoginList.Items.Refresh();

                        setControl(true);

                        if (App.Current.Properties["CurrentEmpWorking"] != null)
                        {
                            _cUser.Content = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList).Emp.Username;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                else if(_typeshow == 4)
                {
                    if (!_emplog.Emp.DecryptedPass.Equals(txtPass.Password.Trim()))
                    {
                        MessageBox.Show("Login fail! Please try again!");
                        return;
                    }

                    App.Current.Properties["CurrentEmpWorking"] = _emplog;
                    _cUser.Content = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList).Emp.Username;

                    this.Close();
                }
            }
        }

        private async void btnAcceptLogin_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string pass = txtPass.Password.Trim();

            try
            {
                btnAcceptLogin.IsEnabled = false;
                PgbLoginProcess.Visibility = Visibility.Visible;
                await Async(username, pass, "", null);
                
                btnAcceptLogin.IsEnabled = true;
                PgbLoginProcess.Visibility = Visibility.Collapsed;
                
                lvLoginList.ItemsSource = _employeeModule.Emploglist;
                lvLoginList.Items.Refresh();

                setControl(true);

                if(App.Current.Properties["CurrentEmpWorking"] != null)
                {
                    _cUser.Content = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList).Emp.Username;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private async void btnAcceptLogout_Click(object sender, RoutedEventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string pass = txtPass.Password.Trim();

            if (!_emplog.Emp.DecryptedPass.Equals(txtPass.Password.Trim()))
            {
                MessageBox.Show("Fail! Please try again!");
                return;
            }

            try
            {
                btnAcceptLogout.IsEnabled = false;
                PgbLoginProcess.Visibility = Visibility.Visible;
                await Async(username, pass, "", _emplog);

                btnAcceptLogout.IsEnabled = true;
                PgbLoginProcess.Visibility = Visibility.Collapsed;
                
                lvLoginList.ItemsSource = _employeeModule.Emploglist;
                lvLoginList.Items.Refresh();

                setControl(true);

                if (App.Current.Properties["CurrentEmpWorking"] != null)
                {
                    _cUser.Content = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList).Emp.Username;
                }
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
                MessageBox.Show("Login fail! Please try again!");
                return;
            }

            App.Current.Properties["CurrentEmpWorking"] = _emplog;
            _cUser.Content = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList).Emp.Username;

            this.Close();
        }

        private void btnAcceptView_Click(object sender, RoutedEventArgs e)
        {
            if (!_emplog.Emp.DecryptedPass.Equals(txtPass.Password.Trim()))
            {
                MessageBox.Show("Login fail! Please try again!");
                return;
            }

            EmployeeDetail ed = new EmployeeDetail(_emplog.Emp.Username, _employeeModule);
            ed.ShowDialog();

            setControl(true);
        }

        private void btnAcceptCancel_Click(object sender, RoutedEventArgs e)
        {
            if (this.Width == 900)
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

        private async Task Async(string username, string pass, string code, EmpLoginList empout)
        {
            try
            {
                await Task.Run(async () =>
                {
                    if (empout != null)
                    {
                        if (_employeeModule.Emploglist.Count == 1)
                        {
                            var orderedTable = ((MainWindow) Window.GetWindow(this)).orderDetailsTemp;
                            if (orderedTable.Count != 0)
                            {
                                MessageBox.Show("You can not logout because still have Tables that in the ordering state out there. Please check again!");
                                return;
                            }
                        }

                        if ((empout.Emp.Username.Equals(username) && (empout.Emp.DecryptedPass.Equals(pass)) || empout.Emp.DecryptedCode.Equals(code)))
                        {

                            _employeeModule.insertWorkingHistory(empout);


                            Dispatcher.Invoke(() =>
                            {
                                checkEmployeeCount();
                            });

                            return;
                        }
                        else
                        {
                            MessageBox.Show("Fail! Please try again!");
                            return;
                        }
                    }

                    bool isFound = await _employeeModule.login(username, pass, code);

     
                    checkEmployeeCount();

                    if (!isFound)
                    {
                        MessageBox.Show("incorrect username or password");
                        return;
                    }

                    setControl(true);
                });
            }
            catch (Exception ex)
            {

            }
        }

        private void checkEmployeeCount()
        {
            if (_employeeModule.Emploglist.Count == 0)
            {

                var orderTemp = ((MainWindow)Window.GetWindow(this)).orderTemp;
                orderTemp.EmpId = "";
                orderTemp.CusId = "CUS0000001";
                orderTemp.Ordertime = DateTime.Now;
                orderTemp.TotalPriceNonDisc = 0;
                orderTemp.TotalPrice = 0;
                orderTemp.CustomerPay = 0;
                orderTemp.PayBack = 0;
                orderTemp.SubEmpId = "";
                orderTemp.Pax = 0;

                ((MainWindow) Window.GetWindow(this)).isOrderOrder = false;
                ((MainWindow)Window.GetWindow(this)).isOrderPrint = false;

                

                App.Current.Properties["CurrentEmpWorking"] = null;
                _main.Close();
                LoginWindow loginWindow = new LoginWindow();
                this.Close();
                loginWindow.Show();
                return;
            }
            else
            {
                _cUser.Content = _employeeModule.Emploglist.Count + " employee(s) available";
                if(App.Current.Properties["CurrentEmpWorking"] != null)
                {
                    _cUser.Content = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList).Emp.Username;
                }
            }
            
            lvLoginList.ItemsSource = _employeeModule.Emploglist;
            lvLoginList.Items.Refresh();
        }

        private void setControl(bool b)
        {
            if(b)
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
    }

    public class EmpLoginList
    {
        private Employee _emp;
        private SalaryNote _empsal;
        private WorkingHistory _empwh;
        private int _timepercent;

        public Employee Emp
        {
            get { return _emp; }
            set { _emp = value; }
        }

        public SalaryNote EmpSal
        {
            get { return _empsal; }
            set { _empsal = value; }
        }

        public WorkingHistory EmpWH
        {
            get { return _empwh; }
            set { _empwh = value; }
        }

        public int TimePercent
        {
            get { return _timepercent; }
            set { _timepercent = value; }
        }
    }


}
