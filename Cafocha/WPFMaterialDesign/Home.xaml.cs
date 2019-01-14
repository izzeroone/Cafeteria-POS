using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Cafocha.GUI.WPFMaterialDesign
{
    /// <summary>
    ///     Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        public Home()
        {
            InitializeComponent();
        }

        private void GitHubButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/luuductrung1234/Pos_4_Asowell");
        }

        private void EmailButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("mailto://luuductrung@itcomma.com");
        }

        private void ExploreButton_OnClick(object sender, RoutedEventArgs e)
        {
            //play a demo video
        }

        private void ExitButton_OnClick(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void DetailsButton_OnClick(object sender, RoutedEventArgs e)
        {
            Process.Start("https://www.facebook.com/Asowel-Rooftop-Cafe-Dining-185441705356870/");
        }
    }
}