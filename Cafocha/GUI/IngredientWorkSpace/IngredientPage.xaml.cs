using System.Collections.Generic;
using System.Windows.Controls;
using Cafocha.BusinessContext;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.WareHouseWorkSpace
{
    /// <summary>
    /// Interaction logic for IngredientPage.xaml
    /// </summary>
    public partial class IngredientPage : Page
    {
        private BusinessModuleLocator _businessModuleLocator;
        

        public IngredientPage(BusinessModuleLocator businessModuleLocator, List<Ingredient> IngdList)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();

            lvItem.ItemsSource = IngdList;
        }
    }
}
