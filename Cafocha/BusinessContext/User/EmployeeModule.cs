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
        private List<Employee> _employeeList = new List<Employee>();
        private readonly RepositoryLocator _unitofwork;

        public EmployeeModule()
        {
            _unitofwork = new RepositoryLocator();
            _employeeList = getEmployees().ToList();
        }

        public EmployeeModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
            _employeeList = getEmployees().ToList();
        }

        public List<EmpLoginList> Emploglist
        {
            get => _emploglist;
            set => _emploglist = value;
        }

        public static EmpLoginList WorkingEmployee
        {
            get
            {
                foreach (var employee in _emploglist)
                {
                    if (employee.IsStartWorking.Equals(true))
                    {
                        return employee;
                    }
                }

                return null;
            }
            set
            {
                if (value == null)
                {
                    foreach (var employee in _emploglist)
                    {
                        employee.IsStartWorking = false;
                    }
                }
            }
            
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
            updateEmployee(employee);
        }

        public void updateEmployee(Employee employee)
        {
            _unitofwork.EmployeeRepository.Update(employee);
            _unitofwork.Save();
        }

        public async Task<bool> login(string username, string password, string code)
        {
            await Task.Run(() =>
            {
                foreach (var emp in getEmployees())
                    if (emp.Username.Equals(username) && emp.DecryptedPass.Equals(password) ||
                        emp.DecryptedCode.Equals(code))
                    {
                        var chemp = _emploglist.Where(x => x.Emp.EmpId.Equals(emp.EmpId)).ToList();
                        if (chemp.Count != 0)
                        {
                            MessageBox.Show("Nhân viên này đã đăng nhập!");
                            return false;
                        }

                         Application.Current.Properties["EmpWorking"] = emp;
                        _emploglist.Add(new EmpLoginList
                        {
                            Emp = emp,
                            IsStartWorking = false,
                            TimePercent = 0
                        });

                        return true;
                    }

                return false;
            });


            return false;
        }

        public async Task<bool> logout(string username, string password, string code)
        {
            await Task.Run(() =>
            {
                foreach (var emp in _employeeList)
                    if (emp.Username.Equals(username) && emp.DecryptedPass.Equals(password) ||
                        emp.DecryptedCode.Equals(code))
                    {
                        var chemp = _emploglist.FirstOrDefault(x => x.Emp.EmpId.Equals(emp.EmpId));
                        _emploglist.Remove(chemp);

                        return true;
                    }

                return false;
            });


            return false;
        }

        public void logoutSync(string username, string password, string code, Action<bool> action)
        {
            Task.Run(() =>
            {
                bool a = false;

                foreach (var emp in _employeeList)
                    if (emp.Username.Equals(username) && emp.DecryptedPass.Equals(password) ||
                        emp.DecryptedCode.Equals(code))
                    {
                        var chemp = _emploglist.FirstOrDefault(x => x.Emp.EmpId.Equals(emp.EmpId));
                        _emploglist.Remove(chemp);

                        a = true;
                    }
                action(a);
            });
        }

        public void startWorkingRecord(EmpLoginList emm)
        {
            var empSalaryNote = _unitofwork.SalaryNoteRepository.Get(sle =>
                sle.EmpId.Equals(emm.Emp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) &&
                sle.ForYear.Equals(DateTime.Now.Year)).LastOrDefault();

            if (empSalaryNote == null || empSalaryNote.IsPaid == 1)
            {
                empSalaryNote = new SalaryNote
                {
                    EmpId = emm.Emp.EmpId,
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
            emm.EmpWH = empWorkHistory;
            emm.EmpSal = empSalaryNote;
            emm.IsStartWorking = true;
        }

        public void endWorkingRecord(EmpLoginList emm)
        {
            emm.EmpWH.EndTime = DateTime.Now;
            _unitofwork.WorkingHistoryRepository.Insert(emm.EmpWH);
            _unitofwork.Save();

            var workingHour = (emm.EmpWH.EndTime - emm.EmpWH.StartTime).TotalHours;
            emm.EmpSal.WorkHour += workingHour;
            emm.EmpSal.SalaryValue += (decimal)(workingHour * emm.Emp.HourWage);
            _unitofwork.SalaryNoteRepository.Update(emm.EmpSal);
            _unitofwork.Save();

            emm.EmpWH = null;
            emm.EmpSal = null;
            emm.IsStartWorking = false;
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

        public bool IsStartWorking { get; set; }

        public static implicit operator List<object>(EmpLoginList v)
        {
            throw new NotImplementedException();
        }
    }
}