using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Cafocha.Entities;
using Cafocha.GUI.EmployeeWorkSpace;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.EmployeeWorkspace
{
    //A class to help manaing order
    public class TakingOrderModule
    {
        private RepositoryLocator _unitofwork;
        private OrderTemp _orderTemp;

        public TakingOrderModule()
        {
            _orderTemp = new OrderTemp();
            this.clearOrder();
        }

        public TakingOrderModule(RepositoryLocator unitofwork)
        {
            _orderTemp = new OrderTemp();
            this.clearOrder();
            _unitofwork = unitofwork;
        }

        public OrderTemp OrderTemp
        {
            get => _orderTemp;
            set => _orderTemp = value;
        }



//        public ICollection<OrderDetailsTempData> OrderDetails
//        {
//            get => _orderTemp.OrderDetailsTemps;
//        }

        public void addOrUpdateOrderDetail(OrderDetailsTemp orderTempDetail)
        {
            var flag = false;

            var item = _orderTemp.OrderDetailsTemps.FirstOrDefault(o => o.ProductId.Equals(orderTempDetail.ProductId));

            if (item != null)
            {
                _orderTemp.OrderDetailsTemps.Remove(item);
            }

            _orderTemp.OrderDetailsTemps.Add(orderTempDetail);
        }

        //This function create a new order temp or update quanity by 1 when click
        public void addProductToOrder(Product pt)
        {
            _orderTemp.Ordertime = DateTime.Now;
            OrderDetailsTemp o = new OrderDetailsTemp();


            //order for each chair

            var orderProductDetailsTemps = _orderTemp.OrderDetailsTemps.Where(x => pt.ProductId.Equals(x.ProductId)).ToList();

            //TODO: Take from wake house
            //                // go to warehouse, check and get the ingredient to make product
            //                if (!TakeFromWareHouseData(o, it))       
            //                {
            //                    return;
            //                }

            // add a product to order
            if (orderProductDetailsTemps.Count == 0)
            {
                o.OrdertempId = _orderTemp.OrdertempId;
                o.ProductId = pt.ProductId;
                o.SelectedStats = pt.StdStats;
                o.Note = "";
                o.Quan = 1;
                o.IsPrinted = 0;
                o.Discount = pt.Discount;

                _orderTemp.OrderDetailsTemps.Add(o);

            }
            else
            {
                foreach (var order in orderProductDetailsTemps)
                {
                    if (!order.SelectedStats.Equals(pt.StdStats) || !order.Note.Equals("") || order.IsPrinted != 0)
                    {
                        o.OrdertempId = _orderTemp.OrdertempId;
                        o.ProductId = pt.ProductId;
                        o.SelectedStats = pt.StdStats;
                        o.Note = "";
                        o.Quan = 1;
                        o.IsPrinted = 0;
                        o.Discount = pt.Discount;

                        _orderTemp.OrderDetailsTemps.Add(o);

                        break;
                    }

                    if (order.SelectedStats.Equals(pt.StdStats) && order.Note.Equals("") && order.IsPrinted == 0)
                    {
                        order.ProductId = pt.ProductId;
                        order.Quan++;

                        break;
                    }
                }
            }
        }

        public void updateOrderDetail(int index,String value)
        {
            var tempdata = OrderDetailsTempData(index);
            tempdata.SelectedStats = value;
            foreach (var cho in _orderTemp.OrderDetailsTemps)
            {
                if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                    && cho.ProductId.Equals(tempdata.ProductId)
                    && cho.SelectedStats.Equals(tempdata.SelectedStats)
                    && cho.Note.Equals(tempdata.Note)
                    && (cho.IsPrinted == 0 && tempdata.IsPrinted == 0))
                {
                    cho.Quan += _orderTemp.OrderDetailsTemps.ElementAt(index).Quan;
                }
            }
        }

        private OrderDetailsTemp OrderDetailsTempData(int index)
        {
            OrderDetailsTemp tempdata = new OrderDetailsTemp();
            tempdata.OrdertempId = _orderTemp.OrderDetailsTemps.ElementAt(index).OrdertempId;
            tempdata.ProductId = _orderTemp.OrderDetailsTemps.ElementAt(index).ProductId;
            tempdata.StatusItems = _orderTemp.OrderDetailsTemps.ElementAt(index).StatusItems;
            tempdata.Quan = _orderTemp.OrderDetailsTemps.ElementAt(index).Quan;
            tempdata.Note = _orderTemp.OrderDetailsTemps.ElementAt(index).Note;
            tempdata.IsPrinted = 0;
            return tempdata;
        }

        public void updateOrderNote(int index, String note)
        {
            var tempdata = OrderDetailsTempData(index);
            tempdata.Note = note;
            if (_orderTemp.OrderDetailsTemps.ElementAt(index).Quan == 1)
            {
                foreach (var cho in _orderTemp.OrderDetailsTemps)
                {
                    if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                        && cho.ProductId.Equals(tempdata.ProductId)
                        && cho.SelectedStats.Equals(tempdata.SelectedStats)
                        && cho.Note.Equals(tempdata.Note)
                        && (cho.IsPrinted == 0 && tempdata.IsPrinted == 0))
                    {
                        cho.Quan++;
                        return;
                    }
                }
                return;
            }

            if (_orderTemp.OrderDetailsTemps.ElementAt(index).Quan > 1)
            {
                foreach (var cho in _orderTemp.OrderDetailsTemps)
                {
                    if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                        && cho.ProductId.Equals(tempdata.ProductId)
                        && cho.SelectedStats.Equals(tempdata.SelectedStats)
                        && cho.Note.Equals(tempdata.Note)
                        && (cho.IsPrinted == 0 && tempdata.IsPrinted == 0))
                    {
                        tempdata.Note = _orderTemp.OrderDetailsTemps.ElementAt(index).Note;
                        tempdata.Quan--;
                        cho.Quan++;

                        return;
                    }
                }

                foreach (var cho in _orderTemp.OrderDetailsTemps)
                {
                    if (cho.OrdertempId.Equals(tempdata.OrdertempId)
                        && cho.ProductId.Equals(tempdata.ProductId)
                        && cho.SelectedStats.Equals(tempdata.SelectedStats)
                        && !cho.Note.Equals(tempdata.Note))
                    {
                        _orderTemp.OrderDetailsTemps.ElementAt(index).Quan--;
                        tempdata.Quan = 1;

                        return;
                    }
                }
            }
        }

        public void deleteOrderDetail(int index, bool isDone)
        {
            if (_orderTemp.OrderDetailsTemps.ElementAt(index).Quan > 1)
            {
                if (!isDone)
                {
                    GiveBackToWareHouseData(_orderTemp.OrderDetailsTemps.ElementAt(index), 1);
                }
                _orderTemp.OrderDetailsTemps.ElementAt(index).Quan--;
            }
            else
            {
                if (!isDone)
                {
                    GiveBackToWareHouseData(_orderTemp.OrderDetailsTemps.ElementAt(index), 1);
                }
                _orderTemp.OrderDetailsTemps.Remove(_orderTemp.OrderDetailsTemps.ElementAt(index));
            }
        }


        public IEnumerable<OrderDetails_Product_Joiner> getOrderDetailsDisplay()
        {
            // chuyen product_id thanh product name
            var query = from orderdetails in _orderTemp.OrderDetailsTemps
                join product in _unitofwork.ProductRepository.Get()
                    on orderdetails.ProductId equals product.ProductId

                select new OrderDetails_Product_Joiner
                {
                    OrderDetailsTemp = orderdetails,
                    Product = product
                };

            return query;

        }

        public void loadTotalPrice()
        {

            // chuyen product_id thanh product name
            var query_item_in_ordertails = from orderdetails in _orderTemp.OrderDetailsTemps
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
            TotalWithDiscount = (decimal)(((float)Total * (100 - _orderTemp.Discount)) / 100.0);

            /*
             * If the current order isn't in Set Order  Mode
             * Use the casual calculate method to compute the Total Price
             */

            _orderTemp.TotalPrice = (decimal)Math.Round(TotalWithDiscount, 3);
            _orderTemp.TotalPriceNonDisc = (decimal)Math.Round(Total, 3);
            _orderTemp.Svc = Math.Round(Svc, 3);
            _orderTemp.Vat = Math.Round(Vat, 3);
            _orderTemp.SaleValue = Math.Round(SaleValue, 3);
        }

        public bool ConvertTableToOrder(OrderNote newOrder)
        {
            if (_orderTemp != null)
            {
                newOrder.OrdernoteId = _unitofwork.OrderRepository.AutoGeneteId_DBAsowell(newOrder).OrdernoteId;
                newOrder.CusId = _orderTemp.CusId;
                newOrder.EmpId = _orderTemp.EmpId;
                newOrder.Pax = _orderTemp.Pax;
                newOrder.Ordertime = _orderTemp.Ordertime;
                newOrder.TotalPriceNonDisc = _orderTemp.TotalPriceNonDisc;
                newOrder.TotalPrice = _orderTemp.TotalPrice;
                newOrder.Svc = _orderTemp.Svc;
                newOrder.Vat = _orderTemp.Vat;
                newOrder.SaleValue = _orderTemp.SaleValue;
                newOrder.Discount = _orderTemp.Discount;
                newOrder.SubEmpId = _orderTemp.SubEmpId;
            }
            else return false;

            Dictionary<string, OrderNoteDetail> newDetailsList = new Dictionary<string, OrderNoteDetail>();
            foreach (var details in _orderTemp.OrderDetailsTemps)
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

        public void saveOrderToDB(OrderNote orderNote)
        {
            _unitofwork.OrderRepository.Insert(orderNote);
            _unitofwork.Save();
        }

        public void clearOrder()
        {

            _orderTemp.EmpId = (App.Current.Properties["EmpLogin"] as Employee).EmpId;
            _orderTemp.CusId = "CUS0000001";
            _orderTemp.Discount = 0;
            _orderTemp.Ordertime = DateTime.Now;
            _orderTemp.TotalPriceNonDisc = 0;
            _orderTemp.SaleValue = 0;
            _orderTemp.Svc = 0;
            _orderTemp.Vat = 0;
            _orderTemp.TotalPrice = 0;
            _orderTemp.CustomerPay = 0;
            _orderTemp.PayBack = 0;
            _orderTemp.SubEmpId = "";
            _orderTemp.Pax = 0;
            _orderTemp.OrderMode = 0;
            _orderTemp.OrderDetailsTemps.Clear();
        }

        private bool TakeFromWareHouseData(OrderDetailsTemp orderDetails, Product orderingProduct)
        {
            var prodOfOrderDetails =
                _unitofwork.ProductRepository.Get(x => x.ProductId.Equals(orderingProduct.ProductId), includeProperties: "ProductDetails").FirstOrDefault();
            if (prodOfOrderDetails != null)
            {
                // if product have no product details
                if (prodOfOrderDetails.ProductDetails.Count == 0)
                {
                    // still allow to order but no ingredient relate to this product for tracking
                    return true;
                }


                var wareHouseDict = new Dictionary<WareHouse, double?>();
                // going to warehouse and take the contain of each ingredient
                foreach (var prodDetails in prodOfOrderDetails.ProductDetails)
                {
                    var quan = prodDetails.Quan;
                    var ingd =
                        _unitofwork.IngredientRepository.Get(x => x.IgdId.Equals(prodDetails.IgdId))
                            .FirstOrDefault();
                    if (ingd == null)
                    {
                        MessageBox.Show("Something went wrong cause of the Ingredient's information");
                        return false;
                    }
                    var wareHouse =
                        _unitofwork.WareHouseRepository.Get(x => x.WarehouseId.Equals(ingd.WarehouseId))
                            .FirstOrDefault();
                    if (wareHouse == null)
                    {
                        MessageBox.Show("Something went wrong cause of the WareHouse's information");
                        return false;
                    }

                    var temple_Contain = wareHouse.Contain;

                    if (temple_Contain < quan)
                    {
                        MessageBox.Show("This Product can not order now. Please check to WareHouse for Ingredient's stock!");
                        return false;
                    }
                    else
                    {
                        temple_Contain -= quan;
                    }

                    wareHouseDict.Add(wareHouse, temple_Contain);
                }

                // when all ingredient are enough to make product
                foreach (var item in wareHouseDict)
                {
                    item.Key.Contain = item.Value;
                }
                _unitofwork.Save();
            }
            else
            {
                MessageBox.Show("This Product is not existed in database! Please check the Product's information");
                return false;
            }

            return true;
        }

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
    }
}
