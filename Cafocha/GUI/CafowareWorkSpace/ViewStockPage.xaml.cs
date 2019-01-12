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
        }

        private void SearchIBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var filter = SearchIBox.Text.Trim();

            if (filter.Length == 0)
                lvStock.ItemsSource = _stockList.Where(p => p.Deleted.Equals(0));
            else
            {
                lvStock.ItemsSource = _stockList.Where(p => p.Name.ToLower().Contains(filter.ToLower()) && p.Deleted.Equals(0));
            }


        }

        private void SearchIBox_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }
    }
}