using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Cafocha.BusinessContext;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.Entities;
using Microsoft.Win32;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for ProductUpdateDialog.xaml
    /// </summary>
    public partial class ProductUpdateDialog : Window
    {
        private readonly BusinessModuleLocator _businessModuleLocator;

        private readonly Product _currentProduct = new Product();
        private List<Stock> _igreList;
        private readonly List<ProductModule.PDTemp> _pdtList;
        private readonly List<ProductDetail> _proDe;

        private string browseImagePath = "";

        private bool iscboRaise;

        private bool isRaiseEvent;

        public bool isRaiseIngreShowEvent = false;

        private readonly string startupProjectPath =
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        public ProductUpdateDialog(BusinessModuleLocator businessModuleLocator, Product pro)
        {
            _businessModuleLocator = businessModuleLocator;
            _currentProduct = pro;
            _proDe = _businessModuleLocator.ProductModule.getAllProductDetails(_currentProduct.ProductId).ToList();
            _pdtList = _businessModuleLocator.ProductModule.PdtList;
            InitializeComponent();

            Loaded += ProductCreatorPage_Loaded;

            _pdtList.Clear();

            initComboBox();
            initDataCurrentProduct();
        }

        private void initDataCurrentProduct()
        {
            txtName.Text = _currentProduct.Name;
            txtInfo.Text = _currentProduct.Info;
            cboType.SelectedItem = _currentProduct.Type;
            txtImageName.Text = _currentProduct.ImageLink;
            txtDiscount.Text = _currentProduct.Discount.ToString();
            txtPrice.Text = string.Format("{0:0.000}", _currentProduct.Price);

            var ing = _businessModuleLocator.WarehouseModule.IngredientList;

            foreach (var pd in _proDe)
            {
                var curing = ing.FirstOrDefault(x => x.StoId.Equals(pd.IgdId));
                if (curing != null) _pdtList.Add(new ProductModule.PDTemp {ProDe = pd, Ingre = curing});
            }

            
            CalSuggestPrice();
        }

        private void ProductCreatorPage_Loaded(object sender, RoutedEventArgs e)
        {
            _igreList = _businessModuleLocator.WarehouseModule.IngredientList;
            
        }

        private void initComboBox()
        {
            iscboRaise = true;
            cboType.Items.Add(ProductType.Drink);
            cboType.Items.Add(ProductType.Topping);
            cboType.Items.Add(ProductType.Dessert);
            cboType.Items.Add(ProductType.Other);
            cboType.SelectedIndex = 0;

            iscboRaise = false;
        }

        private void LvAvaibleIngredient_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var lv = sender as ListView;
            var ingre = lv.SelectedItem as Stock;

            if (ingre == null) return;

            if (_pdtList.Count != 0)
            {
                var igre = _pdtList.Where(x => x.ProDe.IgdId.Equals(ingre.StoId)).FirstOrDefault();
                if (igre != null)
                {
                    MessageBox.Show("This Ingredient is already exist in Product Details List! Please choose another!");
                    return;
                }
            }

            var newPD = new ProductDetail
            {
                PdetailId = "",
                ProductId = _currentProduct.ProductId,
                IgdId = ingre.StoId,
                Quan = 0,
                UnitUse = ""
            };

            isRaiseEvent = true;
            //_currentProduct.ProductDetails.Add(newPD);
            _pdtList.Add(new ProductModule.PDTemp {ProDe = newPD, Ingre = ingre});
            isRaiseEvent = false;
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text)) e.Handled = !char.IsNumber(e.Text[0]);
        }


        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (_pdtList.Count() != 0)
                    foreach (var pd in _pdtList)
                        if (pd.ProDe.UnitUse.Equals("") || pd.ProDe.UnitUse == null || pd.ProDe.Quan < 1)
                        {
                            MessageBox.Show("Please check information of " + pd.Ingre.Name +
                                            " again! Something is not valid!");
                            return;
                        }

                //check name
                var name = txtName.Text.Trim();
                if (name.Length == 0)
                {
                    MessageBox.Show("Điền tên sản phẩm!");
                    txtName.Focus();
                    return;
                }

                //check info
                var info = txtInfo.Text.Trim();

                //check type
                var type = (int) cboType.SelectedItem;

                //check imagename
                var imgname = txtImageName.Text.Trim();
                if (imgname.Length == 0)
                {
                    MessageBox.Show("Chọn hình cho sản phẩm!");
                    btnLinkImg.Focus();
                    return;
                }

                //check discount
                //
                int discount = 0;
                if (string.IsNullOrEmpty(txtDiscount.Text.Trim())) ;

                else
                    discount = int.Parse(txtDiscount.Text.Trim());


                //check price
                decimal price = 0;
                if (string.IsNullOrEmpty(txtPrice.Text.Trim())) ;

                else
                    price = decimal.Parse(txtPrice.Text.Trim());

                _currentProduct.Name = name;
                _currentProduct.Info = info;
                _currentProduct.Type = type;
                _currentProduct.ImageLink = imgname;
                _currentProduct.Discount = discount;
                _currentProduct.Price = price;

                var destinationFile = startupProjectPath + "\\Images\\Products" + txtImageName.Text.Trim();
                try
                {
                    if (!string.IsNullOrEmpty(browseImagePath))
                    {
                        File.Delete(destinationFile);
                        File.Copy(browseImagePath, destinationFile);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                _businessModuleLocator.ProductModule.updateProduct(_currentProduct, _proDe, _pdtList);

                MessageBox.Show("Cập nhật sản phẩm " + _currentProduct.Name + "(" + _currentProduct.ProductId +
                                ") Thành công!");
                ClearAllData();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể cập nhật sản phẩm! Hãy kiểm tra và thực hiện lại");
            }
            Close();
        }

        private void btnLinkImg_Click(object sender, RoutedEventArgs e)
        {
            var browseFile = new OpenFileDialog();
            browseFile.DefaultExt = ".";
            browseFile.Filter = "All Image Files (*.jpg, *.jpeg)|*.jpg; *.jpeg"; // " | JPEG Files (*.jpeg)|*.jpeg |  JPG Files (*.jpg)|*.jpg";
            var result = browseFile.ShowDialog();

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
            cboType.SelectedIndex = 0;
            txtImageName.Text = "";
            txtDiscount.Text = "";
            txtPrice.Text = "";

            _pdtList.Clear();
            isRaiseEvent = false;
        }

        private void CalSuggestPrice()
        {
            decimal sugprice = 0;
            foreach (var pd in _pdtList) sugprice += (decimal) (pd.ProDe.Quan / 1000) * pd.Ingre.StandardPrice;

        }
    }
}