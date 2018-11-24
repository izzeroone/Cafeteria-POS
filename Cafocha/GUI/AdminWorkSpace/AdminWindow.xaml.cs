using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Cafocha.Entities;
using Cafocha.GUI.Helper.PrintHelper;
using Cafocha.Repository.DAL;
using log4net;
using POS;
using POS.AdminWorkSpace;


namespace Cafocha.GUI.AdminWorkSpace
{ 
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        private AdminwsOfCloudPOS _unitofwork;
        EmployeeListPage empListPage;
        OrderNotePage ordernotepage;
        SalaryPage salarypage;
        ProductDetailPage productdetals;
        internal LoginWindow LoginWindow;
        AdminRe curAdmin;
        CustomerPage ctmP;
        ReceiptNotePage receiptnotepage;
        private statisticsFoodPage FoodPage;
        private StatisticsWorkingHourPage statisticsWorkingHourPage;
        private LiveChartReceiptPage liveChartReceipt;
        private HomePage homePage;
        private ProductCreatorPage productCreator;

        private static readonly ILog AppLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public AdminWindow()
        {
            InitializeComponent();
            _unitofwork = new AdminwsOfCloudPOS();

            try
            {
                var getLoginAdmin = App.Current.Properties["AdLogin"] as AdminRe;
                curAdmin = _unitofwork.AdminreRepository.Get(x => x.AdId.Equals(getLoginAdmin.AdId)).FirstOrDefault();
                if (curAdmin == null)
                {
                    this.Close();
                }
                cUser.Content = curAdmin.Name;

                if (curAdmin.AdRole == (int)AdminReRole.SoftwareAd)
                {
                    btnCreateAdmin.Visibility = Visibility.Visible;
                }

                empListPage = new EmployeeListPage(_unitofwork, curAdmin);
                salarypage = new SalaryPage(_unitofwork, curAdmin);
                liveChartReceipt = new LiveChartReceiptPage(_unitofwork);
                productdetals = new ProductDetailPage(_unitofwork);
                ctmP = new CustomerPage(_unitofwork);
                ordernotepage = new OrderNotePage(_unitofwork, curAdmin);
                receiptnotepage = new ReceiptNotePage(_unitofwork, curAdmin);
                FoodPage = new statisticsFoodPage(_unitofwork);
                statisticsWorkingHourPage = new StatisticsWorkingHourPage(_unitofwork);
                homePage = new HomePage(_unitofwork);
                productCreator = new ProductCreatorPage(_unitofwork);
                myframe.Navigate(homePage);

                DispatcherTimer RefreshTimer = new DispatcherTimer();
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


        private void AdminWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _unitofwork.Dispose();
        }

        private void bntLogout_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["AdLogin"] = null;
            App.Current.Properties["CurrentEmpWorking"] = null;

            LoginWindow = new LoginWindow();
            this.Close();
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
            Cafocha.GUI.AdminWorkSpace.AdminDetailWindow adw = new Cafocha.GUI.AdminWorkSpace.AdminDetailWindow(_unitofwork, curAdmin);
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

        private void bntReceipt_Click(object sender, RoutedEventArgs e)
        {
            myframe.Navigate(receiptnotepage);
        }

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
            var printer = new DoPrintHelper(new EmployeewsOfLocalPOS(), DoPrintHelper.Eod_Printing);
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
            Cafocha.GUI.AdminWorkSpace.AddNewAdminDialog newAdminDialog = new Cafocha.GUI.AdminWorkSpace.AddNewAdminDialog(_unitofwork);
            newAdminDialog.Show();
        }
    }
}
