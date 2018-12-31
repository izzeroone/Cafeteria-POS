using System;
using Cafocha.Entities;
using Cafocha.Repository.Generic;

namespace Cafocha.Repository.DAL
{
    /// <summary>
    /// The Employee WorkSpace Of Asowell (is a Unit Of Work class) that serves one purpose: to make sure that when you use multiple repositories that related each other in Employee WorkSpace situation, and they share a single database context. 
    /// That way, when a Employee WorkSpace Of Asowell is complete you can call the SaveChanges method on that instance of the context and be assured that all related changes will be coordinated.
    /// All that the class needs is a Save method and a property for each repository. 
    /// Each repository property returns a repository instance that has been instantiated using the same database context instance as the other repository instances.
    /// </summary>
    public class RepositoryLocator : IDisposable
    {
        private LocalContext _context;

        // business repo
        private GenericRepository<ApplicationLog> _appLogRepository;
        private GenericRepository<AdminRe> _adminreRepository;
        private GenericRepository<Customer> _customerRepository;
        private GenericRepository<Employee> _employeeRepository;
        private GenericRepository<Ingredient> _ingredientRepository;
        private GenericRepository<Product> _productRepository;
        private GenericRepository<ProductDetail> _productdetailsRepository;
        private GenericRepository<SalaryNote> _salarynoteRepository;
        private GenericRepository<WorkingHistory> _workinghistoryRepository;
        private GenericRepository<OrderNote> _orderRepository;
        private GenericRepository<OrderNoteDetail> _orderDetailsRepository;
        private GenericRepository<ReceiptNote> _receiptNoteRepository;
        private GenericRepository<ReceiptNoteDetail> _receiptnotedetailsRepository;
        private GenericRepository<WareHouse> _wareHouseRepository;
        private GenericRepository<ApWareHouse> _apwareHouseRepository;
        private GenericRepository<Stock> _stockRepository;
        private GenericRepository<StockIn> _stockInRepository;
        private GenericRepository<StockInDetail> _stockInDetailsRepository;
        private GenericRepository<StockOut> _stockOutRepository;
        private GenericRepository<StockOutDetail> _stockOutDetailsRepository;



        public RepositoryLocator()
        {
            _context = new LocalContext();
        }

        public RepositoryLocator(string connectionString)
        {
            _context = new LocalContext(connectionString);
        }



        public GenericRepository<WareHouse> WareHouseRepository
        {
            get
            {
                if (_wareHouseRepository == null)
                {
                    _wareHouseRepository = new GenericRepository<WareHouse>(_context);
                }

                return _wareHouseRepository;
            }
        }

        public GenericRepository<ApplicationLog> AppLogRepository
        {
            get
            {
                if (_appLogRepository == null)
                {
                    _appLogRepository = new GenericRepository<ApplicationLog>(_context);
                }

                return _appLogRepository;
            }
        }

        public GenericRepository<ReceiptNote> ReceiptNoteRepository
        {
            get
            {
                if (_receiptNoteRepository == null)
                {
                    _receiptNoteRepository = new GenericRepository<ReceiptNote>(_context);
                }

                return _receiptNoteRepository;
            }
        }

        public GenericRepository<ReceiptNoteDetail> ReceiptNoteDsetailsRepository
        {
            get
            {
                if (_receiptnotedetailsRepository == null)
                {
                    _receiptnotedetailsRepository = new GenericRepository<ReceiptNoteDetail>(_context);
                }

                return _receiptnotedetailsRepository;
            }
        }

        public GenericRepository<OrderNoteDetail> OrderDetailsRepository
        {
            get
            {
                if (_orderDetailsRepository == null)
                {
                    _orderDetailsRepository = new GenericRepository<OrderNoteDetail>(_context);
                }

                return _orderDetailsRepository;
            }
        }


        public GenericRepository<AdminRe> AdminreRepository
        {
            get
            {
                if (_adminreRepository == null)
                {
                    _adminreRepository = new GenericRepository<AdminRe>(_context);
                }

                return _adminreRepository;
            }
        }

        public GenericRepository<Customer> CustomerRepository
        {
            get
            {
                if (_customerRepository == null)
                {
                    _customerRepository = new GenericRepository<Customer>(_context);
                }

                return _customerRepository;
            }
        }

        public GenericRepository<Employee> EmployeeRepository
        {
            get
            {
                if (_employeeRepository == null)
                {
                    _employeeRepository = new GenericRepository<Employee>(_context);
                }

                return _employeeRepository;
            }
        }

        public GenericRepository<Ingredient> IngredientRepository
        {
            get
            {
                if (_ingredientRepository == null)
                {
                    _ingredientRepository = new GenericRepository<Ingredient>(_context);
                }

                return _ingredientRepository;
            }
        }

        public GenericRepository<Product> ProductRepository
        {
            get
            {
                if (_productRepository == null)
                {
                    _productRepository = new GenericRepository<Product>(_context);
                }

                return _productRepository;
            }
        }

        public GenericRepository<ProductDetail> ProductDetailsRepository
        {
            get
            {
                if (_productdetailsRepository == null)
                {
                    _productdetailsRepository = new GenericRepository<ProductDetail>(_context);
                }

                return _productdetailsRepository;
            }
        }

        public GenericRepository<OrderNote> OrderRepository
        {
            get
            {
                if (_orderRepository == null)
                {
                    _orderRepository = new GenericRepository<OrderNote>(_context);
                }

                return _orderRepository;
            }
        }

        public GenericRepository<SalaryNote> SalaryNoteRepository
        {
            get
            {
                if (_salarynoteRepository == null)
                {
                    _salarynoteRepository = new GenericRepository<SalaryNote>(_context);
                }

                return _salarynoteRepository;
            }
        }

        public GenericRepository<WorkingHistory> WorkingHistoryRepository
        {
            get
            {
                if (_workinghistoryRepository == null)
                {
                    _workinghistoryRepository = new GenericRepository<WorkingHistory>(_context);
                }

                return _workinghistoryRepository;
            }
        }

        public GenericRepository<ApWareHouse> ApWareHouseRepository
        {
            get
            {
                if (_apwareHouseRepository == null)
                {
                    _apwareHouseRepository = new GenericRepository<ApWareHouse>(_context);
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
                    _stockRepository = new GenericRepository<Stock>(_context);
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
                    _stockInRepository = new GenericRepository<StockIn>(_context);
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
                    _stockInDetailsRepository = new GenericRepository<StockInDetail>(_context);
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
                    _stockOutRepository = new GenericRepository<StockOut>(_context);
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
                    _stockOutDetailsRepository = new GenericRepository<StockOutDetail>(_context);
                }
                return _stockOutDetailsRepository;
            }
        }
        public void Save()
        {
            _context.SaveChanges();
        }

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
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
            this._context = new LocalContext();
        }

    }
}





       
