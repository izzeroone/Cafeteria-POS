using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.User;
using Cafocha.Entities;
using Cafocha.GUI.AdminWorkSpace;
using log4net;

namespace Cafocha.GUI.CafowareWorkSpace
{
    /// <summary>
    ///     Interaction logic for APWareHouseWindow.xaml
    /// </summary>
    public partial class CafowareWindow : Window
    {
        private static readonly ILog AppLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly BusinessModuleLocator _businessModuleLocator;
        private readonly CreateStockPage _createStockPage;
        private LoginWindow _loginWindow;
        private readonly StockInPage _stockInPage;
        private readonly StockOutPage _stockOutPage;
        private readonly ViewStockPage _viewStockPage;
        private readonly StockInInfoPage _stockInInfoPage;
        private readonly AdminRe curAdmin;

        private bool isCreateStockRun;

        private bool isStockInRun;

        private bool isStockOutRun;

        private bool isViewStockRun;

        public CafowareWindow()
        {
            InitializeComponent();

            try
            {
                _businessModuleLocator = new BusinessModuleLocator();
                _businessModuleLocator.WarehouseModule.loadStock();
                _viewStockPage = new ViewStockPage(_businessModuleLocator,
                    _businessModuleLocator.WarehouseModule.StockList);

                CUserChip.Content = EmployeeModule.WorkingEmployee.Emp.Name;
                _createStockPage = new CreateStockPage(_businessModuleLocator,
                    _businessModuleLocator.WarehouseModule.StockList);
                _stockInPage = new StockInPage(_businessModuleLocator,
                    _businessModuleLocator.WarehouseModule.StockList);
                _stockOutPage = new StockOutPage(_businessModuleLocator,
                    _businessModuleLocator.WarehouseModule.StockList);


                var RefreshTimer = new DispatcherTimer();
                RefreshTimer.Tick += Refresh_Tick;
                RefreshTimer.Interval = new TimeSpan(0, 1, 0);
                RefreshTimer.Start();

                ViewStock_PreviewMouseLeftButtonUp(null, null);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: \n" + ex.Message);
                AppLog.Error(ex);
            }
        }

        public void Refresh_Tick(object sender, EventArgs e)
        {
            _businessModuleLocator.WarehouseModule.updateStock();

            if (isCreateStockRun) _createStockPage.lvStock.Items.Refresh();
            if (isViewStockRun) _viewStockPage.Refresh();
            if (isStockInRun) _stockInPage.lvDataStock.Items.Refresh();
            if (isStockOutRun) _stockOutPage.lvDataStock.Items.Refresh();
        }

        private void CreateStock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(_createStockPage);
            isCreateStockRun = true;
        }

        private void StockIn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(_stockInPage);
            isStockInRun = true;
        }

        private void StockOut_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(_stockOutPage);
            isStockOutRun = true;
        }

        private void ViewStock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(_viewStockPage);
            isViewStockRun = true;
        }

        private void StockInOutInfo_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(_stockInInfoPage);
            isViewStockRun = true;
        }

        private void bntLogout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["AdLogin"] = null;
            Application.Current.Properties["EmpLogin"] = null;
            _loginWindow = new LoginWindow();
            Close();
            _loginWindow.Show();
        }

        private void EodReport_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
        }
    }
}