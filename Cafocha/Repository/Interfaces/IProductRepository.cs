using System;
using System.Collections.Generic;
using Cafocha.Entities;

namespace Cafocha.Repository.Interfaces
{
    public interface IProductRepository : IDisposable
    {
        IEnumerable<Product> GetAllProducts();
        Product GetProductById(string productId);
        void InsertProduct(Product product);
        void DeleteProduct(string productId);
        void UpdateProduct(Product product);
        void Save();
    }
}