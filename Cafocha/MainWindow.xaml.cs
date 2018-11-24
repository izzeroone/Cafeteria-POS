using System;
using System.Linq;
using System.Windows;
using Cafocha.Repository.DAL;

namespace Cafocha
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            var test = new EmployeewsOfLocalPOS().OrderDetailsTempRepository.Get(x => x.ChairId.Equals(4)).ToList();

            MessageBox.Show(test.ToString());

        }
    }
}
