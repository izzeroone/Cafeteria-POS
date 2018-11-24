using System.Windows;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    /// Interaction logic for DeleteConfirmDialog.xaml
    /// </summary>
    public partial class DeleteConfirmDialog : Window
    {
        MaterialDesignThemes.Wpf.Chip _cUser;
        public bool done;
        private bool _isTable;
        private string reason;

        public DeleteConfirmDialog(MaterialDesignThemes.Wpf.Chip cUser, bool isTable)
        {
            _cUser = cUser;
            done = false;
            _isTable = isTable;
            reason = "";
            InitializeComponent();

            if(_isTable)
            {
                product.Visibility = Visibility.Collapsed;
            }
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if(!_isTable)
            {
                if (rdYes.IsChecked == false && rdNo.IsChecked == false)
                {
                    MessageBox.Show("Is it done? Please check one!");
                    return;
                }
                else if (rdYes.IsChecked == true)
                {
                    this.done = true;   
                }
                else if (rdNo.IsChecked == true)
                {
                    this.done = false;
                }
            }

            if (string.IsNullOrEmpty(txtReason.Text))
            {
                MessageBox.Show("Please write some reason why you delete it!");
                return;
            }
            else
            {
                reason = txtReason.Text;
            }

            this.DialogResult = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;

            this.Close();
        }
    }
}
