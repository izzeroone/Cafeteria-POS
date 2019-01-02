﻿using System;
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
        public int Table { get; set; }
        public int Pax { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPriceNonDisc { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Svc { get; set; }
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
            Pax = targetOrder.Pax;
            Date = targetOrder.Ordertime;
            TotalPriceNonDisc = targetOrder.TotalPriceNonDisc;
            TotalPrice = targetOrder.TotalPrice;
            Svc = targetOrder.Svc;
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
            Pax = targetOrder.Pax;
            Table = targetOrder.Ordertable;
            Date = targetOrder.Ordertime;
            TotalPriceNonDisc = targetOrder.TotalPriceNonDisc;
            TotalPrice = targetOrder.TotalPrice;
            Svc = targetOrder.Svc;
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
                if (orderDetailsTemp.IsPrinted == 1) continue;
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

                    Note = orderDetailsTemp.Note,
                    SelectedStats = orderDetailsTemp.SelectedStats
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

        /// <summary>
        /// Apply the Kitchen Printing to OrderDetails
        /// </summary>
        /// <returns></returns>
//        public Dictionary<string, Dictionary<int, List<OrderDetailsForPrint>>> GetOrderDetailsForKitchen()
//        {
//            var resultList = new Dictionary<string, Dictionary<int, List<OrderDetailsForPrint>>>();
//
//                //Todo: add BreakFast and KidBreakFast
//                //Starter
//                var BFQuery = OrderDetails.Where(od => od.SelectedStats == "BreakFast").ToList();
//                var BFList = new Dictionary<int, List<OrderDetailsForPrint>>();
//                int BFCount = 0;
//                for (int i = 1; i <= TableOwned.ChairAmount; i++)
//                {
//                    var chairBFQuery = BFQuery.Where(od => od.ChairNumber == i).ToList();
//                    BFCount += chairBFQuery.Count;
//                    BFList.Add(i, chairBFQuery);
//                }
//                if (BFCount != 0)
//                {
//                    resultList.Add("Breakfast", BFList);
//                }
//
//                //Starter
//                var starterQuery = OrderDetails.Where(od => od.SelectedStats == "Starter").ToList();
//                var starterList = new Dictionary<int, List<OrderDetailsForPrint>>();
//                int startCount = 0;
//                for (int i = 1; i <= TableOwned.ChairAmount; i++)
//                {
//                    var chairStartQuery = starterQuery.Where(od => od.ChairNumber == i).ToList();
//                    startCount += chairStartQuery.Count;
//                    starterList.Add(i, chairStartQuery);
//                }
//                if (startCount != 0)
//                {
//                    resultList.Add("Starter", starterList);
//                }
//
//
//                //Main
//                var mainQuery = OrderDetails.Where(od => od.SelectedStats == "Main").ToList();
//                var mainList = new Dictionary<int, List<OrderDetailsForPrint>>();
//                int mainCount = 0;
//                for (int i = 1; i <= TableOwned.ChairAmount; i++)
//                {
//                    var chairMainCostQuery = mainQuery.Where(od => od.ChairNumber == i).ToList();
//                    mainCount += chairMainCostQuery.Count;
//                    mainList.Add(i, chairMainCostQuery);
//                }
//                if (mainCount != 0)
//                {
//                    resultList.Add("Main", mainList);
//                }
//
//
//
//                //Dessert
//                var dessertQuery = OrderDetails.Where(od => od.SelectedStats == "Dessert").ToList();
//                var dessertList = new Dictionary<int, List<OrderDetailsForPrint>>();
//                int dessertCount = 0;
//                for (int i = 1; i <= TableOwned.ChairAmount; i++)
//                {
//                    var chairDessertQuery = dessertQuery.Where(od => od.ChairNumber == i).ToList();
//                    dessertCount += chairDessertQuery.Count;
//                    dessertList.Add(i, chairDessertQuery);
//                }
//                if (dessertCount != 0)
//                {
//                    resultList.Add("Dessert", dessertList);
//                }
//
//            }
//
//
//
//            return resultList;
//        }

        /// <summary>
        ///     Apply the Bar Printing to OrderDetails
        /// </summary>
        /// <returns></returns>
//        public Dictionary<string, Dictionary<int, List<OrderDetailsForPrint>>> GetOrderDetailsForBar()
//        {
//            var resultList = new Dictionary<string, Dictionary<int, List<OrderDetailsForPrint>>>();
//
//
//            if (TableOwned != null)
//            {
//                //Beverage
//                var drinkQuery = OrderDetails.Where(od => od.SelectedStats == "Drink").ToList();
//
//
//                //Coffee
//                var coffeeQuery = drinkQuery.Where(od => od.ProductType == (int)ProductType.Coffee).ToList();
//                var coffeeList = new Dictionary<int, List<OrderDetailsForPrint>>();
//                int coffCount = 0;
//                for (int i = 1; i <= TableOwned.ChairAmount; i++)
//                {
//                    var chairCoffeeQuery = coffeeQuery.Where(od => od.ChairNumber == i).ToList();
//                    coffCount += chairCoffeeQuery.Count;
//                    coffeeList.Add(i, chairCoffeeQuery);
//                }
//                if (coffCount != 0)
//                {
//                    resultList.Add(ProductType.Coffee.ToString(), coffeeList);
//                }
//
//
//                //Beverage
//                var beverageQuery = drinkQuery.Where(od => od.ProductType == (int)ProductType.Beverage).ToList();
//                var beverageList = new Dictionary<int, List<OrderDetailsForPrint>>();
//                int bevCount = 0;
//                for (int i = 1; i <= TableOwned.ChairAmount; i++)
//                {
//                    var chairBeverageQuery = beverageQuery.Where(od => od.ChairNumber == i).ToList();
//                    bevCount += chairBeverageQuery.Count;
//                    beverageList.Add(i, chairBeverageQuery);
//                }
//                if (bevCount != 0)
//                {
//                    resultList.Add(ProductType.Beverage.ToString(), beverageList);
//                }
//
//
//
//                //Cocktall
//                var cocktailQuery = drinkQuery.Where(od => od.ProductType == (int)ProductType.Cocktail).ToList();
//                var cocktailList = new Dictionary<int, List<OrderDetailsForPrint>>();
//                int cockCount = 0;
//                for (int i = 1; i <= TableOwned.ChairAmount; i++)
//                {
//                    var chairCocktailQuery = cocktailQuery.Where(od => od.ChairNumber == i).ToList();
//                    cockCount += chairCocktailQuery.Count;
//                    cocktailList.Add(i, chairCocktailQuery);
//                }
//                if (cockCount != 0)
//                {
//                    resultList.Add(ProductType.Cocktail.ToString(), cocktailList);
//                }
//
//
//
//                //Wine
//                var wineQuery = drinkQuery.Where(od => od.ProductType == (int)ProductType.Wine).ToList();
//                var wineList = new Dictionary<int, List<OrderDetailsForPrint>>();
//                int wineCount = 0;
//                for (int i = 1; i <= TableOwned.ChairAmount; i++)
//                {
//                    var chairWineQuery = wineQuery.Where(od => od.ChairNumber == i).ToList();
//                    wineCount += chairWineQuery.Count;
//                    wineList.Add(i, chairWineQuery);
//                }
//                if (wineCount != 0)
//                {
//                    resultList.Add(ProductType.Wine.ToString(), wineList);
//                }
//
//
//
//                //Beer
//                var beerQuery = drinkQuery.Where(od => od.ProductType == (int)ProductType.Beer).ToList();
//                var beerList = new Dictionary<int, List<OrderDetailsForPrint>>();
//                int beerCount = 0;
//                for (int i = 1; i <= TableOwned.ChairAmount; i++)
//                {
//                    var chairBeerQuery = beerQuery.Where(od => od.ChairNumber == i).ToList();
//                    beerCount += chairBeerQuery.Count;
//                    beerList.Add(i, chairBeerQuery);
//                }
//                if (beerCount != 0)
//                {
//                    resultList.Add(ProductType.Beer.ToString(), beerList);
//                }
//
//            }
//
//            return resultList;
//        }

        // Receipt Meta
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

        // Kitchen Meta
        public string[] getMetaKitchenTable()
        {
            return new[]
            {
                "Position",
                "Qty",
                "Product"
            };
        }

        // Bar Meta
        public string[] getMetaBarTable()
        {
            return new[]
            {
                "Position",
                "Qty",
                "Product"
            };
        }
    }
}