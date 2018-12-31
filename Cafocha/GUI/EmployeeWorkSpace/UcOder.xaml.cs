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
using Cafocha.BusinessContext;
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
        private BusinessModuleLocator _businessModuleLocator;
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
            _businessModuleLocator = ((MainWindow)Window.GetWindow(this))._businessModuleLocator;
            EmpLoginList currentEmpList = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);


  

            InitCus_raiseEvent = true;
            initStatus_RaiseEvent = true;
            txtDay.Text = "";
            txtTotal.Text = "";
            wp.Children.Clear();
            lvData.ItemsSource = new List<TakingOrderModule.OrderDetails_Product_Joiner>();
           

         
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
        public void RefreshControl()
        {
            try
            {
                initStatus_RaiseEvent = true;
                // binding
                lvData.ItemsSource = _businessModuleLocator.TakingOrderModule.getOrderDetailsDisplay(); ;
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
            lvData.ItemsSource = _businessModuleLocator.TakingOrderModule.getOrderDetailsDisplay();
            if (!isUnCheckChair)
            {
                loadTotalPrice();
            }
        }

        private void LoadCustomerOwner()
        {
            cboCustomers.ItemsSource = _businessModuleLocator.CustomerModule.getAllCustomer();
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

            if (_businessModuleLocator.TakingOrderModule.OrderTemp.CusId != null)
            {
                InitCus_raiseEvent = true;
                cboCustomers.SelectedValue = _businessModuleLocator.TakingOrderModule.OrderTemp.CusId;
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


                _businessModuleLocator.TakingOrderModule.OrderTemp.CusId =
                    (string)(sender as ComboBox).SelectedValue;
                _businessModuleLocator.TakingOrderModule.OrderTemp.Discount = _businessModuleLocator.CustomerModule.getCustomer(_businessModuleLocator.TakingOrderModule.OrderTemp.CusId).Discount;
                loadTotalPrice();
                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);
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
            _businessModuleLocator.TakingOrderModule.loadTotalPrice();
             txtTotal.Text = string.Format("{0:0.000}", _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice);
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

                _businessModuleLocator.TakingOrderModule.updateOrderDetail(index, (e.OriginalSource as ComboBox).SelectedItem.ToString());
                RefreshControl();

                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);
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
                        checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);
                    }
                    else
                    {
                        PermissionRequired pr = new PermissionRequired(_businessModuleLocator, ((MainWindow)Window.GetWindow(this)).cUser, true, false);
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

                    _businessModuleLocator.TakingOrderModule.deleteOrderDetail(index, isDone);

                    RefreshControl();
                    checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);
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



            InputNote inputnote = new InputNote(_businessModuleLocator.TakingOrderModule.OrderTemp.OrderDetailsTemps.ElementAt(index).Note);

            if (inputnote.ShowDialog() == true)
            {
                _businessModuleLocator.TakingOrderModule.updateOrderNote(index, inputnote.Note.ToLower());
            }


            RefreshControl();
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);
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
            newOrder.TotalPrice = _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice;
            InputTheRestOrderInfoDialog inputTheRest = new InputTheRestOrderInfoDialog(newOrder);
            if (!inputTheRest.MyShowDialog())
            {
                return;
            }



            // convert data and save to database
            if (_businessModuleLocator.TakingOrderModule.ConvertTableToOrder(newOrder))
            {
                _businessModuleLocator.TakingOrderModule.saveOrderToDB(newOrder);
            }
            else
            {
                return;
            }

            // printing
            var printer = new DoPrintHelper(_businessModuleLocator.RepositoryLocator, DoPrintHelper.Receipt_Printing, newOrder);
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
            newOrder.TotalPrice = _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice;
            _businessModuleLocator.TakingOrderModule.ConvertTableToOrder(newOrder);
 
            // printing
            var printer = new DoPrintHelper(_businessModuleLocator.RepositoryLocator, DoPrintHelper.TempReceipt_Printing, newOrder );
            printer.DoPrint();


            // update employee ID that effect to the OrderNote
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);
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
                        PermissionRequired pr = new PermissionRequired(_businessModuleLocator, ((MainWindow)Window.GetWindow(this)).cUser, true, true);
                        pr.ShowDialog();

                        if (App.Current.Properties["AdLogin"] != null)
                        {
                            ClearTheTable_ForDelete();

                            // update employee ID that effect to the OrderNote
                            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);
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
                    checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);
                }
            }
            else
            {
                ClearTheTable_ForDelete();

                // update employee ID that effect to the OrderNote
                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);
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
            _businessModuleLocator.TakingOrderModule.clearOrder();

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
            _businessModuleLocator.TakingOrderModule.clearOrder();

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


        private void checkWorkingAction(EmpLoginList currentEmp)
        {
            if (currentEmp.Emp.EmpId.Equals(_businessModuleLocator.TakingOrderModule.OrderTemp.EmpId))
            {
                return;
            }

            if (_businessModuleLocator.TakingOrderModule.OrderTemp.SubEmpId != null)
            {
                string[] subemplist = _businessModuleLocator.TakingOrderModule.OrderTemp.SubEmpId.Split(',');

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

                _businessModuleLocator.TakingOrderModule.OrderTemp.SubEmpId += currentEmp.Emp.EmpId + ",";
                return;
            }

            _businessModuleLocator.TakingOrderModule.OrderTemp.SubEmpId += currentEmp.Emp.EmpId + ",";

        }


        // INPUT PAX

        // INPUT TOTALPRICE
        private void txtTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_businessModuleLocator == null || _businessModuleLocator.TakingOrderModule.OrderTemp == null || isUcOrderFormLoading || isClearingTable || isClearingTalbe_ForDelete)
                return;
            TextBox txtTotal = (sender as TextBox);
            if (string.IsNullOrEmpty(txtTotal.Text))
                _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice = 0;
            else
            {
                _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice = decimal.Parse(txtTotal.Text);
            }

            // update employee ID that effect to the OrderNote
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);
        }

        // FORMAT TOTALPRICE TEXTBOX
        private void txtTotal_LostFocus(object sender, RoutedEventArgs e)
        {
            // When user leave the Total Price textbox, it will format their input value
            TextBox txtTotal = (sender as TextBox);

            decimal Total = _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice;
            decimal SaleValue = Total;
            decimal Svc = (Total * 5) / 100;
            decimal Vat = ((Total + (Total * 5) / 100) * 10) / 100;
            Total = (Total + (Total * 5) / 100) + (((Total + (Total * 5) / 100) * 10) / 100);

            _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice = Total;
            _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPriceNonDisc = (decimal)Math.Round(Total, 3);
            _businessModuleLocator.TakingOrderModule.OrderTemp.Svc = Math.Round(Svc, 3);
            _businessModuleLocator.TakingOrderModule.OrderTemp.Vat = Math.Round(Vat, 3);
            _businessModuleLocator.TakingOrderModule.OrderTemp.SaleValue = Math.Round(SaleValue, 3);

            txtTotal.Text = string.Format("{0:0.000}", _businessModuleLocator.TakingOrderModule.OrderTemp.TotalPrice);
        }
    }
}
