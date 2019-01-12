using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Entities;

namespace Cafocha.GUI.CafowareWorkSpace
{
    /// <summary>
    ///     Interaction logic for ViewStockPage.xaml
    /// </summary>
    public partial class ViewStockPage : Page
    {
        private BusinessModuleLocator _businessModuleLocator;

        private readonly List<Stock> _stockList;

        public ViewStockPage(BusinessModuleLocator businessModuleLocator, List<Stock> stockList)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            _stockList = stockList;
            
            lvStock.ItemsSource = stockList;

            // init Combobox
            var stockGroupList
                = new List<StockType>(WarehouseModule.StockTypes);
            stockGroupList.Add(new StockType() { StId = "ALL", Deleted = 0, Name = "All" });
            cboGroup.ItemsSource = stockGroupList;
            cboGroup.SelectedIndex = cboGroup.Items.Count - 1;

        }

        private void Page_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                Refresh();
        }

        private void LvStock_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }

        public void Refresh()
        {
            lvStock.Items.Refresh();

            // Dua vao search box va combobox de filter

            var filter = SearchIBox.Text.Trim();
            var selectedGroup = cboGroup.SelectedIndex;

            if (selectedGroup == cboGroup.Items.Count - 1 || cboGroup.SelectedValue.Equals(StockGroup.All))
            {
                if (filter.Length == 0)
                    lvStock.ItemsSource = _stockList.Where(p => p.Deleted.Equals(0));
                else
                    lvStock.ItemsSource = _stockList.Where(p => p.Name.ToLower().Contains(filter.ToLower()) && p.Deleted.Equals(0));
            }
            else
            {
                if (filter.Length == 0)
                    lvStock.ItemsSource = _stockList.Where(p =>
                        p.StId.Equals(cboGroup.SelectedValue) && p.Deleted.Equals(0));
                else
                    lvStock.ItemsSource = _stockList.Where(p =>
                        p.StId.Equals(cboGroup.SelectedValue) && p.Name.ToLower().Contains(filter.ToLower()) && p.Deleted.Equals(0));
            }

        }

        private void SearchIBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Refresh();
        }

        private void SearchIBox_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void cboGroup_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }
    }
}