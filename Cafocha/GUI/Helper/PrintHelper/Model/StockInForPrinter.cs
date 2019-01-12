using System;
using System.Collections.Generic;
using System.Linq;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.Helper.PrintHelper.Model
{
    public class StockInForPrint
    {
        public StockInForPrint()
        {
            StockInDetails = new List<StockInDetailForPrint>();
        }

        public string No { get; set; }
        public string Employee { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalPrice { get; set; }


        public List<StockInDetailForPrint> StockInDetails { get; set; }

        /// <summary>
        ///     Convert data of OrderTemp object to OrderForPrint
        /// </summary>
        /// <param name="table">target table that contain order</param>
        public StockInForPrint getAndConvertStockInForPrint(StockIn stockIn)
        {

            No = stockIn.SiId;
            Employee = stockIn.EmpId;
            Date = DateTime.Now;
            TotalPrice = stockIn.TotalAmount;


            return this;
        }
        public StockInForPrint getAndConvertStockInDetailsForPrint(StockIn stockIn, RepositoryLocator unitofwork)
        {
            // convert
            foreach (var strockInDetail in stockIn.StockInDetails)
                StockInDetails.Add(new StockInDetailForPrint()
                {
                    Name = unitofwork.StockRepository.Get(p => p.StoId == strockInDetail.StoId)
                        .First().Name,
                    Price = strockInDetail.ItemPrice,
                    Quan = strockInDetail.Quan,
                    TotalPrice = strockInDetail.ItemPrice * (decimal) strockInDetail.Quan,
                    Note = strockInDetail.Note
                });


            return this;
        }



        public Dictionary<string, string> getMetaReceiptInfo()
        {
            return new Dictionary<string, string>
            {
                {"No", No},
                {"Date", Date.ToString()},
                {"Employee", Employee},
            };
        }

        public string[] getMetaReceiptTable()
        {
            return new[]
            {
                "Name",
                "Qty",
                "Price",
                "TPrice",
                "Note"
            };
        }
    }
}