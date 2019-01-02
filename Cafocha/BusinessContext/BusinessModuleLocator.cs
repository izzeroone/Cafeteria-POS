using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.BusinessContext.User;
using Cafocha.BusinessContext.WarehouseWorkspace;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext
{
    public class BusinessModuleLocator
    {
        private AdminModule _adminModule;
        private string _connectionString;
        private CustomerModule _customerModule;
        private EmployeeModule _employeeModule;
        private OrderModule _orderModule;
        private ProductModule _productModule;
        private RepositoryLocator _repositoryLocator;

        private TakingOrderModule _takingOrderModule;
        private WarehouseModule _warehouseModule;

        public BusinessModuleLocator()
        {
            if (string.IsNullOrWhiteSpace(_connectionString))
                _repositoryLocator = new RepositoryLocator();
            else
                _repositoryLocator = new RepositoryLocator(_connectionString);
        }

        public BusinessModuleLocator(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
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
                if (string.IsNullOrWhiteSpace(_connectionString))
                    _repositoryLocator = new RepositoryLocator();
                else
                    _repositoryLocator = new RepositoryLocator(_connectionString);
            }
        }

        public RepositoryLocator RepositoryLocator =>
            _repositoryLocator ?? (_repositoryLocator = new RepositoryLocator());

        public WarehouseModule WarehouseModule =>
            _warehouseModule ?? (_warehouseModule = new WarehouseModule(_repositoryLocator));

        public OrderModule OrderModule => _orderModule ?? (_orderModule = new OrderModule(_repositoryLocator));

        public ProductModule ProductModule =>
            _productModule ?? (_productModule = new ProductModule(_repositoryLocator));

        public EmployeeModule EmployeeModule =>
            _employeeModule ?? (_employeeModule = new EmployeeModule(_repositoryLocator));

        public TakingOrderModule TakingOrderModule =>
            _takingOrderModule ?? (_takingOrderModule = new TakingOrderModule(_repositoryLocator));

        public AdminModule AdminModule => _adminModule ?? (_adminModule = new AdminModule(_repositoryLocator));

        public CustomerModule CustomerModule =>
            _customerModule ?? (_customerModule = new CustomerModule(_repositoryLocator));
    }
}