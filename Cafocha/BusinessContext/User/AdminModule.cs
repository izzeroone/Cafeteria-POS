using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.User
{
    public class AdminModule
    {
        private readonly RepositoryLocator _unitofwork;

        public AdminModule()
        {
        }

        public AdminModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public AdminRe getAdmin(string adId)
        {
            return _unitofwork.AdminreRepository.Get(x => x.AdId.Equals(adId)).FirstOrDefault();
        }

        public IEnumerable<AdminRe> getAdmins()
        {
            return _unitofwork.AdminreRepository.Get();
        }

        public void addAdmin(AdminRe admin)
        {
            _unitofwork.AdminreRepository.Insert(admin);
            _unitofwork.Save();
        }

        public void updateAdmin(AdminRe admin)
        {
            _unitofwork.AdminreRepository.Update(admin);
            _unitofwork.Save();
        }

        public async Task<bool> login(string username, string password)
        {
            //Get Admin
            var adList = _unitofwork.AdminreRepository.Get();

            var ad = adList.FirstOrDefault(x =>
                x.Username.Equals(username) && x.DecryptedPass.Equals(password));
            //TODO: fix ad Emp
            var adEmp = _unitofwork.EmployeeRepository.Get(x => x.EmpId.Equals("EMP0000001")).FirstOrDefault();

            if (ad != null)
            {
                Application.Current.Properties["EmpLogin"] = adEmp;
                Application.Current.Properties["AdLogin"] = ad;
                return true;
            }

            return false;
        }
    }
}