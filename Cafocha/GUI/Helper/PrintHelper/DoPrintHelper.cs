﻿using System;
using System.IO;
using System.IO.Packaging;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Xps.Packaging;
using Cafocha.Entities;
using Cafocha.GUI.BusinessModel;
using Cafocha.GUI.Helper.PrintHelper.Model;
using Cafocha.Repository.DAL;

namespace Cafocha.GUI.Helper.PrintHelper
{
    public class DoPrintHelper
    {
        public static readonly int TempReceipt_Printing = 1;
        public static readonly int Kitchen_Printing = 2;
        public static readonly int Bar_Printing = 3;
        public static readonly int Eod_Printing = 4;
        public static readonly int Receipt_Printing = 5;
        public static readonly int Fire_Stater = 6;
        public static readonly int Fire_Main = 7;
        public static readonly int Fire_Dessert = 8;

        private readonly EmployeewsOfLocalPOS _unitofwork;

        private IPrintHelper ph;
        private int type;
        public int OrderMode { get; set; }
        private readonly Entities.Table curTable;
        private readonly OrderNote curOrder;
        private PrintDialog printDlg;

        private string _barPrinter;
        private string _receptionPrinter;
        private string _kitchentPrinter;
        private bool isShowReview;

        public DoPrintHelper(EmployeewsOfLocalPOS unitofwork, int printType, Entities.Table currentTable = null)
        {
            _unitofwork = unitofwork;
            type = printType;
            curTable = currentTable;
            printDlg = new PrintDialog();

            string[] result = ReadWriteData.ReadPrinterSetting();
            if (result != null)
            {
                _receptionPrinter = result[0];
                _kitchentPrinter = result[1];
                _barPrinter = result[2];

                if (int.Parse(result[3]) == 1)
                    isShowReview = true;
                else
                    isShowReview = false;
            }
            else
            {
                _receptionPrinter = "";
                _kitchentPrinter = "";
                _barPrinter = "";
                isShowReview = true;
            }
        }

        public DoPrintHelper(EmployeewsOfLocalPOS unitofwork, int printType, OrderNote currentOrder)
        {
            _unitofwork = unitofwork;
            type = printType;
            curOrder = currentOrder;
            printDlg = new PrintDialog();

            string[] result = ReadWriteData.ReadPrinterSetting();
            if (result != null)
            {
                _receptionPrinter = result[0];
                _kitchentPrinter = result[1];
                _barPrinter = result[2];

                if (int.Parse(result[3]) == 1)
                    isShowReview = true;
                else
                    isShowReview = false;
            }
            else
            {
                _receptionPrinter = "";
                _kitchentPrinter = "";
                _barPrinter = "";
                isShowReview = true;
            }
        }

