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
        private OrderTemp orderTemp;
        private List<Entities.OrderDetailsTemp> orderTempDetails;
        private Employee currentEmp;
        private bool isSetOrderMode = false;

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
            this.orderTemp = ((MainWindow)Window.GetWindow(this)).orderTemp;
            this.orderTempDetails = ((MainWindow)Window.GetWindow(this)).orderDetailsTemp;
            _unitofwork = ((MainWindow)Window.GetWindow(this))._unitofwork;
            EmpLoginList currentEmpList = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);


            isSetOrderMode = false;

            InitCus_raiseEvent = true;
            initStatus_RaiseEvent = true;
            txtDay.Text = "";
            txtTotal.Text = "";
            wp.Children.Clear();
            lvData.ItemsSource = new List<OrderDetails_Product_Joiner>();
           

         
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
            isSetOrderMode = (orderTemp.OrderMode == 1) ? true : false;
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

                _unitofwork = unitofwork;


                // chuyen product_id thanh product name
                var query = from orderdetails in orderTempDetails
                            join product in _unitofwork.ProductRepository.Get()
                                    on orderdetails.ProductId equals product.ProductId

                            select new OrderDetails_Product_Joiner
                            {
                                OrderDetailsTemp = orderdetails,
                                Product = product
                            };

                // binding
                lvData.ItemsSource = query;
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

            // chuyen product_id thanh product name
            var query = from orderdetails in orderTempDetails
                        join product in _unitofwork.ProductRepository.Get()
                        on orderdetails.ProductId equals product.ProductId

                        select new OrderDetails_Product_Joiner
                        {
                            OrderDetailsTemp = orderdetails,
                            Product = product
                        };

            // binding
            lvData.ItemsSource = query;
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

            if (orderTemp.CusId != null)
            {
                InitCus_raiseEvent = true;
                cboCustomers.SelectedValue = orderTemp.CusId;
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
                orderTemp.CusId =
                    (string)(sender as ComboBox).SelectedValue;
                orderTemp.Discount = _unitofwork.CustomerRepository.Get(x => x.CusId.Equals(orderTemp.CusId))
                    .FirstOrDefault().Discount;
                loadTotalPrice();
                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
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
        public class OrderDetails_Product_Joiner : INotifyPropertyChanged
        {
            public OrderDetailsTemp OrderDetailsTemp { get; set; }
            public Product Product { get; set; }

            public string ProductName
            {
                get
                {
                    return Product.Name;
                }
            }
            public decimal Price
            {
                get
                {
                    return Product.Price;
                }
            }
            public int Quan
            {
                get
                {
                    return OrderDetailsTemp.Quan;
                }
                set
                {
                    OrderDetailsTemp.Quan = value;
                    OnPropertyChanged("Quan");
                }
            }
            public ObservableCollection<string> StatusItems
            {
                get
                {
                    return OrderDetailsTemp.StatusItems;
                }
                set
                {
                    OrderDetailsTemp.StatusItems = value;
                    OnPropertyChanged("StatusItems");
                }
            }
            public string SelectedStats
            {
                get
                {
                    return OrderDetailsTemp.SelectedStats;
                }
                set
                {
                    OrderDetailsTemp.SelectedStats = value;
                    OnPropertyChanged("SelectedStats");
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;
            public void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }


        public void loadTotalPrice()
        {
            if (!isSetOrderMode)
            {

                // chuyen product_id thanh product name
                var query_item_in_ordertails = from orderdetails in orderTempDetails
                                               join product in _unitofwork.ProductRepository.Get()
                                               on orderdetails.ProductId equals product.ProductId
                                               select new
                                               {
                                                   item_quan = orderdetails.Quan,
                                                   item_price = product.Price,
                                                   item_discount = product.Discount
                                               };

                // calculate totalPriceNonDisc and TotalPrice
                decimal Svc = 0;
                decimal Vat = 0;
                decimal SaleValue = 0;
                decimal TotalWithDiscount = 0;
                decimal Total = 0;
                foreach (var item in query_item_in_ordertails)
                {
                    Total = (decimal)((float)Total + (float)(item.item_quan * ((float)item.item_price * ((100 - item.item_discount) / 100.0))));
                }

                // tính năng giảm giá cho món có gì đó không ổn => hiện tại Total chính là SaleValue
                SaleValue = Total;
                Svc = (Total * 5) / 100;
                Vat = ((Total + (Total * 5) / 100) * 10) / 100;
                Total = (Total + (Total * 5) / 100) + (((Total + (Total * 5) / 100) * 10) / 100);
                TotalWithDiscount = (decimal)(((float)Total * (100 - orderTemp.Discount)) / 100.0);

                /*
                 * If the current order isn't in Set Order  Mode
                 * Use the casual calculate method to compute the Total Price
                 */

                txtTotal.Text = string.Format("{0:0.000}", TotalWithDiscount);
                orderTemp.TotalPrice = (decimal)Math.Round(TotalWithDiscount, 3);
                orderTemp.TotalPriceNonDisc = (decimal)Math.Round(Total, 3);
                orderTemp.Svc = Math.Round(Svc, 3);
                orderTemp.Vat = Math.Round(Vat, 3);
                orderTemp.SaleValue = Math.Round(SaleValue, 3);
            }
            else
            {
                /*
                 * If the current order is in Set Order  Mode
                 * Just let the User(Admin) input total price
                 */
                txtTotal.Text = string.Format("{0:0.000}", orderTemp.TotalPrice);
            }
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

                string olstat = orderTempDetails[index].OldStat;

                OrderDetailsTemp tempdata = new OrderDetailsTemp();
                tempdata.OrdertempId = orderTempDetails[index].OrdertempId;
                tempdata.ProductId = orderTempDetails[index].ProductId;
                tempdata.StatusItems = orderTempDetails[index].StatusItems;
                tempdata.Quan = orderTempDetails[index].Quan;
                tempdata.Note = orderTempDetails[index].Note;
                tempdata.SelectedStats = (e.OriginalSource as ComboBox).SelectedItem.ToString();
                tempdata.IsPrinted = 0;

                foreach (var cho in orderTempDetails)
                {
                    if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                        && cho.ProductId.Equals(tempdata.ProductId)
                        && cho.SelectedStats.Equals(tempdata.SelectedStats)
                        && cho.Note.Equals(tempdata.Note)
                        && (cho.IsPrinted == 0 && tempdata.IsPrinted == 0))
                    {
                        cho.Quan += orderTempDetails[index].Quan;


                        RefreshControl(_unitofwork);

                        checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                        return;
                    }
                }

                RefreshControl(_unitofwork);

                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
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
                        checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
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

                    if (orderTempDetails[index].Quan > 1)
                    {
                        if (!isDone)
                        {
                            GiveBackToWareHouseData(orderTempDetails[index], 1);
                        }
                        orderTempDetails[index].Quan--;
                    }
                    else
                    {
                        var chairtemp = orderTempDetails[index];

                        if (!isDone)
                        {
                            GiveBackToWareHouseData(orderTempDetails[index], 1);
                        }
                        orderTempDetails.Remove(orderTempDetails[index]);
                        orderTempDetails.RemoveAt(index);
                    }

                    RefreshControl(_unitofwork);
                    checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                    break;
                }
            }

            if (orderTempDetails.Count == 0)
            {
                ClearTheTable();
            }
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

            OrderDetailsTemp tempdata = new OrderDetailsTemp();
            tempdata.OrdertempId = orderTempDetails[index].OrdertempId;
            tempdata.ProductId = orderTempDetails[index].ProductId;
            tempdata.SelectedStats = orderTempDetails[index].SelectedStats;
            tempdata.StatusItems = orderTempDetails[index].StatusItems;
            tempdata.Quan = orderTempDetails[index].Quan;
            tempdata.Note = "";
            tempdata.IsPrinted = 0;

            InputNote inputnote = new InputNote(orderTempDetails[index].Note);
            if (orderTempDetails[index].Note.Equals("") || orderTempDetails[index].Note.Equals(inputnote.Note))
            {
                if (inputnote.ShowDialog() == true)
                {
                    if (orderTempDetails[index].Note.Equals(inputnote.Note.ToLower()))
                    {
                        return;
                    }

                    tempdata.Note = inputnote.Note.ToLower();

                    if (orderTempDetails[index].Quan == 1)
                    {
                        foreach (var cho in orderTempDetails)
                        {
                            if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                                && cho.ProductId.Equals(tempdata.ProductId)
                                && cho.SelectedStats.Equals(tempdata.SelectedStats)
                                && cho.Note.Equals(tempdata.Note)
                                && (cho.IsPrinted == 0 && tempdata.IsPrinted == 0))
                            {
                                cho.Quan++;
                                RefreshControl(_unitofwork);
                                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                                return;
                            }
                        }
                        RefreshControl(_unitofwork);
                        checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                        return;
                    }

                    if (orderTempDetails[index].Quan > 1)
                    {
                        foreach (var cho in orderTempDetails)
                        {
                            if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                                && cho.ProductId.Equals(tempdata.ProductId)
                                && cho.SelectedStats.Equals(tempdata.SelectedStats)
                                && cho.Note.Equals(tempdata.Note)
                                && (cho.IsPrinted == 0 && tempdata.IsPrinted == 0))
                            {
                                tempdata.Note = orderTempDetails[index].Note;
                                tempdata.Quan--;
                                cho.Quan++;
   
                                RefreshControl(_unitofwork);
                                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                                return;
                            }
                        }

                        foreach (var cho in orderTempDetails)
                        {
                            if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                                && cho.ProductId.Equals(tempdata.ProductId)
                                && cho.SelectedStats.Equals(tempdata.SelectedStats)
                                && !cho.Note.Equals(tempdata.Note))
                            {
                                orderTempDetails[index].Quan--;
                                tempdata.Quan = 1;
                                RefreshControl(_unitofwork);
                                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                                return;
                            }
                        }
                    }
                }
            }
            else
            {
                inputnote.ShowDialog();
                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
            }
        }



        private void bntPay_Click(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                MessageBox.Show("No employee on working! Please try again!");
                return;
            }


            bool isChairChecked = false;
            foreach (var chairUIElement in wp.Children)
            {
                ToggleButton btnChair = chairUIElement as ToggleButton;
                if (btnChair.IsChecked == true)
                {
                    isChairChecked = true;
                    break;
                }
            }

            List<OrderDetailsTemp> newOrderDetails = new List<OrderDetailsTemp>();


            // input the rest data
            OrderNote newOrder = new OrderNote();
            newOrder.TotalPrice = orderTemp.TotalPrice;
            InputTheRestOrderInfoDialog inputTheRest = new InputTheRestOrderInfoDialog(newOrder);
            if (!inputTheRest.MyShowDialog())
            {
                return;
            }



            // convert data and save to database
            if (ConvertTableToOrder(newOrder))
            {
                _unitofwork.OrderRepository.Insert(newOrder);
                _unitofwork.Save();
            }
            else
            {
                return;
            }

            // printing
            var printer = new DoPrintHelper(_unitofwork, DoPrintHelper.Receipt_Printing, newOrder);
            printer.OrderMode = orderTemp.OrderMode;
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
            newOrder.TotalPrice = orderTemp.TotalPrice;
            ConvertTableToOrder(newOrder);
 
            // printing
            var printer = new DoPrintHelper(_unitofwork, DoPrintHelper.TempReceipt_Printing, newOrder );
            printer.OrderMode = orderTemp.OrderMode;
            printer.DoPrint();


            // update employee ID that effect to the OrderNote
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
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
                            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
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
                    checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                }
            }
            else
            {
                ClearTheTable_ForDelete();

                // update employee ID that effect to the OrderNote
                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
            }
        }


        /// <summary>
        /// Migrate all Order info in current Table to OrderNote object that will be insert to Database
        /// </summary>
        /// <param name="newOrder"></param>
        private bool ConvertTableToOrder(OrderNote newOrder)
        {

            var currentOrderTemp = orderTemp;
            if (currentOrderTemp != null)
            {
                newOrder.CusId = currentOrderTemp.CusId;
                newOrder.EmpId = currentOrderTemp.EmpId;
                newOrder.Pax = currentOrderTemp.Pax;
                newOrder.Ordertime = currentOrderTemp.Ordertime;
                newOrder.TotalPriceNonDisc = currentOrderTemp.TotalPriceNonDisc;
                newOrder.TotalPrice = currentOrderTemp.TotalPrice;
                newOrder.Svc = currentOrderTemp.Svc;
                newOrder.Vat = currentOrderTemp.Vat;
                newOrder.SaleValue = currentOrderTemp.SaleValue;
                newOrder.Discount = currentOrderTemp.Discount;
                newOrder.SubEmpId = currentOrderTemp.SubEmpId;
            }
            else return false;

            Dictionary<string, OrderNoteDetail> newDetailsList = new Dictionary<string, OrderNoteDetail>();
            foreach (var details in currentOrderTemp.OrderDetailsTemps)
            {
                if (newDetailsList.ContainsKey(details.ProductId))
                {
                    newDetailsList[details.ProductId].Quan += details.Quan;
                }
                else
                {
                    newDetailsList.Add(details.ProductId, new OrderNoteDetail()
                    {
                        ProductId = details.ProductId,
                        Discount = details.Discount,
                        Quan = details.Quan
                    });
                }
            }
            foreach (var newDetails in newDetailsList)
            {
                newOrder.OrderNoteDetails.Add(newDetails.Value);
            }

            return true;
        }


        bool isClearingTable = false;
        /// <summary>
        /// Clean the whole Order info in  current Table
        /// </summary>
        private void ClearTheTable()
        {
            isClearingTable = true;
            var orderOfTable = orderTemp;
            if (orderOfTable != null)
            {
                var ordernotedetails = orderTempDetails
                    .Where(x => x.OrdertempId.Equals(orderOfTable.OrdertempId))
                    .ToList();
                if (ordernotedetails.Count != 0)
                {
                    foreach (var ch in ordernotedetails)
                    {
                        orderTempDetails.Remove(ch);
                    }
                }
            }

            orderTemp.EmpId = (App.Current.Properties["EmpLogin"] as Employee).EmpId;
            orderTemp.CusId = "CUS0000001";
            orderTemp.Discount = 0;
            orderTemp.Ordertime = DateTime.Now;
            orderTemp.TotalPriceNonDisc = 0;
            orderTemp.SaleValue = 0;
            orderTemp.Svc = 0;
            orderTemp.Vat = 0;
            orderTemp.TotalPrice = 0;
            orderTemp.CustomerPay = 0;
            orderTemp.PayBack = 0;
            orderTemp.SubEmpId = "";
            orderTemp.Pax = 0;
            orderTemp.OrderMode = 0;

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
            var orderOfTable = orderTemp;
            {
                var ordernotedetails = orderTempDetails
                    .Where(x => x.OrdertempId.Equals(orderOfTable.OrdertempId))
                    .ToList();
                if (ordernotedetails.Count != 0)
                {
                    foreach (var ch in ordernotedetails)
                    {
                        GiveBackToWareHouseData(ch, ch.Quan);
                        orderTempDetails.Remove(ch);
                    }
                }
            }
            _unitofwork.Save();

            orderTemp.EmpId = (App.Current.Properties["EmpLogin"] as Employee).EmpId;
            orderTemp.CusId = "CUS0000001";
            orderTemp.Discount = 0;
            orderTemp.Ordertime = DateTime.Now;
            orderTemp.TotalPriceNonDisc = 0;
            orderTemp.SaleValue = 0;
            orderTemp.Svc = 0;
            orderTemp.Vat = 0;
            orderTemp.TotalPrice = 0;
            orderTemp.CustomerPay = 0;
            orderTemp.PayBack = 0;
            orderTemp.SubEmpId = "";
            orderTemp.Pax = 0;
            orderTemp.OrderMode = 0;

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
        private void GiveBackToWareHouseData(OrderDetailsTemp orderDetails, int productQuan)
        {
            var prodOfOrderDetails =
                _unitofwork.ProductRepository.Get(x => x.ProductId.Equals(orderDetails.ProductId), includeProperties: "ProductDetails").FirstOrDefault();
            if (prodOfOrderDetails != null)
            {
                if (prodOfOrderDetails.ProductDetails.Count == 0)
                {
                    // not ingredient relate to this product for tracking
                    return;
                }

                var wareHouseDict = new Dictionary<WareHouse, double?>();
                // going to warehouse and give back the contain for each ingredient
                foreach (var prodDetails in prodOfOrderDetails.ProductDetails)
                {
                    var detailsUsingQuan = prodDetails.Quan;
                    var ingd =
                        _unitofwork.IngredientRepository.Get(x => x.IgdId.Equals(prodDetails.IgdId)).FirstOrDefault();
                    if (ingd == null)
                    {
                        MessageBox.Show("Something went wrong cause of the Ingredient's information");
                        return;
                    }
                    var wareHouse =
                        _unitofwork.WareHouseRepository.Get(x => x.WarehouseId.Equals(ingd.WarehouseId)).FirstOrDefault();
                    if (wareHouse == null)
                    {
                        MessageBox.Show("Something went wrong cause of the WareHouse's information");
                        return;
                    }


                    var temple_Contain = wareHouse.Contain;
                    temple_Contain += (detailsUsingQuan * productQuan);
                    wareHouseDict.Add(wareHouse, temple_Contain);
                }


                // when giving back is success full for all ingredient
                // let update the contain data
                foreach (var item in wareHouseDict)
                {
                    item.Key.Contain = item.Value;
                }
                //_cloudPosUnitofwork.Save();
            }
            else
            {
                MessageBox.Show("This Product is not existed in database! Please check the Product's information");
            }

        }

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
        private void TxtCusNum_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (orderTemp == null || isUcOrderFormLoading || isAddInfoInOrderSetMode)
                return;
            TextBox txtCusnum = (sender as TextBox);
            if (string.IsNullOrEmpty(txtCusnum.Text))
                orderTemp.Pax = 0;
            else
                orderTemp.Pax = int.Parse(txtCusnum.Text);

            // update employee ID that effect to the OrderNote
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
            _unitofwork.Save();
        }

        // INPUT TOTALPRICE
        private void txtTotal_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (orderTemp == null || isUcOrderFormLoading || isCalculateTotalPrice || isClearingTable || isClearingTalbe_ForDelete)
                return;
            TextBox txtTotal = (sender as TextBox);
            if (string.IsNullOrEmpty(txtTotal.Text))
                orderTemp.TotalPrice = 0;
            else
            {
                orderTemp.TotalPrice = decimal.Parse(txtTotal.Text);
            }

            // update employee ID that effect to the OrderNote
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
            _unitofwork.Save();
        }

        // FORMAT TOTALPRICE TEXTBOX
        private void txtTotal_LostFocus(object sender, RoutedEventArgs e)
        {
            // When user leave the Total Price textbox, it will format their input value
            TextBox txtTotal = (sender as TextBox);

            decimal Total = orderTemp.TotalPrice;
            decimal SaleValue = Total;
            decimal Svc = (Total * 5) / 100;
            decimal Vat = ((Total + (Total * 5) / 100) * 10) / 100;
            Total = (Total + (Total * 5) / 100) + (((Total + (Total * 5) / 100) * 10) / 100);

            orderTemp.TotalPrice = Total;
            orderTemp.TotalPriceNonDisc = (decimal)Math.Round(Total, 3);
            orderTemp.Svc = Math.Round(Svc, 3);
            orderTemp.Vat = Math.Round(Vat, 3);
            orderTemp.SaleValue = Math.Round(SaleValue, 3);

            txtTotal.Text = string.Format("{0:0.000}", orderTemp.TotalPrice);
        }




        bool isCalculateTotalPrice;
        bool isAddInfoInOrderSetMode = false;
    }
}
