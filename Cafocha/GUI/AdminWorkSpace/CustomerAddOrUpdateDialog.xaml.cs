﻿using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for CustomerAddOrUpdateDialog.xaml
    /// </summary>
    public partial class CustomerAddOrUpdateDialog : Window
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        private readonly Customer _cus;

        public CustomerAddOrUpdateDialog(BusinessModuleLocator businessModuleLocator, Customer cus)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();

            if (cus != null) _cus = cus;
            initData();
        }

        private void initData()
        {
            if (_cus != null)
            {
                txtName.Text = _cus.Name;
                txtPhone.Text = _cus.Phone;
                txtMail.Text = _cus.Email;
                txtDiscount.Text = _cus.Discount.ToString();
            }
        }

        private void bntAddnew_Click(object sender, RoutedEventArgs e)
        {
            //check name
            var namee = txtName.Text.Trim();
            if (namee.Length == 0 || namee.Length > 50)
            {
                MessageBox.Show("Name is not valid!");
                txtName.Focus();
                return;
            }

            //check phone
            var phone = txtPhone.Text.Trim();
            if (phone.Length == 0 || phone.Length > 20)
            {
                MessageBox.Show("Phone is not valid!");
                txtPhone.Focus();
                return;
            }

            //check email
            var email = txtMail.Text.Trim();
            if (!Regex.IsMatch(email, "[\\w\\d]+[@][\\w]+[.][\\w]+"))
            {
                MessageBox.Show("Email is not valid!");
                txtMail.Focus();
                return;
            }

            //check discount
            var discount = int.Parse(txtDiscount.Text.Trim());
            if (discount < 0 || discount > 100)
            {
                MessageBox.Show("Discount value is not valid!");
                txtDiscount.Focus();
                return;
            }

            if (_cus == null) //insert
            {
                var checkcus = new Customer
                {
                    CusId = "",
                    Name = namee,
                    Email = email,
                    Phone = phone,
                    Discount = discount,
                    Deleted = 0
                };

                _businessModuleLocator.CustomerModule.insertCustomer(checkcus);

                MessageBox.Show("Insert " + checkcus.Name + "(" + checkcus.CusId + ") successful!");
                Close();
            }
            else //update
            {
                _cus.Name = namee;
                _cus.Email = email;
                _cus.Phone = phone;
                _cus.Discount = discount;
                _cus.Deleted = 0;

                _businessModuleLocator.CustomerModule.updateCustomer(_cus);

                MessageBox.Show("Update " + _cus.Name + "(" + _cus.CusId + ") successful!");
                Close();
            }
        }

        private void bntCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text)) e.Handled = !char.IsNumber(e.Text[0]);
        }
    }
}