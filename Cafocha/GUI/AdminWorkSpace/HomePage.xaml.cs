using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cafocha.BusinessContext;
using Cafocha.Entities;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private static readonly int FILL_ALL = 0;
        private static readonly int FILL_BY_DAY = 1;
        private static readonly int FILL_BY_MONTH = 2;


        private readonly BusinessModuleLocator _businessModuleLocator;
        public Dictionary<string, int> CountList;
        public List<decimal> PriceList;
        public ChartValues<decimal> Values;


        public HomePage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            // init datasource for Time PieChart
            SeriesCollectionTime = new SeriesCollection();
            PriceList = new List<decimal>();
            FirstPieSeries = new PieSeries
            {
                Title = "0h-12h"
            };
            SecondPieSeries = new PieSeries
            {
                Title = "12h-18h"
            };
            ThirdPieSeries = new PieSeries
            {
                Title = "18h-0h"
            };
            SeriesCollectionTime.Add(FirstPieSeries);
            SeriesCollectionTime.Add(SecondPieSeries);
            SeriesCollectionTime.Add(ThirdPieSeries);


            // init datasource for Employee PieChart
            SeriesCollection = new SeriesCollection();
            EmpPieSeries = new List<PieSeries>();
            foreach (var item in _businessModuleLocator.EmployeeModule.getEmployees())
                EmpPieSeries.Add(new PieSeries {Title = item.EmpId + ": " + item.Name});
            foreach (var item in EmpPieSeries) SeriesCollection.Add(item);

            //init datasource for ColumnChart
            Values = new ChartValues<decimal>();
            SerieColumnChart = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "revenue",
                    Values = Values
                }
            };
            Labels = new List<string>();
            Formatter = value => value.ToString();


            // fill chart at first time
            ColumnChartDatafilling(FILL_ALL);
            ChartDataFilling(FILL_ALL);
            ChartDataFillingByTime(FILL_ALL);
        }

        public Func<ChartPoint, string> PointLabel { get; set; }
        public SeriesCollection SeriesCollection { get; set; }
        private List<PieSeries> EmpPieSeries { get; }


        public Func<decimal, string> Formatter { get; set; }
        public List<string> Labels { get; set; }
        public SeriesCollection SerieColumnChart { get; set; }

        public SeriesCollection SeriesCollectionTime { get; set; }
        public PieSeries FirstPieSeries { get; set; }
        public PieSeries SecondPieSeries { get; set; }
        public PieSeries ThirdPieSeries { get; set; }

        public void RefreshHome()
        {
            rdAll.IsChecked = true;

            ColumnChartDatafilling(FILL_ALL);
            ChartDataFilling(FILL_ALL);
            ChartDataFillingByTime(FILL_ALL);
        }

        private void ColumnChartDatafilling(int filter)
        {
            var orderNoteWithTime = new List<OrderNote>();
            if (filter == FILL_BY_DAY)
                orderNoteWithTime = _businessModuleLocator.RepositoryLocator.OrderRepository.Get(c =>
                    c.OrderTime.Day == DateTime.Now.Day && c.OrderTime.Month == DateTime.Now.Month &&
                    c.OrderTime.Year == DateTime.Now.Year).ToList();
            else if (filter == FILL_BY_MONTH)
                orderNoteWithTime = _businessModuleLocator.RepositoryLocator.OrderRepository.Get(c =>
                        c.OrderTime.Day == DateTime.Now.Day && c.OrderTime.Month == DateTime.Now.Month &&
                        c.OrderTime.Year == DateTime.Now.Year)
                    .ToList();
            else
                orderNoteWithTime = _businessModuleLocator.RepositoryLocator.OrderRepository
                    .Get(c => c.OrderTime.Year == DateTime.Now.Year).ToList();
            decimal count = 0;
            Values.Clear();
            Labels.Clear();

            var RevenueList = new Dictionary<string, decimal>();
            foreach (var item in orderNoteWithTime)
                if (RevenueList.ContainsKey(item.OrderTime.ToString("dd/MM/yyyy")))
                    RevenueList[item.OrderTime.ToString("dd/MM/yyyy")] =
                        RevenueList[item.OrderTime.ToString("dd/MM/yyyy")] + item.TotalPrice;
                else
                    RevenueList.Add(item.OrderTime.ToString("dd/MM/yyyy"), item.TotalPrice);

            foreach (var revenue in RevenueList)
            {
                Labels.Add(revenue.Key);
                Values.Add(revenue.Value);
            }

            DataContext = this;
        }

        private void ChartDataFillingByTime(int filter)
        {
            // filter data
            var orderNoteWithTime = new List<OrderNote>();
            if (filter == FILL_BY_DAY)
                orderNoteWithTime = _businessModuleLocator.RepositoryLocator.OrderRepository.Get(c =>
                    c.OrderTime.Day == DateTime.Now.Day && c.OrderTime.Month == DateTime.Now.Month &&
                    c.OrderTime.Year == DateTime.Now.Year).ToList();
            else if (filter == FILL_BY_MONTH)
                orderNoteWithTime = _businessModuleLocator.RepositoryLocator.OrderRepository.Get(c =>
                    c.OrderTime.Day == DateTime.Now.Day && c.OrderTime.Month == DateTime.Now.Month &&
                    c.OrderTime.Year == DateTime.Now.Year).ToList();
            else
                orderNoteWithTime = _businessModuleLocator.RepositoryLocator.OrderRepository
                    .Get(c => c.OrderTime.Year == DateTime.Now.Year).ToList();


            // calculate data
            decimal TotalPrice1 = 0;
            decimal TotalPrice2 = 0;
            decimal TotalPrice3 = 0;
            decimal TotalePrice_nonDiscount1 = 0;
            decimal TotalePrice_nonDiscount2 = 0;
            decimal TotalePrice_nonDiscount3 = 0;
            foreach (var item in orderNoteWithTime.Where(c => c.OrderTime.Hour >= 0 && c.OrderTime.Hour < 12))
            {
                TotalPrice1 += item.TotalPrice;
                TotalePrice_nonDiscount1 += item.TotalPriceNonDisc;
            }

            foreach (var item in orderNoteWithTime.Where(c => c.OrderTime.Hour >= 12 && c.OrderTime.Hour < 18))
            {
                TotalPrice2 += item.TotalPrice;
                TotalePrice_nonDiscount2 += item.TotalPriceNonDisc;
            }

            foreach (var item in orderNoteWithTime.Where(c =>
                c.OrderTime.Hour >= 18 && c.OrderTime.Hour <= 23 && c.OrderTime.Minute <= 59))
            {
                TotalPrice3 += item.TotalPrice;
                TotalePrice_nonDiscount3 += item.TotalPriceNonDisc;
            }

            txtRevenue.Text = string.Format("{0:0.000}", TotalPrice1 + TotalPrice2 + TotalPrice3);
            txtReceivables.Text = string.Format("{0:0.000}", TotalPrice1 + TotalPrice2 + TotalPrice3);
            txtTotalBills.Text = orderNoteWithTime.Count().ToString();
            txtSaleValue.Text = string.Format("{0:0.000}",
                TotalePrice_nonDiscount1 + TotalePrice_nonDiscount2 + TotalePrice_nonDiscount3);
            txtDiscounts.Text = string.Format("{0:0.000}",
                TotalePrice_nonDiscount1 + TotalePrice_nonDiscount2 + TotalePrice_nonDiscount3 -
                (TotalPrice1 + TotalPrice2 + TotalPrice3));
            // binding
            FirstPieSeries.Values = new ChartValues<ObservableValue> {new ObservableValue((double) TotalPrice1)};
            FirstPieSeries.DataLabels = true;

            SecondPieSeries.Values = new ChartValues<ObservableValue> {new ObservableValue((double) TotalPrice2)};
            SecondPieSeries.DataLabels = true;

            ThirdPieSeries.Values = new ChartValues<ObservableValue> {new ObservableValue((double) TotalPrice3)};
            ThirdPieSeries.DataLabels = true;
        }

        private void ChartDataFilling(int filter)
        {
            var orderNoteWithTime = new List<OrderNote>();
            if (filter == FILL_BY_DAY)
                orderNoteWithTime = _businessModuleLocator.RepositoryLocator.OrderRepository.Get(c =>
                    c.OrderTime.Day == DateTime.Now.Day && c.OrderTime.Month == DateTime.Now.Month &&
                    c.OrderTime.Year == DateTime.Now.Year).ToList();
            else if (filter == FILL_BY_MONTH)
                orderNoteWithTime = _businessModuleLocator.RepositoryLocator.OrderRepository.Get(c =>
                        c.OrderTime.Day == DateTime.Now.Day && c.OrderTime.Month == DateTime.Now.Month &&
                        c.OrderTime.Year == DateTime.Now.Year)
                    .ToList();
            else
                orderNoteWithTime = _businessModuleLocator.RepositoryLocator.OrderRepository
                    .Get(c => c.OrderTime.Year == DateTime.Now.Year).ToList();


            decimal count = 0;
            foreach (var itemserie in EmpPieSeries)
            {
                var data = itemserie.Title.Split(':');
                var empId = data[0];

                foreach (var item2 in orderNoteWithTime.Where(c => c.EmpId.Equals(empId))) count += item2.TotalPrice;
                itemserie.Values = new ChartValues<ObservableValue> {new ObservableValue((double) count)};
                itemserie.DataLabels = true;
                count = 0;
            }
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (PieChart) chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries) chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }


        private void RdToday_OnClick(object sender, RoutedEventArgs e)
        {
            ChartDataFillingByTime(1);
            ChartDataFilling(1);
            ColumnChartDatafilling(1);
        }

        private void RdAll_OnClick(object sender, RoutedEventArgs e)
        {
            ChartDataFillingByTime(0);
            ChartDataFilling(0);
            ColumnChartDatafilling(0);
        }

        private void RdMonth_OnClick(object sender, RoutedEventArgs e)
        {
            ChartDataFillingByTime(2);
            ChartDataFilling(2);
            ColumnChartDatafilling(2);
        }
    }
}