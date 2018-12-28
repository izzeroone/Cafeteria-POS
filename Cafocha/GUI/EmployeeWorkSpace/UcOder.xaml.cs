using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.Entities;
using Cafocha.GUI.Helper.PrintHelper;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.EmployeeWorkSpace
{
    /// <summary>
    /// Interaction logic for UcOder.xaml
    /// </summary>
    public partial class UcOder : UserControl
    {
        private EmployeewsOfLocalPOS _unitofwork;
        private OrderModule _orderModule;
        private Employee currentEmp;


        public UcOder()
        {
            InitializeComponent();

            this.Loaded += UcOder_Loaded;
            this.Unloaded += UcOder_Unloaded;
        }



        private bool isUcOrderFormLoading;
        private void UcOder_Loaded(object sender, RoutedEventArgs e)
        {
            isUcOrderFormLoading = true;
            _unitofwork = ((MainWindow)Window.GetWindow(this))._unitofwork;
            _orderModule = ((MainWindow)Window.GetWindow(this))._orderModule;
            EmpLoginList currentEmpList = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);


  

            InitCus_raiseEvent = true;
            initStatus_RaiseEvent = true;
            txtDay.Text = "";
            txtTotal.Text = "";
            wp.Children.Clear();
            lvData.ItemsSource = new List<OrderModule.OrderDetails_Product_Joiner>();
           

         
            if (currentEmpList != null)
            {
                currentEmp = currentEmpList.Emp;

                if (currentEmp != null)
                {
                    //All in one so everything is 
                    this.bntPay.IsEnabled = true;
                    //                    if (currentEmp.EmpRole == (int)EmployeeRole.Cashier)
                    //                    {
                    //                        this.bntPay.IsEnabled = true;
                    //                    }
                    //                    else
                    //                    {
                    //                        this.bntPay.IsEnabled = false;
                    //                    }
                }
            }

            LoadCustomerOwner();
            txtTotal.Text = "";
            txtTotal.IsEnabled = true;
            isUcOrderFormLoading = false;
        }

        private void UcOder_Unloaded(object sender, RoutedEventArgs e)
        {
            //((MainWindow)Window.GetWindow(this)).currentChair = null;
        }


        /// <summary>
        /// show all orderdetails in the current checked chair.
        /// allow to modify these orderdetails
        /// </summary>
        public void RefreshControl(EmployeewsOfLocalPOS unitofwork)
        {
            try
            {
                initStatus_RaiseEvent = true;
                // binding
                lvData.ItemsSource = _orderModule.getOrderDetailsDisplay(); ;
                loadTotalPrice();

                initStatus_RaiseEvent = false;
            }
            catch (Exception ex)
            {
                MainWindow.AppLog.Error(ex);
            }
        }

        /// <summary>
        /// show all orderdetails of all chairs in the table.
        /// but not allow to modify these orderdetails
        /// </summary>
        public void RefreshControlAllChair()
        {
            // binding
            lvData.ItemsSource = _orderModule.getOrderDetailsDisplay();
            if (!isUnCheckChair)
            {
                loadTotalPrice();
            }
        }

        private void LoadCustomerOwner()
        {
            cboCustomers.ItemsSource = _unitofwork.CustomerRepository.Get();
            cboCustomers.SelectedValuePath = "CusId";
            cboCustomers.DisplayMemberPath = "Name";
            cboCustomers.MouseEnter += (sender, args) =>
            {
                cboCustomers.Background.Opacity = 100;
            };
            cboCustomers.MouseLeave += (sender, args) =>
            {
                cboCustomers.Background.Opacity = 0;
            };

            if (_orderModule.OrderTemp.CusId != null)
            {
                InitCus_raiseEvent = true;
                cboCustomers.SelectedValue = _orderModule.OrderTemp.CusId;
            }
            InitCus_raiseEvent = false;
        }

        private bool InitCus_raiseEvent = false;
        private void CboCustomers_SeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!InitCus_raiseEvent)
            {
                if (App.Current.Properties["CurrentEmpWorking"] == null)
                {
                    MessageBox.Show("No employee on working! Please try again!");
                    return;
                }

                //((MainWindow) Window.GetWindow(this)).currentTable.TableOrder.CusId = (string) ((sender as ComboBox).SelectedItem as Customer).CusId;
                _orderModule.OrderTemp.CusId =
                    (string)(sender as ComboBox).SelectedValue;
                _orderModule.OrderTemp.Discount = _unitofwork.CustomerRepository.Get(x => x.CusId.Equals(_orderModule.OrderTemp.CusId))
                    .FirstOrDefault().Discount;
                loadTotalPrice();
                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _orderModule.OrderTemp);
            }
        }

        private void NumberOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Text))
            {
                e.Handled = !Char.IsNumber(e.Text[0]);
            }
        }


        bool isUnCheckChair = false;
       

        /// <summary>
        /// inner class that use to store the joined data from
        /// orderdetails entities and product entities
        /// </summary>
      

        public void loadTotalPrice()
        {
            _orderModule.loadTotalPrice();
             txtTotal.Text = string.Format("{0:0.000}", _orderModule.OrderTemp.TotalPrice);
        }


        bool initStatus_RaiseEvent = false;
        private void cboStatus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!initStatus_RaiseEvent)
            {
                if (App.Current.Properties["CurrentEmpWorking"] == null)
                {
                    MessageBox.Show("No employee on working! Please try again!");
                    return;
                }


                DependencyObject dep = (DependencyObject)e.OriginalSource;

                while ((dep != null) && !(dep is ListViewItem))
                {
                    dep = VisualTreeHelper.GetParent(dep);
                }

                if (dep == null)
                    return;

                int index = lvData.ItemContainerGenerator.IndexFromContainer(dep);
                if (index < 0)
                {
                    return;
                }

                _orderModule.updateOrderDetail(index, (e.OriginalSource as ComboBox).SelectedItem.ToString());
                RefreshControl(_unitofwork);

                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _orderModule.OrderTemp);
            }
        }

        //ToDo: Set the WareHouse's contain back when the order didn't call any more
        private void bntDelete_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                MessageBox.Show("No employee on working! Please try again!");
                return;
            }

            int i = 0;
            foreach (ToggleButton btn in wp.Children)
            {
                if (btn.IsChecked == false)
                {
                    i++;
                }
            }
            if (i == 0)
            {
                MessageBox.Show("Choose exactly which chair you want to decrease food's quantity!");
                return;
            }

            bool isDone = false;

            //TODO: check whether printer or not
            if (true)
            {
                MessageBoxResult mess = MessageBox.Show("Invoice of this table is already printed! You can not edit this table! You must have higher permission for this action? Do you want to continue?", "Warning!", MessageBoxButton.YesNo);
                if (mess == MessageBoxResult.Yes)
                {
                    if (App.Current.Properties["AdLogin"] != null)
                    {
                        DeleteConfirmDialog dcd = new DeleteConfirmDialog(((MainWindow)Window.GetWindow(this)).cUser, false);
                        if (dcd.ShowDialog() == false)
                        {
                            return;
                        }
                        isDone = dcd.done;
                        // update employee ID that effect to the OrderNote
                        checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _orderModule.OrderTemp);
                    }
                    else
                    {
                        PermissionRequired pr = new PermissionRequired(_unitofwork, ((MainWindow)Window.GetWindow(this)).cUser, true, false);
                        if (pr.ShowDialog() == false)
                        {
                            return;
                        }
                    }
                }
                else
                {
                    return;
                }
            }

            DependencyObject dep = (DependencyObject)e.OriginalSource;
            OrderDetailsTemp o = new OrderDetailsTemp();
            int index;

            foreach (ToggleButton btn in wp.Children)
            {
                if (btn.IsChecked == true)
                {
                    //delete chair order note

                    while ((dep != null) && !(dep is ListViewItem))
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }

                    if (dep == null)
                        return;

                    index = lvData.ItemContainerGenerator.IndexFromContainer(dep);

                    _orderModule.deleteOrderDetail(index, isDone);

                    RefreshControl(_unitofwork);
                    checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _orderModule.OrderTemp);
                    break;
                }
            }

            ClearTheTable();
