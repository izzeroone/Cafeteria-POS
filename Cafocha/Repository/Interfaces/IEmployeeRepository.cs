using System;
using System.Collections.Generic;
using Cafocha.Entities;

namespace Cafocha.Repository.Interfaces
{
    public interface IEmployeeRepository : IDisposable
    {
        IEnumerable<Employee> GetAllEmployees();
        Employee GetEmployeeById(string employeeId);
        void InsertEmployee(Employee employee);
        void DeleteEmployee(string employeeId);
        void UpdateEmployee(Employee employee);
        void Save();
    }
}
