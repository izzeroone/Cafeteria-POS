using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.WarehouseWorkspace
{
    public class OrderModule
    {
        RepositoryLocator _unitofwork;

        public OrderModule()
        {
            _unitofwork = new RepositoryLocator();
        }

        public OrderModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IEnumerable<OrderNote> getOrdernoteList()
        {
            var ordernotelist = _unitofwork.OrderRepository.Get(includeProperties: "Employee,Customer").ToList();
//            ordernotelist = ordernotelist.Where(x => x.Employee.Manager.Equals(admin.AdId)).ToList();
            return ordernotelist;
        }

        public IEnumerable<OrderNoteDetail> getOrderNoteDetail(string orderNoteID)
        {
            return _unitofwork.OrderDetailsRepository.Get(c => c.OrdernoteId.Equals(orderNoteID));
        }
    }
}
