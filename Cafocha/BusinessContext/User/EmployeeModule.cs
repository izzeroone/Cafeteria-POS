﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Cafocha.Entities;
using Cafocha.GUI.EmployeeWorkSpace;
using Cafocha.Repository.DAL;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;

namespace Cafocha.BusinessContext.User
{
    public class EmployeeModule
    {
        private RepositoryLocator _unitofwork;
        private static List<EmpLoginList> _emploglist = new List<EmpLoginList>();
        private List<Employee> _employee;

        public List<EmpLoginList> Emploglist
        {
            get => _emploglist;
            set => _emploglist = value;
        }

        public EmployeeModule()
        {
            _employee = getEmployees().ToList();
        }

        public EmployeeModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IEnumerable<Employee> getEmployees()
        {
            return _unitofwork.EmployeeRepository.Get(x => x.Deleted == 0);
        }

        public IEnumerable<Employee> getEmployeeWithAd(string adId)
        {
            return _unitofwork.EmployeeRepository.Get(x => x.Manager.Equals(adId) && x.Deleted.Equals(0));
        }

        public void updateEmployeePassword(Employee employee, string newPassword)
        {
            employee.Pass = newPassword;
            _unitofwork.EmployeeRepository.Update(employee);
            _unitofwork.Save();

            var emplog = _emploglist.First(x => x.Emp.Username.Equals(employee.Username));
            if (emplog != null)
            {
                emplog.Emp.Pass = newPassword;
            }
        }

        public Employee getEmployee(string username)
        {
            return  _unitofwork.EmployeeRepository.Get(e => e.Username.Equals(username)).First();
        }

        public IEnumerable<WorkingHistory> getWorkingHistoryOfEmployee(Employee employee, int month, int year)
        {
            return _unitofwork.WorkingHistoryRepository.Get(w =>
                w.EmpId.Equals(employee.EmpId) && w.StartTime.Month.Equals(month) &&
                w.StartTime.Year.Equals(year));
        }

        public void insertEmployee(Employee employee)
        {
            _unitofwork.EmployeeRepository.Insert(employee);
            _unitofwork.Save();
        }

        public void insertWorkingHistory(EmpLoginList emptLoginList)
        { 
            emptLoginList.EmpWH.EndTime = DateTime.Now;
            _unitofwork.WorkingHistoryRepository.Insert(emptLoginList.EmpWH);
            _unitofwork.Save();

            var workH = emptLoginList.EmpWH.EndTime - emptLoginList.EmpWH.StartTime;
            emptLoginList.EmpSal = _unitofwork.SalaryNoteRepository.Get(sle => sle.EmpId.Equals(emptLoginList.Emp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) && sle.ForYear.Equals(DateTime.Now.Year)).First();
            emptLoginList.EmpSal.WorkHour += workH.Hours + (workH.Minutes / 60.0) + (workH.Seconds / 3600.0);
            emptLoginList.EmpSal.SalaryValue = (decimal)(emptLoginList.EmpSal.WorkHour * emptLoginList.Emp.HourWage);
            _unitofwork.SalaryNoteRepository.Update(emptLoginList.EmpSal);
            _unitofwork.Save();

            _emploglist.Remove(emptLoginList);
        }

        public void deleteEmployee(Employee employee)
        {
            employee.Deleted = 1;
            updateemployee(employee);
        }
        public void updateemployee(Employee employee)
        {
            _unitofwork.EmployeeRepository.Update(employee);
            _unitofwork.Save();
        }

        public async Task<bool> login(string username, string password, string code)
        {
            foreach (Employee emp in _employee)
            {
                if ((emp.Username.Equals(username) && (emp.DecryptedPass.Equals(password)) || emp.DecryptedCode.Equals(code)))
                {
                    var chemp = _emploglist.Where(x => x.Emp.EmpId.Equals(emp.EmpId)).ToList();
                    if (chemp.Count != 0)
                    {
                        MessageBox.Show("This employee is already login!");
                        return false;
                    }

                    try
                    {
                        SalaryNote empSalaryNote = _unitofwork.SalaryNoteRepository.Get(sle => sle.EmpId.Equals(emp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) && sle.ForYear.Equals(DateTime.Now.Year)).First();

                        App.Current.Properties["EmpSN"] = empSalaryNote;
                        WorkingHistory empWorkHistory = new WorkingHistory { ResultSalary = empSalaryNote.SnId, EmpId = empSalaryNote.EmpId };
                        App.Current.Properties["EmpWH"] = empWorkHistory;
                    }
                    catch (Exception ex)
                    {
                        SalaryNote empSalary = new SalaryNote { EmpId = emp.EmpId, SalaryValue = 0, WorkHour = 0, ForMonth = DateTime.Now.Month, ForYear = DateTime.Now.Year, IsPaid = 0 };
                        _unitofwork.SalaryNoteRepository.Insert(empSalary);
                        _unitofwork.Save();
                        WorkingHistory empWorkHistory = new WorkingHistory { ResultSalary = empSalary.SnId, EmpId = empSalary.EmpId };
                        App.Current.Properties["EmpWH"] = empWorkHistory;
                        App.Current.Properties["EmpSN"] = empSalary;
                    }

                   _emploglist.Add(new EmpLoginList { Emp = emp, EmpSal = App.Current.Properties["EmpSN"] as SalaryNote, EmpWH = App.Current.Properties["EmpWH"] as WorkingHistory, TimePercent = 0 });

                    return true;
                    //end create
                }


            }

            return false;
        }
    }
}
