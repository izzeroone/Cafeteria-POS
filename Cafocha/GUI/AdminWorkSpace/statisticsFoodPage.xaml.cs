using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using Cafocha.Entities;
using Cafocha.Repository.DAL;
using LiveCharts;
using LiveCharts.Wpf;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    /// Interaction logic for statisticsFoodPage.xaml
    /// </summary>
    public partial class statisticsFoodPage : Page
    {
        AdminwsOfCloudPOS _unitofwork;

        private ChartValues<int> Values;
        public SeriesCollection SeriesCollection { get; set; }
        
        public Dictionary<string, int> CountList;
        public Func<decimal, string> Formatter { get; set; }
        public List<string> Labels { get; set; }
       

        
        public statisticsFoodPage(AdminwsOfCloudPOS unitofwork)
        {
            // init data
            _unitofwork = unitofwork;
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

        private void ChartDataFilling(bool isfilter)
        {
            CountList.Clear();

            List<OrderNoteDetail> orderDetailsWithTime = new List<OrderNoteDetail>();
            if (isfilter)
            {
                orderDetailsWithTime = _unitofwork.OrderNoteDetailsRepository.Get(x =>
                    x.OrderNote.Ordertime.Year == DpTimeFilter.SelectedDate.Value.Year
                    && x.OrderNote.Ordertime.Month == DpTimeFilter.SelectedDate.Value.Month).ToList();
            }
            else
            {
                orderDetailsWithTime = _unitofwork.OrderNoteDetailsRepository.Get().ToList();
            }
            

            // var td = from o in OrderList join pr in ProductList on o.ProductId equals pr.ProductId select o;
            int count = 0;
            foreach (var item in _unitofwork.ProductRepository.Get())
            {
                foreach (var item2 in orderDetailsWithTime.Where(o => o.ProductId.Equals(item.ProductId)))
                {
                    count += item2.Quan;
                }
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
