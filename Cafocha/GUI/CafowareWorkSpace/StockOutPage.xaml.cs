using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.CafowareWorkSpace
{
    /// <summary>
    ///     Interaction logic for StockOutPage.xaml
    /// </summary>
    public partial class StockOutPage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        internal StockOut _currentStockOut;
        private readonly List<Stock> _stockList;
        internal List<StockOutDetail> _stockOutDetailsList;


        /*********************************
         * Form Manipulate
         *********************************/


        private readonly List<int> ErrorDetailsItem = new List<int>();


        public StockOutPage(BusinessModuleLocator businessModuleLocator, List<Stock> stockList)
        {
            _businessModuleLocator = businessModuleLocator;
            _stockList = stockList;

            InitializeComponent();

            lvDataStock.ItemsSource = _stockList;

            _stockOutDetailsList = new List<StockOutDetail>();
            _currentStockOut = new StockOut
            {
                AdId = (Application.Current.Properties["AdLogin"] as AdminRe).AdId,
                StockOutDetails = _stockOutDetailsList
            };

            lvDataStockOut.ItemsSource = _stockOutDetailsList;

            LoadStockOutData();
        }

        private void LoadStockOutData()
        {
            _currentStockOut.TotalAmount = 0;
            foreach (var details in _stockOutDetailsList)
                _currentStockOut.TotalAmount += details.ItemPrice * details.Quan;
            txtTotalPrice.Text = string.Format("{0:0.000}", _currentStockOut.TotalAmount);
        }


        /*********************************
         * Manipulate Each Stock
         *********************************/

        private void lvDataStock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var stock = (Stock) lvDataStock.SelectedItem;
            if (stock == null)
                return;

            if (checkWareHouse(stock))
            {
                var r = new StockOutDetail();

                var foundIteminReceipt = _stockOutDetailsList.FirstOrDefault(c => c.StockId.Equals(stock.StoId));
                if (foundIteminReceipt == null)
                {
                    r.StockId = stock.StoId;
                    r.Quan = 1;
                    r.ItemPrice = stock.StandardPrice;
                    _stockOutDetailsList.Add(r);
                }
                else
                {
                    foundIteminReceipt.Quan++;
                }

                lvDataStockOut.Items.Refresh();
                LoadStockOutData();
            }
        }

        private bool checkWareHouse(Stock stock)
        {
            var details = _currentStockOut.StockOutDetails.FirstOrDefault(x => x.StockId.Equals(stock.StoId));
            var wareHouse = _businessModuleLocator.WarehouseModule.getApWareHouse(stock.ApwarehouseId);
            if (details != null)
            {
                if (wareHouse != null)
                {
                    if (wareHouse.Contain < details.Quan + 1)
                    {
                        MessageBox.Show("Doesn't have enough this kind of Stock in Warehouse to take out!");
                        return false;
                    }
                }
                else
                {
                    MessageBox.Show("Warehouse doen't contain this Stock. Please check again!");
                    return false;
                }
            }
            else
            {
                if (wareHouse.Contain == 0)
                {
                    MessageBox.Show("Doesn't have enough this kind of Stock in Warehouse to take out!");
                    return false;
                }
            }

            return true;
        }


        /*********************************
         * Manipulate Each StockInDetails
         *********************************/

        private void txtItemPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textboxItemPrice = sender as TextBox;


            int index;
            var r = new StockOutDetail();
            var dep = (DependencyObject) e.OriginalSource;
            while (dep != null && !(dep is ListViewItem)) dep = VisualTreeHelper.GetParent(dep);
            if (dep == null)
                return;
            index = lvDataStockOut.ItemContainerGenerator.IndexFromContainer(dep);


            try
            {
                if (textboxItemPrice.Text == null || textboxItemPrice.Text.Length == 0)
                {
                    MessageBox.Show("The Price of Output Stock can not be blank!");
                    if (!ErrorDetailsItem.Contains(index))
                        ErrorDetailsItem.Add(index);
                    return;
                }

                _stockOutDetailsList[index].ItemPrice = decimal.Parse(textboxItemPrice.Text);

                LoadStockOutData();
                if (ErrorDetailsItem.Contains(index))
                    ErrorDetailsItem.Remove(index);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong when try to calculate the input data. Please check your input");
                if (!ErrorDetailsItem.Contains(index))
                    ErrorDetailsItem.Add(index);
            }
        }

        private void txtQuan_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textboxQuan = sender as TextBox;


            int index;
            var r = new StockOutDetail();
            var dep = (DependencyObject) e.OriginalSource;
            while (dep != null && !(dep is ListViewItem)) dep = VisualTreeHelper.GetParent(dep);
            if (dep == null)
                return;
            index = lvDataStockOut.ItemContainerGenerator.IndexFromContainer(dep);


            try
            {
                if (textboxQuan.Text == null || textboxQuan.Text.Length == 0)
                {
                    MessageBox.Show("The quantity of Output Stock can not be blank!");
                    if (!ErrorDetailsItem.Contains(index))
                        ErrorDetailsItem.Add(index);
                    return;
                }

                _stockOutDetailsList[index].Quan = int.Parse(textboxQuan.Text);

                LoadStockOutData();
                if (ErrorDetailsItem.Contains(index))
                    ErrorDetailsItem.Remove(index);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong when try to calculate the input data. Please check your input");
                if (!ErrorDetailsItem.Contains(index))
                    ErrorDetailsItem.Add(index);
            }
        }

        private void bntDelete_Click(object sender, RoutedEventArgs e)
        {
            int index;
            var r = new StockOutDetail();
            var dep = (DependencyObject) e.OriginalSource;
            while (dep != null && !(dep is ListViewItem)) dep = VisualTreeHelper.GetParent(dep);
            if (dep == null)
                return;
            index = lvDataStockOut.ItemContainerGenerator.IndexFromContainer(dep);


            if (_stockOutDetailsList[index].Quan > 1 && !ErrorDetailsItem.Contains(index))
            {
                r.Quan = _stockOutDetailsList[index].Quan - 1;
                r.StockId = _stockOutDetailsList[index].StockId;
                r.ItemPrice = _stockOutDetailsList[index].ItemPrice;
                _stockOutDetailsList[index] = r;
            }
            else
            {
                _stockOutDetailsList.RemoveAt(index);
                if (ErrorDetailsItem.Contains(index))
                    ErrorDetailsItem.Remove(index);
            }

            lvDataStockOut.Items.Refresh();
            LoadStockOutData();
        }

        private void bntAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ErrorDetailsItem.Count != 0)
                {
                    MessageBox.Show("Something is not correct. Please check all your input again!");
                    return;
                }

                if (_currentStockOut.StockOutDetails.Count == 0)
                {
                    MessageBox.Show("You have to choose the stock you want to take out");
                    return;
                }

                _businessModuleLocator.WarehouseModule.addStockOut(_currentStockOut);

                _stockOutDetailsList = new List<StockOutDetail>();
                lvDataStockOut.ItemsSource = _stockOutDetailsList;
                lvDataStockOut.Items.Refresh();

                _currentStockOut = new StockOut
                {
                    AdId = (Application.Current.Properties["AdLogin"] as AdminRe).AdId,
                    StockOutDetails = _stockOutDetailsList
                };


                LoadStockOutData();
                MessageBox.Show("Stock out successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Something went wrong when trying to input the new StockOut Receipt! May be you should reload this app or call for support!");
            }
        }

        private void bntDelAll_Click(object sender, RoutedEventArgs e)
        {
            ErrorDetailsItem.Clear();
            _stockOutDetailsList.Clear();
            lvDataStockOut.Items.Refresh();
            LoadStockOutData();
        }
    }
}