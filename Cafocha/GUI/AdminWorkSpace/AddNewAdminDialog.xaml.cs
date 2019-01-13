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
            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.NoResize;
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
                    MessageBox.Show("Tên tài khoản không hợp lệ!");
                    txtUsername.Focus();
                    return;
                }

                //check pass
                if (pass.Length == 0 || pass.Length > 50)
                {
                    MessageBox.Show("Mật khẩu không hợp lệ!");
                    txtPass.Focus();
                    return;
                }

                var passcon = txtCon.Password.Trim();
                if (!passcon.Equals(pass))
                {
                    MessageBox.Show("Mật khẩu không khớp!");
                    txtCon.Focus();
                    return;
                }


                //check name
                var name = txtName.Text.Trim();
                if (name.Length == 0 || name.Length > 50)
                {
                    MessageBox.Show("Tên không hợp lệ!");
                    txtName.Focus();
                    return;
                }

                //                var newemp = _unitofwork.EmployeeRepository.Get(x => x.Username.Equals(username)).ToList();
                var newemp = _businessModuleLocator.EmployeeModule.getEmployee(username);
                if (newemp == null)
                {
                    // Adding
                    var newAd = new AdminRe
                    {
                        AdId = "",
                        Username = username,
                        Pass = pass,
                        Name = name,
                        AdRole = 1
                    };

                    _businessModuleLocator.AdminModule.addAdmin(newAd);

                    MessageBox.Show("Thêm " + newAd.Name + "(" + newAd.AdId + ") thành công!");
                    Close();
                }
                else
                {
                    MessageBox.Show("Tên tài khoản đã tồn tại vui lòng đặt tên khác!");
                    txtUsername.Focus();
                    return;
                }
        }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Đã có lỗi xảy ra, vui lòng kiểm tra lại thông tin nhập hoặc kết nối CSDL!");
            }
}

        private void checkUser(string username)
        {
            var newemp = _businessModuleLocator.EmployeeModule.getEmployee(username);

            if (newemp != null)
            {
                MessageBox.Show("Tên tài khoản đã tồn tại, vui lòng nhập tên khác!");
                txtUsername.Focus();
                return;
            }
            return;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}