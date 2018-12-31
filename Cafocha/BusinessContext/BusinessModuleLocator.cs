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

        public BusinessModuleLocator()
        {
            _repositoryLocator = new RepositoryLocator();
        }

        public BusinessModuleLocator(RepositoryLocator repositoryLocator)
        {
            _repositoryLocator = repositoryLocator;
        }

        private TakingOrderModule _takingOrderModule;
        private AdminModule _adminModule;
        private CustomerModule _customerModule;
        private EmployeeModule _employeeModule;
        private ProductModule _productModule;
        private ReceiptNoteModule _receiptNoteModule;
        private IngredientModule _ingredientModule;
        private OrderModule _orderModule;
        private WarehouseModule _warehouseModule;

        public RepositoryLocator RepositoryLocator
        {
            get { return _repositoryLocator ?? (_repositoryLocator = new RepositoryLocator()); }
        }

        public WarehouseModule WarehouseModule
        {
            get { return _warehouseModule ?? (_warehouseModule = new WarehouseModule()); }
        }
        public OrderModule OrderModule
        {
            get { return _orderModule ?? (_orderModule = new OrderModule()); }
        }
        public IngredientModule IngredientModule
        {
            get { return _ingredientModule ?? (_ingredientModule = new IngredientModule()); }
        }
        public ReceiptNoteModule ReceiptNoteModule
        {
            get { return _receiptNoteModule ?? (_receiptNoteModule = new ReceiptNoteModule()); }
        }
        public ProductModule ProductModule
        {
            get { return _productModule ?? (_productModule = new ProductModule()); }
        }
        public EmployeeModule EmployeeModule
        {
            get { return _employeeModule ?? (_employeeModule = new EmployeeModule()); }
        }

        public TakingOrderModule TakingOrderModule
        {
            get { return _takingOrderModule ?? (_takingOrderModule = new TakingOrderModule()); }
        }

        public AdminModule AdminModule      
        {
            get { return _adminModule ?? (_adminModule = new AdminModule()); }
        }

        public CustomerModule CustomerModule
        {
            get { return _customerModule ?? (_customerModule = new CustomerModule()); }
        }
    }
}
