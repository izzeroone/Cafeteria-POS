using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.EmployeeWorkspace
{
    class ProductModule
    {
        private EmployeewsOfLocalPOS _unitofwork;

        public ProductModule()
        {
            _unitofwork = new EmployeewsOfLocalPOS();
        }
        public ProductModule(EmployeewsOfLocalPOS unitofwork)
        {
            _unitofwork = unitofwork;
        }

        /// <summary>
        /// Get data
        /// </summary>
        /// <param name="filter">Lambda expression to filtering data</param>
        /// <param name="orderBy">Lambda expression to ordering data</param>
        /// <param name="includeProperties">the properties represent the relationship with other entities (use ',' to seperate these properties)</param>
        /// <returns></returns>
        public IEnumerable<Product> Get(
            Expression<Func<Product, bool>> filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
            string includeProperties = "")
        {
            return _unitofwork.ProductRepository.Get(filter, orderBy, includeProperties);
        }

        public Product GetById(object id)
        {
            return _unitofwork.ProductRepository.GetById(id);
        }

        public void Insert(Product entity)
        {
            _unitofwork.ProductRepository.Insert(entity);
        }

        public void Delete(object id)
        {
            _unitofwork.ProductRepository.Delete(id);
        }

        public void Delete(Product entityTODelete)
        {
            _unitofwork.ProductRepository.Delete(entityTODelete);
        }

        public void Update(Product entityToUpdate)
        {
            _unitofwork.ProductRepository.Update(entityToUpdate);
        }


        public Product AutoGeneteId_DBAsowell(Product entity)
        {
            return _unitofwork.ProductRepository.AutoGeneteId_DBAsowell(entity);
        }
    }
}
