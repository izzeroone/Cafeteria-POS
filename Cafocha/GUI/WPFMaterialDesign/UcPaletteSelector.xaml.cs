using System.Windows.Controls;

namespace Cafocha.GUI.WPFMaterialDesign
{
    /// <summary>
    /// Interaction logic for UcPaletteSelector.xaml
    /// </summary>
    public partial class UcPaletteSelector : UserControl
    {
        public UcPaletteSelector()
        {
            InitializeComponent();
            this.DataContext = new PaletteSelectorViewModel();
        }
    }
}
