using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cafocha.BusinessContext.EmployeeWorkspace;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.EmployeeWorkspace.Tests
{
    [TestClass()]
    public class TakingOrderModuleTests
    {
        private static TakingOrderModule _takingOrderModule;
        private static RepositoryLocator _repositoryLocator;
        private static Product _firstProduct;
        private static Product _secondProduct;

        [AssemblyInitialize]
        public static void init(TestContext testContext)
        {
             _repositoryLocator = new RepositoryLocator(new FakeLocalContext());
             _takingOrderModule = new TakingOrderModule(_repositoryLocator);
             _firstProduct = new Product()
            {
                ProductId = "1",
                Deleted = 0,
                Name = "Galaxy Milk Tea",
                Price = 20,
                Info = "Nothing fancy",
                Type = (int)ProductType.Drink

            };

            _secondProduct = new Product()
            {
                ProductId = "2",
                Deleted = 0,
                Name = "Galaxy Milk Tea",
                Price = 2,
                Info = "Nothing fancy",
                Type = (int)ProductType.Topping
            };
            _repositoryLocator.ProductRepository.Insert(_firstProduct);
            _repositoryLocator.ProductRepository.Insert(_secondProduct);
        }

        [TestMethod()]
        public void add1ProductToEmplyOrderTest()
        {
            //Act
            _takingOrderModule.OrderTemp = new OrderTemp();

            //Arrange
            _takingOrderModule.addProductToOrder(_firstProduct);
            //Assert
            Assert.AreEqual(1, _takingOrderModule.OrderTemp.OrderDetailsTemps.Count);
            Assert.AreEqual(_firstProduct.ProductId, _takingOrderModule.OrderTemp.OrderDetailsTemps.ElementAt(0).ProductId);
            Assert.AreEqual(1, _takingOrderModule.OrderTemp.OrderDetailsTemps.ElementAt(0).Quan);
        }

        [TestMethod()]
        public void add2SameProductToEmplyOrderTest()
        {
            //Act
            _takingOrderModule.OrderTemp = new OrderTemp();

            //Arrange
            _takingOrderModule.addProductToOrder(_firstProduct);
            _takingOrderModule.addProductToOrder(_firstProduct);
            //Assert
            Assert.AreEqual(1, _takingOrderModule.OrderTemp.OrderDetailsTemps.Count);
            Assert.AreEqual(_firstProduct.ProductId, _takingOrderModule.OrderTemp.OrderDetailsTemps.ElementAt(0).ProductId);
            Assert.AreEqual(2, _takingOrderModule.OrderTemp.OrderDetailsTemps.ElementAt(0).Quan);
        }

        [TestMethod()]
        public void add2DiffirentProductToEmplyOrderTest()
        {
            //Act
            _takingOrderModule.OrderTemp = new OrderTemp();

            //Arrange
            _takingOrderModule.addProductToOrder(_firstProduct);
            _takingOrderModule.addProductToOrder(_secondProduct);

            //Assert
            Assert.AreEqual(2, _takingOrderModule.OrderTemp.OrderDetailsTemps.Count);
            Assert.AreEqual(_firstProduct.ProductId, _takingOrderModule.OrderTemp.OrderDetailsTemps.ElementAt(0).ProductId);
            Assert.AreEqual(_secondProduct.ProductId, _takingOrderModule.OrderTemp.OrderDetailsTemps.ElementAt(1).ProductId);
            Assert.AreEqual(1, _takingOrderModule.OrderTemp.OrderDetailsTemps.ElementAt(0).Quan);
            Assert.AreEqual(1, _takingOrderModule.OrderTemp.OrderDetailsTemps.ElementAt(0).Quan);
        }
        [TestMethod()]
        public void updateOrderNoteTest()
        {
            //Act
            _takingOrderModule.OrderTemp = new OrderTemp();
            _takingOrderModule.addProductToOrder(_firstProduct);
            var note = "No sugar!";
            //Arrange
            _takingOrderModule.updateOrderNote(0, note);
            Assert.AreEqual(1, _takingOrderModule.OrderTemp.OrderDetailsTemps.Count);
            Assert.AreEqual(note, _takingOrderModule.OrderTemp.OrderDetailsTemps.ElementAt(0).Note);
        }

        [TestMethod()]
        public void loadTotalPriceTest()
        {
            //Act
            _takingOrderModule.OrderTemp = new OrderTemp();

            //Arrange
            _takingOrderModule.addProductToOrder(_firstProduct);
            _takingOrderModule.addProductToOrder(_secondProduct);
            _takingOrderModule.loadTotalPrice();

            //Assert
            Assert.AreEqual(_firstProduct.Price + _secondProduct.Price,_takingOrderModule.OrderTemp.TotalPrice);
            Assert.AreEqual((_firstProduct.Price + _secondProduct.Price) * 10 / 100, _takingOrderModule.OrderTemp.Vat);
        }


        [TestMethod()]
        public void convertTableToOrderTest()
        {
            //Act
            _takingOrderModule.OrderTemp = new OrderTemp();
            _takingOrderModule.addProductToOrder(_firstProduct);
            _takingOrderModule.addProductToOrder(_firstProduct);
            _takingOrderModule.addProductToOrder(_secondProduct);
            _takingOrderModule.loadTotalPrice();
            OrderNote newOrder = new OrderNote();
            var expectPrice = _firstProduct.Price * 2 + _secondProduct.Price;
            _takingOrderModule.convertTableToOrder(newOrder);

            //Assert
            Assert.AreEqual(expectPrice, newOrder.TotalPrice);
            Assert.AreEqual(expectPrice * 10 / 100, newOrder.Vat);
            Assert.AreEqual(expectPrice * 90 / 100, newOrder.SaleValue);
            Assert.AreEqual(2, newOrder.OrderNoteDetails.Count);
        }

        [TestMethod()]
        public void clearOrderTest()
        {
            //Act
            _takingOrderModule.OrderTemp = new OrderTemp();

            //Arrange
            _takingOrderModule.clearOrder();
            _takingOrderModule.loadTotalPrice();

            //Assert
            Assert.AreEqual(0, _takingOrderModule.OrderTemp.TotalPrice);
            Assert.AreEqual(0, _takingOrderModule.OrderTemp.Vat);
            Assert.AreEqual(0, _takingOrderModule.OrderTemp.OrderDetailsTemps.Count);
        }
    }
}