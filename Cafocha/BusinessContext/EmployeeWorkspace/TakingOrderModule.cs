using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.EmployeeWorkspace
{
    //A class to help manaing order
    public class TakingOrderModule
    {
        private readonly RepositoryLocator _unitofwork;

        public TakingOrderModule()
        {
            OrderTemp = new OrderTemp();
            clearOrder();
        }

        public TakingOrderModule(RepositoryLocator unitofwork)
        {
            OrderTemp = new OrderTemp();
            clearOrder();
            _unitofwork = unitofwork;
        }

        public OrderTemp OrderTemp { get; set; }


//        public ICollection<OrderDetailsTempData> OrderDetails
//        {
//            get => _orderTemp.OrderDetailsTemps;
//        }

//        public void addOrUpdateOrderDetail(OrderDetailsTemp orderTempDetail)
//        {
//            var flag = false;
//
//            var item = OrderTemp.OrderDetailsTemps.FirstOrDefault(o => o.ProductId.Equals(orderTempDetail.ProductId));
//
//            if (item != null) OrderTemp.OrderDetailsTemps.Remove(item);
//
//            OrderTemp.OrderDetailsTemps.Add(orderTempDetail);
//        }

        //This function create a new order temp or update quanity by 1 when click
        public void addProductToOrder(Product pt)
        {
            OrderTemp.Ordertime = DateTime.Now;
            var o = new OrderDetailsTemp();


            //order for each chair

            var orderProductDetailsTemps =
                OrderTemp.OrderDetailsTemps.Where(x => pt.ProductId.Equals(x.ProductId)).ToList();

            //TODO: Take from wake house
            //                // go to warehouse, check and get the ingredient to make product
            //                if (!TakeFromWareHouseData(o, it))       
            //                {
            //                    return;
            //                }

            // add a product to order
            if (orderProductDetailsTemps.Count == 0)
            {
                o.OrdertempId = OrderTemp.OrdertempId;
                o.ProductId = pt.ProductId;
                o.SelectedStats = pt.StdStats;
                o.Note = "";
                o.Quan = 1;
                o.IsPrinted = 0;
                o.Discount = pt.Discount;

                OrderTemp.OrderDetailsTemps.Add(o);
            }
            else
            {
                foreach (var order in orderProductDetailsTemps)
                {
                    if (!order.SelectedStats.Equals(pt.StdStats) || !order.Note.Equals("") || order.IsPrinted != 0)
                    {
                        o.OrdertempId = OrderTemp.OrdertempId;
                        o.ProductId = pt.ProductId;
                        o.SelectedStats = pt.StdStats;
                        o.Note = "";
                        o.Quan = 1;
                        o.IsPrinted = 0;
                        o.Discount = pt.Discount;

                        OrderTemp.OrderDetailsTemps.Add(o);

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

        private OrderDetailsTemp OrderDetailsTempData(int index)
        {
            var tempdata = new OrderDetailsTemp();
            tempdata.OrdertempId = OrderTemp.OrderDetailsTemps.ElementAt(index).OrdertempId;
            tempdata.ProductId = OrderTemp.OrderDetailsTemps.ElementAt(index).ProductId;
            tempdata.StatusItems = OrderTemp.OrderDetailsTemps.ElementAt(index).StatusItems;
            tempdata.Quan = OrderTemp.OrderDetailsTemps.ElementAt(index).Quan;
            tempdata.Note = OrderTemp.OrderDetailsTemps.ElementAt(index).Note;
            tempdata.IsPrinted = 0;
            return tempdata;
        }

        public void updateOrderNote(int index, string note)
        {
            OrderTemp.OrderDetailsTemps.ElementAt(index).Note = note;
        }

//        public void deleteOrderDetail(int index, bool isDone)
//        {
//            if (_orderTemp.OrderDetailsTemps.ElementAt(index).Quan > 1)
//            {
//                if (!isDone)
//                {
//                    GiveBackToWareHouseData(_orderTemp.OrderDetailsTemps.ElementAt(index), 1);
//                }
//                _orderTemp.OrderDetailsTemps.ElementAt(index).Quan--;
//            }
//            else
//            {
//                if (!isDone)
//                {
//                    GiveBackToWareHouseData(_orderTemp.OrderDetailsTemps.ElementAt(index), 1);
//                }
//                _orderTemp.OrderDetailsTemps.Remove(_orderTemp.OrderDetailsTemps.ElementAt(index));
//            }
//        }


        public IEnumerable<OrderDetails_Product_Joiner> getOrderDetailsDisplay()
        {
            // chuyen product_id thanh product name
            var query = from orderdetails in OrderTemp.OrderDetailsTemps
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
            var query_item_in_ordertails = from orderdetails in OrderTemp.OrderDetailsTemps
                join product in _unitofwork.ProductRepository.Get()
                    on orderdetails.ProductId equals product.ProductId
                select new
                {
                    item_quan = orderdetails.Quan,
                    item_price = product.Price,
                    item_discount = product.Discount
                };

            // calculate totalPriceNonDisc and TotalPrice
            decimal Vat = 0;
            decimal SaleValue = 0;
            decimal TotalWithDiscount = 0;
            decimal Total = 0;
            foreach (var item in query_item_in_ordertails)
                Total = (decimal) ((float) Total +
                                   (float) (item.item_quan *
                                            ((float) item.item_price * ((100 - item.item_discount) / 100.0))));

            SaleValue = Total * 90 / 100;
            Vat = Total * 10 / 100;
            TotalWithDiscount = (decimal) ((float) Total * (100 - OrderTemp.Discount) / 100.0);

            /*
             * If the current order isn't in Set Order  Mode
             * Use the casual calculate method to compute the Total Price
             */

            OrderTemp.TotalPrice = Math.Round(TotalWithDiscount, 3);
            OrderTemp.TotalPriceNonDisc = Math.Round(Total, 3);
            OrderTemp.Svc = 0;
            OrderTemp.Vat = Math.Round(Vat, 3);
            OrderTemp.SaleValue = Math.Round(SaleValue, 3);
        }

        public bool convertTableToOrder(OrderNote newOrder)
        {
            if (OrderTemp != null)
            {
                newOrder.OrdernoteId = _unitofwork.OrderRepository.AutoGeneteId_DBAsowell(newOrder).OrdernoteId;
                newOrder.CusId = OrderTemp.CusId;
                newOrder.EmpId = OrderTemp.EmpId;
                newOrder.Pax = OrderTemp.Pax;
                newOrder.Ordertime = OrderTemp.Ordertime;
                newOrder.TotalPriceNonDisc = OrderTemp.TotalPriceNonDisc;
                newOrder.TotalPrice = OrderTemp.TotalPrice;
                newOrder.Svc = OrderTemp.Svc;
                newOrder.Vat = OrderTemp.Vat;
                newOrder.SaleValue = OrderTemp.SaleValue;
                newOrder.Discount = OrderTemp.Discount;
                newOrder.SubEmpId = OrderTemp.SubEmpId;
            }
            else
            {
                return false;
            }

            var newDetailsList = new Dictionary<string, OrderNoteDetail>();
            foreach (var details in OrderTemp.OrderDetailsTemps)
                if (newDetailsList.ContainsKey(details.ProductId))
                    newDetailsList[details.ProductId].Quan += details.Quan;
                else
                    newDetailsList.Add(details.ProductId, new OrderNoteDetail
                    {
                        OrdernoteId = newOrder.OrdernoteId,
                        ProductId = details.ProductId,
                        Discount = details.Discount,
                        Quan = details.Quan
                    });
            foreach (var newDetails in newDetailsList) newOrder.OrderNoteDetails.Add(newDetails.Value);

            return true;
        }

        public void saveOrderToDB(OrderNote orderNote)
        {
            _unitofwork.OrderRepository.Insert(orderNote);
            _unitofwork.Save();
        }

        public void clearOrder()
        {
            if (Application.Current != null)
            {
                OrderTemp.EmpId = (Application.Current.Properties["EmpLogin"] as Employee).EmpId;
            }
            OrderTemp.CusId = "CUS0000001";
            OrderTemp.Discount = 0;
            OrderTemp.Ordertime = DateTime.Now;
            OrderTemp.TotalPriceNonDisc = 0;
            OrderTemp.SaleValue = 0;
            OrderTemp.Svc = 0;
            OrderTemp.Vat = 0;
            OrderTemp.TotalPrice = 0;
            OrderTemp.CustomerPay = 0;
            OrderTemp.PayBack = 0;
            OrderTemp.SubEmpId = "";
            OrderTemp.Pax = 0;
            OrderTemp.OrderMode = 0;
            OrderTemp.OrderDetailsTemps.Clear();
        }

//        private bool TakeFromWareHouseData(OrderDetailsTemp orderDetails, Product orderingProduct)
//        {
//            var prodOfOrderDetails =
//                _unitofwork.ProductRepository.Get(x => x.ProductId.Equals(orderingProduct.ProductId), includeProperties: "ProductDetails").FirstOrDefault();
//            if (prodOfOrderDetails != null)
//            {
//                // if product have no product details
//                if (prodOfOrderDetails.ProductDetails.Count == 0)
//                {
//                    // still allow to order but no ingredient relate to this product for tracking
//                    return true;
//                }
//
//
//                var wareHouseDict = new Dictionary<WareHouse, double?>();
//                // going to warehouse and take the contain of each ingredient
//                foreach (var prodDetails in prodOfOrderDetails.ProductDetails)
//                {
//                    var quan = prodDetails.Quan;
//                    var ingd =
//                        _unitofwork.IngredientRepository.Get(x => x.IgdId.Equals(prodDetails.IgdId))
//                            .FirstOrDefault();
//                    if (ingd == null)
//                    {
//                        MessageBox.Show("Something went wrong cause of the Ingredient's information");
//                        return false;
//                    }
//                    var wareHouse =
//                        _unitofwork.WareHouseRepository.Get(x => x.WarehouseId.Equals(ingd.WarehouseId))
//                            .FirstOrDefault();
//                    if (wareHouse == null)
//                    {
//                        MessageBox.Show("Something went wrong cause of the WareHouse's information");
//                        return false;
//                    }
//
//                    var temple_Contain = wareHouse.Contain;
//
//                    if (temple_Contain < quan)
//                    {
//                        MessageBox.Show("This Product can not order now. Please check to WareHouse for Ingredient's stock!");
//                        return false;
//                    }
//                    else
//                    {
//                        temple_Contain -= quan;
//                    }
//
//                    wareHouseDict.Add(wareHouse, temple_Contain);
//                }
//
//                // when all ingredient are enough to make product
//                foreach (var item in wareHouseDict)
//                {
//                    item.Key.Contain = item.Value;
//                }
//                _unitofwork.Save();
//            }
//            else
//            {
//                MessageBox.Show("This Product is not existed in database! Please check the Product's information");
//                return false;
//            }
//
//            return true;
//        }
//
//        private void GiveBackToWareHouseData(OrderDetailsTemp orderDetails, int productQuan)
//        {
//            var prodOfOrderDetails =
//                _unitofwork.ProductRepository.Get(x => x.ProductId.Equals(orderDetails.ProductId), includeProperties: "ProductDetails").FirstOrDefault();
//            if (prodOfOrderDetails != null)
//            {
//                if (prodOfOrderDetails.ProductDetails.Count == 0)
//                {
//                    // not ingredient relate to this product for tracking
//                    return;
//                }
//
//                var wareHouseDict = new Dictionary<WareHouse, double?>();
//                // going to warehouse and give back the contain for each ingredient
//                foreach (var prodDetails in prodOfOrderDetails.ProductDetails)
//                {
//                    var detailsUsingQuan = prodDetails.Quan;
//                    var ingd =
//                        _unitofwork.IngredientRepository.Get(x => x.IgdId.Equals(prodDetails.IgdId)).FirstOrDefault();
//                    if (ingd == null)
//                    {
//                        MessageBox.Show("Something went wrong cause of the Ingredient's information");
//                        return;
//                    }
//                    var wareHouse =
//                        _unitofwork.WareHouseRepository.Get(x => x.WarehouseId.Equals(ingd.WarehouseId)).FirstOrDefault();
//                    if (wareHouse == null)
//                    {
//                        MessageBox.Show("Something went wrong cause of the WareHouse's information");
//                        return;
//                    }
//
//
//                    var temple_Contain = wareHouse.Contain;
//                    temple_Contain += (detailsUsingQuan * productQuan);
//                    wareHouseDict.Add(wareHouse, temple_Contain);
//                }
//
//
//                // when giving back is success full for all ingredient
//                // let update the contain data
//                foreach (var item in wareHouseDict)
//                {
//                    item.Key.Contain = item.Value;
//                }
//                //_cloudPosUnitofwork.Save();
//            }
//            else
//            {
//                MessageBox.Show("This Product is not existed in database! Please check the Product's information");
//            }
//
//        }

        public class OrderDetails_Product_Joiner : INotifyPropertyChanged
        {
            public OrderDetailsTemp OrderDetailsTemp { get; set; }
            public Product Product { get; set; }

            public string ProductName => Product.Name;

            public decimal Price => Product.Price;

            public int Quan
            {
                get => OrderDetailsTemp.Quan;
                set
                {
                    OrderDetailsTemp.Quan = value;
                    OnPropertyChanged("Quan");
                }
            }

            public ObservableCollection<string> StatusItems
            {
                get => OrderDetailsTemp.StatusItems;
                set
                {
                    OrderDetailsTemp.StatusItems = value;
                    OnPropertyChanged("StatusItems");
                }
            }

            public string SelectedStats
            {
                get => OrderDetailsTemp.SelectedStats;
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