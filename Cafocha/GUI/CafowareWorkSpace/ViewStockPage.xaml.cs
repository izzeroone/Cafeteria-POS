using System.Collections.Generic;
using System.Windows.Controls;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.CafowareWorkSpace
{
    /// <summary>
    /// Interaction logic for ViewStockPage.xaml
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

        private void Page_GotFocus(object sender, System.Windows.RoutedEventArgs e)
        {
        }

        private void Page_IsVisibleChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                lvItem.Items.Refresh();
            }
        }
    }
}
