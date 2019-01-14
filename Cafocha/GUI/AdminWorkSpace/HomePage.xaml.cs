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

            dpStartDay.SelectedDate = new DateTime(1990, 1, 1);
            dpEndDay.SelectedDate = DateTime.UtcNow.AddHours((TimeZoneInfo.Local.BaseUtcOffset.Hours));

            dpEndDay.SelectedDate = DateTime.Now;

            dpStartDay.SelectedDateChanged += TxtStartDay_OnSelectedDateChanged;
            dpEndDay.SelectedDateChanged += TxtStartDay_OnSelectedDateChanged;

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
                EmpPieSeries.Add(new PieSeries { Title = item.EmpId + ": " + item.Name });
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
            ColumnChartDatafilling();
            ChartDataFilling();
            ChartDataFillingByTime();
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
            ColumnChartDatafilling();
            ChartDataFilling();
            ChartDataFillingByTime();
        }

        private void ColumnChartDatafilling()
        {
            // filter data
            var startDate = dpStartDay.SelectedDate.Value;
            var endDate = dpEndDay.SelectedDate.Value;

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 23, 59, 59);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);


            var orderNoteWithTime = new List<OrderNote>(_businessModuleLocator.RepositoryLocator.OrderRepository.Get().ToList()
                .Where(obj => IsInTimeRange(obj.OrderTime, startDate, endDate)).ToList());


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

        public static bool IsInTimeRange(DateTime obj, DateTime timeRangeFrom, DateTime timeRangeTo)
        {
            long time = obj.Ticks, t1From = timeRangeFrom.Ticks, t1To = timeRangeTo.Ticks;

            return time >= t1From && time <= t1To;
        }

        public static bool IsInTimeRange(DateTime? datePay, DateTime startDate, DateTime endDate)
        {
            DateTime UpdatedTime = datePay ?? DateTime.Now;

            return IsInTimeRange(UpdatedTime, startDate, endDate);
        }

        private void ChartDataFillingByTime()
        {
            // filter data
            var startDate = dpStartDay.SelectedDate.Value;
            var endDate = dpEndDay.SelectedDate.Value;

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 23, 59, 59);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);


            var orderNoteWithTime = new List<OrderNote>(_businessModuleLocator.RepositoryLocator.OrderRepository.Get().ToList()
                .Where(obj => IsInTimeRange(obj.OrderTime, startDate, endDate)).ToList());
            var stockInWithTime = new List<StockIn>(_businessModuleLocator.RepositoryLocator.StockInRepository.Get().ToList());
            stockInWithTime = stockInWithTime.Where(obj => IsInTimeRange(obj.InTime,startDate,endDate)).ToList();
            var stockOutWithTime = new List<StockOut>(_businessModuleLocator.RepositoryLocator.StockOutRepository.Get().ToList()
                .Where(obj => IsInTimeRange(obj.OutTime, startDate, endDate)).ToList());
            var salaryNoteWithTime = new List<SalaryNote>(_businessModuleLocator.RepositoryLocator.SalaryNoteRepository.Get().ToList()
                .Where(obj => IsInTimeRange(obj.DatePay, startDate, endDate)).ToList());

            
            // calculate data
            decimal TotalPriceSang = 0;
            decimal TotalPriceChieu = 0;
            decimal TotalPriceToi = 0;

            decimal TotalePrice_nonDiscountSang = 0;
            decimal TotalePrice_nonDiscountChieu = 0;
            decimal TotalePrice_nonDiscountToi = 0;

            foreach (var item in orderNoteWithTime.Where(c => c.OrderTime.Hour >= 0 && c.OrderTime.Hour < 12))
            {
                TotalPriceSang += item.TotalPrice;
                TotalePrice_nonDiscountSang += item.TotalPriceNonDisc;
            }

            foreach (var item in orderNoteWithTime.Where(c => c.OrderTime.Hour >= 12 && c.OrderTime.Hour < 18))
            {
                TotalPriceChieu += item.TotalPrice;
                TotalePrice_nonDiscountChieu += item.TotalPriceNonDisc;
            }

            foreach (var item in orderNoteWithTime.Where(c =>
                c.OrderTime.Hour >= 18 && c.OrderTime.Hour <= 23 && c.OrderTime.Minute <= 59))
            {
                TotalPriceToi += item.TotalPrice;
                TotalePrice_nonDiscountToi += item.TotalPriceNonDisc;
            }

            txtTotalBills.Text = orderNoteWithTime.Count().ToString();

            txtDiscounts.Text = string.Format("{0:0.000}",
                TotalePrice_nonDiscountSang + TotalePrice_nonDiscountChieu + TotalePrice_nonDiscountToi -
                (TotalPriceSang + TotalPriceChieu + TotalPriceToi));

            var totalStockIn = (decimal)0;
            foreach (var stockIn in stockInWithTime)
            {
                totalStockIn += stockIn.TotalAmount;
            }
            txtTotalStockIn.Text = string.Format("{0:0.000}", totalStockIn);
            var totalSalary = (decimal)0;
            foreach (var salary in salaryNoteWithTime)
            {
                totalSalary += salary.SalaryValue;
            }
            txtTotalSalary.Text = string.Format("{0:0.000}", totalSalary);

            var totalReceive = TotalPriceSang + TotalPriceChieu + TotalPriceToi;
            txtReceivables.Text = string.Format("{0:0.000}", totalReceive);
            txtProfit.Text = string.Format("{0:0.000}", totalReceive - totalStockIn - totalSalary);


            // binding
            FirstPieSeries.Values = new ChartValues<ObservableValue> { new ObservableValue((double)TotalPriceSang) };
            FirstPieSeries.DataLabels = true;

            SecondPieSeries.Values = new ChartValues<ObservableValue> { new ObservableValue((double)TotalPriceChieu) };
            SecondPieSeries.DataLabels = true;

            ThirdPieSeries.Values = new ChartValues<ObservableValue> { new ObservableValue((double)TotalPriceToi) };
            ThirdPieSeries.DataLabels = true;
        }


        private void ChartDataFilling()
        {
            // filter data
            var startDate = dpStartDay.SelectedDate.Value;
            var endDate = dpEndDay.SelectedDate.Value;

            startDate = new DateTime(startDate.Year, startDate.Month, startDate.Day, 23, 59, 59);
            endDate = new DateTime(endDate.Year, endDate.Month, endDate.Day, 23, 59, 59);


            var orderNoteWithTime = new List<OrderNote>(_businessModuleLocator.RepositoryLocator.OrderRepository.Get().ToList()
                .Where(obj => IsInTimeRange(obj.OrderTime, startDate, endDate)).ToList());


            decimal count = 0;
            foreach (var itemserie in EmpPieSeries)
            {
                var data = itemserie.Title.Split(':');
                var empId = data[0];

                foreach (var item2 in orderNoteWithTime.Where(c => c.EmpId.Equals(empId))) count += item2.TotalPrice;
                itemserie.Values = new ChartValues<ObservableValue> { new ObservableValue((double)count) };
                itemserie.DataLabels = true;
                count = 0;
            }
        }

        private void Chart_OnDataClick(object sender, ChartPoint chartpoint)
        {
            var chart = (PieChart)chartpoint.ChartView;

            //clear selected slice.
            foreach (PieSeries series in chart.Series)
                series.PushOut = 0;

            var selectedSeries = (PieSeries)chartpoint.SeriesView;
            selectedSeries.PushOut = 8;
        }
        
        private void TxtStartDay_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ChartDataFillingByTime();
            ChartDataFilling();
            ColumnChartDatafilling();
        }
    }
}