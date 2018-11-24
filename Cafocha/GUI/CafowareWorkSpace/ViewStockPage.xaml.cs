using System.Collections.Generic;
using System.Windows.Controls;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.CafowareWorkSpace
{
    /// <summary>
    /// Interaction logic for ViewStockPage.xaml
    /// </summary>
    public partial class ViewStockPage : Page
    {
        private AdminwsOfCloudAPWH _unitofwork;

        public ViewStockPage(AdminwsOfCloudAPWH unitofwork, List<Stock> stockList)
        {
            _unitofwork = unitofwork;
            InitializeComponent();

            lvItem.ItemsSource = stockList;
        }
    }
}
