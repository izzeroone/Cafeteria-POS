using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.WarehouseWorkspace
{
    class WarehouseModule
    {
        private List<Stock> _stockList;
        AdminwsOfCloudAPWH _unitofwork;

        public WarehouseModule()
        {
            _unitofwork = new AdminwsOfCloudAPWH();
        }

        public WarehouseModule(AdminwsOfCloudAPWH unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public List<Stock>StockList
        {
            get => _stockList;
            set => _stockList = value;
        }

        public void loadStock()
        {
            _unitofwork.StockRepository
                .Get(c => c.Deleted.Equals(0), includeProperties: "APWareHouse").ToList();
        }

        public void updateStock()
        {
            foreach (var stock in _unitofwork.StockRepository.Get(includeProperties: "APWareHouse"))
            {
                if (stock.Deleted == 1)
                {
                    var deletedIngd = StockList.FirstOrDefault(x => x.StoId.Equals(stock.StoId));
                    if (deletedIngd != null)
                    {
                        StockList.Remove(deletedIngd);
                    }
                    continue;
                }

                var curStock = StockList.FirstOrDefault(x => x.StoId.Equals(stock.StoId));
                if (curStock == null)
                {
                    StockList.Add(stock);
                }
                else
                {
                    curStock.Name = stock.Name;
                    curStock.Info = stock.Info;
                    curStock.UnitIn = stock.UnitIn;
                    curStock.UnitOut = stock.UnitOut;
                    curStock.StandardPrice = stock.StandardPrice;

                    curStock.ApWareHouse.Contain = stock.ApWareHouse.Contain;
                    curStock.ApWareHouse.StdContain = stock.ApWareHouse.StdContain;
                }
            }
        }

        public void deleteStock(Stock stock)
        {
            stock.Deleted = 1;
            _unitofwork.StockRepository.Update(stock);
            _unitofwork.Save();
        }

        public void insertWarehouse(ApWareHouse apWareHouse)
        {
            _unitofwork.ApWareHouseRepository.Insert(apWareHouse);
            _unitofwork.Save();
        }

        public void insertStock(Stock stock)
        {
            _unitofwork.StockRepository.Insert(stock);
            _unitofwork.Save();
        }

        public void updateStock(Stock stock)
        {
            _unitofwork.StockRepository.Update(stock);
            _unitofwork.Save();
        }
    }
}
