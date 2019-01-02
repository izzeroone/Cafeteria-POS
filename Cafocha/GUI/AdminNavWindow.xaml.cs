using System.Windows;
using Cafocha.Entities;
using Cafocha.GUI.AdminWorkSpace;
using Cafocha.GUI.CafowareWorkSpace;

namespace Cafocha.GUI
{
    /// <summary>
    ///     Interaction logic for AdminNavWindow.xaml
    /// </summary>
    public partial class AdminNavWindow : Window
    {
        public AdminNavWindow()
        {
            InitializeComponent();

            var curAd = Application.Current.Properties["AdLogin"] as AdminRe;


            // Control layout depend on logging Admin
            if (curAd.AdRole == (int) AdminReRole.CafochaAd)
            {
//                stpAdpress.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (curAd.AdRole == (int) AdminReRole.CafowareAd) stpAsowel.Visibility = Visibility.Collapsed;
            }
        }


        private void GotoWareHouseWSButton_OnClick(object sender, RoutedEventArgs e)
        {
//            WareHouseWindow whWindow = new WareHouseWindow();
//            whWindow.Show();

            var cfWindows = new CafowareWindow();
            cfWindows.Show();
            Close();
        }

        private void GotoAdminWSButton_OnClick(object sender, RoutedEventArgs e)
        {
            var adminWindow = new AdminWindow();
            adminWindow.Show();

            Close();
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void GotoWareHouseIGButton_OnClick(object sender, RoutedEventArgs e)
        {
            var cfWindows = new CafowareWindow();
            cfWindows.Show();
        }
    }
}