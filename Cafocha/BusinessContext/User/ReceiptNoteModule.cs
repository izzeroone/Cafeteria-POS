using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.User
{
    public class ReceiptNoteModule
    {
        RepositoryLocator _unitofwork;

        public ReceiptNoteModule()
        {
        }

        public ReceiptNoteModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IEnumerable<ReceiptNote> getAllReceiveNotes()
        {
            return _unitofwork.ReceiptNoteRepository.Get(includeProperties: "Employee");
        }

        public IEnumerable<ReceiptNote> getAllReceiptNotesByMonth(int month)
        {
            if (month <= 0 || month >= 13)
            {
                return new List<ReceiptNote>();
            }

            return _unitofwork.ReceiptNoteRepository.Get(c => c.Inday.Month == month);
        }

        public IEnumerable<ReceiptNoteDetail> getReceiptNoteDetails(string receiptID)
        {
            return _unitofwork.ReceiptNoteDsetailsRepository.Get(c => c.RnId.Equals(receiptID));
        }
        public IEnumerable<ReceiptNoteDetail> getAllReceiveNoteDetails()
        {
            return _unitofwork.ReceiptNoteDsetailsRepository.Get(includeProperties: "Ingredient").ToList();
        }
    }
}
