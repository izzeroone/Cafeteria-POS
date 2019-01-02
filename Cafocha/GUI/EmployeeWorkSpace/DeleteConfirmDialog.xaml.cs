using System.Windows;
using MaterialDesignThemes.Wpf;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for DeleteConfirmDialog.xaml
    /// </summary>
    public partial class DeleteConfirmDialog : Window
    {
        private Chip _cUser;
        private readonly bool _isTable;
        public bool done;
        private string reason;

        public DeleteConfirmDialog(Chip cUser, bool isTable)
        {
            _cUser = cUser;
            done = false;
            _isTable = isTable;
            reason = "";
            InitializeComponent();

            if (_isTable) product.Visibility = Visibility.Collapsed;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (!_isTable)
            {
                if (rdYes.IsChecked == false && rdNo.IsChecked == false)
                {
                    MessageBox.Show("Is it done? Please check one!");
                    return;
                }

                if (rdYes.IsChecked == true)
                    done = true;
                else if (rdNo.IsChecked == true) done = false;
            }

            if (string.IsNullOrEmpty(txtReason.Text))
            {
                MessageBox.Show("Please write some reason why you delete it!");
                return;
            }

            reason = txtReason.Text;

            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;

            Close();
        }
    }
}