using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Entities;
using Cafocha.Repository.DAL;
using Microsoft.Win32;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    /// Interaction logic for ProductCreatorPage.xaml
    /// </summary>
    public partial class ProductCreatorPage : Page
    {
        private BusinessModuleLocator _businessModuleLocator;
        List<Ingredient> _igreList;
        private List<ProductModule.PDTemp> _pdtList;

        string browseImagePath = "";
        string startupProjectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        Product _currentProduct = new Product();
        public ProductCreatorPage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            _pdtList = _businessModuleLocator.ProductModule.PdtList;
            InitializeComponent();

            this.Loaded += ProductCreatorPage_Loaded;

            _currentProduct = new Product();

            initComboBox();
        }

        public bool isRaiseIngreShowEvent = false;
        private void ProductCreatorPage_Loaded(object sender, RoutedEventArgs e)
        {
            _igreList = _businessModuleLocator.IngredientModule.getAllIngredients().ToList();
            lvAvaibleIngredient.ItemsSource = _igreList;

            _pdtList.Clear();
        }

        private void initComboBox()
        {
            iscboRaise = true;
            cboType.Items.Add(ProductType.Beverage);
            cboType.Items.Add(ProductType.Food);
            cboType.Items.Add(ProductType.Beer);
            cboType.Items.Add(ProductType.Wine);
            cboType.Items.Add(ProductType.Snack);
            cboType.Items.Add(ProductType.Other);
            cboType.Items.Add(ProductType.Coffee);
            cboType.Items.Add(ProductType.Cocktail);
            cboType.SelectedItem = ProductType.Beverage;

            cboStatus.Items.Add("Drink");
            cboStatus.Items.Add("Starter");
            cboStatus.Items.Add("Main");
            cboStatus.Items.Add("Dessert");
            cboStatus.SelectedItem = "Drink";
            iscboRaise = false;
        }

        private void LvAvaibleIngredient_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView lv = sender as ListView;
            var ingre = lv.SelectedItem as Ingredient;

            if (ingre == null)
            {
                return;
            }

            if (_pdtList.Count != 0)
            {
                var igre = _pdtList.Where(x => x.ProDe.IgdId.Equals(ingre.IgdId)).FirstOrDefault();
                if (igre != null)
                {
                    MessageBox.Show("This Ingredient is already exist in Product Details List! Please choose another!");
                    return;
                }
            }

            ProductDetail newPD = new ProductDetail
            {
                PdetailId = "",
                ProductId = "",
                IgdId = ingre.IgdId,
                Quan = 0,
                UnitUse = ""
            };

            isRaiseEvent = true;
            //_currentProduct.ProductDetails.Add(newPD);
            _pdtList.Add(new ProductModule.PDTemp { ProDe = newPD, Ingre = ingre });
            lvDetails.ItemsSource = _pdtList;
            lvDetails.Items.Refresh();
            isRaiseEvent = false;
        }

        private void BntDeleteItem_OnClick(object sender, RoutedEventArgs e)
        {
            DependencyObject dep = (DependencyObject)e.OriginalSource;
            while ((dep != null) && !(dep is ListViewItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            int index = lvDetails.ItemContainerGenerator.IndexFromContainer(dep);

            if (index < 0)
                return;

            isRaiseEvent = true;
            //_currentProduct.ProductDetails.Remove(PDTempData.pdtList[index].ProDe);
            _pdtList.RemoveAt(index);
            CalSuggestPrice();
            lvDetails.ItemsSource = _pdtList;
            lvDetails.Items.Refresh();
            isRaiseEvent = false;
        }

        bool isRaiseEvent = false;
        private void cboUnitUse_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isRaiseEvent)
            {
                ComboBox cbo = sender as ComboBox;
                if (cbo.SelectedItem.Equals(""))
                {
                    return;
                }

                DependencyObject dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is ListViewItem))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep == null)
                    return;

                int index = lvDetails.ItemContainerGenerator.IndexFromContainer(dep);
                if (index < 0)
                {
                    return;
                }

                isRaiseEvent = true;
                if (cboStatus.SelectedItem.Equals("Time"))
                {
                    _currentProduct.ProductDetails.ToList()[index].Quan = 1;
                    _pdtList[index].ProDe.Quan = 1;
                }

                //_currentProduct.ProductDetails.ToList()[index].UnitUse = cbo.SelectedItem.ToString();
                _pdtList[index].ProDe.UnitUse = cbo.SelectedItem.ToString();
                CalSuggestPrice();
                isRaiseEvent = false;
            }
        }

        private void txtQuantity_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!isRaiseEvent)
            {
                DependencyObject dep = (DependencyObject)e.OriginalSource;
                while ((dep != null) && !(dep is ListViewItem))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep == null)
                    return;

                int index = lvDetails.ItemContainerGenerator.IndexFromContainer(dep);
                if (index < 0)
                {
                    return;
                }

                if ((sender as TextBox).Text.Trim().Equals("") || (sender as TextBox).Text.Trim() == null)
                {
                    return;
                }

                isRaiseEvent = true;
                //_currentProduct.ProductDetails.ToList()[index].Quan = int.Parse((sender as TextBox).Text.Trim());
                _pdtList[index].ProDe.Quan = int.Parse((sender as TextBox).Text.Trim());
                CalSuggestPrice();
                isRaiseEvent = false;
            }
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
                e.Handled = !Char.IsNumber(e.Text[0]);
            }
        }

        bool iscboRaise = false;
        private void cboType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!iscboRaise)
            {
                ComboBox ty = sender as ComboBox;
                if (ty.SelectedItem == null)
                {
                    return;
                }
                else if ((ty.SelectedValue).Equals(ProductType.Food))
                {
                    cboStatus.Items.Clear();
                    cboStatus.Items.Add("Starter");
                    cboStatus.Items.Add("Main");
                    cboStatus.Items.Add("Dessert");
                    cboStatus.SelectedItem = "Starter";
                }
                else if ((ty.SelectedValue).Equals(ProductType.Beverage)
                    || (ty.SelectedValue).Equals(ProductType.Beer)
                    || (ty.SelectedValue).Equals(ProductType.Cocktail)
                    || (ty.SelectedValue).Equals(ProductType.Coffee)
                    || (ty.SelectedValue).Equals(ProductType.Wine))
                {
                    cboStatus.Items.Clear();
                    cboStatus.Items.Add("Drink");
                    cboStatus.SelectedItem = "Drink";
                }
                else
                {
                    cboStatus.Items.Clear();
                    cboStatus.Items.Add("Drink");
                    cboStatus.Items.Add("Starter");
                    cboStatus.Items.Add("Main");
                    cboStatus.Items.Add("Dessert");
                    cboStatus.SelectedItem = "Drink";
                }
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pdtList.Count() != 0)
                {
                    foreach (var pd in _pdtList)
                    {
                        if (pd.ProDe.UnitUse.Equals("") || pd.ProDe.UnitUse == null || pd.ProDe.Quan < 1)
                        {
                            MessageBox.Show("Please check information of " + pd.Ingre.Name + " again! Something is not valid!");
                            return;
                        }
                    }
                }

                //check name
                string name = txtName.Text.Trim();
                if (name.Length == 0)
                {
                    MessageBox.Show("Name is not valid!");
                    txtName.Focus();
                    return;
                }

                //check info
                string info = txtInfo.Text.Trim();

                //check type
                int type = (int)cboType.SelectedItem;

                //check imagename
                string imgname = txtImageName.Text.Trim();
                if (imgname.Length == 0)
                {
                    MessageBox.Show("Please choose a image to continue!");
                    btnLinkImg.Focus();
                    return;
                }

                //check discount
                //

                //check standard status
                string stdstt = cboStatus.SelectedItem.ToString();

                //check price
                decimal price = 0;
                if (string.IsNullOrEmpty(txtPrice.Text.Trim()))
                {
                    if(string.IsNullOrEmpty(txtSusggestPrice.Text.Trim()))
                    {
                        MessageBox.Show("Price is not valid!");
                        txtPrice.Focus();
                        return;
                    }
                    else
                    {
                        price = decimal.Parse(txtSusggestPrice.Text.Trim());
                    }
                }
                else
                {
                    price = decimal.Parse(txtPrice.Text.Trim());
                }

                _currentProduct.ProductId = "";
                _currentProduct.Name = name;
                _currentProduct.Info = info;
                _currentProduct.Type = type;
                _currentProduct.ImageLink = imgname;
                _currentProduct.Discount = 0;
                _currentProduct.StdStats = stdstt;
                _currentProduct.Price = price;

                //C:\Program Files\ITComma\Asowel POS\Project POS\POS\POS
                string destinationFile = startupProjectPath + "\\Images\\Products\\" + txtImageName.Text.Trim();
                try
                {
                    //using (FileStream fs = new FileStream("D:\\tableImagePath.txt", FileMode.Create))
                    //{
                    //    using (StreamWriter sWriter = new StreamWriter(fs, Encoding.UTF8))
                    //    {
                    //        sWriter.WriteLine(startupProjectPath);
                    //    }
                    //}

                    if(File.Exists(destinationFile))
                    {
                        MessageBoxResult mess = MessageBox.Show("This product's image is already exist! Do you want to replace it?", "Warning!", MessageBoxButton.YesNo);
                        if(mess == MessageBoxResult.Yes)
                        {
                            File.Delete(destinationFile);
                        }
                        else
                        {
                            MessageBox.Show("Please choose another image for this product or rename new image and try again!");
                            return;
                        }
                    }
                    
                    File.Copy(browseImagePath, destinationFile);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                _businessModuleLocator.ProductModule.insertProduct(_currentProduct, _pdtList);

                MessageBox.Show("Add new product " + _currentProduct.Name + "(" + _currentProduct.ProductId + ") successful!");
                ClearAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnLinkImg_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog browseFile = new OpenFileDialog();
            browseFile.DefaultExt = ".";
            browseFile.Filter = "All Image Files (*.png, *.jpg, *.jpeg)|*.png; *.jpg; *.jpeg"; // " | JPEG Files (*.jpeg)|*.jpeg | PNG Files (*.png)|*.png | JPG Files (*.jpg)|*.jpg";
            Nullable<bool> result = browseFile.ShowDialog();

            if (result == true)
            {
                txtImageName.Text = browseFile.SafeFileName;
                browseImagePath = browseFile.FileName;
            }
        }

        private void btnClear_Click(object sender, RoutedEventArgs e)
        {
            ClearAllData();
        }

        private void ClearAllData()
        {
            isRaiseEvent = true;
            txtName.Text = "";
            txtInfo.Text = "";
            cboType.SelectedItem = ProductType.Beverage;
            txtImageName.Text = "";
            txtDiscount.Text = "";
            cboStatus.SelectedItem = "Drink";
            txtSusggestPrice.Text = "";
            txtPrice.Text = "";

            lvDetails.ItemsSource = new List<ProductDetail>();
            lvDetails.UnselectAll();
            lvDetails.Items.Refresh();
            lvAvaibleIngredient.UnselectAll();
            lvAvaibleIngredient.Items.Refresh();
            _pdtList.Clear();
            isRaiseEvent = false;
        }

        private void CalSuggestPrice()
        {
            decimal sugprice = 0;
            foreach (var pd in _pdtList)
            {
                sugprice += ((decimal)(pd.ProDe.Quan / 1000) * pd.Ingre.StandardPrice);
            }

            txtSusggestPrice.Text = sugprice + "";
        }

    }
}
