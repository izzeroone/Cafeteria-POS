using System.Windows.Controls;
using MaterialDesignThemes.Wpf;

namespace Cafocha.GUI.WPFMaterialDesign
{
    /// <summary>
    ///     Interaction logic for UcPaletteSelector.xaml
    /// </summary>
    public partial class UcPaletteSelector : UserControl
    {
        public UcPaletteSelector()
        {
            InitializeComponent();
            DataContext = new PaletteSelectorViewModel();
        }
    }
}