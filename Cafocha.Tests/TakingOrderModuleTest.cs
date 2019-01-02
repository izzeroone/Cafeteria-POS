// <copyright file="TakingOrderModuleTest.cs">Copyright ©  2018</copyright>

using System;
using System.Collections.Generic;
using Cafocha.BusinessContext.EmployeeWorkspace;
using Cafocha.Entities.CustomEntities;
using Cafocha.Repository.DAL;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OrderNote = Cafocha.Entities.CustomEntities.OrderNote;
using Product = Cafocha.Entities.CustomEntities.Product;

namespace Cafocha.Tests
{
    /// <summary>This class contains parameterized unit tests for TakingOrderModule</summary>
    [PexClass(typeof(TakingOrderModule))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TakingOrderModuleTest
    {
        /// <summary>Test stub for .ctor()</summary>
        [PexMethod]
        public TakingOrderModule ConstructorTest()
        {
            TakingOrderModule target = new TakingOrderModule();
            return target;
            // TODO: add assertions to method TakingOrderModuleTest.ConstructorTest()
        }

        /// <summary>Test stub for .ctor(RepositoryLocator)</summary>
        [PexMethod]
        public TakingOrderModule ConstructorTest01(RepositoryLocator unitofwork)
        {
            TakingOrderModule target = new TakingOrderModule(unitofwork);
            return target;
            // TODO: add assertions to method TakingOrderModuleTest.ConstructorTest01(RepositoryLocator)
        }

        /// <summary>Test stub for ConvertTableToOrder(OrderNote)</summary>
        [PexMethod]
        public bool ConvertTableToOrderTest(
            [PexAssumeUnderTest]TakingOrderModule target,
            OrderNote newOrder
        )
        {
            bool result = target.ConvertTableToOrder(newOrder);
            return result;
            // TODO: add assertions to method TakingOrderModuleTest.ConvertTableToOrderTest(TakingOrderModule, OrderNote)
        }

        /// <summary>Test stub for get_OrderTemp()</summary>
        [PexMethod]
        public OrderTemp OrderTempGetTest([PexAssumeUnderTest]TakingOrderModule target)
        {
            OrderTemp result = target.OrderTemp;
            return result;
            // TODO: add assertions to method TakingOrderModuleTest.OrderTempGetTest(TakingOrderModule)
        }

        /// <summary>Test stub for set_OrderTemp(OrderTemp)</summary>
        [PexMethod]
        public void OrderTempSetTest([PexAssumeUnderTest]TakingOrderModule target, OrderTemp value)
        {
            target.OrderTemp = value;
            // TODO: add assertions to method TakingOrderModuleTest.OrderTempSetTest(TakingOrderModule, OrderTemp)
        }

        /// <summary>Test stub for addOrUpdateOrderDetail(OrderDetailsTemp)</summary>
        [PexMethod]
        public void addOrUpdateOrderDetailTest(
            [PexAssumeUnderTest]TakingOrderModule target,
            OrderDetailsTemp orderTempDetail
        )
        {
            target.addOrUpdateOrderDetail(orderTempDetail);
            // TODO: add assertions to method TakingOrderModuleTest.addOrUpdateOrderDetailTest(TakingOrderModule, OrderDetailsTemp)
        }

        /// <summary>Test stub for addProductToOrder(Product)</summary>
        [PexMethod]
        public void addProductToOrderTest([PexAssumeUnderTest]TakingOrderModule target, Product pt)
        {
            target.addProductToOrder(pt);
            // TODO: add assertions to method TakingOrderModuleTest.addProductToOrderTest(TakingOrderModule, Product)
        }

        /// <summary>Test stub for clearOrder()</summary>
        [PexMethod]
        public void clearOrderTest([PexAssumeUnderTest]TakingOrderModule target)
        {
            target.clearOrder();
            // TODO: add assertions to method TakingOrderModuleTest.clearOrderTest(TakingOrderModule)
        }

        /// <summary>Test stub for getOrderDetailsDisplay()</summary>
        [PexMethod]
        public IEnumerable<TakingOrderModule.OrderDetails_Product_Joiner> getOrderDetailsDisplayTest([PexAssumeUnderTest]TakingOrderModule target)
        {
            IEnumerable<TakingOrderModule.OrderDetails_Product_Joiner> result
               = target.getOrderDetailsDisplay();
            return result;
            // TODO: add assertions to method TakingOrderModuleTest.getOrderDetailsDisplayTest(TakingOrderModule)
        }

        /// <summary>Test stub for loadTotalPrice()</summary>
        [PexMethod]
        public void loadTotalPriceTest([PexAssumeUnderTest]TakingOrderModule target)
        {
            target.loadTotalPrice();
            // TODO: add assertions to method TakingOrderModuleTest.loadTotalPriceTest(TakingOrderModule)
        }

        /// <summary>Test stub for saveOrderToDB(OrderNote)</summary>
        [PexMethod]
        public void saveOrderToDBTest(
            [PexAssumeUnderTest]TakingOrderModule target,
            OrderNote orderNote
        )
        {
            target.saveOrderToDB(orderNote);
            // TODO: add assertions to method TakingOrderModuleTest.saveOrderToDBTest(TakingOrderModule, OrderNote)
        }

        /// <summary>Test stub for updateOrderDetail(Int32, String)</summary>
        [PexMethod]
        public void updateOrderDetailTest(
            [PexAssumeUnderTest]TakingOrderModule target,
            int index,
            string value
        )
        {
            target.updateOrderDetail(index, value);
            // TODO: add assertions to method TakingOrderModuleTest.updateOrderDetailTest(TakingOrderModule, Int32, String)
        }

        /// <summary>Test stub for updateOrderNote(Int32, String)</summary>
        [PexMethod]
        public void updateOrderNoteTest(
            [PexAssumeUnderTest]TakingOrderModule target,
            int index,
            string note
        )
        {
            target.updateOrderNote(index, note);
            // TODO: add assertions to method TakingOrderModuleTest.updateOrderNoteTest(TakingOrderModule, Int32, String)
        }
    }
}
