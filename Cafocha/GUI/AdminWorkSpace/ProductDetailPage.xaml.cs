using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.Entities;
using Cafocha.GUI.CafowareWorkSpace;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for ProductDetailPage.xaml
    /// </summary>
    public partial class ProductDetailPage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        private Stock _ingre;
        private List<Stock> allIngre;
        private List<Product> allProduct;
        private List<ProductDetail> allProductDetails;
        private List<ProductModule.PDTemp> allProductDetailsWithName;
        //        private IngredientAddOrUpdateDialog _ingreAddOrUpdate;

        public ProductDetailPage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            Loaded += ProductDetailPage_Loaded;
        }

        private void ProductDetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            initPageData();
        }

        private void initPageData()
        {
            allProduct = _businessModuleLocator.ProductModule.getAllProduct().ToList();
            lvProduct.ItemsSource = allProduct;
            allProductDetails = _businessModuleLocator.ProductModule.getAllProductDetails().ToList();
            allIngre = _businessModuleLocator.WarehouseModule.IngredientList;
            allProductDetailsWithName = new List<ProductModule.PDTemp>();
            this.generatorProductDetailsWithName();

            cboType.Items.Add(ProductType.All);
            cboType.Items.Add(ProductType.Drink);
            cboType.Items.Add(ProductType.Topping);
            cboType.Items.Add(ProductType.Dessert);
            cboType.Items.Add(ProductType.Other);
        }

        private void lvData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var pro = lvProduct.SelectedItem as Product;
            if (pro == null) return;

            allProductDetails = _businessModuleLocator.ProductModule.getAllProductDetails(pro.ProductId).ToList();
            this.generatorProductDetailsWithName();
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
           
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            var filter = SearchBox.Text.Trim();

            refreshData(filter);
        }

        private void refreshData(string filter)
        {
            try
            {

                if (filter.Length == 0)
                {
                    lvProduct.ItemsSource =
                        _businessModuleLocator.ProductModule.getAllProduct((int) cboType.SelectedItem);
                    return;
                }

                lvProduct.ItemsSource =
                    _businessModuleLocator.ProductModule.getAllProduct((int) cboType.SelectedItem, filter);
            }
            catch (Exception ex)
            {
                if (filter.Length == 0)
                {
                    lvProduct.ItemsSource = _businessModuleLocator.ProductModule.getAllProduct().ToList();
                    return;
                }

                lvProduct.ItemsSource = _businessModuleLocator.ProductModule.getAllProduct(filter);
            }
            lvProduct.Items.Refresh();
        }


        private void cboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //var allProduct = _unitofwork.ProductRepository.Get(p => p.Deleted.Equals(0)).ToList();

            //if(SearchBox.Text.Trim().Equals(""))
            //{
            //    try
            //    {
            //        lvProduct.ItemsSource = allProduct.Where(p => p.Type == ((int)(sender as ComboBox).SelectedValue));
            //    }
            //    catch (Exception ex)
            //    {
            //        lvProduct.ItemsSource = allProduct;
            //    }
            //}
            //else
            //{
            //    try
            //    {
            //        lvProduct.ItemsSource = allProduct.Where(p => p.Type == ((int)(sender as ComboBox).SelectedValue) && p.Name.Contains(SearchBox.Text.Trim()));
            //    }
            //    catch (Exception ex)
            //    {
            //        lvProduct.ItemsSource = allProduct.Where(p => p.Name.Contains(SearchBox.Text.Trim()));
            //    }
            //}

            var selectedVal = (int) (sender as ComboBox).SelectedValue;
            if (selectedVal == -1)
                lvProduct.ItemsSource = allProduct;
            else
                lvProduct.ItemsSource = allProduct.Where(p => p.Type == selectedVal);
        }

        private void bntEditPro_Click(object sender, RoutedEventArgs e)
        {
            var curPro = lvProduct.SelectedItem as Product;
            if (curPro == null)
            {
                MessageBox.Show("Please choose exactly which product you want to update!");
                return;
            }

            //var pup = new ProductUpdateDialog(_businessModuleLocator, curPro);
            //((AdminWindow) Window.GetWindow(this)).myframe.Navigate(pup);

            ProductUpdateDialog productUpdateDialog = new ProductUpdateDialog(_businessModuleLocator, curPro);
            productUpdateDialog.ShowDialog();

            refreshListData();

        }

        private void bntDelPro_Click(object sender, RoutedEventArgs e)
        {
            var delPro = lvProduct.SelectedItem as Product;
            if (delPro == null)
            {
                MessageBox.Show("Please choose exactly which product you want to delete!");
                return;
            }

            var delMess =
                MessageBox.Show(
                    "This action will delete all following product details! Do you want to delete " + delPro.Name +
                    "(" + delPro.ProductId + ")?", "Warning! Are you sure?", MessageBoxButton.YesNo);
            if (delMess == MessageBoxResult.Yes)
            {
                _businessModuleLocator.ProductModule.deleteProduct(delPro);
                refreshListData();
            }
        }

        private void SearchIBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
        }

        private void generatorProductDetailsWithName()
        {
            allProductDetailsWithName.Clear();
            foreach (var pd in allProductDetails)
            {
                var curing = allIngre.FirstOrDefault(x => x.StoId == (pd.IgdId));
                if (curing != null) allProductDetailsWithName.Add(new ProductModule.PDTemp { ProDe = pd, Ingre = curing });
            }
        }
        private void refreshListData()
        {
            lvProduct.ItemsSource = _businessModuleLocator.ProductModule.getAllProduct();
            lvProduct.UnselectAll();
            lvProduct.Items.Refresh();
        }

        private void BntAddPro_Click(object sender, RoutedEventArgs e)
        {
            //var pup = new ProductCreatorDialog(_businessModuleLocator);
            //((AdminWindow)Window.GetWindow(this)).myframe.Navigate(pup);

            ProductCreatorDialog productCreatorDialog = new ProductCreatorDialog(_businessModuleLocator);
            productCreatorDialog.ShowDialog();

            refreshListData();

        }
    }
}