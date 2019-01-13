using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cafocha.Entities;
using Cafocha.GUI.EmployeeWorkSpace;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.User
{
    public class EmployeeModule
    {
        private static List<EmpLoginList> _emploglist = new List<EmpLoginList>();
        private static EmpLoginList _workingEmployee;
        private List<Employee> _employee = new List<Employee>();
        private readonly RepositoryLocator _unitofwork;

        public EmployeeModule()
        {
            _unitofwork = new RepositoryLocator();
            _employee = getEmployees().ToList();
        }

        public EmployeeModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
            _employee = getEmployees().ToList();
        }

        public List<EmpLoginList> Emploglist
        {
            get => _emploglist;
            set => _emploglist = value;
        }

        public static EmpLoginList WorkingEmployee
        {
            get => _workingEmployee;
            set => _workingEmployee = value;
        }
        public IEnumerable<Employee> getEmployees()
        {
            return _unitofwork.EmployeeRepository.Get(x => x.Deleted == 0);
        }

        public IEnumerable<Employee> getEmployeeWithAd(string adId)
        {
            return _unitofwork.EmployeeRepository.Get(x => x.Manager.Equals(adId) && x.Deleted.Equals(0));
        }

        public bool isUsernameAvaiable(string username)
        {
            return _unitofwork.EmployeeRepository.Get(x => x.Username.Equals(username)) == null;
        }
        public void updateEmployeePassword(Employee employee, string newPassword)
        {
            employee.Pass = newPassword;
            _unitofwork.EmployeeRepository.Update(employee);
            _unitofwork.Save();

            var emplog = _emploglist.First(x => x.Emp.Username.Equals(employee.Username));
            if (emplog != null) emplog.Emp.Pass = newPassword;
        }

        public Employee getEmployee(string username)
        {
            return _unitofwork.EmployeeRepository.Get(e => e.Username.Equals(username)).FirstOrDefault();
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
            emptLoginList.EmpSal = _unitofwork.SalaryNoteRepository.Get(sle =>
                sle.EmpId.Equals(emptLoginList.Emp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) &&
                sle.ForYear.Equals(DateTime.Now.Year)).First();
            emptLoginList.EmpSal.WorkHour += workH.Hours + workH.Minutes / 60.0 + workH.Seconds / 3600.0;
            emptLoginList.EmpSal.SalaryValue = (decimal) (emptLoginList.EmpSal.WorkHour * emptLoginList.Emp.HourWage);
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
            await Task.Run(() =>
            {
                foreach (var emp in _employee)
                    if (emp.Username.Equals(username) && emp.DecryptedPass.Equals(password) ||
                        emp.DecryptedCode.Equals(code))
                    {
                        var chemp = _emploglist.Where(x => x.Emp.EmpId.Equals(emp.EmpId)).ToList();
                        if (chemp.Count != 0)
                        {
                            MessageBox.Show("This employee is already login!");
                            return false;
                        }
                        _workingEmployee = new EmpLoginList();
                        _workingEmployee.Emp = emp;
                                    Application.Current.Properties["EmpWorking"] = WorkingEmployee.Emp;
                        _emploglist.Add(new EmpLoginList
                        {
                            Emp = emp,
                            TimePercent = 0
                        });

                        return true;
                    }

                return false;
            });


            return false;
        }

        public void startWorkingRecord()
        {
            var empSalaryNote = _unitofwork.SalaryNoteRepository.Get(sle =>
                sle.EmpId.Equals(_workingEmployee.Emp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) &&
                sle.ForYear.Equals(DateTime.Now.Year)).LastOrDefault();

            if (empSalaryNote == null || empSalaryNote.IsPaid == 1)
            {
                empSalaryNote = new SalaryNote
                {
                    EmpId = _workingEmployee.Emp.EmpId,
                    SalaryValue = 0,
                    WorkHour = 0,
                    ForMonth = DateTime.Now.Month,
                    ForYear = DateTime.Now.Year,
                    IsPaid = 0
                }; _unitofwork.SalaryNoteRepository.Insert(empSalaryNote);
            }


            
            _unitofwork.Save();
            var empWorkHistory = new WorkingHistory
                { StartTime = DateTime.Now, ResultSalary = empSalaryNote.SnId, EmpId = empSalaryNote.EmpId };
            _workingEmployee.EmpWH = empWorkHistory;
            _workingEmployee.EmpSal = empSalaryNote;
        }

        public void endWorkingRecord()
        {
            _workingEmployee.EmpWH.EndTime = DateTime.Now;
            _unitofwork.WorkingHistoryRepository.Insert(_workingEmployee.EmpWH);
            _unitofwork.Save();

            var workingHour = (_workingEmployee.EmpWH.EndTime - _workingEmployee.EmpWH.StartTime).TotalHours;
            _workingEmployee.EmpSal.WorkHour += workingHour;
            _workingEmployee.EmpSal.SalaryValue += (decimal)(workingHour * _workingEmployee.Emp.HourWage);
            _unitofwork.SalaryNoteRepository.Update(_workingEmployee.EmpSal);
            _unitofwork.Save();

            _workingEmployee.EmpWH = null;
            _workingEmployee.EmpSal = null;
        }

        public void endWorking()
        {
            endWorkingRecord();
        }

        public void paySalaryNote(SalaryNote salaryNote)
        {
            if (salaryNote.IsPaid == 1)
            {
                return;
            }

            foreach (var workingHistory in salaryNote.WorkingHistories)
            {
                workingHistory.IsPaid = 1;
                _unitofwork.WorkingHistoryRepository.Update(workingHistory);
            }

            salaryNote.IsPaid = 1;
            salaryNote.DatePay = DateTime.Now;
            _unitofwork.SalaryNoteRepository.Update(salaryNote);
            _unitofwork.Save();
        }
    }

    public class EmpLoginList
    {
        public Employee Emp { get; set; }

        public SalaryNote EmpSal { get; set; }

        public WorkingHistory EmpWH { get; set; }

        public int TimePercent { get; set; }

        public static implicit operator List<object>(EmpLoginList v)
        {
            throw new NotImplementedException();
        }
    }
}