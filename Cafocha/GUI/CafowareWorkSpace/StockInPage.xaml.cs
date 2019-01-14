using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.Helper.PrintHelper;
using Cafocha.BusinessContext.User;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Entities;
using Cafocha.GUI.EmployeeWorkSpace;

namespace Cafocha.GUI.CafowareWorkSpace
{
    /// <summary>
    ///     Interaction logic for StockInPage.xaml
    /// </summary>
    public partial class StockInPage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        internal StockIn _currentStockIn;
        internal List<StockInDetail> _stockInDetailsList;
        private readonly List<Stock> _stockList;
        internal WarehouseModule _warehouseModule;


        /*********************************
         * Form Manipulate
         *********************************/
          

        private readonly List<int> ErrorDetailsItem = new List<int>();


        public StockInPage(BusinessModuleLocator businessModuleLocator, List<Stock> stockList)
        {
            _businessModuleLocator = businessModuleLocator;
            _stockList = stockList;
            InitializeComponent();
            lvDataStock.ItemsSource = _stockList;

            _stockInDetailsList = new List<StockInDetail>();
            _currentStockIn = new StockIn
            {
                EmpId = EmployeeModule.WorkingEmployee.Emp.EmpId,
                StockInDetails = _stockInDetailsList
            };

            lvDataStockIn.ItemsSource = _stockInDetailsList;

            LoadStockInData();
        }

        private void LoadStockInData()
        {
            _currentStockIn.TotalAmount = 0;
            foreach (var details in _stockInDetailsList)
                _currentStockIn.TotalAmount += details.ItemPrice * (decimal) details.Quan;
            txtTotalPrice.Text = string.Format("{0:0.000}", _currentStockIn.TotalAmount);
        }


        /*********************************
         * Manipulate Each Stock
         *********************************/

        private void lvDataStock_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            var stock = (Stock) lvDataStock.SelectedItem;
            if (stock == null)
                return;


            var r = new StockInDetail();

            var foundIteminReceipt = _stockInDetailsList.FirstOrDefault(c => c.StoId.Equals(stock.StoId));
            if (foundIteminReceipt == null)
            {
                r.StoId = stock.StoId;
                r.Stock = stock;
                r.Quan = 1;
                r.ItemPrice = stock.StandardPrice;
                _stockInDetailsList.Add(r);
            }
            else
            {
                foundIteminReceipt.Quan++;
            }

