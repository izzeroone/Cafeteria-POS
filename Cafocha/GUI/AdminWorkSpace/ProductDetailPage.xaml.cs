using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Entities;
using Cafocha.Repository.DAL;
using POS.AdminWorkSpace;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    /// Interaction logic for ProductDetailPage.xaml
    /// </summary>
    public partial class ProductDetailPage : Page
    {
        private BusinessModuleLocator _businessModuleLocator;
        private List<Product> allProduct;
        private List<ProductDetail> allProductDetails;
        private List<Stock> allIngre;
        private Stock _ingre;
//        private IngredientAddOrUpdateDialog _ingreAddOrUpdate;

        public ProductDetailPage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            this.Loaded += ProductDetailPage_Loaded;
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
            lvDetails.ItemsSource = allProductDetails;
            allIngre = _businessModuleLocator.WarehouseModule.IngredientList;
            lvIngredient.ItemsSource = allIngre;

            cboType.Items.Add(ProductType.All);
            cboType.Items.Add(ProductType.Drink);
            cboType.Items.Add(ProductType.Topping);
            cboType.Items.Add(ProductType.Dessert);
            cboType.Items.Add(ProductType.Other);

            //init Ingredient Type
            cboTypeI.Items.Add("All");
            cboTypeI.Items.Add("Other");
            cboTypeI.Items.Add("Dry");
            cboTypeI.Items.Add("Dairy");
            cboTypeI.Items.Add("Vegetable");
            cboTypeI.Items.Add("Fee");
            cboTypeI.SelectedItem = "All";
        }

        private void lvData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Product pro = lvProduct.SelectedItem as Product;
            if(pro == null)
            {
                return;
            }

            lvDetails.ItemsSource = _businessModuleLocator.ProductModule.getAllProductDetails(pro.ProductId);
        }

        private void SearchBox_GotFocus(object sender, RoutedEventArgs e)
        {
            SearchIBox.Text = "";
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            string filter = SearchBox.Text.Trim();

            refreshData(filter);
        }

        private void refreshData(string filter)
        {
            try
            {
                if (filter.Length == 0)
                {
                    lvProduct.ItemsSource = _businessModuleLocator.ProductModule.getAllProduct((int) cboType.SelectedItem);
                    return;
                }

                lvProduct.ItemsSource = _businessModuleLocator.ProductModule.getAllProduct((int) cboType.SelectedItem, filter);
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
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchBox.Text.Trim();

            refreshData(filter);
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

            int selectedVal = ((int)(sender as ComboBox).SelectedValue);
            if (selectedVal == -1)
            {
                lvProduct.ItemsSource = allProduct;
            }
            else
            {
                lvProduct.ItemsSource = allProduct.Where(p => p.Type == selectedVal);
            }

        }

        private void bntEditPro_Click(object sender, RoutedEventArgs e)
        {
            Product curPro = lvProduct.SelectedItem as Product;
            if (curPro == null)
            {
                MessageBox.Show("Please choose exactly which product you want to update!");
                return;
            }

            ProductUpdatePage pup = new ProductUpdatePage(_businessModuleLocator, curPro);
            ((AdminWindow)Window.GetWindow(this)).myframe.Navigate(pup);
        }

        private void bntDelPro_Click(object sender, RoutedEventArgs e)
        {
            Product delPro = lvProduct.SelectedItem as Product;
            if (delPro == null)
            {
                MessageBox.Show("Please choose exactly which product you want to delete!");
                return;
            }

            MessageBoxResult delMess = MessageBox.Show("This action will delete all following product details! Do you want to delete " + delPro.Name + "(" + delPro.ProductId + ")?", "Warning! Are you sure?", MessageBoxButton.YesNo);
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

        private void SearchIBox_KeyDown(object sender, KeyEventArgs e)
        {
            string filter = SearchIBox.Text.Trim();
            int selectedI = cboTypeI.SelectedIndex;

            if (selectedI < 0 || cboTypeI.SelectedValue.Equals("All"))
            {
                if (filter.Length == 0)
                {
                    lvIngredient.ItemsSource = allIngre.Where(p => p.Deleted.Equals(0));
                }
                else
                {
                    lvIngredient.ItemsSource = allIngre.Where(p => p.Name.Contains(filter) && p.Deleted.Equals(0));
                }
            }
            else
            {
                if (filter.Length == 0)
                {
                    lvIngredient.ItemsSource = allIngre.Where(p => p.Deleted.Equals(0));
                }
                else
                {
                    lvIngredient.ItemsSource = allIngre.Where(p => p.Name.Contains(filter) && p.Deleted.Equals(0));
                }
            }
        }

        private void SearchIBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchIBox.Text.Trim();
            int selectedI = cboTypeI.SelectedIndex;

            if (selectedI < 0 || cboTypeI.SelectedValue.Equals("All"))
            {
                if (filter.Length == 0)
                {
                    lvIngredient.ItemsSource = allIngre.Where(p => p.Deleted.Equals(0));
                }
                else
                {
                    lvIngredient.ItemsSource = allIngre.Where(p => p.Name.Contains(filter) && p.Deleted.Equals(0));
                }
            }
            else
            {
                if (filter.Length == 0)
                {
                    lvIngredient.ItemsSource = allIngre.Where(p =>  p.Deleted.Equals(0));
                }
                else
                {
                    lvIngredient.ItemsSource = allIngre.Where(p => p.Name.Contains(filter) && p.Deleted.Equals(0));
                }
            }
        }

        private void cboTypeI_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string filter = SearchIBox.Text.Trim();
            int selectedI = (sender as ComboBox).SelectedIndex;

            if (filter.Length == 0)
            {
                if (selectedI < 0 || (sender as ComboBox).SelectedValue.Equals("All"))
                {
                    lvIngredient.ItemsSource = allIngre;
                }
                else
                {
                    lvIngredient.ItemsSource = allIngre;
                }
            }
            else
            {
                if (selectedI < 0 || (sender as ComboBox).SelectedValue.Equals("All"))
                {
                    lvIngredient.ItemsSource = allIngre.Where(x => x.Name.Contains(filter));
                }
                else
                {
                    lvIngredient.ItemsSource = allIngre.Where(x => x.Name.Contains(filter));
                }
            }
        }

