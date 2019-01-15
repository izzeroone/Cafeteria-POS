using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.Helper.PrintHelper.Report;
using Cafocha.Entities;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for StockHistoryPage.xaml
    /// </summary>
    public partial class StockHistoryPage : Page
    {
        public class StockInOut
        {
            public StockInOut(StockIn stockIn)
            {
                Id = stockIn.StockinId;
                EmployeeName = stockIn.Employee.Name;
                Time = stockIn.InTime;
                TotalAmount = stockIn.TotalAmount;
                IsStockIn = true;
            }

            public StockInOut(StockOut stockOut)
            {
                Id = stockOut.StockoutId;
                EmployeeName = stockOut.Employee.Name;
                Time = stockOut.OutTime;
                TotalAmount = stockOut.TotalAmount;
                IsStockIn = false;
            }

            public string Id { get; set; }


            public string EmployeeName { get; set; }


            public System.DateTime Time { get; set; }

            public bool IsStockIn { get; set; }

            public string Note { get; set; }


            public decimal TotalAmount { get; set; }
        }

        private readonly BusinessModuleLocator _businessModuleLocator;

        private readonly List<StockInOut> _stockInOutList = new List<StockInOut>();

        private List<OrderNote> filtero = new List<OrderNote>();

        private List<OrderNoteDetail> filterod = new List<OrderNoteDetail>();
        private bool isRaiseEvent = true;


        private List<StockIn> stockInList;
        private List<StockOut> stockOutList;

//        private List<StockInDetail> stockInDetail;
//        private List<StockOutDetail> stockOutDetail;


        public StockHistoryPage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();

            cboProduct.Items.Add("All");
            cboProduct.Items.Add("Nhập");
            cboProduct.Items.Add("Xuất");

            cboProduct.SelectedIndex = 0;

            Refresh();
           
            ((INotifyCollectionChanged) lvStockInOut.Items).CollectionChanged += ListView_CollectionChanged;

       

            Loaded += Page_Loaded;
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            _stockInOutList.Clear();

            stockInList = new List<StockIn>(_businessModuleLocator.WarehouseModule.getStockInList());
            stockOutList = new List<StockOut>(_businessModuleLocator.WarehouseModule.getStockOutList());
            

            // search filter
            var filterSearch = txtSearchBox.Text.Trim();
            if (!string.IsNullOrEmpty(filterSearch))
            {
                stockInList = stockInList.Where(x => Regex.IsMatch(x.StockinId, filterSearch, RegexOptions.IgnoreCase)
                                                        || Regex.IsMatch(x.Employee.Name, filterSearch, RegexOptions.IgnoreCase)
                                                        || Regex.IsMatch(x.TotalAmount.ToString(), filterSearch, RegexOptions.IgnoreCase)
                                                     ).ToList();

                stockOutList = stockOutList.Where(x => Regex.IsMatch(x.StockoutId, filterSearch, RegexOptions.IgnoreCase)
                                                        || Regex.IsMatch(x.Employee.Name, filterSearch, RegexOptions.IgnoreCase)
                                                        || Regex.IsMatch(x.TotalAmount.ToString(), filterSearch, RegexOptions.IgnoreCase)
                                                     ).ToList();
            }

            // combox type fiter
            switch (cboProduct.SelectedIndex)
            {
                case 1:
                    stockOutList.Clear();
                    break;
                case 2:
                    stockInList.Clear();
                    break;
            }

            // combox type fiter
            if (pickDate.SelectedDate != null)
            {
                var time = pickDate.SelectedDate.Value;
            
                stockInList = stockInList.Where(x => x.InTime.Day == time.Day && x.InTime.Month == time.Month && x.InTime.Year == time.Year
                                                     ).ToList();

                stockOutList = stockOutList.Where(x => x.OutTime.Day == time.Day && x.OutTime.Month == time.Month && x.OutTime.Year == time.Year
                                                     ).ToList();
            }


            // to History list
            foreach (var stockIn in stockInList)
            {
                _stockInOutList.Add(new StockInOut(stockIn));
            }

            foreach (var stockOut in stockOutList)
            {
                _stockInOutList.Add(new StockInOut(stockOut));
            }
            
            lvStockInOut.ItemsSource = _stockInOutList.OrderByDescending(x => x.Time).ToList();

            lvStockInOut.Items.Refresh();
        }

        private void ListView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                DataChange();
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void lvStockInOut_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var stockInOut = lvStockInOut.SelectedItem as StockInOut;
            if (stockInOut != null)
            {
                if (stockInOut.IsStockIn)
                {
                    lvStockInOutDetail.ItemsSource =
                        _businessModuleLocator.WarehouseModule.getStockInDetail(stockInOut.Id);
                }
                else
                {
                    lvStockInOutDetail.ItemsSource =
                        _businessModuleLocator.WarehouseModule.getStockOutDetail(stockInOut.Id);
                }
            }
            else
                lvStockInOutDetail.ItemsSource = null;
        }

        private void cboProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void txtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Refresh();

        }

        private void pickOrderDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            Refresh();
        }

        private void BtnOverViewReport_OnClick(object sender, RoutedEventArgs e)
        {
            var optionDialog = new ReportOptionDialog(new OrderNoteReport(), _businessModuleLocator);
            optionDialog.Show();
        }

        private void LvStockInOut_OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DataChange();
        }

        private void LvStockInOut_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DataChange();
        }

        private void DataChange()
        {
//            for (int i = 0; i < lvStockInOut.Items.Count; i++)
//            {
//                ListViewItem item = (ListViewItem)lvStockInOut.Items.GetItemAt(i);
//                var stockInOuts = (List<StockInOut>)lvStockInOut.ItemsSource;
//                var stockInOut = stockInOuts[i];
//
//                if (stockInOut.IsStockIn)
//                {
//                    item.Background = Brushes.LightGray;
//
//                }
//            }
        }
    }
}