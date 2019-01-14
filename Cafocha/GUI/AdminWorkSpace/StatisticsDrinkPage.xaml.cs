using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Cafocha.BusinessContext;
using Cafocha.Entities;
using LiveCharts;
using LiveCharts.Wpf;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for statisticsFoodPage.xaml
    /// </summary>
    public partial class StatisticsDrinkPage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;

        public Dictionary<string, int> CountList;

        private readonly ChartValues<int> Values;


        public StatisticsDrinkPage(BusinessModuleLocator businessModuleLocator)
        {
            // init data
            _businessModuleLocator = businessModuleLocator;
            Values = new ChartValues<int>();
            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "amount",
                    Values = Values
                }
            };
            Labels = new List<string>();
            Formatter = value => value.ToString();
            CountList = new Dictionary<string, int>();

            // init UI
            InitializeComponent();

            // Fill data to chart
            ChartDataFilling(false);
        }

        public SeriesCollection SeriesCollection { get; set; }
        public Func<decimal, string> Formatter { get; set; }
        public List<string> Labels { get; set; }

        private void ChartDataFilling(bool isfilter)
        {
            CountList.Clear();

            var orderDetailsWithTime = new List<OrderNoteDetail>();
            if (isfilter)
                orderDetailsWithTime = _businessModuleLocator.RepositoryLocator.OrderDetailsRepository.Get(x =>
                    x.OrderNote.OrderTime.Year == DpTimeFilter.SelectedDate.Value.Year
                    && x.OrderNote.OrderTime.Month == DpTimeFilter.SelectedDate.Value.Month).ToList();
            else
                orderDetailsWithTime = _businessModuleLocator.RepositoryLocator.OrderDetailsRepository.Get().ToList();


            // var td = from o in OrderList join pr in ProductList on o.ProductId equals pr.ProductId select o;
            var count = 0;
            foreach (var item in _businessModuleLocator.RepositoryLocator.ProductRepository.Get())
            {
                foreach (var item2 in orderDetailsWithTime.Where(o => o.ProductId.Equals(item.ProductId)))
                    count += item2.Quan;
                CountList.Add(item.Name, count);
                count = 0;
            }


            Values.Clear();
            Labels.Clear();
            foreach (var item in CountList)
            {
                Values.Add(item.Value);
                Labels.Add(item.Key);
            }

            DataContext = this;
        }

        private void DpTimeFilter_OnSelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            ChartDataFilling(true);
        }
    }
}