            lvDataStockIn.Items.Refresh();
            LoadStockInData();
        }


        /*********************************
         * Manipulate Each StockInDetails
         *********************************/

        private void txtQuan_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textboxQuan = sender as TextBox;


            int index;
            var r = new StockInDetail();
            var dep = (DependencyObject) e.OriginalSource;
            while (dep != null && !(dep is ListViewItem)) dep = VisualTreeHelper.GetParent(dep);
            if (dep == null)
                return;
            index = lvDataStockIn.ItemContainerGenerator.IndexFromContainer(dep);


            try
            {
                if (textboxQuan.Text == null || textboxQuan.Text.Length == 0)
                {
                    MessageBox.Show("The quantity of Input Stock can not be blank!");
                    if (!ErrorDetailsItem.Contains(index))
                        ErrorDetailsItem.Add(index);
                    return;
                }

                _stockInDetailsList[index].Quan = int.Parse(textboxQuan.Text);

                LoadStockInData();
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

        private void txtItemPrice_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textboxItemPrice = sender as TextBox;


            int index;
            var r = new StockInDetail();
            var dep = (DependencyObject) e.OriginalSource;
            while (dep != null && !(dep is ListViewItem)) dep = VisualTreeHelper.GetParent(dep);
            if (dep == null)
                return;
            index = lvDataStockIn.ItemContainerGenerator.IndexFromContainer(dep);


            try
            {
                if (textboxItemPrice.Text == null || textboxItemPrice.Text.Length == 0)
                {
                    MessageBox.Show("The Price of Input Stock can not be blank!");
                    if (!ErrorDetailsItem.Contains(index))
                        ErrorDetailsItem.Add(index);
                    return;
                }

                _stockInDetailsList[index].ItemPrice = decimal.Parse(textboxItemPrice.Text);

                LoadStockInData();
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
            var r = new StockInDetail();
            var dep = (DependencyObject) e.OriginalSource;
            while (dep != null && !(dep is ListViewItem)) dep = VisualTreeHelper.GetParent(dep);
            if (dep == null)
                return;
            index = lvDataStockIn.ItemContainerGenerator.IndexFromContainer(dep);


            if (_stockInDetailsList[index].Quan > 1 && !ErrorDetailsItem.Contains(index))
            {
                r.Quan = _stockInDetailsList[index].Quan - 1;
                r.StoId = _stockInDetailsList[index].StoId;
                r.Stock = _stockInDetailsList[index].Stock;
                r.ItemPrice = _stockInDetailsList[index].ItemPrice;
                _stockInDetailsList[index] = r;
            }
            else
            {
                _stockInDetailsList.RemoveAt(index);
                if (ErrorDetailsItem.Contains(index))
                    ErrorDetailsItem.Remove(index);
            }

            lvDataStockIn.Items.Refresh();
            LoadStockInData();
        }

        private void bntEdit_Click(object sender, RoutedEventArgs e)
        {
            int index;
            var r = new StockInDetail();
            var dep = (DependencyObject) e.OriginalSource;

            while (dep != null && !(dep is ListViewItem)) dep = VisualTreeHelper.GetParent(dep);

            if (dep == null)
                return;
            index = lvDataStockIn.ItemContainerGenerator.IndexFromContainer(dep);
            var inputNote = new InputNote(_stockInDetailsList[index].Note);
            if (_stockInDetailsList[index].Note == null || _stockInDetailsList[index].Note.Equals("") ||
                _stockInDetailsList[index].Note.Equals(inputNote.Note))
            {
                if (inputNote.ShowDialog() == true)
                {
                    r.Note = inputNote.Note;
                    r.StoId = _stockInDetailsList[index].StoId;
                    r.Stock = _stockInDetailsList[index].Stock;
                    r.Quan = _stockInDetailsList[index].Quan;
                    r.ItemPrice = _stockInDetailsList[index].ItemPrice;
                    _stockInDetailsList[index] = r;
                }
            }
            else
            {
                inputNote.ShowDialog();
            }

            lvDataStockIn.Items.Refresh();
        }

        private void bntAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ErrorDetailsItem.Count != 0)
                {
                    MessageBox.Show("Lỗi, xin kiểm tra lại dữ liệu đầu vào");
                    return;
                }

                if (_currentStockIn.StockInDetails.Count == 0)
                {
                    MessageBox.Show("Bạn chưa chọn NVL cần nhập!");
                    return;
                }
                
                _businessModuleLocator.WarehouseModule.addStockIn(_currentStockIn);

                _stockInDetailsList = new List<StockInDetail>();
                lvDataStockIn.ItemsSource = _stockInDetailsList;
                lvDataStockIn.Items.Refresh();

                _currentStockIn = new StockIn()
                {
                    EmpId = EmployeeModule.WorkingEmployee.Emp.EmpId,
                    StockInDetails = _stockInDetailsList
                };

                
                LoadStockInData();

                MessageBoxResult rsltMessageBox = MessageBox.Show("Đã thêm phiếu nhập thành công!\nBạn có muốn in phiếu nhập?",
                    "",
                    MessageBoxButton.YesNo);
                if (rsltMessageBox == MessageBoxResult.Yes)
                    print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nhập kho, vui lòng kiểm tra lại kết nối hoặc dữ liệu đầu vào");
            }
        }

        private void bntDelAll_Click(object sender, RoutedEventArgs e)
        {
            ErrorDetailsItem.Clear();
            _stockInDetailsList.Clear();
            lvDataStockIn.Items.Refresh();
            LoadStockInData();
        }

        private void BntPrint_Click(object sender, RoutedEventArgs e)
        {
            print();
        }

        private void print()
        {
            var printHelper = new DoPrintHelper(_businessModuleLocator.RepositoryLocator,
                DoPrintHelper.StockIn_Printing, _currentStockIn);
            printHelper.DoPrint();
        }
    }
}