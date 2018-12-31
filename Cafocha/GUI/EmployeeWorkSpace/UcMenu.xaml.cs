using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.EmployeeWorkSpace
{ 
    /// <summary>
    /// Interaction logic for UcMenu.xaml
    /// </summary>
public partial class UcMenu : UserControl
 {
        private RepositoryLocator _unitofwork;
        private ProductModule _productModule;
        private TakingOrderModule _takingOrderModule;
        public UcMenu()
        {
            InitializeComponent();

            this.Loaded += UcMenu_Loaded;
        }



        internal bool IsRefreshMenu = true;
        public void UcMenu_Loaded(object sender, RoutedEventArgs e)
        {
            this._productModule = ((MainWindow)Window.GetWindow(this))._productModule;
            this._takingOrderModule = ((MainWindow)Window.GetWindow(this)).takingOrderModule;
            if (IsRefreshMenu)
            {
                try
                {
                    lvCategoryBreakFast.ItemsSource =
                        _productModule.Get(p => p.StdStats.Equals("BreakFast"));
                    lvCategoryStarter.ItemsSource =
                        _productModule.Get(p => p.StdStats.Equals("Starter"));
                    lvCategoryMain.ItemsSource =
                        _productModule.Get(p => p.StdStats.Equals("Main"));
                    lvCategoryDessert.ItemsSource =
                        _productModule.Get(p => p.StdStats.Equals("Dessert"));
                    lvCategoryBeverages.ItemsSource =
                        _productModule.Get(p => (p.Type == (int)ProductType.Beverage || p.Type == (int)ProductType.Coffee));
                    lvCategoryBeer.ItemsSource =
                        _productModule.Get(p => p.Type == (int)ProductType.Beer);
                    lvCategoryWine.ItemsSource =
                        _productModule.Get(p => p.Type == (int)ProductType.Wine);
                    lvCategoryOther.ItemsSource =
                        _productModule.Get(p => p.Type == (int)ProductType.Other);


                    IsRefreshMenu = false;


                }
                catch (Exception ex)
                {

                }
            }
        }

  


        //ToDo: Need to update the contain in Warehouse database when new order occur
        private void lvCategory_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                return;
            }
            ListBox lbSelected = sender as ListBox;


            var item = lbSelected.SelectedItem;
            Product it = (Product)lbSelected.SelectedItem;
            if (item != null)
            {

                _takingOrderModule.addProductToOrder(it);
                lbSelected.UnselectAll();

                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _takingOrderModule.OrderTemp);
                ((MainWindow)Window.GetWindow(this)).en.ucOrder.RefreshControl(_unitofwork);
                ((MainWindow)Window.GetWindow(this)).en.ucOrder.txtDay.Text = _takingOrderModule.OrderTemp.Ordertime.ToString("dd/MM/yyyy H:mm:ss");
            }

        }



        private void Search_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string filter = SearchBox.Text.Trim();
                checkSearch(filter);
            }
        }

        TabItem curItem = new TabItem();
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = SearchBox.Text.Trim();

            if (filter.Length == 0)
            {
                lvCategoryBreakFast.ItemsSource = _productModule.Get(p => p.StdStats.Equals("BreakFast"));
                lvCategoryStarter.ItemsSource = _productModule.Get(p => p.StdStats.Equals("Starter"));
                lvCategoryMain.ItemsSource = _productModule.Get(p => p.StdStats.Equals("Main"));
                lvCategoryDessert.ItemsSource = _productModule.Get(p => p.StdStats.Equals("Dessert"));
                lvCategoryBeverages.ItemsSource = _productModule.Get(p => p.Type == (int)ProductType.Beverage);
                lvCategoryBeer.ItemsSource = _productModule.Get(p => p.Type == (int)ProductType.Beer);
                lvCategoryWine.ItemsSource = _productModule.Get(p => p.Type == (int)ProductType.Wine);
                lvCategoryOther.ItemsSource = _productModule.Get(p => p.Type == (int)ProductType.Other);
                return;
            }

            checkSearch(filter);
        }

        //check khi Search
        private void checkSearch(string filter)
        {
            if (ItemBreakFast.IsSelected == true)
            {
                lvCategoryBreakFast.ItemsSource = _productModule.Get(p => p.StdStats.Equals("BreakFast") && p.Name.Contains(filter));
                lvCategoryBreakFast.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemBreakFast;
            }

            if (ItemStarter.IsSelected == true)
            {
                lvCategoryStarter.ItemsSource = _productModule.Get(p => p.StdStats.Equals("Starter") && p.Name.Contains(filter));
                lvCategoryStarter.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemStarter;
            }

            if (ItemMain.IsSelected == true)
            {
                lvCategoryMain.ItemsSource = _productModule.Get(p => p.StdStats.Equals("Main") && p.Name.Contains(filter));
                lvCategoryMain.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemMain;
            }

            if (ItemDessert.IsSelected == true)
            {
                lvCategoryDessert.ItemsSource = _productModule.Get(p => p.StdStats.Equals("Dessert") && p.Name.Contains(filter));
                lvCategoryDessert.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemDessert;
            }

            if (ItemBeverages.IsSelected == true)
            {
                lvCategoryBeverages.ItemsSource = _productModule.Get(p => p.Type == (int)ProductType.Beverage && p.Name.Contains(filter));
                lvCategoryBeverages.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemBeverages;
            }

            if (ItemBeer.IsSelected == true)
            {
                lvCategoryBeer.ItemsSource = _productModule.Get(p => p.Type == (int)ProductType.Beer && p.Name.Contains(filter));
                lvCategoryBeer.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemBeer;
            }

            if (ItemWine.IsSelected == true)
            {
                lvCategoryWine.ItemsSource = _productModule.Get(p => p.Type == (int)ProductType.Wine && p.Name.Contains(filter));
                lvCategoryWine.PreviewMouseLeftButtonUp += lvCategory_PreviewMouseLeftButtonUp;
                curItem = ItemWine;
            }

            if (ItemOther.IsSelected == true)
            {
                lvCategoryOther.ItemsSource = _productModule.Get(p => p.Type == (int)ProductType.Other && p.Name.Contains(filter));
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

        private void checkWorkingAction(EmpLoginList currentEmp, OrderTemp ordertempcurrenttable)
        {
            if (currentEmp.Emp.EmpId.Equals(ordertempcurrenttable.EmpId))
            {
                return;
            }

            if (ordertempcurrenttable.SubEmpId != null)
            {
                string[] subemplist = ordertempcurrenttable.SubEmpId.Split(',');

                for (int i = 0; i < subemplist.Count(); i++)
                {
                    if (subemplist[i].Equals(""))
                    {
                        continue;
                    }

                    if (currentEmp.Emp.EmpId.Equals(subemplist[i]))
                    {
                        return;
                    }
                }

                ordertempcurrenttable.SubEmpId += currentEmp.Emp.EmpId + ",";
                return;
            }

            ordertempcurrenttable.SubEmpId += currentEmp.Emp.EmpId + ",";

        }

    }
}