        public void DoPrint()
        {
            if (curTable == null && type != Eod_Printing && curOrder == null)
            {
                return;
            }

            try
            {
                // Create a PrintHelper
                CreatePrintHelper();

                // Create a FlowDocument dynamically.
                FlowDocument doc = ph.CreateDocument();
                doc.Name = "FlowDoc";

                // Read the FlowDoucument xaml file
                //Stream flowDocumentStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("TestWPF.PrintWindow.xaml");
                //FileStream fs = new FileStream(startupProjectPath + "\\FlowDocument1.xaml", FileMode.Open, FileAccess.Read);
                //FlowDocument flowDocument = (FlowDocument)XamlReader.Load(fs);

                PrintToReal(doc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        /// <summary>
        /// CHOOSING PRINT TO PAPER OR PRINT TO WINDOWS
        /// </summary>
        /// <param name="doc"></param>
        private void PrintToReal(FlowDocument doc)
        {
            // Create IDocumentPaginatorSource from FlowDocument
            IDocumentPaginatorSource idpSource = doc;

            if (!isShowReview)
            {
                // Call PrintDocument method to send document to printer
                printDlg.PrintDocument(idpSource.DocumentPaginator, "bill printing");
            }
            else
            {
                // convert FlowDocument to FixedDocument
                var paginator = idpSource.DocumentPaginator;
                var package = Package.Open(new MemoryStream(), FileMode.Create, FileAccess.ReadWrite);
                var packUri = new Uri("pack://temp.xps");
                PackageStore.RemovePackage(packUri);
                PackageStore.AddPackage(packUri, package);
                var xps = new XpsDocument(package, CompressionOption.NotCompressed, packUri.ToString());
                XpsDocument.CreateXpsDocumentWriter(xps).Write(paginator);
                FixedDocument fdoc = xps.GetFixedDocumentSequence().References[0].GetDocument(true);

                DocumentViewer previewWindow = new DocumentViewer
                {
                    Document = fdoc
                };
                Window printpriview = new Window();
                printpriview.Content = previewWindow;
                printpriview.Title = "Print Preview";
                printpriview.Show();
            }
        }

        private void CreatePrintHelper()
        {
            // Create Print Helper
            if (type == Receipt_Printing)
            {
                if (!string.IsNullOrEmpty(_receptionPrinter))
                    printDlg.PrintQueue = new PrintQueue(new PrintServer(), _receptionPrinter);

                ph = new ReceiptPrintHelper()
                {
                    Owner = new Owner()
                    {
                        ImgName = "logo.png",
                        Address = "Address: f.7th, Fafilm Building, 6 St.Thai Van Lung, w.Ben Nghe, HCM City, Viet Nam",
                        Phone = "",
                        PageName = "RECEIPT"
                    },

                    Order = new OrderForPrint().GetAndConvertOrder(curOrder, _unitofwork).GetAndConverOrderDetails(curOrder, _unitofwork),

                    OrderMode = OrderMode
                };
            }


            if (type == TempReceipt_Printing)
            {
                if(!string.IsNullOrEmpty(_receptionPrinter))
                    printDlg.PrintQueue =new PrintQueue(new PrintServer(), _receptionPrinter);

                ph = new ReceiptPrintHelper()
                {
                    Owner = new Owner()
                    {
                        ImgName = "logo.png",
                        Address = "Address: f.7th, Fafilm Building, 6 St.Thai Van Lung, w.Ben Nghe, HCM City, Viet Nam",
                        Phone = "",
                        PageName = "RECEIPT"
                    },

                    Order = new OrderForPrint().GetAndConvertOrder(curTable, _unitofwork).GetAndConverOrderDetails(curTable, _unitofwork,TempReceipt_Printing),

                    OrderMode = OrderMode
                };
            }

            if (type == Bar_Printing)
            {
                if (!string.IsNullOrEmpty(_barPrinter))
                    printDlg.PrintQueue = new PrintQueue(new PrintServer(), _barPrinter);

                ph = new BarPrintHelper()
                {
                    Order = new OrderForPrint().GetAndConvertOrder(curTable, _unitofwork).GetAndConverOrderDetails(curTable, _unitofwork, Bar_Printing)
                };
            }

            if (type == Kitchen_Printing)
            {
                if (!string.IsNullOrEmpty(_kitchentPrinter))
                    printDlg.PrintQueue = new PrintQueue(new PrintServer(), _kitchentPrinter);

                ph = new KitchenPrintHelper()
                {
                    Order = new OrderForPrint().GetAndConvertOrder(curTable, _unitofwork).GetAndConverOrderDetails(curTable, _unitofwork, Kitchen_Printing)
                };
            }

            if (type == Eod_Printing)
            {
                if (!string.IsNullOrEmpty(_receptionPrinter))
                    printDlg.PrintQueue = new PrintQueue(new PrintServer(), _receptionPrinter);

                ph = new EndOfDayPrintHelper(_unitofwork);
            }

            if (type == Fire_Stater)
            {
                if (!string.IsNullOrEmpty(_kitchentPrinter))
                    printDlg.PrintQueue = new PrintQueue(new PrintServer(), _kitchentPrinter);

                ph = new FirePrintHelper()
                {
                    TableNumer = curTable.TableNumber,
                    Mess = "STARTER FIRE!"
                };
            }

            if (type == Fire_Main)
            {
                if (!string.IsNullOrEmpty(_kitchentPrinter))
                    printDlg.PrintQueue = new PrintQueue(new PrintServer(), _kitchentPrinter);

                ph = new FirePrintHelper()
                {
                    TableNumer = curTable.TableNumber,
                    Mess = "MAIN FIRE!"
                };
            }

            if (type == Fire_Dessert)
            {
                if (!string.IsNullOrEmpty(_kitchentPrinter))
                    printDlg.PrintQueue = new PrintQueue(new PrintServer(), _kitchentPrinter);

                ph = new FirePrintHelper()
                {
                    TableNumer = curTable.TableNumber,
                    Mess = "DESSERT FIRE!"
                };
            }
        }
    }
}
