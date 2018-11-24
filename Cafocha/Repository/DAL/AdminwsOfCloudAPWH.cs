using System;
using Cafocha.Entities;
using Cafocha.Repository.Generic;


namespace Cafocha.Repository.DAL
{
    public class AdminwsOfCloudAPWH : IDisposable
    {
        private LocalContext _localContext;
        private GenericRepository<AdminRe> _adminRepository;
        private GenericRepository<ApWareHouse> _apwareHouseRepository;
        private GenericRepository<Stock> _stockRepository;
        private GenericRepository<StockIn> _stockInRepository;
        private GenericRepository<StockInDetail> _stockInDetailsRepository;
        private GenericRepository<StockOut> _stockOutRepository;
        private GenericRepository<StockOutDetail> _stockOutDetailsRepository;



        public AdminwsOfCloudAPWH()
        {
            _localContext = new LocalContext();
        }

        public AdminwsOfCloudAPWH(string connectionString)
        {
            _localContext = new LocalContext(connectionString);
        }


        public GenericRepository<AdminRe> AdminreRepository
        {
            get
            {
                if (_adminRepository == null)
                {
                    _adminRepository = new GenericRepository<AdminRe>(_localContext);
                }
                return _adminRepository;
            }
        }
        public GenericRepository<ApWareHouse> ApWareHouseRepository
        {
            get
            {
                if (_apwareHouseRepository == null)
                {
                    _apwareHouseRepository = new GenericRepository<ApWareHouse>(_localContext);
                }
                return _apwareHouseRepository;
            }
        }
        public GenericRepository<Stock> StockRepository
        {
            get
            {
                if (_stockRepository == null)
                {
                    _stockRepository = new GenericRepository<Stock>(_localContext);
                }
                return _stockRepository;
            }
        }
        public GenericRepository<StockIn> StockInRepository
        {
            get
            {
                if (_stockInRepository == null)
                {
                    _stockInRepository = new GenericRepository<StockIn>(_localContext);
                }
                return _stockInRepository;
            }
        }
        public GenericRepository<StockInDetail> StockInDetailsRepository
        {
            get
            {
                if (_stockInDetailsRepository == null)
                {
                    _stockInDetailsRepository = new GenericRepository<StockInDetail>(_localContext);
                }
                return _stockInDetailsRepository;
            }
        }
        public GenericRepository<StockOut> StockOutRepository
        {
            get
            {
                if (_stockOutRepository == null)
                {
                    _stockOutRepository = new GenericRepository<StockOut>(_localContext);
                }
                return _stockOutRepository;
            }
        }
        public GenericRepository<StockOutDetail> StockOutDetailsRepository
        {
            get
            {
                if (_stockOutDetailsRepository == null)
                {
                    _stockOutDetailsRepository = new GenericRepository<StockOutDetail>(_localContext);
                }
                return _stockOutDetailsRepository;
            }
        }

        public void Save()
        {
            _localContext.SaveChanges();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _localContext.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Refresh()
        {
            this.Save();
            this.Dispose();
            this._localContext = new LocalContext();
        }
    }
}
