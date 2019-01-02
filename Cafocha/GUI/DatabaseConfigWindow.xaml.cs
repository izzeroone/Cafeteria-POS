using System.Windows;
using Cafocha.GUI.BusinessModel;

namespace Cafocha.GUI
{
    /// <summary>
    ///     Interaction logic for DatabaseConfigWindow.xaml
    /// </summary>
    public partial class DatabaseConfigWindow : Window
    {
        public DatabaseConfigWindow()
        {
            InitializeComponent();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            var initialCatalog = txtInitialCatalog.Text.Trim();
            var source = txtDataSource.Text.Trim();
            var userId = txtUserId.Text.Trim();
            var pass = txtPassword.Password.Trim();

            if (initialCatalog.Length == 0 || source.Length == 0 || userId.Length == 0 || pass.Length == 0)
            {
                MessageBox.Show("Some input field is not correct! Please check!");
                return;
            }

            //App.Current.Properties["InitialCatalog"] = initialCatalog;
            //App.Current.Properties["Source"] = source;
            //App.Current.Properties["UserId"] = userId;
            //App.Current.Properties["Password"] = pass;
            //App.Current.Properties["IsConfigDB"] = "true";

            var connectionString = string.Format(
                "data source={0};initial catalog={1};user id={2};password={3};MultipleActiveResultSets=True;App=EntityFramework",
                source, initialCatalog, userId, pass);
            Application.Current.Properties["ConnectionString"] = connectionString;
            ReadWriteData.WriteDBConfig(connectionString);

            MessageBox.Show("Please reset the Application!");
            Close();
        }
    }
}