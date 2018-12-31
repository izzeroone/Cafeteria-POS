using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Documents;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.Entities;
using Cafocha.GUI.Helper.PrintHelper;
using Cafocha.Repository.DAL;
using log4net;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// the object that store all repository you want to get data DBAsowell
        /// in Employee WorkSpace
        /// </summary>
        internal BusinessModuleLocator _businessModuleLocator;

        Employee emp;
        SalaryNote empSln;
        internal Entry en;
        internal Info info;
        internal LoginWindow LoginWindow;
        internal SettingFoodPage st;
        internal ChangeThemePage chtm;
        internal AllEmployeeLogin ael;
        internal OrderTemp orderTemp;
        internal List<OrderDetailsTemp> orderDetailsTemp;
        internal Boolean isOrderPrint;
        internal Boolean isOrderOrder;

        internal static readonly ILog AppLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        DispatcherTimer WorkTimer;
        DispatcherTimer CheckWorkingTimer;

        public MainWindow()
        {
            InitializeComponent();
            emp = App.Current.Properties["EmpLogin"] as Employee;
            _businessModuleLocator = new BusinessModuleLocator();
            cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";

            orderTemp = new OrderTemp();
            orderDetailsTemp = new List<OrderDetailsTemp>();
            isOrderOrder = false;
            isOrderPrint = false;

            //string[] config = ReadWriteData.ReadDBConfig();
            //if (config != null)
            //{
            //    _unitofwork = new RepositoryLocator(config[0], config[1], config[2], config[3]);
            //}
            //else
            //{
            //    _unitofwork = new RepositoryLocator();
            //}

            {
                en = new Entry();
                info = new Info();
                st = new SettingFoodPage(_businessModuleLocator);

                WorkTimer = new DispatcherTimer();
                WorkTimer.Tick += WorkTime_Tick;
                WorkTimer.Interval = new TimeSpan(0, 0, 1);
                WorkTimer.Start();

                DispatcherTimer RefreshTimer = new DispatcherTimer();
                RefreshTimer.Tick += Refresh_Tick;
                RefreshTimer.Interval = new TimeSpan(0, 2, 0);
                RefreshTimer.Start();

                CheckWorkingTimer = new DispatcherTimer();
                CheckWorkingTimer.Tick += CheckWorking_Tick;
                CheckWorkingTimer.Interval = new TimeSpan(0, 5, 0);

                this.Loaded += (sender, args) =>
                {
                    bntEntry.IsEnabled = true;
                    myFrame.Navigate(en);
                };

                this.Closing += (sender, args) =>
                {
                    WorkTimer.Stop();
                    _businessModuleLocator.RepositoryLocator.Dispose();
                };

            }
        }

        private void Refresh_Tick(object sender, EventArgs e)
        {
            en.ucMenu.IsRefreshMenu = true;
            en.ucMenu.UcMenu_Loaded(en.ucMenu, null);
        }

        private void WorkTime_Tick(object sender, EventArgs e)
        {
            DateTime nowWH = DateTime.Now;
            DateTime startWH = (App.Current.Properties["EmpWH"] as WorkingHistory).StartTime;
            var timer = nowWH - startWH;
            string fH = "", fm = "", fs = "";
            fH = timer.Hours.ToString();
            fm = timer.Minutes.ToString();
            fs = timer.Seconds.ToString();

            if (timer.Hours < 10)
            {
                fH = "0" + timer.Hours;
            }
            if (timer.Minutes < 10)
            {
                fm = "0" + timer.Minutes;
            }
            if (timer.Seconds < 10)
            {
                fs = "0" + timer.Seconds;
            }

            txtTimeWk.Text = fH + ":" + fm + ":" + fs;
        }

        private void CheckWorking_Tick(object sender, EventArgs e)
        {
            //check admin
            if (App.Current.Properties["AdLogin"] != null)
            {
                App.Current.Properties["AdLogin"] = null;

                if (App.Current.Properties["CurrentEmpWorking"] != null)
                {
                    App.Current.Properties["CurrentEmpWorking"] = null;
                }
                else if (App.Current.Properties["CurrentEmpWorking"] == null)
                {
                    cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";
                }

                CheckWorkingTimer.Stop();
                return;
            }

            //check employee
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";
            }
            else if (App.Current.Properties["CurrentEmpWorking"] != null)
            {
                App.Current.Properties["CurrentEmpWorking"] = null;
                cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";
            }

            if (bntEntry.IsEnabled == false)
            {
                bntEntry.IsEnabled = true;
            }

            CheckWorkingTimer.Stop();
        }

        private void bntEntry_Click(object sender, RoutedEventArgs e)
        {
            myFrame.Navigate(en);
            bntEntry.IsEnabled = false;
        }


        private void DemoItemsListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var dependencyObject = Mouse.Captured as DependencyObject;
            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            MenuToggleButton.IsChecked = false;
        }

        private void lbiFoodList_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(st);
            bntEntry.IsEnabled = true;
        }

        private void btnStartWorking_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["AdLogin"] != null)
            {
                return;
            }

            if (App.Current.Properties["CurrentEmpWorking"] != null)
            {
                MessageBox.Show("It's have some employee on working! Please wait!");
                return;
            }

            AllEmployeeLogin ael = new AllEmployeeLogin((MainWindow)Window.GetWindow(this), _businessModuleLocator, cUser, 4);
            ael.ShowDialog();

            if (CheckWorkingTimer != null)
            {
                CheckWorkingTimer.Start();
            }
        }

        private void btnEndWorking_Click(object sender, RoutedEventArgs e)
        {
            //check admin
            if (App.Current.Properties["AdLogin"] != null)
            {
                App.Current.Properties["AdLogin"] = null;

                if (App.Current.Properties["CurrentEmpWorking"] != null)
                {
                    cUser.Content = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList).Emp.Username;
                }
                else if (App.Current.Properties["CurrentEmpWorking"] == null)
                {
                    cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";
                }

                return;
            }

            //check employee
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";
            }
            else if (App.Current.Properties["CurrentEmpWorking"] != null)
            {
                App.Current.Properties["CurrentEmpWorking"] = null;
                cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";
            }

            if (CheckWorkingTimer != null)
            {
                CheckWorkingTimer.Stop();
            }
        }

        private void btnOtherEmp_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["AdLogin"] != null)
            {
                return;
            }

            AllEmployeeLogin ael = new AllEmployeeLogin((MainWindow)Window.GetWindow(this), _businessModuleLocator,  cUser, 1);
            ael.ShowDialog();
        }

        private void btnEmpDetail_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["AdLogin"] != null)
            {
                return;
            }

            AllEmployeeLogin ael = new AllEmployeeLogin((MainWindow)Window.GetWindow(this), _businessModuleLocator,  cUser, 2);
            ael.ShowDialog();
        }

        private void bntLogout_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["AdLogin"] != null)
            {
                return;
            }

            AllEmployeeLogin ael = new AllEmployeeLogin((MainWindow)Window.GetWindow(this), _businessModuleLocator, cUser, 3);
            ael.ShowDialog();

        }

        private void lbiChangeTheme_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            chtm = new ChangeThemePage();
            chtm.Show();
        }

        private void LbiEODReport_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var printer = new DoPrintHelper(_businessModuleLocator.RepositoryLocator, DoPrintHelper.Eod_Printing);
            printer.DoPrint();
        }
    }
}
