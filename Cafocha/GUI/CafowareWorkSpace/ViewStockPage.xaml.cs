using System.Collections.Generic;
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

        public ViewStockPage(BusinessModuleLocator businessModuleLocator, List<Stock> stockList)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();

            lvItem.ItemsSource = stockList;
        }

        private void Page_GotFocus(object sender, RoutedEventArgs e)
        {
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible) lvItem.Items.Refresh();
        }
    }
}