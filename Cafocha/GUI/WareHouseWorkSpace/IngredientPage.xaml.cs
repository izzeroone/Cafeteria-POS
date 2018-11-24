using System.Collections.Generic;
using System.Windows.Controls;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.WareHouseWorkSpace
{
    /// <summary>
    /// Interaction logic for IngredientPage.xaml
    /// </summary>
    public partial class IngredientPage : Page
    {
        private AdminwsOfCloudPOS _unitofwork;
        

        public IngredientPage(AdminwsOfCloudPOS unitofwork, List<Ingredient> IngdList)
        {
            _unitofwork = unitofwork;
            InitializeComponent();

            lvItem.ItemsSource = IngdList;
        }
    }
}
