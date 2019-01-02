using System;
using System.Windows;
using System.Windows.Forms;
using Cafocha.BusinessContext;
using Cafocha.GUI.Helper.PrintHelper.Report;
using MessageBox = System.Windows.MessageBox;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for ReportOptionDialog.xaml
    /// </summary>
    public partial class ReportOptionDialog : Window
    {
        private static string folderPath = AppPath.ApplicationPath + "\\SerializedData";
        private readonly BusinessModuleLocator _businessModuleLocator;
        private readonly IListPdfReport _reportHelper;
        private DateTime endTime;
        private DateTime startTime;

        public ReportOptionDialog(IListPdfReport reportHelper, BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            _reportHelper = reportHelper;
            InitializeComponent();

            DpFrom.SelectedDate = DateTime.Now;
            DpTo.SelectedDate = DateTime.Now;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnOk_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reportHelper != null && DpFrom.SelectedDate.Value != null && DpTo.SelectedDate.Value != null)
                {
                    // generate report
                    if (ChbOverviewReport.IsChecked == true)
                        _reportHelper.CreatePdfReport(_businessModuleLocator.RepositoryLocator,
                            DpFrom.SelectedDate.Value.Date,
                            DpTo.SelectedDate.Value.Date, folderPath);
                    else if (ChbDetailsReport.IsChecked == true)
                        _reportHelper.CreateDetailsPdfReport(_businessModuleLocator.RepositoryLocator,
                            DpFrom.SelectedDate.Value.Date,
                            DpTo.SelectedDate.Value.Date, folderPath);
                    else
                        _reportHelper.CreateEntityPdfReport(_businessModuleLocator.RepositoryLocator,
                            DpFrom.SelectedDate.Value.Date,
                            DpTo.SelectedDate.Value.Date, folderPath);

                    MessageBox.Show("new report was generated, please check your folder (path):\n\n" + folderPath);

                    Close();
                }
                else
                {
                    MessageBox.Show("Please select the duration of time that you want to create Report!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Generate new report fail! Something went wrong.");
            }
        }

        private void BtnFastChoiceMonthRpt_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reportHelper != null && DpFrom.SelectedDate.Value != null && DpTo.SelectedDate.Value != null)
                {
                    // generate report
                    if (ChbOverviewReport.IsChecked == true)
                        _reportHelper.CreateMonthPdfReport(_businessModuleLocator.RepositoryLocator, folderPath);

                    MessageBox.Show("new report was generated, please check your folder (path):\n\n" + folderPath);

                    Close();
                }
                else
                {
                    MessageBox.Show("Please select the duration of time that you want to create Report!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Generate new report fail! Something went wrong.");
            }
        }

        private void BtnFastChoiceDayRpt_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reportHelper != null && DpFrom.SelectedDate.Value != null && DpTo.SelectedDate.Value != null)
                {
                    // generate report
                    if (ChbOverviewReport.IsChecked == true)
                        _reportHelper.CreateDayPdfReport(_businessModuleLocator.RepositoryLocator, folderPath);

                    MessageBox.Show("new report was generated, please check your folder (path):\n\n" + folderPath);

                    Close();
                }
                else
                {
                    MessageBox.Show("Please select the duration of time that you want to create Report!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Generate new report fail! Something went wrong.");
            }
        }

        private void BtnFastChoiceYearRpt_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reportHelper != null && DpFrom.SelectedDate.Value != null && DpTo.SelectedDate.Value != null)
                {
                    // generate report
                    if (ChbOverviewReport.IsChecked == true)
                        _reportHelper.CreateYearPdfReport(_businessModuleLocator.RepositoryLocator, folderPath);

                    MessageBox.Show("new report was generated, please check your folder (path):\n\n" + folderPath);

                    Close();
                }
                else
                {
                    MessageBox.Show("Please select the duration of time that you want to create Report!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Generate new report fail! Something went wrong.");
            }
        }


        /// <summary>
        ///     Select Directory to store Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK) folderPath = dialog.SelectedPath;
            }
        }
    }
}