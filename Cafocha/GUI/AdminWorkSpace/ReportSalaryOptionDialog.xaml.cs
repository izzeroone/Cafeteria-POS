using System;
using System.Windows;
using Cafocha.BusinessContext;
using Cafocha.GUI.Helper.PrintHelper.Report;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    /// Interaction logic for ReportSalaryOptionDialog.xaml
    /// </summary>
    public partial class ReportSalaryOptionDialog : Window
    {
        private DateTime startTime;
        private DateTime endTime;
        private IListPdfReport _reportHelper;
        private BusinessModuleLocator _businessModuleLocator;
        private static string folderPath = AppPath.ApplicationPath + "\\SerializedData";


        public ReportSalaryOptionDialog(IListPdfReport reportHelper, BusinessModuleLocator businessModuleLocator)
        {
            InitializeComponent();

            _businessModuleLocator = businessModuleLocator;
            _reportHelper = reportHelper;

            DpFrom.SelectedDate = DateTime.Now;
            DpTo.SelectedDate = DateTime.Now;
        }



        private void BtnOk_OnClickClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reportHelper != null && DpFrom.SelectedDate.Value != null && DpTo.SelectedDate.Value != null)
                {
                    // generate report
                    if (ChbOverviewReport.IsChecked == true)
                    {
                        _reportHelper.CreatePdfReport(_businessModuleLocator.RepositoryLocator, DpFrom.SelectedDate.Value.Date,
                            DpTo.SelectedDate.Value.Date, folderPath);
                    }
                    else if (ChbDetailsReport.IsChecked == true)
                    {
                        _reportHelper.CreateDetailsPdfReport(_businessModuleLocator.RepositoryLocator, DpFrom.SelectedDate.Value.Date,
                            DpTo.SelectedDate.Value.Date, folderPath);
                    }
                    else
                    {
                        _reportHelper.CreateEntityPdfReport(_businessModuleLocator.RepositoryLocator, DpFrom.SelectedDate.Value.Date,
                            DpTo.SelectedDate.Value.Date, folderPath);
                    }

                    MessageBox.Show("new report was generated, please check your folder (path):\n\n" + folderPath);

                    this.Close();
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

        private void BtnCancel_OnClickncel_OnClick(object sender, RoutedEventArgs e)
        {
            this.Close();
        }


        private void BtnFastChoiceMonthRpt_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_reportHelper != null && DpFrom.SelectedDate.Value != null && DpTo.SelectedDate.Value != null)
                {
                    // generate report
                    if (ChbOverviewReport.IsChecked == true)
                    {
                        _reportHelper.CreateMonthPdfReport(_businessModuleLocator.RepositoryLocator, folderPath);
                    }

                    MessageBox.Show("new report was generated, please check your folder (path):\n\n" + folderPath);

                    this.Close();
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
                    {
                        _reportHelper.CreateDayPdfReport(_businessModuleLocator.RepositoryLocator, folderPath);
                    }

                    MessageBox.Show("new report was generated, please check your folder (path):\n\n" + folderPath);

                    this.Close();
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
                    {
                        _reportHelper.CreateYearPdfReport(_businessModuleLocator.RepositoryLocator, folderPath);
                    }

                    MessageBox.Show("new report was generated, please check your folder (path):\n\n" + folderPath);

                    this.Close();
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
        /// Select Directory to store Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    folderPath = dialog.SelectedPath;
                }
            }
        }
    }
}
