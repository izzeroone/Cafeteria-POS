using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Cafocha.Entities;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    ///     Interaction logic for InputTheRestOrderInfoDialog.xaml
    /// </summary>
    public partial class InputTheRestOrderInfoDialog : Window
    {
        private string _payMethod;
        private readonly OrderNote currentOrder;

        public InputTheRestOrderInfoDialog(OrderNote currentOrder)
        {
            InitializeComponent();

            this.currentOrder = currentOrder;
            TbTotalPrice.Text = TbTotalPrice.Text + string.Format("{0:0.000}", currentOrder.TotalPrice) + " VND";
            _payMethod = "";
            IsSuccess = false;

            CboPaymentMethod.ItemsSource = new List<string>
            {
                "Cash",
                "Cheque",
                "Deferred",
                "International",
                "Credit",
                "OnAcount"
            };

            CboPaymentMethod.SelectedIndex = 0;
        }

        public bool IsSuccess { get; set; }

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var cboPayment = sender as ComboBox;
            _payMethod = cboPayment.SelectedValue.ToString();
        }

        public bool MyShowDialog()
        {
            ShowDialog();
            return IsSuccess;
        }

        private void DoPayment_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_payMethod))
            {
                MessageBox.Show("please choose Payment Method!");
                return;
            }


            if (_payMethod == PaymentMethod.Cash.ToString())
                currentOrder.paymentMethod = (int) PaymentMethod.Cash;
            else if (_payMethod == PaymentMethod.Cheque.ToString())
                currentOrder.paymentMethod = (int) PaymentMethod.Cheque;
            else if (_payMethod == PaymentMethod.Credit.ToString())
                currentOrder.paymentMethod = (int) PaymentMethod.Credit;
            else if (_payMethod == PaymentMethod.Deferred.ToString())
                currentOrder.paymentMethod = (int) PaymentMethod.Deferred;
            else if (_payMethod == PaymentMethod.International.ToString())
                currentOrder.paymentMethod = (int) PaymentMethod.International;
            else if (_payMethod == PaymentMethod.OnAcount.ToString())
                currentOrder.paymentMethod = (int) PaymentMethod.OnAcount;


            try
            {
                decimal cusPay;
                if (string.IsNullOrWhiteSpace(KbInput.InputValue))
                    cusPay = currentOrder.TotalPrice;
                else
                    cusPay = decimal.Parse(KbInput.InputValue);


                if (cusPay < currentOrder.TotalPrice)
                {
                    MessageBox.Show("All payment ground up to higher number!");
                    return;
                }

                currentOrder.CustomerPay = cusPay;
                currentOrder.PayBack = currentOrder.CustomerPay - currentOrder.TotalPrice;
                IsSuccess = true;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Incorrect input!");
            }
        }


        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}