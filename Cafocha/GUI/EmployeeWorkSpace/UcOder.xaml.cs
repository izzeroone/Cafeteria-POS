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
        private List<Entities.OrderDetailsTemp> orderDetails;
        private Employee currentEmp;
        private bool isSetOrderMode = false;

        public UcOder()
        {
            InitializeComponent();

            this.Loaded += UcOder_Loaded;
            this.Unloaded += UcOder_Unloaded;
        }

        public void setOrderData(OrderTemp orderTemp, List<OrderDetailsTemp> orderDetails)
        {
            this.orderTemp = orderTemp;
            this.orderDetails = orderDetails;
        }

        private bool isUcOrderFormLoading;
        private void UcOder_Loaded(object sender, RoutedEventArgs e)
        {
            isUcOrderFormLoading = true;
            _unitofwork = ((MainWindow)Window.GetWindow(this))._unitofwork;
            EmpLoginList currentEmpList = (App.Current.Properties["CurrentEmpWorking"] as EmpLoginList);

            InitCus_raiseEvent = false;
            initStatus_RaiseEvent = false;
         

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


            //TODO: Load data from memory rather than database
//            orderTemp = _unitofwork.OrderTempRepository.Get(x => x.TableOwned.Value.Equals(currentTable.TableId)).First();
//            orderDetails = _unitofwork.OrderDetailsTempRepository.Get(x => x.OrdertempId.Equals(orderTemp.OrdertempId)).ToList();


            txtCusNum.Text = orderTemp.Pax.ToString();
            LoadCustomerOwner();
            txtTotal.Text = "";
            isSetOrderMode = (orderTemp.OrderMode == 1) ? true : false;
            if (isSetOrderMode)
            {
                btnSetOrder.IsChecked = true;
                txtTotal.IsEnabled = true;
            }
            else
            {
                btnSetOrder.IsChecked = false;
                txtTotal.IsEnabled = false;
            }

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
                var query = from orderdetails in this.orderDetails
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
            var query = from orderdetails in orderDetails
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
        }


        ToggleButton curChair;

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
                _unitofwork.OrderTempRepository.Update(orderTemp);
                _unitofwork.Save();
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
                var query_item_in_ordertails = from orderdetails in orderDetails
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

                string olstat = orderDetails[index].OldStat;

                OrderDetailsTemp tempdata = new OrderDetailsTemp();
                tempdata.ChairId = orderDetails[index].ChairId;
                tempdata.OrdertempId = orderDetails[index].OrdertempId;
                tempdata.ProductId = orderDetails[index].ProductId;
                tempdata.StatusItems = orderDetails[index].StatusItems;
                tempdata.Quan = orderDetails[index].Quan;
                tempdata.Note = orderDetails[index].Note;
                tempdata.SelectedStats = (e.OriginalSource as ComboBox).SelectedItem.ToString();
                tempdata.IsPrinted = 0;

                foreach (var cho in orderDetails)
                {
                    if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                        && cho.ChairId.Equals(tempdata.ChairId)
                        && cho.ProductId.Equals(tempdata.ProductId)
                        && cho.SelectedStats.Equals(tempdata.SelectedStats)
                        && cho.Note.Equals(tempdata.Note)
                        && (cho.IsPrinted == 0 && tempdata.IsPrinted == 0))
                    {
                        cho.Quan += orderDetails[index].Quan;

                        _unitofwork.OrderDetailsTempRepository.Delete(orderDetails[index]);
                        _unitofwork.Save();

                        RefreshControl(_unitofwork);

                        checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                        return;
                    }
                }

                _unitofwork.OrderDetailsTempRepository.Delete(orderDetails[index]);
                _unitofwork.Save();
                _unitofwork.OrderDetailsTempRepository.Insert(tempdata);
                _unitofwork.Save();

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
           

            DependencyObject dep = (DependencyObject)e.OriginalSource;
            OrderDetailsTemp o = new OrderDetailsTemp();
            int index;

            foreach (ToggleButton btn in wp.Children)
            {
                if (btn.IsChecked == true)
                {
                    while ((dep != null) && !(dep is ListViewItem))
                    {
                        dep = VisualTreeHelper.GetParent(dep);
                    }

                    if (dep == null)
                        return;

                    index = lvData.ItemContainerGenerator.IndexFromContainer(dep);

                    if (orderDetails[index].Quan > 1)
                    {
                        if (!isDone)
                        {
                            GiveBackToWareHouseData(orderDetails[index], 1);
                        }
                        orderDetails[index].Quan--;
                        _unitofwork.OrderDetailsTempRepository.Update(orderDetails[index]);
                        _unitofwork.Save();
                    }
                    else
                    {
                        var chairtemp = orderDetails[index];

                        if (!isDone)
                        {
                            GiveBackToWareHouseData(orderDetails[index], 1);
                        }
                        orderDetails.Remove(orderDetails[index]);
                        orderDetails.RemoveAt(index);
                        _unitofwork.OrderDetailsTempRepository.Delete(chairtemp);
                        _unitofwork.Save();
                    }



                    RefreshControl(_unitofwork);
                    checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                    break;
                }
            }

            if (orderDetails.Count == 0)
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
            tempdata.ChairId = orderDetails[index].ChairId;
            tempdata.OrdertempId = orderDetails[index].OrdertempId;
            tempdata.ProductId = orderDetails[index].ProductId;
            tempdata.SelectedStats = orderDetails[index].SelectedStats;
            tempdata.StatusItems = orderDetails[index].StatusItems;
            tempdata.Quan = orderDetails[index].Quan;
            tempdata.Note = "";
            tempdata.IsPrinted = 0;

            InputNote inputnote = new InputNote(orderDetails[index].Note);
            if (orderDetails[index].Note.Equals("") || orderDetails[index].Note.Equals(inputnote.Note))
            {
                if (inputnote.ShowDialog() == true)
                {
                    if (orderDetails[index].Note.Equals(inputnote.Note.ToLower()))
                    {
                        return;
                    }

                    tempdata.Note = inputnote.Note.ToLower();

                    if (orderDetails[index].Quan == 1)
                    {
                        foreach (var cho in orderDetails)
                        {
                            if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                                && cho.ChairId.Equals(tempdata.ChairId)
                                && cho.ProductId.Equals(tempdata.ProductId)
                                && cho.SelectedStats.Equals(tempdata.SelectedStats)
                                && cho.Note.Equals(tempdata.Note)
                                && (cho.IsPrinted == 0 && tempdata.IsPrinted == 0))
                            {
                                cho.Quan++;
                                _unitofwork.OrderDetailsTempRepository.Delete(orderDetails[index]);
                                _unitofwork.Save();
                                _unitofwork.OrderDetailsTempRepository.Update(cho);
                                _unitofwork.Save();
                                RefreshControl(_unitofwork);
                                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                                return;
                            }
                        }

                        _unitofwork.OrderDetailsTempRepository.Delete(orderDetails[index]);
                        _unitofwork.Save();
                        _unitofwork.OrderDetailsTempRepository.Insert(tempdata);
                        _unitofwork.Save();
                        RefreshControl(_unitofwork);
                        checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                        return;
                    }

                    if (orderDetails[index].Quan > 1)
                    {
                        foreach (var cho in orderDetails)
                        {
                            if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                                && cho.ChairId.Equals(tempdata.ChairId)
                                && cho.ProductId.Equals(tempdata.ProductId)
                                && cho.SelectedStats.Equals(tempdata.SelectedStats)
                                && cho.Note.Equals(tempdata.Note)
                                && (cho.IsPrinted == 0 && tempdata.IsPrinted == 0))
                            {
                                tempdata.Note = orderDetails[index].Note;
                                tempdata.Quan--;
                                cho.Quan++;
                                _unitofwork.OrderDetailsTempRepository.Update(cho);
                                _unitofwork.Save();
                                _unitofwork.OrderDetailsTempRepository.Delete(orderDetails[index]);
                                _unitofwork.Save();
                                _unitofwork.OrderDetailsTempRepository.Insert(tempdata);
                                _unitofwork.Save();
                                RefreshControl(_unitofwork);
                                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                                return;
                            }
                        }

                        foreach (var cho in orderDetails)
                        {
                            if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                                && cho.ChairId.Equals(tempdata.ChairId)
                                && cho.ProductId.Equals(tempdata.ProductId)
                                && cho.SelectedStats.Equals(tempdata.SelectedStats)
                                && !cho.Note.Equals(tempdata.Note))
                            {
                                orderDetails[index].Quan--;
                                _unitofwork.OrderDetailsTempRepository.Update(orderDetails[index]);
                                _unitofwork.Save();
                                tempdata.Quan = 1;
                                _unitofwork.OrderDetailsTempRepository.Insert(tempdata);
                                _unitofwork.Save();
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



        //ToDo: FIX THE PAYING METHOD
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
   
            if (isChairChecked)
            {
                if (isSetOrderMode)
                {
                    MessageBox.Show("Current table is in Set Order Mode, you can not pay per chair!");
                    return;
                }

                // devide the chair data to another Order
                OrderNote newOrder = new OrderNote();
                if (!SplitChairToOrder(newOrder))
                {
                    return;
                }

                //TODO: Fix order note Id is missing

                // input the rest data
                InputTheRestOrderInfoDialog inputTheRest = new InputTheRestOrderInfoDialog(newOrder);
                if (!inputTheRest.MyShowDialog())
                {
                    return;
                }




                // save to database
                _unitofwork.OrderRepository.Insert(newOrder);

                //Set orderdetil id in roder\
                foreach (var orderNoteDetail in newOrder.OrderNoteDetails)
                {
                    orderNoteDetail.OrdernoteId = newOrder.OrdernoteId;
                }

                _unitofwork.Save();


                // printing
                var printer = new DoPrintHelper(_unitofwork, DoPrintHelper.Receipt_Printing, newOrder);
                printer.OrderMode = orderTemp.OrderMode;
                printer.DoPrint();

                //// clean the old chair order data
                DeleteChairOrderDetails();

            }
            else
            {
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
        }

        private void BntPrint_OnClick(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                MessageBox.Show("No employee on working! Please try again!");
                return;
            }

            if (currentTable == null)
                return;

            // printing
            var printer = new DoPrintHelper(_unitofwork, DoPrintHelper.TempReceipt_Printing, currentTable);
            printer.OrderMode = orderTemp.OrderMode;
            printer.DoPrint();

            // update IsPrinted for Table's Order
            currentTable.IsPrinted = 1;
            _unitofwork.TableRepository.Update(currentTable);
            _unitofwork.Save();

            // update employee ID that effect to the OrderNote
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
        }

        private void BtnGo_OnClick(object sender, RoutedEventArgs e)
        {
            if (App.Current.Properties["CurrentEmpWorking"] == null)
            {
                MessageBox.Show("No employee on working! Please try again!");
                return;
            }

            if (currentTable == null)
                return;

            // check order
            bool isHaveDrink = false;
            bool isHaveFood = false;
            int orderCount = 0;
            var chairQuery = _unitofwork.ChairRepository.Get(x => x.TableOwned.Value.Equals(currentTable.TableId));
            foreach (var chair in chairQuery)
            {
                var orderDetailsQuery =
                    _unitofwork.OrderDetailsTempRepository.Get(x => x.ChairId.Equals(chair.ChairId) && x.IsPrinted == 0);
                orderCount += orderDetailsQuery.Count();
                foreach (var details in orderDetailsQuery)
                {
                    if (details.SelectedStats != "Drink" && !isHaveFood)
                    {
                        isHaveFood = true;
                    }

                    if (details.SelectedStats == "Drink" && !isHaveDrink)
                    {
                        isHaveDrink = true;
                    }
                }

                if (isHaveDrink && isHaveFood)
                    break;
            }

            if (orderCount == 0)
                return;

            try
            {
                // printing
                if (isHaveFood)
                {
                    var printer1 = new DoPrintHelper(_unitofwork, DoPrintHelper.Kitchen_Printing, currentTable);
                    printer1.DoPrint();
                }
                if (isHaveDrink)
                {
                    var printer2 = new DoPrintHelper(_unitofwork, DoPrintHelper.Bar_Printing, currentTable);
                    printer2.DoPrint();
                }

                List<OrderDetailsTemp> newOrderDetails = new List<OrderDetailsTemp>();
                orderTemp = _unitofwork.OrderTempRepository.Get(x => x.TableOwned.Value.Equals(currentTable.TableId)).First();
                orderDetails = _unitofwork.OrderDetailsTempRepository.Get(x => x.OrdertempId.Equals(orderTemp.OrdertempId)).ToList();
                foreach (var orderDetails in orderDetails.ToList())
                {
                    if (orderDetails.IsPrinted == 0)
                    {
                        newOrderDetails.Add(new OrderDetailsTemp()
                        {
                            OrdertempId = orderDetails.OrdertempId,
                            ProductId = orderDetails.ProductId,
                            ChairId = orderDetails.ChairId,
                            SelectedStats = orderDetails.SelectedStats,
                            Note = orderDetails.Note,
                            IsPrinted = 1,
                            Discount = orderDetails.Discount,
                            Quan = orderDetails.Quan,
                        });
                        _unitofwork.OrderDetailsTempRepository.Delete(orderDetails);
                    }
                }
                foreach (var newDetails in newOrderDetails)
                {
                    bool isupdate = false;
                    foreach (var orderDetailsTemp in orderDetails.Where(x => x.IsPrinted == 1))
                    {
                        if (newDetails.OrdertempId.Equals(orderDetailsTemp.OrdertempId)
                            && newDetails.ChairId.Equals(orderDetailsTemp.ChairId)
                            && newDetails.ProductId.Equals(orderDetailsTemp.ProductId)
                            && newDetails.SelectedStats.Equals(orderDetailsTemp.SelectedStats)
                            && newDetails.Note.Equals(orderDetailsTemp.Note))
                        {
                            orderDetailsTemp.Quan += newDetails.Quan;
                            _unitofwork.OrderDetailsTempRepository.Update(orderDetailsTemp);
                            isupdate = true;
                            break;
                        }
                    }

                    if (!isupdate)
                        _unitofwork.OrderDetailsTempRepository.Insert(newDetails);
                }
                _unitofwork.Save();
                RefreshControl(_unitofwork, currentTable);
                checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            // update IsPrinted for Table's Order
            currentTable.IsPrinted = 1;
            _unitofwork.TableRepository.Update(currentTable);
            _unitofwork.Save();

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

            bool pass = false;
            if (currentTable.IsPrinted == 1)
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
        /// Split current selected Chair in current Table to the new stand alone Order
        /// </summary>
        /// <param name="newwOrder"></param>
        private Boolean SplitChairToOrder(OrderNote newOrder)
        {
            if (currentTable == null)
                return false;


            // calculate totalPriceNonDisc and TotalPrice
            decimal Svc = 0;
            decimal Vat = 0;
            decimal SaleValue = 0;
            decimal Total = 0;
            decimal TotalWithDiscount = 0;
            foreach (var item in currentChair.OrderDetailsTemps)
            {
                var prod = _unitofwork.ProductRepository.Get(x => x.ProductId.Equals(item.ProductId)).FirstOrDefault();
                if (prod == null)
                    return false;
                Total = (decimal)((float)Total + (float)(item.Quan * ((float)prod.Price * ((100 - item.Discount) / 100.0))));
            }
            // tính năng giảm giá cho món có gì đó không ổn => hiện tại Total chính là SaleValue
            SaleValue = Total;
            Svc = (Total * 5) / 100;
            Vat = ((Total + (Total * 5) / 100) * 10) / 100;
            Total = (Total + (Total * 5) / 100) + (((Total + (Total * 5) / 100) * 10) / 100);
            TotalWithDiscount = (decimal)(((float)Total * (100 - orderTemp.Discount)) / 100.0);


            var currentOrderTemp = _unitofwork.OrderTempRepository.Get(x => x.TableOwned.Value.Equals(currentTable.TableId))
                .FirstOrDefault();
            if (currentOrderTemp != null)
            {
                newOrder.CusId = currentOrderTemp.CusId;
                newOrder.EmpId = currentOrderTemp.EmpId;
                newOrder.Pax = 1;
                newOrder.Ordertable = currentTable.TableNumber;
                newOrder.Ordertime = (DateTime) currentOrderTemp.Ordertime;
                newOrder.TotalPriceNonDisc = Math.Round(Total, 3);
                newOrder.TotalPrice = Math.Round(TotalWithDiscount, 3);
                newOrder.Svc = Math.Round(Svc, 3);
                newOrder.Vat = Math.Round(Vat, 3);
                newOrder.SaleValue = Math.Round(SaleValue, 3);
                newOrder.Discount = (int) currentOrderTemp.Discount;
                newOrder.SubEmpId = currentOrderTemp.SubEmpId;
            }
            else return false;


            Dictionary<string, OrderNoteDetail> newDetailsList = new Dictionary<string, OrderNoteDetail>();
            foreach (var details in currentChair.OrderDetailsTemps)
            {
                if (newDetailsList.ContainsKey(details.ProductId))
                {
                    newDetailsList[details.ProductId].Quan += details.Quan;
                }
                else
                {
                    newDetailsList.Add(details.ProductId, new OrderNoteDetail()
                    {
                        OrdernoteId = newOrder.OrdernoteId,
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


        /// <summary>
        /// Migrate all Order info in current Table to OrderNote object that will be insert to Database
        /// </summary>
        /// <param name="newOrder"></param>
        private bool ConvertTableToOrder(OrderNote newOrder)
        {
            if (currentTable == null)
                return false;

            var currentOrderTemp = _unitofwork.OrderTempRepository.Get(x => x.TableOwned.Value.Equals(currentTable.TableId))
                .FirstOrDefault();
            if (currentOrderTemp != null)
            {
                newOrder.CusId = currentOrderTemp.CusId;
                newOrder.EmpId = currentOrderTemp.EmpId;
                newOrder.Pax = currentOrderTemp.Pax;
                newOrder.Ordertable = currentTable.TableNumber;
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
            Entities.Table curTable = currentTable;
            var orderOfTable = _unitofwork.OrderTempRepository.Get(x => x.TableOwned.Value.Equals(curTable.TableId)).FirstOrDefault();
            if (orderOfTable != null)
            {
                var ordernotedetails = orderDetails
                    .Where(x => x.OrdertempId.Equals(orderOfTable.OrdertempId))
                    .ToList();
                if (ordernotedetails.Count != 0)
                {
                    foreach (var ch in ordernotedetails)
                    {
                        orderDetails.Remove(ch);
                        _unitofwork.OrderDetailsTempRepository.Delete(ch);
                        _unitofwork.Save();
                    }
                }
            }
            _unitofwork.Save();

            orderTemp.EmpId = (App.Current.Properties["EmpLogin"] as Employee).EmpId;
            orderTemp.CusId = "CUS0000001";
            orderTemp.Discount = 0;
            orderTemp.TableOwned = curTable.TableId;
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

            curTable.IsOrdered = 0;
            curTable.IsPrinted = 0;

            ((MainWindow)Window.GetWindow(this)).initProgressTableChair();
            LoadCustomerOwner();
            RefreshControlAllChair();
            _unitofwork.OrderTempRepository.Update(orderTemp);
            _unitofwork.Save();

            //Table b = new Table(_unitofwork, _cloudPosUnitofwork);
            var cur = (((MainWindow)Window.GetWindow(this)).b).currentTableList.Where(x => x.TableId.Equals(curTable.TableId)).FirstOrDefault();
            if (cur != null)
            {
                cur.IsOrdered = 0;
                cur.IsPrinted = 0;
            }

            isClearingTable = false;

            ((MainWindow)Window.GetWindow(this)).currentTable = null;
            ((MainWindow)Window.GetWindow(this)).b.isTablesDataChange = true;
            ((MainWindow)Window.GetWindow(this)).myFrame.Navigate(((MainWindow)Window.GetWindow(this)).b);
            ((MainWindow)Window.GetWindow(this)).bntTable.IsEnabled = false;
            ((MainWindow)Window.GetWindow(this)).bntDash.IsEnabled = true;
            ((MainWindow)Window.GetWindow(this)).bntEntry.IsEnabled = true;
        }

        bool isClearingTalbe_ForDelete = false;
        private void ClearTheTable_ForDelete()
        {
            isClearingTalbe_ForDelete = true;
            Entities.Table curTable = currentTable;
            var orderOfTable = _unitofwork.OrderTempRepository.Get(x => x.TableOwned.Value.Equals(curTable.TableId)).FirstOrDefault();
            if (orderOfTable != null)
            {
                var ordernotedetails = orderDetails
                    .Where(x => x.OrdertempId.Equals(orderOfTable.OrdertempId))
                    .ToList();
                if (ordernotedetails.Count != 0)
                {
                    foreach (var ch in ordernotedetails)
                    {
                        GiveBackToWareHouseData(ch, ch.Quan);
                        orderDetails.Remove(ch);
                        _unitofwork.OrderDetailsTempRepository.Delete(ch);
                        _unitofwork.Save();
                    }
                }
            }
            _unitofwork.Save();

            orderTemp.EmpId = (App.Current.Properties["EmpLogin"] as Employee).EmpId;
            orderTemp.CusId = "CUS0000001";
            orderTemp.Discount = 0;
            orderTemp.TableOwned = curTable.TableId;
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

            curTable.IsOrdered = 0;
            curTable.IsPrinted = 0;

            ((MainWindow)Window.GetWindow(this)).initProgressTableChair();
            LoadCustomerOwner();
            RefreshControlAllChair();
            _unitofwork.OrderTempRepository.Update(orderTemp);
            _unitofwork.Save();

            //Table b = new Table(_unitofwork, _cloudPosUnitofwork);
            var cur = (((MainWindow)Window.GetWindow(this)).b).currentTableList.Where(x => x.TableId.Equals(curTable.TableId)).FirstOrDefault();
            if (cur != null)
            {
                cur.IsOrdered = 0;
                cur.IsPrinted = 0;
            }

            isClearingTalbe_ForDelete = false;

            ((MainWindow)Window.GetWindow(this)).currentTable = null;
            ((MainWindow)Window.GetWindow(this)).b.isTablesDataChange = true;
            ((MainWindow)Window.GetWindow(this)).myFrame.Navigate(((MainWindow)Window.GetWindow(this)).b);
            ((MainWindow)Window.GetWindow(this)).bntTable.IsEnabled = false;
            ((MainWindow)Window.GetWindow(this)).bntDash.IsEnabled = true;
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

        private void DeleteChairOrderDetails()
        {
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

            foreach (ToggleButton btn in wp.Children)
            {
                if (btn.IsChecked == true)
                {
                    //delete chair order note
                    var chairoftable = _unitofwork.ChairRepository.Get(x => x.TableOwned.Value.Equals(currentTable.TableId)).ToList();
                    var foundchair = chairoftable.SingleOrDefault(x => x.ChairNumber.Equals(currentChair.ChairNumber)
                                            && x.TableOwned.Equals(currentChair.TableOwned));
                    var chairordernotedetails = orderDetails.Where(x => x.ChairId == foundchair.ChairId).ToList();

                    if (chairordernotedetails.Count != 0)
                    {
                        foreach (var ch in chairordernotedetails)
                        {
                            _unitofwork.OrderDetailsTempRepository.Delete(ch);
                            _unitofwork.Save();
                        }

                        currentTable.IsOrdered = 0;
                    }

                    foreach (Entities.Chair chair in chairoftable)
                    {
                        var chairorderdetailstemp = _unitofwork.OrderDetailsTempRepository.Get(x => x.ChairId.Equals(chair.ChairId)).ToList();
                        if (chairorderdetailstemp.Count != 0)
                        {
                            currentTable.IsOrdered = 1;
                        }
                        break;
                    }

                    _unitofwork.TableRepository.Update(currentTable);
                    _unitofwork.Save();

                    ((MainWindow)Window.GetWindow(this)).initProgressTableChair();
                    RefreshControl(_unitofwork, currentTable);
                    checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
                    break;
                }
            }

            if (currentTable.IsOrdered == 0)
            {
                //Table b = new Table(_unitofwork, _cloudPosUnitofwork);
                var cur = (((MainWindow)Window.GetWindow(this)).b).currentTableList.Where(x => x.TableId.Equals(currentTable.TableId)).FirstOrDefault();
                if (cur != null)
                {
                    cur.IsOrdered = 0;
                    cur.IsPrinted = 0;
                }

                ((MainWindow)Window.GetWindow(this)).currentTable = null;
                ((MainWindow)Window.GetWindow(this)).b.isTablesDataChange = true;
                ((MainWindow)Window.GetWindow(this)).myFrame.Navigate(((MainWindow)Window.GetWindow(this)).b);
                ((MainWindow)Window.GetWindow(this)).bntTable.IsEnabled = false;
                ((MainWindow)Window.GetWindow(this)).bntDash.IsEnabled = true;
                ((MainWindow)Window.GetWindow(this)).bntEntry.IsEnabled = true;
            }
        }

        private void checkWorkingAction(EmpLoginList currentEmp, OrderTemp ordertempcurrenttable)
        {
            ((MainWindow)Window.GetWindow(this)).b.isTablesDataChange = true;
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
                _unitofwork.OrderTempRepository.Update(ordertempcurrenttable);
                _unitofwork.Save();
                return;
            }

            ordertempcurrenttable.SubEmpId += currentEmp.Emp.EmpId + ",";
            _unitofwork.OrderTempRepository.Update(ordertempcurrenttable);
            _unitofwork.Save();

        }


        // INPUT PAX
        private void TxtCusNum_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            if (currentTable == null || isUcOrderFormLoading || isAddInfoInOrderSetMode)
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
            if (currentTable == null || isUcOrderFormLoading || isCalculateTotalPrice || isClearingTable || isClearingTalbe_ForDelete)
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
        private void btnSetOrder_Checked(object sender, RoutedEventArgs e)
        {
            if (currentTable == null || isUcOrderFormLoading)
                return;
            isSetOrderMode = true;
            txtTotal.IsEnabled = true;
            orderTemp.OrderMode = 1;
            isCalculateTotalPrice = false;

            // add info when order set mode turned on
            isAddInfoInOrderSetMode = true;
            SetOrderModeDialog orderModeDialog = new SetOrderModeDialog(orderTemp);
            orderModeDialog.ShowDialog();
            txtCusNum.Text = orderTemp.Pax.ToString();
            isAddInfoInOrderSetMode = false;

            loadTotalPrice();

            // update employee ID that effect to the OrderNote
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
            _unitofwork.Save();
        }

        private void btnSetOrder_Unchecked(object sender, RoutedEventArgs e)
        {
            if (currentTable == null || isUcOrderFormLoading)
                return;
            isSetOrderMode = false;
            txtTotal.IsEnabled = false;
            orderTemp.OrderMode = 0;
            isCalculateTotalPrice = true;
            loadTotalPrice();

            // update employee ID that effect to the OrderNote
            checkWorkingAction(App.Current.Properties["CurrentEmpWorking"] as EmpLoginList, orderTemp);
            _unitofwork.Save();
        }

    }
}
