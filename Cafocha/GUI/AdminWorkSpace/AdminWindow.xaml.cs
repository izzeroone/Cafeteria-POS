using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Cafocha.BusinessContext;
using Cafocha.Entities;
using Cafocha.GUI.Helper.PrintHelper;
using Cafocha.Repository.DAL;
using log4net;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private static readonly ILog AppLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BusinessModuleLocator _businessModuleLocator;
        private readonly CustomerPage ctmP;
        private readonly AdminRe curAdmin;
        private readonly EmployeeListPage empListPage;
        private readonly statisticsFoodPage FoodPage;
        private readonly HomePage homePage;
        private LiveChartReceiptPage liveChartReceipt;
        internal LoginWindow LoginWindow;
        private readonly OrderNotePage ordernotepage;
        private readonly ProductCreatorPage productCreator;
        private readonly ProductDetailPage productdetals;
        private readonly SalaryPage salarypage;
        private readonly StatisticsWorkingHourPage statisticsWorkingHourPage;

        public AdminWindow()
        {
            _businessModuleLocator = new BusinessModuleLocator();
            InitializeComponent();

            try
            {
                var getLoginAdmin = Application.Current.Properties["AdLogin"] as AdminRe;
                curAdmin = _businessModuleLocator.AdminModule.getAdmin(getLoginAdmin.AdId);
                if (curAdmin == null) Close();
                cUser.Content = curAdmin.Name;

                if (curAdmin.AdRole == (int) AdminReRole.SoftwareAd) btnCreateAdmin.Visibility = Visibility.Visible;

                empListPage = new EmployeeListPage(_businessModuleLocator, curAdmin);
                salarypage = new SalaryPage(_businessModuleLocator, curAdmin);
//                liveChartReceipt = new LiveChartReceiptPage(_businessModuleLocator);
                productdetals = new ProductDetailPage(_businessModuleLocator);
                ctmP = new CustomerPage(_businessModuleLocator);
                ordernotepage = new OrderNotePage(_businessModuleLocator, curAdmin);
//                receiptnotepage = new ReceiptNotePage(_businessModuleLocator, curAdmin);
                FoodPage = new statisticsFoodPage(_businessModuleLocator);
                statisticsWorkingHourPage = new StatisticsWorkingHourPage(_businessModuleLocator);
                homePage = new HomePage(_businessModuleLocator);
                productCreator = new ProductCreatorPage(_businessModuleLocator);
                myframe.Navigate(homePage);

                var RefreshTimer = new DispatcherTimer();
                RefreshTimer.Tick += Refresh_Tick;
                RefreshTimer.Interval = new TimeSpan(0, 2, 0);
                RefreshTimer.Start();

                Closing += AdminWindow_Closing;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: \n" + ex.Message);
                AppLog.Error(ex);
            }
        }

        private void Refresh_Tick(object sender, EventArgs e)
        {
            homePage.RefreshHome();
        }


        private void AdminWindow_Closing(object sender, CancelEventArgs e)
        {
//            _unitofwork.Dispose();
        }

        private void bntLogout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["AdLogin"] = null;
            Application.Current.Properties["CurrentEmpWorking"] = null;

            LoginWindow = new LoginWindow();
            Close();
            LoginWindow.Show();
        }

        private void EmployeeInfo_onClick(object sender, RoutedEventArgs e)
        {
            myframe.Navigate(empListPage);
        }

        private void PopupBox_OnClosed(object sender, RoutedEventArgs e)
        {
        }

        private void SalaryInfo_onClick(object sender, RoutedEventArgs e)
        {
            myframe.Navigate(salarypage);
        }

        private void ProductInfo_onclick(object sender, RoutedEventArgs e)
        {
            myframe.Navigate(productdetals);
        }

        private void BtnDetails_OnClick(object sender, RoutedEventArgs e)
        {
            var adw = new AdminDetailWindow(_businessModuleLocator, curAdmin);
            adw.Show();
        }

        private void bntCustomer_Click(object sender, RoutedEventArgs e)
        {
            myframe.Navigate(ctmP);
        }

        private void bntOrder_Click(object sender, RoutedEventArgs e)
        {
            myframe.Navigate(ordernotepage);
        }

//        private void bntReceipt_Click(object sender, RoutedEventArgs e)
//        {
//            myframe.Navigate(receiptnotepage);
//        }

        private void View_Statistics_Quantity_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myframe.Navigate(FoodPage);
        }

        private void ViewstaticWH_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myframe.Navigate(statisticsWorkingHourPage);
        }

        private void HomePage_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myframe.Navigate(homePage);
        }

        private void EODReport_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //ToDo: May be shoud close the repository after print
            var printer = new DoPrintHelper(new RepositoryLocator(), DoPrintHelper.Eod_Printing);
            printer.DoPrint();
        }

        private void BntCreateNewProduct_OnClick(object sender, RoutedEventArgs e)
        {
            myframe.Navigate(productCreator);
        }

        private void ViewstaticReAndEx_OnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myframe.Navigate(liveChartReceipt);
        }

        private void BtnCreateAdmin_OnClick(object sender, RoutedEventArgs e)
        {
            var newAdminDialog = new AddNewAdminDialog(_businessModuleLocator);
            newAdminDialog.Show();
        }
    }
}