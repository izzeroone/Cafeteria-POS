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

        //This function create a new order temp or update quanity by 1 when click
        public void addProductToOrder(Product pt)
        {
            OrderTemp.Ordertime = DateTime.Now;
            var o = new OrderDetailsTemp();

            var orderProduct =
                OrderTemp.OrderDetailsTemps.FirstOrDefault(x => pt.ProductId.Equals(x.ProductId));

            if (orderProduct == null)
            {
                o.OrdertempId = OrderTemp.OrdertempId;
                o.ProductId = pt.ProductId;
                o.Note = "";
                o.Quan = 1;
                o.Discount = pt.Discount;

                OrderTemp.OrderDetailsTemps.Add(o);
            }
            else
            {
                orderProduct.ProductId = pt.ProductId;
                orderProduct.Quan++;
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
            return tempdata;
        }

        public void updateOrderNote(int index, string note)
        {
            OrderTemp.OrderDetailsTemps.ElementAt(index).Note = note;
        }

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
                newOrder.OrderTime = OrderTemp.Ordertime;
                newOrder.TotalPriceNonDisc = OrderTemp.TotalPriceNonDisc;
                newOrder.TotalPrice = OrderTemp.TotalPrice;
                newOrder.Vat = OrderTemp.Vat;
                newOrder.SaleValue = OrderTemp.SaleValue;
                newOrder.Discount = OrderTemp.Discount;
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
            OrderTemp.EmpId = (Application.Current.Properties["EmpWorking"] as Employee).EmpId;
            OrderTemp.CusId = "CUS0000001";
            OrderTemp.Discount = 0;
            OrderTemp.Ordertime = DateTime.Now;
            OrderTemp.TotalPriceNonDisc = 0;
            OrderTemp.SaleValue = 0;
            OrderTemp.Vat = 0;
            OrderTemp.TotalPrice = 0;
            OrderTemp.CustomerPay = 0;
            OrderTemp.PayBack = 0;
            OrderTemp.OrderDetailsTemps.Clear();
        }

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

            public event PropertyChangedEventHandler PropertyChanged;

            public void OnPropertyChanged(string propertyName)
            {
                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}