using System;
using System.Collections.Generic;
using System.Linq;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.Helper.PrintHelper.Model
{
    public class OrderForPrint
    {
        public OrderForPrint()
        {
            OrderDetails = new List<OrderDetailsForPrint>();
        }

        public string No { get; set; }
        public string Casher { get; set; }
        public string Customer { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPriceNonDisc { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Vat { get; set; }
        public decimal SaleValue { get; set; }
        public decimal CustomerPay { get; set; }
        public decimal PayBack { get; set; }


        public List<OrderDetailsForPrint> OrderDetails { get; set; }

        /// <summary>
        ///     Convert data of OrderTemp object to OrderForPrint
        /// </summary>
        /// <param name="table">target table that contain order</param>
        public OrderForPrint GetAndConvertOrder(OrderTemp orderTemp)
        {
            var targetOrder = orderTemp;
            No = targetOrder.OrdertempId.ToString();
            Casher = targetOrder.EmpId;
            Customer = targetOrder.CusId;
            Date = targetOrder.Ordertime;
            TotalPriceNonDisc = targetOrder.TotalPriceNonDisc;
            TotalPrice = targetOrder.TotalPrice;
            Vat = targetOrder.Vat;
            SaleValue = targetOrder.SaleValue;
            CustomerPay = targetOrder.CustomerPay;
            PayBack = targetOrder.PayBack;

            return this;
        }

        public OrderForPrint GetAndConvertOrder(OrderNote targetOrder)
        {
            No = targetOrder.OrdernoteId;
            Casher = targetOrder.EmpId;
            Customer = targetOrder.CusId;
            Date = targetOrder.OrderTime;
            TotalPriceNonDisc = targetOrder.TotalPriceNonDisc;
            TotalPrice = targetOrder.TotalPrice;
            Vat = targetOrder.Vat;
            SaleValue = targetOrder.SaleValue;
            CustomerPay = targetOrder.CustomerPay;
            PayBack = targetOrder.PayBack;

            return this;
        }

        /// <summary>
        ///     Convert the list of OrderDetailsTemp's data to OrderDetailForPrint
        /// </summary>
        /// <param name="targetTable"></param>
        /// <param name="unitofwork"></param>
        /// <returns></returns>
        public OrderForPrint GetAndConverOrderDetails(RepositoryLocator unitofwork, int printType)
        {
            // get OrderDetailsTemp data from target Table
            var targetOrderDetails = new List<OrderDetailsTemp>();

            // convert
            foreach (var orderDetailsTemp in targetOrderDetails)
            {
                OrderDetails.Add(new OrderDetailsForPrint
                {
                    Quan = orderDetailsTemp.Quan,
                    ProductName = unitofwork.ProductRepository.Get(p => p.ProductId == orderDetailsTemp.ProductId)
                        .First().Name,
                    ProductPrice = unitofwork.ProductRepository.Get(p => p.ProductId == orderDetailsTemp.ProductId)
                        .First().Price,

                    ProductId = orderDetailsTemp.ProductId,
                    ProductType = unitofwork.ProductRepository.Get(p => p.ProductId == orderDetailsTemp.ProductId)
                        .First().Type,

                    Note = orderDetailsTemp.Note
                });
            }


            return this;
        }

        public OrderForPrint GetAndConverOrderDetails(OrderNote targetOrder, RepositoryLocator unitofwork)
        {
            // convert
            foreach (var orderDetailsTemp in targetOrder.OrderNoteDetails)
                OrderDetails.Add(new OrderDetailsForPrint
                {
                    Quan = orderDetailsTemp.Quan,
                    ProductName = unitofwork.ProductRepository.Get(p => p.ProductId == orderDetailsTemp.ProductId)
                        .First().Name,
                    ProductPrice = unitofwork.ProductRepository.Get(p => p.ProductId == orderDetailsTemp.ProductId)
                        .First().Price,

                    ProductId = orderDetailsTemp.ProductId,
                    ProductType = unitofwork.ProductRepository.Get(p => p.ProductId == orderDetailsTemp.ProductId)
                        .First().Type
                });


            return this;
        }


        /// <summary>
        ///     Apply the Receipt Printing to OrderDetails
        /// </summary>
        /// <returns></returns>
        public List<OrderDetailsForPrint> GetOrderDetailsForReceipt()
        {
            var resultList = new List<OrderDetailsForPrint>();

            foreach (var resultItem in OrderDetails)
            {
                var ishad = false;
                foreach (var periodItem in resultList)
                    if (periodItem.ProductId == resultItem.ProductId)
                    {
                        periodItem.Quan += resultItem.Quan;

                        ishad = true;
                        break;
                    }

                if (!ishad) resultList.Add(resultItem);
            }


            return resultList;
        }

        public Dictionary<string, string> getMetaReceiptInfo()
        {
            return new Dictionary<string, string>
            {
                {"No", No},
                {"Date", Date.ToString()},
                {"Casher", Casher},
                {"Customer", Customer}
            };
        }

        public string[] getMetaReceiptTable()
        {
            return new[]
            {
                "Product Price",
                "Qty",
                "Price",
                "Amt"
            };
        }
    }
}