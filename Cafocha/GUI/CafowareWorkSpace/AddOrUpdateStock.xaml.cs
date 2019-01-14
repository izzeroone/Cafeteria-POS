using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Entities;

namespace Cafocha.GUI.CafowareWorkSpace
{
    /// <summary>
    ///     Interaction logic for CreateStockPage.xaml
    /// </summary>
    public partial class AddOrUpdateStock : Window
    {
        private readonly BusinessModuleLocator _businessModuleLocator;

        private Stock _currentNewStock = new Stock();

        private Stock _selectedStock;

        public AddOrUpdateStock(BusinessModuleLocator businessModuleLocator, Stock editStock)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            initComboBox();
            if (editStock != null)
            {
                btnAdd.Visibility = Visibility.Collapsed;
            }
            else
            {
                btnUpdate.Visibility = Visibility.Collapsed;
            }
            _selectedStock = editStock;
            putStockDataToForm();
        }



        private void initComboBox()
        {
            cboStockGroup.ItemsSource = WarehouseModule.StockTypes; 
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

        private void putStockDataToForm()
        {
            if (_selectedStock == null)
            {
                return;
            }
            //put data to form
            txtName.Text = _selectedStock.Name;
            txtInfo.Text = _selectedStock.Info;
            cboStockGroup.SelectedValue = _selectedStock.StId;


            cboUnit.SelectedItem = _selectedStock.Unit;
            txtSupplier.Text = _selectedStock.Supplier;
            txtPrice.Text = _selectedStock.StandardPrice.ToString();
        }

       
       

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
                    MessageBox.Show("Tên không hợp lệ!");
                    txtName.Focus();
                    return;
                }

                //check info
                var info = txtInfo.Text.Trim();

                var group = cboStockGroup.SelectedValue;
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
                _currentNewStock.StId = (string) @group;
                _currentNewStock.Unit = unit;
                _currentNewStock.Supplier = supplier;
                _currentNewStock.StandardPrice = price;

                _businessModuleLocator.WarehouseModule.insertStock(_currentNewStock);


                MessageBox.Show("Thêm nguyên vật liệu " + _currentNewStock.Name + "(" + _currentNewStock.StoId +
                                ") thành công!");
                this.Close();

                // refesh data
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể thêm nguyên vật liệu");
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
            cboStockGroup.SelectedIndex = 0;
            cboUnit.SelectedIndex = 0;
            txtSupplier.Text = "";
            txtPrice.Text = "";

            _currentNewStock = new Stock();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            //check name
            var name = txtName.Text.Trim();
            if (name.Length == 0)
            {
                MessageBox.Show("Tên không hợp lệ!");
                txtName.Focus();
                return;
            }

            //check info
            var info = txtInfo.Text.Trim();

            var group = cboStockGroup.SelectedValue;
            var unitIn = cboUnit.SelectedItem.ToString();

            //check supplier
            var supplier = txtSupplier.Text;

            //check price
            var price = decimal.Parse(txtPrice.Text.Trim());


            _selectedStock.Name = name;
            _selectedStock.Info = info;
            _selectedStock.StId = (string) @group;
            _selectedStock.Unit = unitIn;
            _selectedStock.Supplier = supplier;
            _selectedStock.StandardPrice = price;

            _businessModuleLocator.WarehouseModule.updateStock(_selectedStock);


            MessageBox.Show("Cập nhật nguyên vật liệu " + _selectedStock.Name + "(" + _selectedStock.StoId + ") thành công!");
            this.Close();
        }
    }
}