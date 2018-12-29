using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Entities;
using Cafocha.Repository.DAL;
using log4net;

namespace Cafocha.GUI.CafowareWorkSpace
{
    /// <summary>
    /// Interaction logic for APWareHouseWindow.xaml
    /// </summary>
    public partial class CafowareWindow : Window
    {
        AdminwsOfCloudAPWH _unitofwork;
        internal WarehouseModule _warehouseModule;
        private CreateStockPage _createStockPage;
        private StockInPage _stockInPage;
        private StockOutPage _stockOutPage;
        private ViewStockPage _viewStockPage;
        private LoginWindow _loginWindow;
        private AdminRe curAdmin;

        private static readonly ILog AppLog = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public CafowareWindow()
        {
            InitializeComponent();

            try
            {
                _unitofwork = new AdminwsOfCloudAPWH();
                _warehouseModule = new WarehouseModule();
                _warehouseModule.loadStock();
                _viewStockPage = new ViewStockPage(_unitofwork, _warehouseModule.StockList);



                if (App.Current.Properties["AdLogin"] != null)
                {
                    AdminRe getAdmin = App.Current.Properties["AdLogin"] as AdminRe;
                    List<AdminRe> adList = _unitofwork.AdminreRepository.Get().ToList();
                    curAdmin = adList.FirstOrDefault(x =>
                        x.Username.Equals(getAdmin.Username) && x.DecryptedPass.Equals(getAdmin.DecryptedPass));
                    CUserChip.Content = curAdmin.Name;
                    _createStockPage = new CreateStockPage(_unitofwork, _warehouseModule.StockList);
                    _stockInPage = new StockInPage(_unitofwork, _warehouseModule.StockList);
                    _stockOutPage = new StockOutPage(_unitofwork, _warehouseModule.StockList);
                }


                DispatcherTimer RefreshTimer = new DispatcherTimer();
                RefreshTimer.Tick += Refresh_Tick;
                RefreshTimer.Interval = new TimeSpan(0, 1, 0);
                RefreshTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: \n" + ex.Message);
                AppLog.Error(ex);
            }
        }

        public void Refresh_Tick(object sender, EventArgs e)
        {
           _warehouseModule.loadStock();

            if (isCreateStockRun)
            {
                _createStockPage.lvStock.Items.Refresh();
            }
            if (isViewStockRun)
            {
                _viewStockPage.lvItem.Items.Refresh();
            }
            if (isStockInRun)
            {
                _stockInPage.lvDataStock.Items.Refresh();
            }
            if (isStockOutRun)
            {
                _stockOutPage.lvDataStock.Items.Refresh();
            }
        }

        bool isCreateStockRun = false;
        private void CreateStock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(_createStockPage);
            isCreateStockRun = true;
        }

        bool isStockInRun = false;
        private void StockIn_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            
            myFrame.Navigate(_stockInPage);
            isStockInRun = true;
        }

        bool isStockOutRun = false;
        private void StockOut_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(_stockOutPage);
            isStockOutRun = true;
        }

        bool isViewStockRun = false;
        private void ViewStock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            myFrame.Navigate(_viewStockPage);
            isViewStockRun = true;
        }

        private void bntLogout_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["AdLogin"] = null;
            App.Current.Properties["EmpLogin"] = null;
            _loginWindow = new LoginWindow();
            this.Close();
            _loginWindow.Show();
        }

        private void EodReport_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
