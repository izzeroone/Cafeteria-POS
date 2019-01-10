using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Windows.Forms;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UITesting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UITest.Extension;
using Keyboard = Microsoft.VisualStudio.TestTools.UITesting.Keyboard;


namespace AutomationTesting
{
    /// <summary>
    /// Summary description for CodedUITest1
    /// </summary>
    [CodedUITest]
    public class TakingOrderTest
    {
        public TakingOrderTest()    
        {
        }

        [TestMethod]
        public void takingOrderWithoutStartWorking()
        {

            this.UIMap.loginAsEmployee();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.showMustLoginDialogAssert();
            this.UIMap.closeDialog();

            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        [TestMethod]
        public void addSingleItemToOrder()
        {
            this.UIMap.loginAsEmployee();
            this.UIMap.startWorking();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.singleItemAssert();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        [TestMethod]
        public void addTwoItemToOrder()
        {
            this.UIMap.loginAsEmployee();
            this.UIMap.startWorking();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.twoItemAssert();
           // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        [TestMethod]
        public void addTwoDiffirentItemToOrder()
        {
            this.UIMap.loginAsEmployee();
            this.UIMap.startWorking();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.clickOnSecondProduct();
            this.UIMap.twoProductAssert();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }
        [TestMethod]
        public void deleteOrderTest()
        {
            this.UIMap.loginAsEmployee();
            this.UIMap.startWorking();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.deleteItem();
            this.UIMap.emplyOrderAssert();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        [TestMethod]
        public void clearOrderTest()
        {
            this.UIMap.loginAsEmployee();
            this.UIMap.startWorking();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.clearButtonClick();
            this.UIMap.emplyOrderAssert();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        [TestMethod]
        public void buzzEmplyOrder()
        {
            this.UIMap.loginAsEmployee();
            this.UIMap.startWorking();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.buzzOrder();
            this.UIMap.buzzPreviewAssert();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        [TestMethod]
        public void payEmplyOrder()
        {
            this.UIMap.loginAsEmployee();
            this.UIMap.startWorking();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.payEmplyAmount();
            this.UIMap.buzzPreviewAssert();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        [TestMethod]
        public void payLessMoneyOrder()
        {
            this.UIMap.loginAsEmployee();
            this.UIMap.startWorking();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.payLessThanAmount();
            this.UIMap.assertNotEnoughMoney();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        [TestMethod]
        public void payMoreMoney()
        {
            this.UIMap.loginAsEmployee();
            this.UIMap.startWorking();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.clickOnFirstProduct();
            this.UIMap.payMoreThanAmount();
            this.UIMap.buzzPreviewAssert();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }
        #region Additional test attributes

        //You can use the following additional attributes as you write your tests:

        //Use TestInitialize to run code before running each test 
        [TestInitialize()]
        public void MyTestInitialize()
        {

            this.UIMap.launchApplication();
            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        ////Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {

            this.UIMap.closeApplication();

            // To generate code for this test, select "Generate Code for Coded UI Test" from the shortcut menu and select one of the menu items.
        }

        #endregion

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        private TestContext testContextInstance;

        public UIMap UIMap
        {
            get
            {
                if (this.map == null)
                {
                    this.map = new UIMap();
                }

                return this.map;
            }
        }

        private UIMap map;
    }
}
