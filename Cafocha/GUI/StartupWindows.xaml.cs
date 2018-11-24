using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using POS;

namespace Cafocha.GUI
{
    /// <summary>
    /// Interaction logic for StartupWindows.xaml
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

                    
                    LoginWindow loginWindow = new LoginWindow();
                    loginWindow.Show();
                    this.Close();
                });

            });
        }
    }
}
