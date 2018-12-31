using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.User
{
    public class CustomerModule
    {
        RepositoryLocator _unitofwork;

        public CustomerModule()
        {
        }

        public CustomerModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
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
            _unitofwork.CustomerRepository.Insert(customer);
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
