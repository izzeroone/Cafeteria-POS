using System.Collections.Generic;
using System.Linq;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.WarehouseWorkspace
{
    public class OrderModule
    {
        private readonly RepositoryLocator _unitofwork;

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

        public IEnumerable<OrderNote> getOrdernoteByMonth(int month)
        {
            if (month <= 0 || month >= 13) return new List<OrderNote>();

            return _unitofwork.OrderRepository.Get(c => c.Ordertime.Month == month);
        }

        public IEnumerable<OrderNoteDetail> getOrderNoteDetail(string orderNoteID)
        {
            return _unitofwork.OrderDetailsRepository.Get(c => c.OrdernoteId.Equals(orderNoteID));
        }

        public IEnumerable<OrderNoteDetail> getAllOrderNoteDetails()
        {
            return _unitofwork.OrderDetailsRepository.Get(includeProperties: "Product").ToList();
        }
    }
}