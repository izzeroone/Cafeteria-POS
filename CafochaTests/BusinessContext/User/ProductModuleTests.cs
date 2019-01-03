using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cafocha.BusinessContext.EmployeeWorkspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.EmployeeWorkspace.Tests
{
    [TestClass()]
    public class ProductModuleTests
    {
        private static ProductModule _productModule;
        private static RepositoryLocator _repositoryLocator;
        [AssemblyInitialize()]
        public static void init()
        {
            _repositoryLocator = new RepositoryLocator();
            _productModule = new ProductModule(_repositoryLocator);
        }

        [TestMethod()]
        public void ProductModuleTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getProductTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getAllProductDetailsTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getAllProductTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getAllProductTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getAllProductTest2()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getAllProductTest3()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getAllProductDetailsTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void insertProductTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void updateProductTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void updateProductTest1()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void deleteProductTest()
        {
            Assert.Fail();
        }
    }
}