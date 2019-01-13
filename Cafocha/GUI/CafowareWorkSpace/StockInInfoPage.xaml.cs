using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cafocha.BusinessContext;
using Cafocha.Entities;
using Cafocha.GUI.Helper.PrintHelper.Report;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for StockInInfoPage.xaml
    /// </summary>
    public partial class StockInInfoPage : Page
    {
        public class StockInOut
        {
            public StockInOut(StockIn stockIn)
            {
                Id = stockIn.SiId;
                EmId = stockIn.EmpId;
                Time = stockIn.Intime;
                TotalAmount = stockIn.TotalAmount;
                IsStockIn = true;
            }

            public StockInOut(StockOut stockOut)
            {
                Id = stockOut.StockoutId;
                EmId = stockOut.EmpId;
                Time = stockOut.OutTime;
                TotalAmount = stockOut.TotalAmount;
                IsStockIn = false;
            }

            public string Id { get; set; }


            public string EmId { get; set; }


            public DateTime Time { get; set; }

            public bool IsStockIn { get; set; }


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


        public StockInInfoPage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();

            Refresh();
            
            ((INotifyCollectionChanged) lvStockInOut.Items).CollectionChanged += ListView_CollectionChanged;

            Loaded += Page_Loaded;
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
//            filtero = new List<OrderNote>();
//            lvStockInOut.UnselectAll();
//            lvStockInOutDetail.UnselectAll();
//            if (isRaiseEvent)
//            {
//                var cbopro = sender as ComboBox;
//                var proid = cbopro.SelectedValue.ToString();
//                if (!proid.Equals("--"))
//                {
//                    filterod = _ordernotedetailslist.Where(x => x.ProductId.Equals(proid)).ToList();
//                    var odd = filterod.GroupBy(x => x.OrdernoteId).Select(y => y.ToList()).ToList();
//
//                    foreach (var i in odd)
//                    foreach (var j in i)
//                    {
//                        filtero.Add(_stockInOutList.Where(x => x.OrdernoteId.Equals(j.OrdernoteId)).FirstOrDefault());
//                        break;
//                    }
//
//                    if (filtero.Count != 0 && pickOrderDate.SelectedDate == null)
//                    {
//                        lvStockInOut.ItemsSource = filtero;
//                        lvStockInOut.Items.Refresh();
//                        lvStockInOutDetail.ItemsSource = new List<OrderNoteDetail>();
//                        lvStockInOutDetail.Items.Refresh();
//                    }
//                    else if (filtero.Count != 0 && pickOrderDate.SelectedDate != null)
//                    {
//                        lvStockInOut.ItemsSource = filtero.Where(x =>
//                            x.OrderTime.ToShortDateString()
//                                .Equals(((DateTime) pickOrderDate.SelectedDate).ToShortDateString())).ToList();
//                        lvStockInOut.Items.Refresh();
//                        lvStockInOutDetail.ItemsSource = new List<OrderNoteDetail>();
//                        lvStockInOutDetail.Items.Refresh();
//                    }
//                    else
//                    {
//                        lvStockInOut.ItemsSource = new List<OrderNote>();
//                        lvStockInOut.Items.Refresh();
//                        lvStockInOutDetail.ItemsSource = new List<OrderNoteDetail>();
//                        lvStockInOutDetail.Items.Refresh();
//                    }
//                }
//                else
//                {
//                    if (pickOrderDate.SelectedDate == null)
//                    {
//                        lvStockInOut.ItemsSource = _stockInOutList;
//                        lvStockInOut.Items.Refresh();
//                        lvStockInOutDetail.ItemsSource = new List<OrderNoteDetail>();
//                        lvStockInOutDetail.Items.Refresh();
//                    }
//                    else
//                    {
//                        lvStockInOut.ItemsSource = _stockInOutList.Where(x =>
//                            x.OrderTime.ToShortDateString()
//                                .Equals(((DateTime) pickOrderDate.SelectedDate).ToShortDateString())).ToList();
//                        lvStockInOut.Items.Refresh();
//                        lvStockInOutDetail.ItemsSource = new List<OrderNoteDetail>();
//                        lvStockInOutDetail.Items.Refresh();
//                    }
//                }
//            }
        }

        private void txtSearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
        }

        private void txtSearchBox_KeyDown(object sender, KeyEventArgs e)
        {
        }

        private void lvOrderNoteDetails_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }

        private void pickOrderDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
//            var pick = sender as DatePicker;
//            if (pick.SelectedDate == null) return;
//
//            if (cboProduct.SelectedValue.Equals("--"))
//            {
//                lvStockInOut.ItemsSource = _stockInOutList.Where(x =>
//                    x.OrderTime.ToShortDateString().Equals(((DateTime) pick.SelectedDate).ToShortDateString()));
//                lvStockInOut.Items.Refresh();
//                lvStockInOutDetail.ItemsSource = new List<OrderNoteDetail>();
//                lvStockInOutDetail.Items.Refresh();
//            }
//            else
//            {
//                if (filtero.Count != 0)
//                {
//                    lvStockInOut.ItemsSource = filtero.Where(x =>
//                        x.OrderTime.ToShortDateString().Equals(((DateTime) pick.SelectedDate).ToShortDateString()));
//                    lvStockInOut.Items.Refresh();
//                }
//                else
//                {
//                    lvStockInOut.ItemsSource = new List<OrderNote>();
//                    lvStockInOut.Items.Refresh();
//                }
//
//                lvStockInOutDetail.ItemsSource = new List<OrderNoteDetail>();
//                lvStockInOutDetail.Items.Refresh();
//            }
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

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Refresh();
        }

        private void Refresh()
        {
            _stockInOutList.Clear();

            stockInList = _businessModuleLocator.WarehouseModule.getStockInList();
            stockOutList = _businessModuleLocator.WarehouseModule.getStockOutList();

            //            stockInDetail = _businessModuleLocator.WarehouseModule.getStockInDetail();
            //            stockOutDetail = _businessModuleLocator.WarehouseModule.getStockOutDetail();

            foreach (var stockIn in stockInList)
            {
                _stockInOutList.Add(new StockInOut(stockIn));
            }

            foreach (var stockOut in stockOutList)
            {
                _stockInOutList.Add(new StockInOut(stockOut));
            }

            lvStockInOut.ItemsSource = _stockInOutList;

        }
    }
}