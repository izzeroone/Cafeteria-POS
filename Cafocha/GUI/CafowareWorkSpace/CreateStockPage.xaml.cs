using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.CafowareWorkSpace
{
    /// <summary>
    ///     Interaction logic for CreateStockPage.xaml
    /// </summary>
    public partial class CreateStockPage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;

        private Stock _currentNewStock = new Stock();

        private Stock _selectedStock;
        private readonly List<Stock> _stockList;

        public CreateStockPage(BusinessModuleLocator businessModuleLocator, List<Stock> stockList)
        {
            _businessModuleLocator = businessModuleLocator;
            _stockList = stockList;
            InitializeComponent();
            lvStock.ItemsSource = _stockList;
            initComboBox();
        }

        public CreateStockPage(BusinessModuleLocator businessModuleLocator, Stock editStock)
        {
            _businessModuleLocator = businessModuleLocator;
            _stockList = _businessModuleLocator.WarehouseModule.StockList;
            InitializeComponent();
            lvStock.ItemsSource = _stockList;
            initComboBox();
            _selectedStock = editStock;
            putStockDataToForm();
        }

        public CreateStockPage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            _stockList = _businessModuleLocator.WarehouseModule.StockList;
            InitializeComponent();
            lvStock.ItemsSource = _stockList;
            initComboBox();
        }

        private void initComboBox()
        {
            var stockGroupList
                = new List<StockType> (_businessModuleLocator.WarehouseModule.StockTypes);
            stockGroupList.Add(new StockType() {StId = "ALL", Deleted = 0, Name = "All"});
            cboGroup.ItemsSource = stockGroupList;
            cboStockGroup.ItemsSource = _businessModuleLocator.WarehouseModule.StockTypes; ;
            cboStockGroup.SelectedIndex = 0;

            cboUnit.Items.Add("pcs");
            cboUnit.Items.Add("bot");
            cboUnit.Items.Add("can");
            cboUnit.Items.Add("ml");
            cboUnit.SelectedIndex = 0;
        }


        /*********************************
        * Controls
        *********************************/

        private void cboGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var filter = SearchIBox.Text.Trim();
            var item = (((sender as ComboBox).SelectedItem) as StockType).StId;
            var all = (((sender as ComboBox).SelectedItem) as StockType).StId == "ALL" ;
            
            if (filter.Length == 0)
            {
                if (all || (sender as ComboBox).SelectedValue.Equals(StockGroup.All))
                    lvStock.ItemsSource = _stockList;
                else
                    lvStock.ItemsSource =
                        _stockList.Where(x => x.StId.Equals(item));
            }
            else
            {
                if (all|| (sender as ComboBox).SelectedValue.Equals(StockGroup.All))
                    lvStock.ItemsSource = _stockList.Where(x => x.Name.Contains(filter));
                else
                    lvStock.ItemsSource = _stockList.Where(x =>
                        x.StId.Equals(item) && x.Name.Contains(filter));
            }
        }

        private void putStockDataToForm()
        {
            if (_selectedStock == null)
            {
                return;
            }
            //put data to form
            txtName.Text = _selectedStock.Name;
            txtInfo.Text = _selectedStock.Info;
            cboStockGroup.SelectedItem = _selectedStock.StId;


            cboUnit.SelectedItem = _selectedStock.Unit;
            txtSupplier.Text = _selectedStock.Supplier;
            txtPrice.Text = _selectedStock.StandardPrice.ToString();
        }

        private void bntDel_Click(object sender, RoutedEventArgs e)
        {
            if (lvStock.SelectedItem == null)
            {
                MessageBox.Show("Stock must be selected to delete! Choose again!");
                return;
            }

            var delStock = lvStock.SelectedItem as Stock;
            if (delStock != null)
            {
                var delMess =
                    MessageBox.Show(
                        "This action will delete all following stock details! Do you want to delete " + delStock.Name +
                        "(" + delStock.StoId + ")?", "Warning! Are you sure?", MessageBoxButton.YesNo);
                if (delMess == MessageBoxResult.Yes)
                {
                    _businessModuleLocator.WarehouseModule.deleteStock(delStock);

                    // refesh data
                    ((CafowareWindow) Window.GetWindow(this)).Refresh_Tick(null, new EventArgs());
                    lvStock.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Please choose stock you want to delete and try again!");
            }
        }


        /*********************************
        * Manipulate Search Box
        *********************************/

        private void SearchIBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = SearchIBox.Text.Trim();
            var selectedStock = cboGroup.SelectedIndex;

            if (selectedStock < 0 || cboGroup.SelectedValue.Equals(StockGroup.All))
            {
                if (filter.Length == 0)
                    lvStock.ItemsSource = _stockList.Where(p => p.Deleted.Equals(0));
                else
                    lvStock.ItemsSource = _stockList.Where(p => p.Name.Contains(filter) && p.Deleted.Equals(0));
            }
            else
            {
                if (filter.Length == 0)
                    lvStock.ItemsSource = _stockList.Where(p =>
                        p.StId.Equals((String) cboGroup.SelectedItem) && p.Deleted.Equals(0));
                else
                    lvStock.ItemsSource = _stockList.Where(p =>
                        p.StId.Equals((String) cboGroup.SelectedItem) && p.Name.Contains(filter) && p.Deleted.Equals(0));
            }
        }

        private void SearchIBox_GotFocus(object sender, RoutedEventArgs e)
        {
        }


        /*********************************
        * Manipulate Form
        *********************************/

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text)) e.Handled = !char.IsNumber(e.Text[0]);
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //check name
                var name = txtName.Text.Trim();
                if (name.Length == 0)
                {
                    MessageBox.Show("Name is not valid!");
                    txtName.Focus();
                    return;
                }

                //check info
                var info = txtInfo.Text.Trim();

                var group = (String) cboStockGroup.SelectedItem;
                var unit = cboUnit.SelectedItem.ToString();

                //check supplier
                var supplier = txtSupplier.Text;

                //check price
                var price = decimal.Parse(txtPrice.Text.Trim());


                var newWareHouse = new ApWareHouse
                {
                    ApwarehouseId = "",
                    Name = "",
                    Contain = 0,
                    StdContain = 100
                };

                _businessModuleLocator.WarehouseModule.insertWarehouse(newWareHouse);


                _currentNewStock.ApwarehouseId = newWareHouse.ApwarehouseId;
                _currentNewStock.Name = name;
                _currentNewStock.Info = info;
                _currentNewStock.StId = group;
                _currentNewStock.Unit = unit;
                _currentNewStock.Supplier = supplier;
                _currentNewStock.StandardPrice = price;

                _businessModuleLocator.WarehouseModule.insertStock(_currentNewStock);


                MessageBox.Show("Add new stock " + _currentNewStock.Name + "(" + _currentNewStock.StoId +
                                ") successful!");
                clearAllData();

                // refesh data
                ((CafowareWindow) Window.GetWindow(this)).Refresh_Tick(null, new EventArgs());
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong. Can not add new stock. Please check your input again!");
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            clearAllData();
        }

        private void clearAllData()
        {
            txtName.Text = "";
            txtInfo.Text = "";
            cboStockGroup.SelectedItem = StockGroup.Cosmetics;
            cboUnit.SelectedIndex = 0;
            txtSupplier.Text = "";
            txtPrice.Text = "";

            _currentNewStock = new Stock();
            lvStock.Items.Refresh();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (lvStock.SelectedIndex == -1)
            {
                MessageBox.Show("Bạn chưa chọn mặt hàng");
                return;
            }
            //check name
            var name = txtName.Text.Trim();
            if (name.Length == 0)
            {
                MessageBox.Show("Name is not valid!");
                txtName.Focus();
                return;
            }

            //check info
            var info = txtInfo.Text.Trim();

            var group = (string) cboStockGroup.SelectedItem;
            var unitIn = cboUnit.SelectedItem.ToString();

            //check supplier
            var supplier = txtSupplier.Text;

            //check price
            var price = decimal.Parse(txtPrice.Text.Trim());


            _selectedStock.Name = name;
            _selectedStock.Info = info;
            _selectedStock.StId = group;
            _selectedStock.Unit = unitIn;
            _selectedStock.Supplier = supplier;
            _selectedStock.StandardPrice = price;

            _businessModuleLocator.WarehouseModule.updateStock(_selectedStock);


            MessageBox.Show("Update stock " + _selectedStock.Name + "(" + _selectedStock.StoId + ") successful!");
            clearAllData();

            _selectedStock = null;
            // refesh data
            ((CafowareWindow) Window.GetWindow(this)).Refresh_Tick(null, new EventArgs());
        }

        private void LvStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedStock = lvStock.SelectedItem as Stock;
            //put data to form
            putStockDataToForm();
        }
    }
}