//            if (orderTempDetails.Count == 0)
//            {
//                
//            }
        }

        private void bntEdit_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                MessageBox.Show("No employee on working! Please try again!");
                return;
            }

            DependencyObject dep = (DependencyObject)e.OriginalSource;

            while ((dep != null) && !(dep is ListViewItem))
            {
                dep = VisualTreeHelper.GetParent(dep);
            }

            if (dep == null)
                return;

            int index = lvData.ItemContainerGenerator.IndexFromContainer(dep);



            InputNote inputnote = new InputNote(_orderModule.OrderTemp.OrderDetailsTemps.ElementAt(index).Note);

            if (inputnote.ShowDialog() == true)
            {
                _orderModule.updateOrderNote(index, inputnote.Note.ToLower());
            }


            RefreshControl(_unitofwork);
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _orderModule.OrderTemp);
        }



        private void bntPay_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                MessageBox.Show("No employee on working! Please try again!");
                return;
            }


            List<OrderDetailsTemp> newOrderDetails = new List<OrderDetailsTemp>();


            // input the rest data
            OrderNote newOrder = new OrderNote();
            newOrder.TotalPrice = _orderModule.OrderTemp.TotalPrice;
            InputTheRestOrderInfoDialog inputTheRest = new InputTheRestOrderInfoDialog(newOrder);
            if (!inputTheRest.MyShowDialog())
            {
                return;
            }



            // convert data and save to database
            if (_orderModule.ConvertTableToOrder(newOrder))
            {
                _orderModule.saveOrderToDB(newOrder);
            }
            else
            {
                return;
            }

            // printing
            var printer = new DoPrintHelper(_unitofwork, DoPrintHelper.Receipt_Printing, newOrder);
            printer.DoPrint();

            // clean the old table data
            ClearTheTable();
            
        }

        private void BntPrint_OnClick(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                MessageBox.Show("No employee on working! Please try again!");
                return;
            }

            // input the rest data
            OrderNote newOrder = new OrderNote();
            newOrder.TotalPrice = _orderModule.OrderTemp.TotalPrice;
            _orderModule.ConvertTableToOrder(newOrder);
 
            // printing
            var printer = new DoPrintHelper(_unitofwork, DoPrintHelper.TempReceipt_Printing, newOrder );
            printer.DoPrint();


            // update employee ID that effect to the OrderNote
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _orderModule.OrderTemp);
        }

        //ToDo: Set the contain back when the order didn't call any more
        private void BntDelete_OnClick(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                MessageBox.Show("No employee on working! Please try again!");
                return;
            }

            //TODO: FIX whether print or not
            if (true)
            {
                if (App.Current.Properties["AdLogin"] == null)
                {
                    MessageBoxResult mess = MessageBox.Show("This table is already printed! You must have higher permission for this action? Do you want to continue?", "Warning!", MessageBoxButton.YesNo);
                    if (mess == MessageBoxResult.Yes)
                    {
                        PermissionRequired pr = new PermissionRequired(_unitofwork, ((MainWindow)Window.GetWindow(this)).cUser, true, true);
                        pr.ShowDialog();

                        if (App.Current.Properties["AdLogin"] != null)
                        {
                            ClearTheTable_ForDelete();

                            // update employee ID that effect to the OrderNote
                            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _orderModule.OrderTemp);
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    ClearTheTable_ForDelete();

                    // update employee ID that effect to the OrderNote
                    checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _orderModule.OrderTemp);
                }
            }
            else
            {
                ClearTheTable_ForDelete();

                // update employee ID that effect to the OrderNote
                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _orderModule.OrderTemp);
            }
        }


        /// <summary>
        /// Migrate all Order info in current Table to OrderNote object that will be insert to Database
        /// </summary>
        /// <param name="newOrder"></param>



        bool isClearingTable = false;
        /// <summary>
        /// Clean the whole Order info in  current Table
        /// </summary>
        private void ClearTheTable()
        {
            isClearingTable = true;
            _orderModule.clearOrder();

//        TODO: Reset is printed or not    
//            curTable.IsOrdered = 0;
//            curTable.IsPrinted = 0;

            LoadCustomerOwner();
            RefreshControlAllChair();

           //TODO: clear order or print
//            if (cur != null)
//            {
//                cur.IsOrdered = 0;
//                cur.IsPrinted = 0;
//            }

            isClearingTable = false;

            ((MainWindow)Window.GetWindow(this)).bntEntry.IsEnabled = true;
        }

        bool isClearingTalbe_ForDelete = false;
        private void ClearTheTable_ForDelete()
        {
            isClearingTalbe_ForDelete = true;
            _orderModule.clearOrder();

            LoadCustomerOwner();
            RefreshControlAllChair();

            //TODO: clear order or print
            //            if (cur != null)
            //            {
            //                cur.IsOrdered = 0;
            //                cur.IsPrinted = 0;
            //            }

            isClearingTalbe_ForDelete = false;


            ((MainWindow)Window.GetWindow(this)).bntEntry.IsEnabled = true;
        }


        /// <summary>
        /// Give Back the Ingredient contain when delete Product from Order
        /// </summary>
        /// <param name="orderDetails">The OrderDetails that contain a give back Product</param>
        /// <param name="productQuan">give back product quantity</param>


        private void checkWorkingAction(EmpLoginList currentEmp, OrderTemp ordertempcurrenttable)
        {
            if (currentEmp.Emp.EmpId.Equals(ordertempcurrenttable.EmpId))
            {
                return;
            }

            if (ordertempcurrenttable.SubEmpId != null)
            {
                string[] subemplist = ordertempcurrenttable.SubEmpId.Split(',');

                for (int i = 0; i < subemplist.Count(); i++)
                {
                    if (subemplist[i].Equals(""))
                    {
                        continue;
                    }

                    if (currentEmp.Emp.EmpId.Equals(subemplist[i]))
                    {
                        return;
                    }
                }

                ordertempcurrenttable.SubEmpId += currentEmp.Emp.EmpId + ",";
                return;
            }

            ordertempcurrenttable.SubEmpId += currentEmp.Emp.EmpId + ",";

        }


        // INPUT PAX

        // INPUT TOTALPRICE
        private void txtTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_orderModule == null || _orderModule.OrderTemp == null || isUcOrderFormLoading || isClearingTable || isClearingTalbe_ForDelete)
                return;
            TextBox txtTotal = (sender as TextBox);
            if (string.IsNullOrEmpty(txtTotal.Text))
                _orderModule.OrderTemp.TotalPrice = 0;
            else
            {
                _orderModule.OrderTemp.TotalPrice = decimal.Parse(txtTotal.Text);
            }

            // update employee ID that effect to the OrderNote
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, _orderModule.OrderTemp);
            _unitofwork.Save();
        }

        // FORMAT TOTALPRICE TEXTBOX
        private void txtTotal_LostFocus(object sender, RoutedEventArgs e)
        {
            // When user leave the Total Price textbox, it will format their input value
            TextBox txtTotal = (sender as TextBox);

            decimal Total = _orderModule.OrderTemp.TotalPrice;
            decimal SaleValue = Total;
            decimal Svc = (Total * 5) / 100;
            decimal Vat = ((Total + (Total * 5) / 100) * 10) / 100;
            Total = (Total + (Total * 5) / 100) + (((Total + (Total * 5) / 100) * 10) / 100);

            _orderModule.OrderTemp.TotalPrice = Total;
            _orderModule.OrderTemp.TotalPriceNonDisc = (decimal)Math.Round(Total, 3);
            _orderModule.OrderTemp.Svc = Math.Round(Svc, 3);
            _orderModule.OrderTemp.Vat = Math.Round(Vat, 3);
            _orderModule.OrderTemp.SaleValue = Math.Round(SaleValue, 3);

            txtTotal.Text = string.Format("{0:0.000}", _orderModule.OrderTemp.TotalPrice);
        }
    }
}
