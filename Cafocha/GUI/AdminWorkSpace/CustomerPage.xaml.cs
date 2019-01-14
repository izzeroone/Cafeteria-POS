using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Cafocha.BusinessContext;
using Cafocha.Entities;

namespace Cafocha.GUI.AdminWorkSpace
{
    /// <summary>
    ///     Interaction logic for CustomerPage.xaml
    /// </summary>
    public partial class CustomerPage : Page
    {
        private readonly BusinessModuleLocator _businessModuleLocator;
        private CustomerAddOrUpdateDialog _cusAddOrUpdate;
        private List<Customer> allcus;
        private Customer ctm;

        public CustomerPage(BusinessModuleLocator businessModuleLocator)
        {
            _businessModuleLocator = businessModuleLocator;
            InitializeComponent();
            allcus = _businessModuleLocator.CustomerModule.getAllCustomer().ToList();
            lvDataCustomer.ItemsSource = allcus;
        }

        private void lvDataCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ctm = lvDataCustomer.SelectedItem as Customer;
            if (ctm == null)
            {
                txtID.Text = "";
                txtName.Text = "";
                txtMail.Text = "";
                txtPhone.Text = "";
                txtDiscount.Text = "0";
                return;
            }

            txtID.Text = ctm.CusId;
            txtName.Text = ctm.Name;
            txtMail.Text = ctm.Email;
            txtPhone.Text = ctm.Phone;
            txtDiscount.Text = ctm.Discount.ToString();
        }

        private void bntAddnew_Click(object sender, RoutedEventArgs e)
        {
            _cusAddOrUpdate = new CustomerAddOrUpdateDialog(_businessModuleLocator, null);
            _cusAddOrUpdate.ShowDialog();

            allcus = _businessModuleLocator.CustomerModule.getAllCustomer().ToList();
            lvDataCustomer.ItemsSource = allcus;
            lvDataCustomer.UnselectAll();
            lvDataCustomer.Items.Refresh();
        }

        private void bntUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (lvDataCustomer.SelectedItem == null)
            {
                MessageBox.Show("Chọn khách hàng muốn cập nhật!");
                return;
            }

            _cusAddOrUpdate = new CustomerAddOrUpdateDialog(_businessModuleLocator, lvDataCustomer.SelectedItem as Customer);
            _cusAddOrUpdate.ShowDialog();
            lvDataCustomer.UnselectAll();
            lvDataCustomer.Items.Refresh();
        }

        private void bntDel_Click(object sender, RoutedEventArgs e)
        {
            if (lvDataCustomer.SelectedItem == null)
            {
                MessageBox.Show("Chọn khách hàng muốn xóa!");
                return;
            }

            var delCus = lvDataCustomer.SelectedItem as Customer;
            if (delCus != null)
            {
                var delMess = MessageBox.Show("Bạn có muốn xóa " + delCus.Name + "(" + delCus.CusId + ") khỏi danh sách?",
                    "Cảnh báo!?", MessageBoxButton.YesNo);
                if (delMess == MessageBoxResult.Yes)
                {
                    _businessModuleLocator.CustomerModule.deleteCustomer(delCus);
                    allcus = _businessModuleLocator.CustomerModule.getAllCustomer().ToList();
                    lvDataCustomer.ItemsSource = allcus;
                    lvDataCustomer.UnselectAll();
                    lvDataCustomer.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Chọn khách hàng muốn xóa!");
            }
        }
    }
}