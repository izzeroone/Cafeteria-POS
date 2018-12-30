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
        AdminwsOfCloudAPWH _unitofwork;

        public AdminModule()
        {
        }

        public AdminModule(AdminwsOfCloudAPWH unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IEnumerable<AdminRe> getAdmins()
        {
            return _unitofwork.AdminreRepository.Get();
        }
    }
}
