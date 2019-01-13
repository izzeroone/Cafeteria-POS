using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.User;
using Cafocha.Entities;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for UcMenu.xaml
    /// </summary>
    public partial class UcMenu : UserControl
    {
        private BusinessModuleLocator _businessModuleLocator;

        private TabItem curItem = new TabItem();


        internal bool IsRefreshMenu = true;

        public UcMenu()
        {
            InitializeComponent();

            Loaded += UcMenu_Loaded;
        }

        public void UcMenu_Loaded(object sender, RoutedEventArgs e)
        {
            _businessModuleLocator = ((MainWindow) Window.GetWindow(this))._businessModuleLocator;
            if (IsRefreshMenu)
                try
                {
                    refreshMenu();


                    IsRefreshMenu = false;
                }
                catch (Exception ex)
                {
                }
        }

        private void refreshMenu()
        {
            lvCategoryDessert.ItemsSource =
                _businessModuleLocator.ProductModule.Get(p => p.Type == (int) ProductType.Dessert);
            lvCategoryDrink.ItemsSource =
                _businessModuleLocator.ProductModule.Get(p => p.Type == (int) ProductType.Drink);
            lvCategoryBeer.ItemsSource =
                _businessModuleLocator.ProductModule.Get(p => p.Type == (int) ProductType.Topping);
            lvCategoryOther.ItemsSource =
                _businessModuleLocator.ProductModule.Get(p => p.Type == (int) ProductType.Other);
        }


        //ToDo: Need to update the contain in Warehouse database when new order occur
        private void lvCategory_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (EmployeeModule.WorkingEmployee.EmpWH == null)
            {
                MessageBox.Show("Bạn nên bắt đầu phiên làm việc trước");
                return;
            }

            var lbSelected = sender as ListBox;


            var item = lbSelected.SelectedItem;
            var it = (Product) lbSelected.SelectedItem;
            if (item != null)
            {
                _businessModuleLocator.TakingOrderModule.addProductToOrder(it);
                lbSelected.UnselectAll();

                ((MainWindow) Window.GetWindow(this)).en.ucOrder.RefreshControl();
                ((MainWindow) Window.GetWindow(this)).en.ucOrder.txtDay.Text =
                    _businessModuleLocator.TakingOrderModule.OrderTemp.Ordertime.ToString("dd/MM/yyyy H:mm:ss");
            }
        }


        private void Search_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var filter = SearchBox.Text.Trim();
                checkSearch(filter);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = SearchBox.Text.Trim();

            if (filter.Length == 0)
            {
                refreshMenu();
                return;
            }

            checkSearch(filter);
        }

        //check khi Search
        private void checkSearch(string filter)
        {
            if (ItemDessert.IsSelected)
            {
                lvCategoryDessert.ItemsSource = _businessModuleLocator.ProductModule.Get(p =>
                    p.Type == (int) ProductType.Dessert && p.Name.Contains(filter));
                lvCategoryDessert.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemDessert;
            }

            if (ItemBeverages.IsSelected)
            {
                lvCategoryDrink.ItemsSource = _businessModuleLocator.ProductModule.Get(p =>
                    p.Type == (int) ProductType.Drink && p.Name.Contains(filter));
                lvCategoryDrink.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemBeverages;
            }

            if (ItemBeer.IsSelected)
            {
                lvCategoryBeer.ItemsSource = _businessModuleLocator.ProductModule.Get(p =>
                    p.Type == (int) ProductType.Topping && p.Name.Contains(filter));
                lvCategoryBeer.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemBeer;
            }

            if (ItemOther.IsSelected)
            {
                lvCategoryOther.ItemsSource = _businessModuleLocator.ProductModule.Get(p =>
                    p.Type == (int) ProductType.Other && p.Name.Contains(filter));
                lvCategoryOther.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemOther;
            }
        }


        private void TabItem_GotFocus(object sender, RoutedEventArgs e)
        {
            //TabItem sen = sender as TabItem;

            //if (curItem == null)
            //{
            //    return;
            //}

            //if (!sen.Name.Equals(curItem.Name))
            //{
            //    SearchBox.Text = "";
            //}
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}