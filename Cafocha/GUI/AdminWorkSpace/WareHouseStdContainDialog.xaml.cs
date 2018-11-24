using System;
using System.Windows;
using System.Windows.Input;

namespace POS.AdminWorkSpace
{
    /// <summary>
    /// Interaction logic for WareHouseStdContainDialog.xaml
    /// </summary>
    public partial class WareHouseStdContainDialog : Window
    {
        public WareHouseStdContainDialog()
        {
            InitializeComponent();

            this.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(txtStdContain.Text.Trim()))
            {
                MessageBox.Show("Standard Contain is not valid!");
                return;
            }

            int std = int.Parse(txtStdContain.Text.Trim());
            if(std < 1 || std > int.MaxValue)
            {
                MessageBox.Show("Standard Contain is not valid!");
                txtStdContain.Focus();
                return;
            }
            App.Current.Properties["StdContain"] = std;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Properties["StdContain"] = null;
            this.Close();
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
                e.Handled = !Char.IsNumber(e.Text[0]);
            }
        }

    }
}
