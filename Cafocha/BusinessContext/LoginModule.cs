using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Cafocha.Entities;
using Cafocha.GUI;
using Cafocha.GUI.CafowareWorkSpace;
using Cafocha.GUI.EmployeeWorkSpace;
using log4net;

namespace Cafocha.BusinessContext
{
    public class LoginModule
    {
        private static readonly ILog AppLog = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly LoginWindow _loginWindow;

        public LoginModule(LoginWindow loginWindow)
        {
            _loginWindow = loginWindow;
        }

        public async Task LoginAsync(string username, string pass)
        {
            try
            {
                await Task.Run(() =>
                {
                    var empList = _loginWindow._businessModuleLocator.RepositoryLocator.EmployeeRepository.Get()
                        .ToList();

                    var emp = empList.FirstOrDefault(x => x.Username.Equals(username) && x.DecryptedPass.Equals(pass));
                    if (emp != null)
                    {
                        Application.Current.Properties["EmpLogin"] = emp;
                        if (emp.EmpRole == (int) EmployeeRole.Stock)
                        {
                            _loginWindow.Dispatcher.Invoke(() =>
                            {
                                var wareHouse = new CafowareWindow();
                                wareHouse.Show();
                            });
                        }
                        else
                        {
                            try
                            {
                                var empSalaryNote = _loginWindow._businessModuleLocator.RepositoryLocator
                                    .SalaryNoteRepository.Get(sle =>
                                        sle.EmpId.Equals(emp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) &&
                                        sle.ForYear.Equals(DateTime.Now.Year)).First();

                                Application.Current.Properties["EmpSN"] = empSalaryNote;
                                var empWorkHistory = new WorkingHistory
                                {
                                    ResultSalary = empSalaryNote.SnId,
                                    EmpId = empSalaryNote.EmpId
                                };
                                Application.Current.Properties["EmpWH"] = empWorkHistory;
                            }
                            catch (Exception ex)
                            {
                                var empSalary = new SalaryNote
                                {
                                    EmpId = emp.EmpId,
                                    SalaryValue = 0,
                                    WorkHour = 0,
                                    ForMonth = DateTime.Now.Month,
                                    ForYear = DateTime.Now.Year,
                                    IsPaid = 0
                                };
                                _loginWindow._businessModuleLocator.RepositoryLocator.SalaryNoteRepository.Insert(
                                    empSalary);
                                _loginWindow._businessModuleLocator.RepositoryLocator.Save();
                                var empWorkHistory = new WorkingHistory
                                {
                                    ResultSalary = empSalary.SnId,
                                    EmpId = empSalary.EmpId
                                };
                                Application.Current.Properties["EmpWH"] = empWorkHistory;
                                Application.Current.Properties["EmpSN"] = empSalary;
                            }

                            _loginWindow.Dispatcher.Invoke(() =>
                            {
                                _loginWindow._businessModuleLocator.EmployeeModule.Emploglist.Clear();
                                _loginWindow._businessModuleLocator.EmployeeModule.Emploglist.Add(new EmpLoginList
                                {
                                    Emp = emp,
                                    EmpSal = Application.Current.Properties["EmpSN"] as SalaryNote,
                                    EmpWH = Application.Current.Properties["EmpWH"] as WorkingHistory,
                                    TimePercent = 0
                                });
                                var t = _loginWindow._businessModuleLocator.EmployeeModule.Emploglist;
                                var main = new MainWindow();
                                main.Show();
                            });
                        }
                    }
                    else
                    {
                        //Get Admin
                        var adList = _loginWindow._businessModuleLocator.RepositoryLocator.AdminreRepository.Get()
                            .ToList();

                        var ad = adList.FirstOrDefault(x =>
                            x.Username.Equals(username) && x.DecryptedPass.Equals(pass));
                        //TODO: fix ad Emp
                        var adEmp = empList.FirstOrDefault(x => x.EmpId.Equals("EMP0000002"));
                        if (ad != null)
                        {
                            Application.Current.Properties["EmpLogin"] = adEmp;
                            Application.Current.Properties["AdLogin"] = ad;

                            _loginWindow.Dispatcher.Invoke(() =>
                            {
                                var navwindow = new AdminNavWindow();
                                navwindow.Show();
                            });
                        }

                        if (ad == null && emp == null)
                        {
                            MessageBox.Show("incorrect username or password");
                            return;
                        }
                    }


                    _loginWindow.Dispatcher.Invoke(() => { _loginWindow.Close(); });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: \n" + ex.Message);
                AppLog.Error(ex);
            }
        }

        public async Task LoginByCodeAsync(string code)
        {
            try
            {
                await Task.Run(() =>
                {
                    var empList = _loginWindow._businessModuleLocator.RepositoryLocator.EmployeeRepository.Get()
                        .ToList();
                    var loginEmp = empList.FirstOrDefault(x => x.DecryptedCode.Equals(code));
                    if (loginEmp != null)
                    {
                        Application.Current.Properties["EmpLogin"] = loginEmp;

                        if (loginEmp.EmpRole == (int) EmployeeRole.Stock)
                        {
                            _loginWindow.Dispatcher.Invoke(() =>
                            {
                                var wareHouse = new CafowareWindow();
                                wareHouse.Show();
                            });
                        }
                        else
                        {
                            try
                            {
                                var empSalaryNote = _loginWindow._businessModuleLocator.RepositoryLocator
                                    .SalaryNoteRepository.Get(sle =>
                                        sle.EmpId.Equals(loginEmp.EmpId) && sle.ForMonth.Equals(DateTime.Now.Month) &&
                                        sle.ForYear.Equals(DateTime.Now.Year)).First();

                                Application.Current.Properties["EmpSN"] = empSalaryNote;
                                var empWorkHistory = new WorkingHistory
                                {
                                    ResultSalary = empSalaryNote.SnId,
                                    EmpId = empSalaryNote.EmpId
                                };
                                Application.Current.Properties["EmpWH"] = empWorkHistory;
                            }
                            catch (Exception ex)
                            {
                                var empSalary = new SalaryNote
                                {
                                    EmpId = loginEmp.EmpId,
                                    SalaryValue = 0,
                                    WorkHour = 0,
                                    ForMonth = DateTime.Now.Month,
                                    ForYear = DateTime.Now.Year,
                                    IsPaid = 0
                                };
                                _loginWindow._businessModuleLocator.RepositoryLocator.SalaryNoteRepository.Insert(
                                    empSalary);
                                _loginWindow._businessModuleLocator.RepositoryLocator.Save();
                                var empWorkHistory = new WorkingHistory
                                {
                                    ResultSalary = empSalary.SnId,
                                    EmpId = empSalary.EmpId
                                };
                                Application.Current.Properties["EmpWH"] = empWorkHistory;
                                Application.Current.Properties["EmpSN"] = empSalary;
                            }

                            _loginWindow.Dispatcher.Invoke(() =>
                            {
                                _loginWindow._businessModuleLocator.EmployeeModule.Emploglist.Clear();
                                _loginWindow._businessModuleLocator.EmployeeModule.Emploglist.Add(new EmpLoginList
                                {
                                    Emp = loginEmp,
                                    EmpSal = Application.Current.Properties["EmpSN"] as SalaryNote,
                                    EmpWH = Application.Current.Properties["EmpWH"] as WorkingHistory,
                                    TimePercent = 0
                                });

                                var main = new MainWindow();
                                main.Show();
                            });
                        }
                    }
                    else
                    {
                        MessageBox.Show("incorrect username or password");
                        return;
                    }

                    _loginWindow.Dispatcher.Invoke(() => { _loginWindow.Close(); });
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Something went wrong: \n" + ex.Message);
                AppLog.Error(ex);
            }
        }
    }
}