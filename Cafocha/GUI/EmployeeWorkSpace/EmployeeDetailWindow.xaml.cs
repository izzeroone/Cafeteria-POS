using System;
using System.Collections.Generic;
using System.Windows;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for EmployeeDetail.xaml
    /// </summary>
    public partial class EmployeeDetail : Window
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        private Employee em;

        public EmployeeDetail(string UserName, BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            loadData(UserName);
            InitlsWh();
        }

        private void loadData(string UserName)
        {
            em = _businessModuleLocator.EmployeeModule.getEmployee(UserName);

            EmployeeInfo.DataContext = em;
        }

        private void InitlsWh()
        {
            ShowWHData.showWHList.Clear();
            var whListAll =
                _businessModuleLocator.EmployeeModule.getWorkingHistoryOfEmployee(em, DateTime.Now.Month,
                    DateTime.Now.Year);
            foreach (var i in whListAll)
            {
                var newWH = new ShowWH();
                newWH.WorkTime = formatString((i.EndTime - i.StartTime).Hours, (i.EndTime - i.StartTime).Minutes,
                    (i.EndTime - i.StartTime).Seconds);
                newWH.WorkDate = i.StartTime;

                var h = (i.EndTime - i.StartTime).Hours;
                var m = (i.EndTime - i.StartTime).Minutes;
                var s = (i.EndTime - i.StartTime).Seconds;

                newWH.TimePercent = (int) ((h + m / 60.0 + s / 3600.0) / 24.0 * 100);

                ShowWHData.showWHList.Add(newWH);
            }

            lsWH.ItemsSource = ShowWHData.showWHList;
        }

        private string formatString(int hours, int minutes, int seconds)
        {
            var st = "";
            string fH = "", fm = "", fs = "";
            fH = hours + "";
            fm = minutes + "";
            fs = seconds + "";
            if (hours < 10) fH = "0" + fH;
            if (minutes < 10) fm = "0" + fm;
            if (seconds < 10) fs = "0" + fs;
            st = fH + ":" + fm + ":" + fs;
            return st;
        }

        private void btn_Click(object sender, RoutedEventArgs e)
        {
            var empPass = new EmployeeChangePass(_businessModuleLocator, em);
            empPass.ShowDialog();
        }
    }

    public class ShowWH
    {
        public DateTime WorkDate { get; set; }

        public int TimePercent { get; set; }

        public string WorkTime { get; set; }
    }

    public class ShowWHData
    {
        public static List<ShowWH> showWHList = new List<ShowWH>();
    }
}