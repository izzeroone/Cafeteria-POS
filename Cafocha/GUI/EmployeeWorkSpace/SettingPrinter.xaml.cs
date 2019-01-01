using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cafocha.GUI.BusinessModel;
using POS;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    /// Interaction logic for SettingPrinter.xaml
    /// </summary>
    public partial class SettingPrinter : Page
    {
        private bool isLoading;

        public SettingPrinter()
        {
            InitializeComponent();

            isLoading = true;
            this.Loaded += SettingTableSize_Loaded;
            
        }

        private void SettingTableSize_Loaded(object sender, RoutedEventArgs e)
        {
//            txtWidth.Text = ReadWriteData.readTableSize()[0];
//            txtHeight.Text = ReadWriteData.readTableSize()[1];

            string[] result = ReadWriteData.ReadPrinterSetting();
            if (result != null)
            {
                txtReceptionPrinter.Text = result[0];
                txtKitPrinter.Text = result[1];
                txtBarPrinter.Text = result[2];

                if (int.Parse(result[3]) == 1)
                    chbShowReviewWin.IsChecked = true;
                else
                    chbShowReviewWin.IsChecked = false;
            }
            else
            {
                txtReceptionPrinter.Text = "";
                txtKitPrinter.Text = "";
                txtBarPrinter.Text = "";
                chbShowReviewWin.IsChecked = true;
            }

            isLoading = false;
        }

        private void CheckNumber_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
                e.Handled = !Char.IsNumber(e.Text[0]);
            }
        }


        private void BtnPrinterApply_OnClick(object sender, RoutedEventArgs e)
        {
            if (txtKitPrinter.Text.Trim().Length == 0 || txtBarPrinter.Text.Trim().Length == 0 || txtReceptionPrinter.Text.Trim().Length == 0)
            {
                MessageBox.Show("Please input all Printer Name that required here!");
                return;
            }

            if(chbShowReviewWin.IsChecked == true)
                ReadWriteData.WritePrinterSetting(txtReceptionPrinter.Text + "," + txtKitPrinter.Text + "," + txtBarPrinter.Text + "," + "1");
            else
                ReadWriteData.WritePrinterSetting(txtReceptionPrinter.Text + "," + txtKitPrinter.Text + "," + txtBarPrinter.Text + "," + "0");

            btnPrinterApply.Background = Brushes.Orange;
        }

        private void BtnPrinterCancel_OnClick(object sender, RoutedEventArgs e)
        {
            string[] result = ReadWriteData.ReadPrinterSetting();
            if (result != null)
            {
                txtReceptionPrinter.Text = result[0];
                txtKitPrinter.Text = result[1];
                txtBarPrinter.Text = result[2];

                if (int.Parse(result[3]) == 1)
                    chbShowReviewWin.IsChecked = true;
                else
                    chbShowReviewWin.IsChecked = false;
            }
            else
            {
                txtReceptionPrinter.Text = "";
                txtKitPrinter.Text = "";
                txtBarPrinter.Text = "";
                chbShowReviewWin.IsChecked = true;
            }

            btnPrinterApply.Background = Brushes.Orange;
        }

        private void TxtReceptionPrinter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isLoading)
                btnPrinterApply.Background = Brushes.Red;
        }

        private void TxtKitPrinter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isLoading)
                btnPrinterApply.Background = Brushes.Red;
        }

        private void TxtBarPrinter_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isLoading)
                btnPrinterApply.Background = Brushes.Red;
        }

        private void ChbShowReviewWin_OnChecked(object sender, RoutedEventArgs e)
        {
            if (!isLoading)
                btnPrinterApply.Background = Brushes.Red;
        }

        private void ChbShowReviewWin_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (!isLoading)
                btnPrinterApply.Background = Brushes.Red;
        }
    }
}
