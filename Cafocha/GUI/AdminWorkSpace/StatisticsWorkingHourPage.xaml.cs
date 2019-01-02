using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cafocha.BusinessContext;
using Cafocha.Entities;
using LiveCharts;
using LiveCharts.Wpf;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for StatisticsWorkingHourPage.xaml
    /// </summary>
    public partial class StatisticsWorkingHourPage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        private readonly AdminRe curAdmin;
        private readonly ChartValues<double> Values;
        public Dictionary<string, double> WHList;


        public StatisticsWorkingHourPage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();

            curAdmin = Application.Current.Properties["AdLogin"] as AdminRe;

            Values = new ChartValues<double>();


            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Working Hour",
                    Values = Values
                }
            };
            Labels = new List<string>();
            Formatter = value => value.ToString();
            WHList = new Dictionary<string, double>();
            ChartDataFilling(false);
        }

        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> Formatter { get; set; }
        public List<string> Labels { get; set; }

        private void ChartDataFilling(bool isfilter)
        {
            WHList.Clear();

            var SalaryDetailsWithTime = new List<SalaryNote>();
            if (isfilter)
                SalaryDetailsWithTime = _businessModuleLocator.RepositoryLocator.SalaryNoteRepository.Get(x =>
                    x.ForYear == DpTimeFilter.SelectedDate.Value.Year
                    && x.ForMonth == DpTimeFilter.SelectedDate.Value.Month).ToList();
            else
                SalaryDetailsWithTime = _businessModuleLocator.RepositoryLocator.SalaryNoteRepository.Get().ToList();


            // var td = from o in OrderList join pr in ProductList on o.ProductId equals pr.ProductId select o;
            double count = 0;
            foreach (var item in _businessModuleLocator.RepositoryLocator.EmployeeRepository.Get(x =>
                x.Deleted == 0 && x.Manager.Equals(curAdmin.AdId)))
            {
                foreach (var item2 in SalaryDetailsWithTime.Where(o => o.EmpId.Equals(item.EmpId)))
                    count = item2.WorkHour;
                WHList.Add(item.Name, count);
                count = 0;
            }


            Values.Clear();
            Labels.Clear();
            foreach (var item in WHList)
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