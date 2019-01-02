using System.Windows;
using System.Windows.Input;

namespace POS.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for WareHouseStdContainDialog.xaml
    /// </summary>
    public partial class WareHouseStdContainDialog : Window
    {
        public WareHouseStdContainDialog()
        {
            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtStdContain.Text.Trim()))
            {
                MessageBox.Show("Standard Contain is not valid!");
                return;
            }

            var std = int.Parse(txtStdContain.Text.Trim());
            if (std < 1 || std > int.MaxValue)
            {
                MessageBox.Show("Standard Contain is not valid!");
                txtStdContain.Focus();
                return;
            }

            Application.Current.Properties["StdContain"] = std;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Properties["StdContain"] = null;
            Close();
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text)) e.Handled = !char.IsNumber(e.Text[0]);
        }
    }
}