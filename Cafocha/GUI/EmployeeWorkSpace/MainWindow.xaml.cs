﻿using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.Helper.PrintHelper;
using Cafocha.BusinessContext.User;
using Cafocha.Entities;
using log4net;
using Application = System.Windows.Application;
using MessageBox = System.Windows.MessageBox;
using ScrollBar = System.Windows.Controls.Primitives.ScrollBar;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal static readonly ILog AppLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        ///     the object that store all repository you want to get data DBAsowell
        ///     in Employee WorkSpace
        /// </summary>
        internal BusinessModuleLocator _businessModuleLocator;

        private readonly DispatcherTimer CheckWorkingTimer;
        internal ChangeThemePage chtm;

        private Employee emp;
        private SalaryNote empSln;
        internal Entry en;
        internal Info info;
        internal bool isOrderOrder;
        internal bool isOrderPrint;
        internal LoginWindow LoginWindow;
        private readonly DispatcherTimer WorkTimer;

        public MainWindow()
        {
            InitializeComponent();
            _businessModuleLocator = new BusinessModuleLocator();
            emp = EmployeeModule.WorkingEmployee.Emp;

            cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";
            var t = _businessModuleLocator.EmployeeModule.Emploglist;
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

                WorkTimer = new DispatcherTimer();
                WorkTimer.Tick += WorkTime_Tick;
                WorkTimer.Interval = new TimeSpan(0, 0, 1);
                WorkTimer.Stop();


                Loaded += (sender, args) =>
                {
                    bntEntry.IsEnabled = true;
                    myFrame.Navigate(en);
                };

                Closing += (sender, args) =>
                {
                    WorkTimer.Stop();
                    _businessModuleLocator.RepositoryLocator.Dispose();
                };
            }
        }

        private void WorkTime_Tick(object sender, EventArgs e)
        {
            var nowWH = DateTime.Now;
            if (EmployeeModule.WorkingEmployee.EmpWH == null)
            {
                return;
            }
            var startWH = EmployeeModule.WorkingEmployee.EmpWH.StartTime;
            var timer = nowWH - startWH;
            string fH = "", fm = "", fs = "";
            fH = timer.Hours.ToString();
            fm = timer.Minutes.ToString();
            fs = timer.Seconds.ToString();

            if (timer.Hours < 10) fH = "0" + timer.Hours;
            if (timer.Minutes < 10) fm = "0" + timer.Minutes;
            if (timer.Seconds < 10) fs = "0" + timer.Seconds;

            txtTimeWk.Text = fH + ":" + fm + ":" + fs;
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


        private void btnStartWorking_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Properties["AdLogin"] != null) return;

            if (EmployeeModule.WorkingEmployee.EmpWH != null)
            {
                MessageBox.Show("Có ai đó đang sử dụng!");
                return;
            }

            _businessModuleLocator.EmployeeModule.startWorkingRecord();
            cUser.Content = "Đang làm việc";
            if (WorkTimer != null) WorkTimer.Start();
        }

        private void btnEndWorking_Click(object sender, RoutedEventArgs e)
        {
            endWorking();
        }

        private bool endWorking()
        {
            //check admin
            if (Application.Current.Properties["AdLogin"] != null)
            {
                Application.Current.Properties["AdLogin"] = null;

                if (EmployeeModule.WorkingEmployee.EmpWH != null)
                    cUser.Content = EmployeeModule.WorkingEmployee.Emp.Username;
                else if (EmployeeModule.WorkingEmployee.EmpWH == null)
                    cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";

                return false;
            }

            //check employee
            if (EmployeeModule.WorkingEmployee.EmpWH == null)
            {
                MessageBox.Show("Cannot end working when you are not started working!");
                cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";
            }
            else if (EmployeeModule.WorkingEmployee.EmpWH != null)
            {
                if (_businessModuleLocator.TakingOrderModule.OrderTemp.OrderDetailsTemps.Count != 0)
                {
                    MessageBox.Show("Bạn đang có đơn hàng, không thể kết thúc phiên làm việc");
                    return false;
                }

                _businessModuleLocator.EmployeeModule.endWorking();
                cUser.Content = _businessModuleLocator.EmployeeModule.Emploglist.Count() + " employee(s) available";
            }

            if (CheckWorkingTimer != null) CheckWorkingTimer.Stop();
            return true;
        }

        private void btnOtherEmp_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Properties["AdLogin"] != null) return;

//            var ael = new AllEmployeeLogin((MainWindow) GetWindow(this), _businessModuleLocator, cUser, 1);
//            ael.ShowDialog();
        }

        private void btnEmpDetail_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.Properties["AdLogin"] != null) return;

            var ed = new EmployeeDetail(EmployeeModule.WorkingEmployee.Emp.Username, _businessModuleLocator);
            ed.Show();
        }

        private void bntLogout_Click(object sender, RoutedEventArgs e)
        {
            //if (Application.Current.Properties["AdLogin"] != null)
            //    return;


            //if (Application.Current.Properties["CurrentEmpWorking"] != null)
            //    MessageBox.Show("You should end working before log out!");

            //var ael = new AllEmployeeLogin((MainWindow) GetWindow(this), _businessModuleLocator, cUser, 3);
            //ael.ShowDialog();
            Application.Current.Properties["AdLogin"] = null;

            if (EmployeeModule.WorkingEmployee.EmpWH != null)
            {
                var dialogResult = MessageBox.Show("Bạn đang trong phiên làm việc.\n Bạn có muốn kết thúc phiên làm việc và đăng xuất!", "Đăng xuất", MessageBoxButton.YesNo);
                if (dialogResult == MessageBoxResult.Yes)
                {
                    if (endWorking())
                    {

                        EmployeeModule.WorkingEmployee = null;
                        Dispatcher.Invoke(() =>
                        {
                            var main = new LoginWindow();
                            main.Show();
                            Close();
                        });
                    }
                }

            }
            else
            {
                EmployeeModule.WorkingEmployee = null;
                Dispatcher.Invoke(() =>
                {
                    var main = new LoginWindow();
                    main.Show();
                    Close();
                });
            }
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