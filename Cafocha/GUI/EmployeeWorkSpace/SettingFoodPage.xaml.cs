using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for SettingFood.xaml
    /// </summary>
    public partial class SettingFoodPage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;

        public SettingFoodPage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            lvData.ItemsSource = _businessModuleLocator.ProductModule.getAllProduct();
            for (var i = 0; i <= 100; i++) cbopromotion.Items.Add(i.ToString());
        }

        private void lvData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            bntUpdate.IsEnabled = true;
            var pro = lvData.SelectedItem as Product;

            txtID.Text = pro.ProductId;
            txtName.Text = pro.Name;
            //txtPrice.Text = pro.Price.ToString();
            txtPrice.Text = string.Format("{0:0.000}", pro.Price);
            cbopromotion.SelectedItem = pro.Discount.ToString();
        }

        private void bntUpdate_Click(object sender, RoutedEventArgs e)
        {
            var p = _businessModuleLocator.ProductModule.getProduct(txtID.Text);
            p.Discount = int.Parse(cbopromotion.SelectedValue.ToString());
            _businessModuleLocator.ProductModule.updateProduct(p);

            MessageBox.Show("Cập nhật thành công");
        }

        private void txtPrice_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}