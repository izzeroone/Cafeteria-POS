using System;
using System.Collections.Generic;
using Cafocha.Entities;

namespace Cafocha.Repository.Interfaces
{
    public interface ICustomerRepository : IDisposable
    {
        IEnumerable<Customer> GetAllCustomers();
        Customer GetCustomerById(string customerId);
        void InsertCustomer(Customer customer);
        void DeleteCustomer(string customerId);
        void UpdateCustomer(Customer customer);
        void Save();
    }
}
