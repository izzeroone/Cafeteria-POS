using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.EmployeeWorkspace
{
    public class ProductModule
    {
        private readonly RepositoryLocator _unitofwork;

        public ProductModule()
        {
            _unitofwork = new RepositoryLocator();
            PdtList = new List<PDTemp>();
        }

        public ProductModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
            PdtList = new List<PDTemp>();
        }

        public List<PDTemp> PdtList { get; set; }

        /// <summary>
        ///     Get data
        /// </summary>
        /// <param name="filter">Lambda expression to filtering data</param>
        /// <param name="orderBy">Lambda expression to ordering data</param>
        /// <param name="includeProperties">
        ///     the properties represent the relationship with other entities (use ',' to seperate
        ///     these properties)
        /// </param>
        /// <returns></returns>
        public IEnumerable<Product> Get(
            Expression<Func<Product, bool>> filter = null,
            Func<IQueryable<Product>, IOrderedQueryable<Product>> orderBy = null,
            string includeProperties = "")
        {
            return _unitofwork.ProductRepository.Get(p => p.Deleted.Equals(0), orderBy, includeProperties);
        }

        public Product getProduct(string productID)
        {
            return _unitofwork.ProductRepository.GetById(productID);
        }

        public IEnumerable<ProductDetail> getAllProductDetails(string productId)
        {
            return _unitofwork.ProductDetailsRepository.Get(c => c.ProductId.Equals(productId));
        }

        public IEnumerable<Product> getAllProduct()
        {
            return _unitofwork.ProductRepository.Get(x => x.Deleted == 0);
        }

        public IEnumerable<Product> getAllProduct(int type)
        {
            return _unitofwork.ProductRepository.Get(p => p.Type.Equals(type) && p.Deleted.Equals(0));
        }

        public IEnumerable<Product> getAllProduct(string filter)
        {
            return _unitofwork.ProductRepository.Get(p => p.Name.Contains(filter) && p.Deleted.Equals(0));
        }


        public IEnumerable<Product> getAllProduct(int type, string filter)
        {
            return _unitofwork.ProductRepository.Get(p =>
                p.Type.Equals(type) && p.Name.Contains(filter) && p.Deleted.Equals(0));
        }


        public IEnumerable<ProductDetail> getAllProductDetails()
        {
            return _unitofwork.ProductDetailsRepository.Get(includeProperties: "Product");
        }

        public void insertProduct(Product product, List<PDTemp> pdTempData)
        {
            product.ProductId = _unitofwork.ProductRepository.AutoGeneteId_DBAsowell(product).ProductId;
            _unitofwork.ProductRepository.Insert(product);
            _unitofwork.Save();

            if (pdTempData.Count() != 0)
                foreach (var pd in pdTempData)
                {
                    pd.ProDe.ProductId = product.ProductId;
                    _unitofwork.ProductDetailsRepository.Insert(pd.ProDe);
                    _unitofwork.Save();
                }
        }

        public void updateProduct(Product product)
        {
            _unitofwork.ProductRepository.Update(product);
            _unitofwork.Save();
        }

        public void updateProduct(Product product, List<ProductDetail> productDetails, List<PDTemp> pdTempData)
        {
            updateProduct(product);

            if (PdtList.Count() != 0)
            {
                foreach (var pd in productDetails) _unitofwork.ProductDetailsRepository.Delete(pd);
                _unitofwork.Save();

                foreach (var pd in PdtList)
                {
                    pd.ProDe.ProductId = product.ProductId;
                    pd.ProDe.IgdId = pd.Ingre.StoId;
                    _unitofwork.ProductDetailsRepository.Insert(pd.ProDe);
                    _unitofwork.Save();
                }
            }
        }

        public void deleteProduct(Product product)
        {
            product.Deleted = 1;
            var pdofingre = _unitofwork.ProductDetailsRepository.Get(x => x.ProductId.Equals(product.ProductId))
                .ToList();
            if (pdofingre.Count != 0)
            {
                foreach (var pd in pdofingre) _unitofwork.ProductDetailsRepository.Delete(pd);
                _unitofwork.Save();
            }

            _unitofwork.ProductRepository.Update(product);
            _unitofwork.Save();
        }

        public class PDTemp
        {
            public ProductDetail ProDe { get; set; }

            public Stock Ingre { get; set; }

            public List<string> UnitUseT => new List<string> {"", "g", "ml"};
        }
    }
}