//        private void bntAdd_Click(object sender, RoutedEventArgs e)
//        {
//            _ingreAddOrUpdate = new IngredientAddOrUpdateDialog(_businessModuleLocator, null);
//            _ingreAddOrUpdate.ShowDialog();
//
//            lvIngredient.ItemsSource = _businessModuleLocator.IngredientModule.getAllIngredients();
//            lvIngredient.UnselectAll();
//            lvIngredient.Items.Refresh();
//        }

//        private void bntEdit_Click(object sender, RoutedEventArgs e)
//        {
//            _ingre = lvIngredient.SelectedItem as Ingredient;
//
//            if (lvIngredient.SelectedItem == null)
//            {
//                MessageBox.Show("Ingredient must be selected to update! Choose again!");
//                return;
//            }
//
//            _ingreAddOrUpdate = new IngredientAddOrUpdateDialog(_businessModuleLocator, _ingre);
//            _ingreAddOrUpdate.ShowDialog();
//
//            lvProduct.UnselectAll();
//            lvProduct.Items.Refresh();
//            lvDetails.UnselectAll();
//            lvDetails.Items.Refresh();
//            lvIngredient.UnselectAll();
//            lvIngredient.Items.Refresh();
//        }

//        private void bntDel_Click(object sender, RoutedEventArgs e)
//        {
//            if (lvIngredient.SelectedItem == null)
//            {
//                MessageBox.Show("Ingredient must be selected to delete! Choose again!");
//                return;
//            }
//
//            var delIngre = lvIngredient.SelectedItem as Ingredient;
//            if (delIngre != null)
//            {
//                MessageBoxResult delMess = MessageBox.Show("This action will delete all following product details! Do you want to delete " + delIngre.Name + "(" + delIngre.IgdId + ")?", "Warning! Are you sure?", MessageBoxButton.YesNo);
//                if (delMess == MessageBoxResult.Yes)
//                {
//                    _businessModuleLocator.IngredientModule.deleteIngredient(delIngre);
//                    refreshListData();
//                }
//            }
//            else
//            {
//                MessageBox.Show("Please choose ingredient you want to delete and try again!");
//            }
//        }

        private void refreshListData()
        {
            lvProduct.ItemsSource = _businessModuleLocator.ProductModule.getAllProduct();
            lvDetails.ItemsSource = _businessModuleLocator.ProductModule.getAllProductDetails();
            lvIngredient.ItemsSource = _businessModuleLocator.WarehouseModule.IngredientList;
            lvProduct.UnselectAll();
            lvProduct.Items.Refresh();
            lvDetails.UnselectAll();
            lvDetails.Items.Refresh();
            lvIngredient.UnselectAll();
            lvIngredient.Items.Refresh();
        }
    }
}
