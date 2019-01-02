using System;
using System.Collections.Generic;
using System.Windows;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for AddNewAdminDialog.xaml
    /// </summary>
    public partial class AddNewAdminDialog : Window
    {
        private AdminRe _admin;
        internal BusinessModuleLocator _businessModuleLocator;


        public AddNewAdminDialog(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();

            _admin = new AdminRe();
            initControlAdd();
            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.NoResize;
        }

        private void initControlAdd()
        {
            var roleList = new List<dynamic>
            {
                new {role = 1, roleDisplay = "Software Admin"},
                new {role = 2, roleDisplay = "Asowel Admin"},
                new {role = 3, roleDisplay = "AdPress Admin"},
                new {role = 4, roleDisplay = "Higher Admin"}
            };
            cboRole.ItemsSource = roleList;
            cboRole.SelectedValuePath = "role";
            cboRole.DisplayMemberPath = "roleDisplay";
        }


        private void BtnAdd_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var username = txtUsername.Text.Trim();
                var pass = txtPass.Password.Trim();

                //check username
                if (username.Length == 0 || username.Length > 50)
                {
                    MessageBox.Show("Username is not valid!");
                    txtUsername.Focus();
                    return;
                }

//                var newemp = _unitofwork.EmployeeRepository.Get(x => x.Username.Equals(username)).ToList();
                var newemp = _businessModuleLocator.EmployeeModule.getEmployee(username);

                if (newemp != null)
                {
                    MessageBox.Show("Username is already exist! Please try again!");
                    txtUsername.Focus();
                    return;
                }

                //check pass
                if (pass.Length == 0 || pass.Length > 50)
                {
                    MessageBox.Show("Password is not valid!");
                    txtPass.Focus();
                    return;
                }

                var passcon = txtCon.Password.Trim();
                if (!passcon.Equals(pass))
                {
                    MessageBox.Show("Confirm password is not match!");
                    txtCon.Focus();
                    return;
                }


                //check name
                var name = txtName.Text.Trim();
                if (name.Length == 0 || name.Length > 50)
                {
                    MessageBox.Show("Name is not valid!");
                    txtName.Focus();
                    return;
                }

                //check role
                var role = 0;

                if (cboRole.SelectedValue == null)
                {
                    MessageBox.Show("Role must be selected!");
                    return;
                }

                role = (int) cboRole.SelectedValue;

                if (role == 0)
                {
                    MessageBox.Show("Role must be selected!");
                    return;
                }


                // Adding


                var newAd = new AdminRe
                {
                    AdId = "",
                    Username = username,
                    Pass = pass,
                    Name = name,
                    AdRole = role
                };

                _businessModuleLocator.AdminModule.addAdmin(newAd);

                MessageBox.Show("Insert " + newAd.Name + "(" + newAd.AdId + ") successful!");
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Something went wrong. Can not add or update admin info. Please check the details again!");
            }
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}