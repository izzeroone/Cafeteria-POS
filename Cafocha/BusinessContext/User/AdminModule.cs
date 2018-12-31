using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.User
{
    public class AdminModule
    {
        RepositoryLocator _unitofwork;
        
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
    }
}
