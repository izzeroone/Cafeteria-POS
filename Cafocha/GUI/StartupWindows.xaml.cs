using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Cafocha.GUI
{
    /// <summary>
    ///     Interaction logic for StartupWindows.xaml
    /// </summary>
    public partial class StartupWindows : Window
    {
        public StartupWindows()
        {
            InitializeComponent();

            Task.Run(() =>
            {
                Thread.Sleep(4000);
                Dispatcher.Invoke(() =>
                {
                    var loginWindow = new LoginWindow();
                    loginWindow.Show();
                    Close();
                });
            });
        }
    }
}