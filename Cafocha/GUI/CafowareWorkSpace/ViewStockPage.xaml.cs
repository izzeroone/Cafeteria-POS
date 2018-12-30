using System.Collections.Generic;
using System.Windows.Controls;
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
        private WarehouseModule _warehouseModule;

        public ViewStockPage(WarehouseModule warehouseModule, List<Stock> stockList)
        {
            _warehouseModule = warehouseModule;
            InitializeComponent();

            lvItem.ItemsSource = stockList;
        }
    }
}
