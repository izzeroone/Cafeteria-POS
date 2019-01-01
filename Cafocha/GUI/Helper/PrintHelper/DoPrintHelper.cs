using System;
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
        public static readonly int Receipt_Printing = 5;
        public static readonly int Eod_Printing = 3;

        private readonly RepositoryLocator _unitofwork;

        private IPrintHelper ph;
        private int type;

        private readonly OrderNote curOrder;
        private PrintDialog printDlg;

        private string _receptionPrinter;
        private bool isShowReview;

        public DoPrintHelper(RepositoryLocator unitofwork, int printType)
        {
            _unitofwork = unitofwork;
            type = printType;
            printDlg = new PrintDialog();

            string[] result = ReadWriteData.ReadPrinterSetting();
            if (result != null)
            {
                _receptionPrinter = result[0];

                if (int.Parse(result[3]) == 1)
                    isShowReview = true;
                else
                    isShowReview = false;
            }
            else
            {
                _receptionPrinter = "";
                isShowReview = true;
            }
        }

        public DoPrintHelper(RepositoryLocator unitofwork, int printType, OrderNote currentOrder)
        {
            _unitofwork = unitofwork;
            type = printType;
            curOrder = currentOrder;
            printDlg = new PrintDialog();

            string[] result = ReadWriteData.ReadPrinterSetting();
            if (result != null)
            {
                _receptionPrinter = result[0];

                if (int.Parse(result[3]) == 1)
                    isShowReview = true;
                else
                    isShowReview = false;
            }
            else
            {
                _receptionPrinter = "";
                isShowReview = true;
            }
        }

        public void DoPrint()
        {
//            if (curOrder == null)
//            {
//                return;
//            }

            try
            {
                // Create a PrintHelper
                CreatePrintHelper();

                // Create a FlowDocument dynamically.
                FlowDocument doc = ph.CreateDocument();
                doc.Name = "FlowDoc";


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

                var order = new OrderForPrint().GetAndConvertOrder(curOrder);
                order = order.GetAndConverOrderDetails(curOrder, _unitofwork);
                ph = new ReceiptPrintHelper()
                {
                    Owner = new Owner()
                    {
                        ImgName = "logo.png",
                        Address = "Address: f.7th, Fafilm Building, 6 St.Thai Van Lung, w.Ben Nghe, HCM City, Viet Nam",
                        Phone = "",
                        PageName = "RECEIPT"
                    },

                    Order = order,

                };
            }


            if (type == TempReceipt_Printing)
            {
                if(!string.IsNullOrEmpty(_receptionPrinter))
                    printDlg.PrintQueue =new PrintQueue(new PrintServer(), _receptionPrinter);

                var order = new OrderForPrint().GetAndConvertOrder(curOrder);
                order = order.GetAndConverOrderDetails(curOrder, _unitofwork);

                ph = new ReceiptPrintHelper()
                {
                    Owner = new Owner()
                    {
                        ImgName = "logo.png",
                        Address = "Address: f.7th, Fafilm Building, 6 St.Thai Van Lung, w.Ben Nghe, HCM City, Viet Nam",
                        Phone = "",
                        PageName = "RECEIPT"
                    },

                    Order = order
                };
            }

        

            if (type == Eod_Printing)
            {
                if (!string.IsNullOrEmpty(_receptionPrinter))
                    printDlg.PrintQueue = new PrintQueue(new PrintServer(), _receptionPrinter);

                ph = new EndOfDayPrintHelper(_unitofwork);
            }

        }
    }
}
