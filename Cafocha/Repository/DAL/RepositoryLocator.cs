using System;
using Cafocha.Entities;
using Cafocha.Repository.Generic;

namespace Cafocha.Repository.DAL
{
    /// <summary>
    ///     The Employee WorkSpace Of Asowell (is a Unit Of Work class) that serves one purpose: to make sure that when you use
    ///     multiple repositories that related each other in Employee WorkSpace situation, and they share a single database
    ///     context.
    ///     That way, when a Employee WorkSpace Of Asowell is complete you can call the SaveChanges method on that instance of
    ///     the context and be assured that all related changes will be coordinated.
    ///     All that the class needs is a Save method and a property for each repository.
    ///     Each repository property returns a repository instance that has been instantiated using the same database context
    ///     instance as the other repository instances.
    /// </summary>
    public class RepositoryLocator : IDisposable
    {
        private GenericRepository<AdminRe> _adminreRepository;

        // business repo
        private GenericRepository<ApplicationLog> _appLogRepository;
        private GenericRepository<ApWareHouse> _apwareHouseRepository;
        private ILocalContext _context;
        private GenericRepository<Customer> _customerRepository;

        private bool _disposed;
        private GenericRepository<Employee> _employeeRepository;
        private GenericRepository<OrderNoteDetail> _orderDetailsRepository;
        private GenericRepository<OrderNote> _orderRepository;
        private GenericRepository<ProductDetail> _productdetailsRepository;
        private GenericRepository<Product> _productRepository;
        private GenericRepository<SalaryNote> _salarynoteRepository;
        private GenericRepository<StockInDetail> _stockInDetailsRepository;
        private GenericRepository<StockIn> _stockInRepository;
        private GenericRepository<StockOutDetail> _stockOutDetailsRepository;
        private GenericRepository<StockOut> _stockOutRepository;
        private GenericRepository<Stock> _stockRepository;
        private GenericRepository<WareHouse> _wareHouseRepository;
        private GenericRepository<WorkingHistory> _workinghistoryRepository;


        public RepositoryLocator()
        {
            _context = new LocalContext();
        }

        public RepositoryLocator(string connectionString)
        {
            _context = new LocalContext(connectionString);
        }

        public RepositoryLocator(ILocalContext localContext)
        {
            _context = localContext;
        }


        public GenericRepository<WareHouse> WareHouseRepository
        {
            get
            {
                if (_wareHouseRepository == null) _wareHouseRepository = new GenericRepository<WareHouse>(_context);

                return _wareHouseRepository;
            }
        }

        public GenericRepository<ApplicationLog> AppLogRepository
        {
            get
            {
                if (_appLogRepository == null) _appLogRepository = new GenericRepository<ApplicationLog>(_context);

                return _appLogRepository;
            }
        }


        public GenericRepository<OrderNoteDetail> OrderDetailsRepository
        {
            get
            {
                if (_orderDetailsRepository == null)
                    _orderDetailsRepository = new GenericRepository<OrderNoteDetail>(_context);

                return _orderDetailsRepository;
            }
        }


        public GenericRepository<AdminRe> AdminreRepository
        {
            get
            {
                if (_adminreRepository == null) _adminreRepository = new GenericRepository<AdminRe>(_context);

                return _adminreRepository;
            }
        }

        public GenericRepository<Customer> CustomerRepository
        {
            get
            {
                if (_customerRepository == null) _customerRepository = new GenericRepository<Customer>(_context);

                return _customerRepository;
            }
        }

        public GenericRepository<Employee> EmployeeRepository =>
            _employeeRepository ?? (_employeeRepository = new GenericRepository<Employee>(_context));

        public GenericRepository<Product> ProductRepository
        {
            get
            {
                if (_productRepository == null) _productRepository = new GenericRepository<Product>(_context);

                return _productRepository;
            }
        }

        public GenericRepository<ProductDetail> ProductDetailsRepository
        {
            get
            {
                if (_productdetailsRepository == null)
                    _productdetailsRepository = new GenericRepository<ProductDetail>(_context);

                return _productdetailsRepository;
            }
        }

        public GenericRepository<OrderNote> OrderRepository
        {
            get
            {
                if (_orderRepository == null) _orderRepository = new GenericRepository<OrderNote>(_context);

                return _orderRepository;
            }
        }

        public GenericRepository<SalaryNote> SalaryNoteRepository
        {
            get
            {
                if (_salarynoteRepository == null) _salarynoteRepository = new GenericRepository<SalaryNote>(_context);

                return _salarynoteRepository;
            }
        }

        public GenericRepository<WorkingHistory> WorkingHistoryRepository
        {
            get
            {
                if (_workinghistoryRepository == null)
                    _workinghistoryRepository = new GenericRepository<WorkingHistory>(_context);

                return _workinghistoryRepository;
            }
        }

        public GenericRepository<ApWareHouse> ApWareHouseRepository
        {
            get
            {
                if (_apwareHouseRepository == null)
                    _apwareHouseRepository = new GenericRepository<ApWareHouse>(_context);
                return _apwareHouseRepository;
            }
        }

        public GenericRepository<Stock> StockRepository
        {
            get
            {
                if (_stockRepository == null) _stockRepository = new GenericRepository<Stock>(_context);
                return _stockRepository;
            }
        }

        public GenericRepository<StockIn> StockInRepository
        {
            get
            {
                if (_stockInRepository == null) _stockInRepository = new GenericRepository<StockIn>(_context);
                return _stockInRepository;
            }
        }

        public GenericRepository<StockInDetail> StockInDetailsRepository
        {
            get
            {
                if (_stockInDetailsRepository == null)
                    _stockInDetailsRepository = new GenericRepository<StockInDetail>(_context);
                return _stockInDetailsRepository;
            }
        }

        public GenericRepository<StockOut> StockOutRepository
        {
            get
            {
                if (_stockOutRepository == null) _stockOutRepository = new GenericRepository<StockOut>(_context);
                return _stockOutRepository;
            }
        }

        public GenericRepository<StockOutDetail> StockOutDetailsRepository
        {
            get
            {
                if (_stockOutDetailsRepository == null)
                    _stockOutDetailsRepository = new GenericRepository<StockOutDetail>(_context);
                return _stockOutDetailsRepository;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();

            _disposed = true;
        }

        public void Refresh()
        {
            Save();
            Dispose();
            _context = new LocalContext();
        }
    }
}