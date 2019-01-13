using System.Collections.Generic;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.User
{
    public class CustomerModule
    {
        private readonly RepositoryLocator _unitofwork;

        public CustomerModule()
        {
        }

        public CustomerModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public Customer getCustomer(string customerID)
        {
            return _unitofwork.CustomerRepository.GetById(customerID);
        }

        public IEnumerable<Customer> getAllCustomer()
        {
            return _unitofwork.CustomerRepository.Get(x => x.Deleted.Equals(0));
        }

        public void insertCustomer(Customer customer)
        {
            _unitofwork.CustomerRepository.Insert(customer);
            _unitofwork.Save();
        }

        public void updateCustomer(Customer customer)
        {
            _unitofwork.CustomerRepository.Update(customer);
            _unitofwork.Save();
        }

        public void deleteCustomer(Customer customer)
        {
            customer.Deleted = 1;
            _unitofwork.CustomerRepository.Update(customer);
            _unitofwork.Save();
        }
    }
}