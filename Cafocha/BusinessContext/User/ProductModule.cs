using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Entities;
using Cafocha.GUI.AdminWorkSpace;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.EmployeeWorkspace
{
    public class ProductModule
    {
        private RepositoryLocator _unitofwork;
        private List<PDTemp> _pdtList;

        public List<PDTemp> PdtList
        {
            get => _pdtList;
            set => _pdtList = value;
        }

        public ProductModule()
        {
            _unitofwork = new RepositoryLocator();
            _pdtList = new List<PDTemp>();
        }
        public ProductModule(RepositoryLocator unitofwork)
        {
            _unitofwork = unitofwork;
            _pdtList = new List<PDTemp>();
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

        public Product getProduct(string productID)
        {
            return _unitofwork.ProductRepository.GetById(productID);
        }

        public IEnumerable<ProductDetail> getAllProductDetails(string productId)
        {
            return _unitofwork.ProductDetailsRepository.Get(c => c.ProductId.Equals(productId);
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
            return _unitofwork.ProductRepository.Get(p => p.Type.Equals(type) && p.Name.Contains(filter) && p.Deleted.Equals(0));
        }


        public IEnumerable<ProductDetail> getAllProductDetails()
        {
            return _unitofwork.ProductDetailsRepository.Get(includeProperties: "Product");
        }

        public void insertProduct(Product product, List<ProductModule.PDTemp> pdTempData)
        {
            product.ProductId = _unitofwork.ProductRepository.AutoGeneteId_DBAsowell(product).ProductId;
            _unitofwork.ProductRepository.Insert(product);
            _unitofwork.Save();

            if (pdTempData.Count() != 0)
            {
                foreach (var pd in pdTempData)
                {
                    pd.ProDe.ProductId = product.ProductId;
                    _unitofwork.ProductDetailsRepository.Insert(pd.ProDe);
                    _unitofwork.Save();
                }
            }
        }

        public void updateProduct(Product product)
        {
            _unitofwork.ProductRepository.Update(product);
            _unitofwork.Save();
        }

        public void updateProduct(Product product, List<ProductDetail> productDetails ,List<ProductModule.PDTemp> pdTempData)
        {
            updateProduct(product);

            if (_pdtList.Count() != 0)
            {
                foreach (var pd in productDetails)
                {
                    _unitofwork.ProductDetailsRepository.Delete(pd);
                }
                _unitofwork.Save();

                foreach (var pd in _pdtList)
                {
                    pd.ProDe.ProductId = product.ProductId;
                    pd.ProDe.IgdId = pd.Ingre.IgdId;
                    _unitofwork.ProductDetailsRepository.Insert(pd.ProDe);
                    _unitofwork.Save();
                }
            }
        }

        public void deleteProduct(Product product)
        {
            product.Deleted = 1;
            var pdofingre = _unitofwork.ProductDetailsRepository.Get(x => x.ProductId.Equals(product.ProductId)).ToList();
            if (pdofingre.Count != 0)
            {
                foreach (var pd in pdofingre)
                {
                    _unitofwork.ProductDetailsRepository.Delete(pd);
                }
                _unitofwork.Save();
            }

            _unitofwork.ProductRepository.Update(product);
            _unitofwork.Save();
        }

        public class PDTemp
        {
            private ProductDetail _pd;
            private Ingredient _ingre;

            public ProductDetail ProDe
            {
                get { return _pd; }
                set
                {
                    _pd = value;
                }
            }

            public Ingredient Ingre
            {
                get { return _ingre; }
                set
                {
                    _ingre = value;
                }
            }

            public List<string> UnitUseT
            {
                get
                {
                    return new List<string> { "", "g", "ml" };
                }
            }
        }


    }
}
