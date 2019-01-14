﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.Helper.PrintHelper.Report;
using Cafocha.Entities;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for SalaryPage.xaml
    /// </summary>
    public partial class SalaryPage : Page
    {
        private readonly AdminRe _admin;
        private readonly BusinessModuleLocator _businessModuleLocator;
        private IEnumerable<SalaryNote> SalList;
        private IEnumerable<WorkingHistory> WhList;

        public SalaryPage(BusinessModuleLocator businessModuleLocator, AdminRe curAdmin)
        {
            _businessModuleLocator = businessModuleLocator;
            _admin = curAdmin;
            InitializeComponent();


            Loaded += SalaryPage_Loaded;
        }

        private void SalaryPage_Loaded(object sender, RoutedEventArgs args)
        {
            SalList = _businessModuleLocator.RepositoryLocator.SalaryNoteRepository
                .Get(includeProperties: "Employee,WorkingHistories");
            WhList = _businessModuleLocator.RepositoryLocator.WorkingHistoryRepository
                .Get(includeProperties: "Employee");
            lvSalary.ItemsSource = SalList;
            lvWokingHistory.ItemsSource = WhList;
            initMonthYear();
        }

        private void initMonthYear()
        {
            var checkYear = 0;
            cboYear.Items.Clear();
            var year = DateTime.Now.Year;
            cboYear.Items.Add("--");
            cboYear.Items.Add(year);

            var SalList = _businessModuleLocator.RepositoryLocator.SalaryNoteRepository.Get();
            foreach (var s in SalList)
                if (s.ForYear != year)
                    if (checkYear != s.ForYear)
                    {
                        cboYear.Items.Add(s.ForYear);
                        checkYear = s.ForYear;
                    }

            cboMonth.Items.Clear();
            cboMonth.Items.Add("--");
            for (var i = 0; i < 12; i++) cboMonth.Items.Add(i + 1);

            cboYear.SelectedItem = "--";
            cboMonth.SelectedItem = "--";
            cboYear.SelectionChanged += cboYear_SelectionChanged;
            cboMonth.SelectionChanged += cboMonth_SelectionChanged;
        }

        private void lvData_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var sln = lvSalary.SelectedItem as SalaryNote;
            if (sln == null)
                return;
            lvWokingHistory.ItemsSource = WhList.Where(c => c.ResultSalary.Equals(sln.SnId));
            lvWokingHistory.Items.Refresh();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            var filter = SearchBox.Text.Trim();

            if (filter.Length != 0)
            {
                SalList = SalList.Where(x => Regex.IsMatch(x.Employee.Name, filter, RegexOptions.IgnoreCase));
                lvSalary.ItemsSource = SalList;
            }
            else
            {
                SalList = _businessModuleLocator.RepositoryLocator.SalaryNoteRepository
                    .Get(includeProperties: "Employee,WorkingHistories");
                lvSalary.ItemsSource = SalList;
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchBox_KeyDown(sender, null);
        }

        private void cboMonth_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var filter = SearchBox.Text.Trim();
            if (filter.Length != 0) SalList = SalList.Where(x => x.Employee.Name.Contains(filter));

            var cboM = sender as ComboBox;

            if (cboM.Items.Count == 0 || cboYear.Items.Count == 0 || cboM.SelectedItem == null ||
                cboYear.SelectedItem == null) return;

            var month = 0;
            var year = 0;

            try
            {
                month = (int) cboM.SelectedItem;
                year = (int) cboYear.SelectedItem;
            }
            catch (Exception ex)
            {
                if (cboM.SelectedItem.Equals("--"))
                {
                    if (cboYear.SelectedItem.Equals("--"))
                    {
                        lvSalary.ItemsSource = SalList;
                        return;
                    }

                    lvSalary.ItemsSource = SalList.Where(x => x.ForYear.Equals((int) cboYear.SelectedItem));
                    return;
                }

                if (cboYear.SelectedItem.Equals("--"))
                {
                    lvSalary.ItemsSource = SalList.Where(x => x.ForMonth.Equals((int) cboM.SelectedItem));
                    return;
                }
            }

            lvSalary.ItemsSource = SalList.Where(x =>
                x.ForMonth.Equals((int) cboM.SelectedItem) && x.ForYear.Equals((int) cboYear.SelectedItem));
        }

        private void cboYear_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var filter = SearchBox.Text.Trim();
            if (filter.Length != 0) SalList = SalList.Where(x => x.Employee.Name.Contains(filter));

            var cboY = sender as ComboBox;

            if (cboY.Items.Count == 0 || cboMonth.Items.Count == 0 || cboY.SelectedItem == null ||
                cboMonth.SelectedItem == null) return;

            var month = 0;
            var year = 0;

            try
            {
                year = (int) cboY.SelectedItem;
                month = (int) cboMonth.SelectedItem;
            }
            catch (Exception ex)
            {
                if (cboY.SelectedItem.Equals("--"))
                {
                    if (cboMonth.SelectedItem.Equals("--"))
                    {
                        lvSalary.ItemsSource = SalList;
                        return;
                    }

                    lvSalary.ItemsSource = SalList.Where(x => x.ForMonth.Equals((int) cboMonth.SelectedItem));
                    return;
                }

                if (cboMonth.SelectedItem.Equals("--"))
                {
                    lvSalary.ItemsSource = SalList.Where(x => x.ForYear.Equals((int) cboY.SelectedItem));
                    return;
                }
            }

            lvSalary.ItemsSource = SalList.Where(x =>
                x.ForMonth.Equals((int) cboMonth.SelectedItem) && x.ForYear.Equals((int) cboY.SelectedItem));
        }

        //public string ConvertFrom(string str)
        //{
        //    byte[] utf8Bytes = Encoding.UTF8.GetBytes(str);
        //    str = Encoding.UTF8.GetString(utf8Bytes);
        //    return str;
        //}

        private string DecodeFromUtf8(string utf8String)
        {
            // read the string as UTF-8 bytes.
            var encodedBytes = Encoding.UTF8.GetBytes(utf8String);

            // convert them into unicode bytes.
            var unicodeBytes = Encoding.Convert(Encoding.UTF8, Encoding.Unicode, encodedBytes);

            // builds the converted string.
            return Encoding.Unicode.GetString(encodedBytes);
        }

        private void BntPay_Click(object sender, RoutedEventArgs e)
        {
            var sln = lvSalary.SelectedItem as SalaryNote;
            if (sln == null)
            {
                MessageBox.Show("Bạn chưa chọn lương cho nhân viên");
                return;
            }
            if (sln.IsPaid == 1)
            {
                MessageBox.Show("Bạn đã thanh toán cho hóa đơn này");
                return;
            }
            var dialog = MessageBox.Show("Bạn có muốn thanh toán cho nhân viên này", "Thanh toán",
                MessageBoxButton.YesNo);

            if (dialog == MessageBoxResult.Yes)
            {
                _businessModuleLocator.EmployeeModule.paySalaryNote(sln);
            }

            refresh();
        }

        private void refresh()
        {
            SalList = _businessModuleLocator.RepositoryLocator.SalaryNoteRepository
                .Get(includeProperties: "Employee,WorkingHistories");
            WhList = _businessModuleLocator.RepositoryLocator.WorkingHistoryRepository
                .Get(includeProperties: "Employee");
            lvSalary.ItemsSource = SalList;
            lvSalary.Items.Refresh();
        }

    }
}