using System.Windows;
using System.Windows.Controls;

namespace Cafocha.GUI.Support
{
    /// <summary>
    ///     Interaction logic for FloatKeyboardControl.xaml
    /// </summary>
    public partial class FloatKeyboardControl : UserControl
    {
        private RoutedEventHandler _goClick;


        public FloatKeyboardControl()
        {
            InitializeComponent();
        }

        public string InputValue { get; set; }

        public event RoutedEventHandler GoClick
        {
            add => _goClick += value;
            remove => _goClick -= value;
        }


        private void TxtInputValue_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            InputValue = TxtInputValue.Text;
        }

        private void BtnDeleteInput_Click(object sender, RoutedEventArgs e)
        {
            TxtInputValue.Text = "";
            //InputValue = "";
        }

        private void BtnCharacterKey_Click(object sender, RoutedEventArgs e)
        {
            var clickButton = sender as Button;
            TxtInputValue.Text += clickButton.Content.ToString();
            //InputValue += clickButton.Content.ToString();
        }

        private void BtnBackSpace_Click(object sender, RoutedEventArgs e)
        {
            if (TxtInputValue.Text.Length == 0)
                return;

            TxtInputValue.Text = TxtInputValue.Text.Remove(TxtInputValue.Text.Length - 1);
            //InputValue = InputValue.Remove(InputValue.Length - 1);
        }


        private void BtnGo_OnClick(object sender, RoutedEventArgs e)
        {
            _goClick(sender, e);

            TxtInputValue.Text = "";
        }

        private void BtnShift_OnClick(object sender, RoutedEventArgs e)
        {
        }
    }
}