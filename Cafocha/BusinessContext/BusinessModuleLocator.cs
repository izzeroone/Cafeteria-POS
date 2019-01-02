using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.BusinessContext.User;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext
{
    public class BusinessModuleLocator
    {
        private RepositoryLocator _repositoryLocator;
        private string _connectionString;
        public BusinessModuleLocator()
        {
            if (String.IsNullOrWhiteSpace(_connectionString))
            {
                _repositoryLocator = new RepositoryLocator();
            }
            else
            {
                _repositoryLocator = new RepositoryLocator(_connectionString);
            }
        }

        public BusinessModuleLocator(string connectionString)
        {
            if (String.IsNullOrWhiteSpace(connectionString))
            {
                _repositoryLocator = new RepositoryLocator();
            }
            else
            {
                _connectionString = connectionString;
                _repositoryLocator = new RepositoryLocator(_connectionString);
            }
        }

        public BusinessModuleLocator(RepositoryLocator repositoryLocator)
        {
            _repositoryLocator = repositoryLocator;
        }

        public string ConnectionString
        {
            get => _connectionString;
            set
            {
                _connectionString = value;
                if (String.IsNullOrWhiteSpace(_connectionString))
                {
                    _repositoryLocator = new RepositoryLocator();
                }
                else
                {
                    _repositoryLocator = new RepositoryLocator(_connectionString);
                }
            }
        }

        private TakingOrderModule _takingOrderModule;
        private AdminModule _adminModule;
        private CustomerModule _customerModule;
        private EmployeeModule _employeeModule;
        private ProductModule _productModule;
        private OrderModule _orderModule;
        private WarehouseModule _warehouseModule;

        public RepositoryLocator RepositoryLocator
        {
            get { return _repositoryLocator ?? (_repositoryLocator = new RepositoryLocator()); }
        }

        public WarehouseModule WarehouseModule
        {
            get { return _warehouseModule ?? (_warehouseModule = new WarehouseModule(_repositoryLocator)); }
        }
        public OrderModule OrderModule
        {
            get { return _orderModule ?? (_orderModule = new OrderModule(_repositoryLocator)); }
        }

        public ProductModule ProductModule
        {
            get { return _productModule ?? (_productModule = new ProductModule(_repositoryLocator)); }
        }
        public EmployeeModule EmployeeModule
        {
            get { return _employeeModule ?? (_employeeModule = new EmployeeModule(_repositoryLocator)); }
        }

        public TakingOrderModule TakingOrderModule
        {
            get { return _takingOrderModule ?? (_takingOrderModule = new TakingOrderModule(_repositoryLocator)); }
        }

        public AdminModule AdminModule      
        {
            get { return _adminModule ?? (_adminModule = new AdminModule(_repositoryLocator)); }
        }

        public CustomerModule CustomerModule
        {
            get { return _customerModule ?? (_customerModule = new CustomerModule(_repositoryLocator)); }
        }
    }
}
