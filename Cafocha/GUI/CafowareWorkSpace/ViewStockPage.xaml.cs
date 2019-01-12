using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cafocha.BusinessContext;
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
            var stockGroups = Enum.GetValues(typeof(StockGroup)).Cast<StockGroup>();
            foreach (var stockGroup in stockGroups)
            {
                cboGroup.Items.Add(stockGroup);
            }
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
            var selectedStock = cboGroup.SelectedIndex;

            if (selectedStock < 0 || cboGroup.SelectedValue.Equals(StockGroup.All))
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
                        p.Group.Equals((int)cboGroup.SelectedItem) && p.Deleted.Equals(0));
                else
                    lvStock.ItemsSource = _stockList.Where(p =>
                        p.Group.Equals((int)cboGroup.SelectedItem) && p.Name.ToLower().Contains(filter.ToLower()) && p.Deleted.Equals(0));
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