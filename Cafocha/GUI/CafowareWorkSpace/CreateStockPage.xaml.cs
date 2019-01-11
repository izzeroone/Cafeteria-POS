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
            cboGroup.Items.Add(StockGroup.All);
            cboGroup.Items.Add(StockGroup.Ingridient);
            cboGroup.Items.Add(StockGroup.Cosmetics);
            cboGroup.Items.Add(StockGroup.SpaVoucher);
            cboGroup.Items.Add(StockGroup.GymVoucher);
            cboGroup.Items.Add(StockGroup.ResVoucher);
            cboGroup.Items.Add(StockGroup.TravVoucher);
            cboGroup.Items.Add(StockGroup.Food);
            cboGroup.Items.Add(StockGroup.Agricultural);
            cboGroup.Items.Add(StockGroup.Watch);
            cboGroup.Items.Add(StockGroup.TopTen);
            cboGroup.SelectedItem = StockGroup.All;

            cboStockGroup.Items.Add(StockGroup.Ingridient);
            cboStockGroup.Items.Add(StockGroup.Cosmetics);
            cboStockGroup.Items.Add(StockGroup.SpaVoucher);
            cboStockGroup.Items.Add(StockGroup.GymVoucher);
            cboStockGroup.Items.Add(StockGroup.ResVoucher);
            cboStockGroup.Items.Add(StockGroup.TravVoucher);
            cboStockGroup.Items.Add(StockGroup.Food);
            cboStockGroup.Items.Add(StockGroup.Agricultural);
            cboStockGroup.Items.Add(StockGroup.Watch);
            cboStockGroup.Items.Add(StockGroup.TopTen);
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
            var selectedI = (sender as ComboBox).SelectedIndex;

            if (filter.Length == 0)
            {
                if (selectedI < 0 || (sender as ComboBox).SelectedValue.Equals(StockGroup.All))
                    lvStock.ItemsSource = _stockList;
                else
                    lvStock.ItemsSource =
                        _stockList.Where(x => x.Group.Equals((int) (sender as ComboBox).SelectedItem));
            }
            else
            {
                if (selectedI < 0 || (sender as ComboBox).SelectedValue.Equals(StockGroup.All))
                    lvStock.ItemsSource = _stockList.Where(x => x.Name.Contains(filter));
                else
                    lvStock.ItemsSource = _stockList.Where(x =>
                        x.Group.Equals((int) (sender as ComboBox).SelectedItem) && x.Name.Contains(filter));
            }
        }

        private void bntEdit_Click(object sender, RoutedEventArgs e)
        {
            _selectedStock = lvStock.SelectedItem as Stock;

            if (lvStock.SelectedItem == null)
            {
                MessageBox.Show("Stock must be selected to update! Choose again!");
                return;
            }

            lvStock.UnselectAll();
            lvStock.Items.Refresh();
            btnUpdate.Visibility = Visibility.Visible;

            //put data to form
            putStockDataToForm();
        }

        private void putStockDataToForm()
        {

            //put data to form
            txtName.Text = _selectedStock.Name;
            txtInfo.Text = _selectedStock.Info;

            switch (_selectedStock.Group)
            {
                case (int)StockGroup.Ingridient:
                    cboStockGroup.SelectedItem = StockGroup.Ingridient;
                    break;
                case (int)StockGroup.Cosmetics:
                    cboStockGroup.SelectedItem = StockGroup.Cosmetics;
                    break;
                case (int)StockGroup.SpaVoucher:
                    cboStockGroup.SelectedItem = StockGroup.SpaVoucher;
                    break;
                case (int)StockGroup.GymVoucher:
                    cboStockGroup.SelectedItem = StockGroup.GymVoucher;
                    break;
                case (int)StockGroup.ResVoucher:
                    cboStockGroup.SelectedItem = StockGroup.ResVoucher;
                    break;
                case (int)StockGroup.TravVoucher:
                    cboStockGroup.SelectedItem = StockGroup.TravVoucher;
                    break;
                case (int)StockGroup.Food:
                    cboStockGroup.SelectedItem = StockGroup.Food;
                    break;
                case (int)StockGroup.Agricultural:
                    cboStockGroup.SelectedItem = StockGroup.Agricultural;
                    break;
                case (int)StockGroup.Watch:
                    cboStockGroup.SelectedItem = StockGroup.Watch;
                    break;
                case (int)StockGroup.TopTen:
                    cboStockGroup.SelectedItem = StockGroup.TopTen;
                    break;
            }

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
                        p.Group.Equals((int) cboGroup.SelectedItem) && p.Deleted.Equals(0));
                else
                    lvStock.ItemsSource = _stockList.Where(p =>
                        p.Group.Equals((int) cboGroup.SelectedItem) && p.Name.Contains(filter) && p.Deleted.Equals(0));
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

                var group = (int) cboStockGroup.SelectedItem;
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
                _currentNewStock.Group = group;
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
            btnUpdate.Visibility = Visibility.Hidden;
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
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

            var group = (int) cboStockGroup.SelectedItem;
            var unitIn = cboUnit.SelectedItem.ToString();

            //check supplier
            var supplier = txtSupplier.Text;

            //check price
            var price = decimal.Parse(txtPrice.Text.Trim());


            _selectedStock.Name = name;
            _selectedStock.Info = info;
            _selectedStock.Group = group;
            _selectedStock.Unit = unitIn;
            _selectedStock.Supplier = supplier;
            _selectedStock.StandardPrice = price;

            _businessModuleLocator.WarehouseModule.updateStock(_selectedStock);


            MessageBox.Show("Update stock " + _selectedStock.Name + "(" + _selectedStock.StoId + ") successful!");
            clearAllData();

            btnUpdate.Visibility = Visibility.Hidden;
            _selectedStock = null;
            // refesh data
            ((CafowareWindow) Window.GetWindow(this)).Refresh_Tick(null, new EventArgs());
        }
    }
}