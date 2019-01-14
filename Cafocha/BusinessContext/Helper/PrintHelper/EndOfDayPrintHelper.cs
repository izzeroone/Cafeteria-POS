using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Cafocha.Entities;
using Cafocha.Repository.DAL;

namespace Cafocha.BusinessContext.Helper.PrintHelper
{
    public class EndOfDayPrintHelper : IPrintHelper
    {
        private static readonly string startupProjectPath =
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        private readonly RepositoryLocator _cloudPosUnitofwork;

        public EndOfDayPrintHelper(RepositoryLocator cloudPosUnitofwork)
        {
            _cloudPosUnitofwork = cloudPosUnitofwork;
            From = DateTime.Now.Date;
            To = DateTime.Now.Date;
            To = To.AddDays(1);
        }

        public DateTime From { get; set; }
        public DateTime To { get; set; }

        public FlowDocument CreateDocument()
        {
            return CreateEndOfDayDocument();
        }

        public FlowDocument CreateEndOfDayDocument()
        {
            // Create a FlowDocument
            var doc = new FlowDocument();

            // Set Margin
            doc.PagePadding = new Thickness(0);


            // Set PageHeight and PageWidth to "Auto".
            doc.PageHeight = double.NaN;
            doc.PageWidth = 290;

            // Create a Section
            var sec = new Section();


            // Head Text
            var blkHeadText = new BlockUIContainer();
            Generate_HeadText(blkHeadText);


            // Table Total Sales Text
            var blkTableTotalSalesText = new BlockUIContainer
            {
                Margin = new Thickness(0, 10, 0, 0)
            };
            Generate_TableTotalSalesText(blkTableTotalSalesText);


            // Table Payment And Refund Text
            var blkTablePayAndRefundText = new BlockUIContainer
            {
                Margin = new Thickness(0, 10, 0, 0)
            };
            Generate_TablePayAndRefundText(blkTablePayAndRefundText);


//            // Table Receipt Total Text
//            var blkTableReceiptText = new BlockUIContainer
//            {
//                Margin = new Thickness(0, 10, 0, 0)
//            };
//            Generate_TableReceiptText(blkTableReceiptText);


            //// Add Paragraph to Section
            //sec.Blocks.Add(p1);
            sec.Blocks.Add(blkHeadText);
            sec.Blocks.Add(blkTableTotalSalesText);
            sec.Blocks.Add(blkTablePayAndRefundText);
//            sec.Blocks.Add(blkTableReceiptText);


            // Add Section to FlowDocument
            doc.Blocks.Add(sec);


            return doc;
        }


        private void Generate_HeadText(BlockUIContainer blkHeadText)
        {
            // Main stackPanel of Head Text
            var stpHeadText = new StackPanel
            {
                Orientation = Orientation.Vertical
            };

            var stpLogo = new StackPanel();
            var imgOwner = new Image();
            var bimg = new BitmapImage();
            bimg.BeginInit();
            bimg.UriSource = new Uri(startupProjectPath + "\\Images\\logo.png", UriKind.Absolute);
            bimg.EndInit();
            imgOwner.Source = bimg;
            imgOwner.HorizontalAlignment = HorizontalAlignment.Left;
            imgOwner.Margin = new Thickness(85, 0, 0, 0);
            stpLogo.Children.Add(imgOwner);

            var stpPageName = new StackPanel();
            var txtPageName = new TextBlock
            {
                Text = "Báo cáo cuối ngày",
                FontSize = 13,
                FontFamily = new FontFamily("Century Gothic"),
                FontWeight = FontWeights.UltraBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            stpPageName.Children.Add(txtPageName);

            var txtFrom = new TextBlock
            {
                Text = "Từ: " + From.ToShortDateString(),
                FontSize = 11,
                FontFamily = new FontFamily("Century Gothic"),
                FontWeight = FontWeights.UltraBold,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10)
            };

            var txtTo = new TextBlock
            {
                Text = "Đến: " + To.ToShortDateString(),
                FontSize = 11,
                FontFamily = new FontFamily("Century Gothic"),
                FontWeight = FontWeights.UltraBold,
                HorizontalAlignment = HorizontalAlignment.Left,
                Margin = new Thickness(0, 0, 0, 10)
            };


            stpHeadText.Children.Add(stpLogo);
            stpHeadText.Children.Add(stpPageName);
            stpHeadText.Children.Add(txtFrom);
            stpHeadText.Children.Add(txtTo);

            blkHeadText.Child = stpHeadText;
        }

        private void Generate_TableTotalSalesText(BlockUIContainer blkTableTotalSalesText)
        {
            //// Main stackPanel in Table Text
            var stpTableTotalSalesText = new StackPanel();

            // Seperate Line
            var separator1 = new Rectangle();
            separator1.Stroke = new SolidColorBrush(Colors.Black);
            separator1.StrokeThickness = 3;
            separator1.Height = 3;
            separator1.Width = double.NaN;

            //Table Header
            var stpHeader = new StackPanel();
            var tbHeader = new TextBlock
            {
                Text = "Doanh số bán hàng",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 13,
                Margin = new Thickness(0, 5, 0, 5),
                FontWeight = FontWeights.Bold
            };
            stpHeader.Children.Add(tbHeader);

            // Seperate Line
            var separator2 = new Rectangle();
            separator2.Stroke = new SolidColorBrush(Colors.Black);
            separator2.StrokeThickness = 3;
            separator2.Height = 3;
            separator2.Width = double.NaN;


            // Calculate Data
            var totalSalesData = CalculateTotalSales();


            // Create Table
            var dgDataTable = new Grid();
            dgDataTable.Width = 300;
            // set Columns
            for (var i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    var firstCol = new ColumnDefinition();
                    firstCol.Width = new GridLength(180);
                    dgDataTable.ColumnDefinitions.Add(firstCol);
                    continue;
                }

                if (i == 1)
                {
                    var secondCol = new ColumnDefinition();
                    secondCol.Width = new GridLength(80);
                    dgDataTable.ColumnDefinitions.Add(secondCol);
                }
            }

            // set Rows
            for (var i = 0; i < totalSalesData.Count; i++)
            {
                dgDataTable.RowDefinitions.Add(new RowDefinition());
                foreach (var item in totalSalesData.Values) dgDataTable.RowDefinitions.Add(new RowDefinition());
            }

            // Fill Table data
            var rowIndex = 0;
            foreach (var item in totalSalesData)
            {
                var txtMeta = new TextBlock
                {
                    Text = item.Key,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                Grid.SetRow(txtMeta, rowIndex);
                Grid.SetColumn(txtMeta, 0);
                dgDataTable.Children.Add(txtMeta);

                foreach (var keypairvalue in item.Value)
                {
                    var stpLeftData = new StackPanel
                    {
                        Orientation = Orientation.Horizontal
                    };
                    var txtTitle = new TextBlock
                    {
                        Text = keypairvalue.Title + ": ",
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    var txtCount = new TextBlock
                    {
                        Text = keypairvalue.Count.ToString(),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    stpLeftData.Children.Add(txtTitle);
                    stpLeftData.Children.Add(txtCount);
                    Grid.SetRow(stpLeftData, rowIndex + 1);
                    Grid.SetColumn(stpLeftData, 0);
                    dgDataTable.Children.Add(stpLeftData);

                    var txtAmount = new TextBlock
                    {
                        Text = string.Format("{0:0.000}", keypairvalue.Amount),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Right
                    };
                    Grid.SetRow(txtAmount, rowIndex + 1);
                    Grid.SetColumn(txtAmount, 1);
                    dgDataTable.Children.Add(txtAmount);

                    rowIndex++;
                }

                rowIndex++;
            }

            stpTableTotalSalesText.Children.Add(separator1);
            stpTableTotalSalesText.Children.Add(stpHeader);
            stpTableTotalSalesText.Children.Add(separator2);
            stpTableTotalSalesText.Children.Add(dgDataTable);

            blkTableTotalSalesText.Child = stpTableTotalSalesText;
        }

        private void Generate_TablePayAndRefundText(BlockUIContainer blkTablePayAndRefundText)
        {
            //// Main stackPanel in Table Text
            var stpTablePayAndRefundText = new StackPanel();

            // Seperate Line
            var separator1 = new Rectangle();
            separator1.Stroke = new SolidColorBrush(Colors.Black);
            separator1.StrokeThickness = 3;
            separator1.Height = 3;
            separator1.Width = double.NaN;

            //Table Header
            var stpHeader = new StackPanel();
            var tbHeader = new TextBlock
            {
                Text = "Thanh toán và trả tiền",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 12,
                Margin = new Thickness(0, 5, 0, 5),
                FontWeight = FontWeights.Bold
            };
            stpHeader.Children.Add(tbHeader);

            // Seperate Line
            var separator2 = new Rectangle();
            separator2.Stroke = new SolidColorBrush(Colors.Black);
            separator2.StrokeThickness = 3;
            separator2.Height = 3;
            separator2.Width = double.NaN;


            // Calculate Data
            var totalPayAndRefundData = CalculatePayAndRefund();


            // Create Table
            var dgDataTable = new Grid();
            dgDataTable.Width = 300;
            // set Columns
            for (var i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    var firstCol = new ColumnDefinition();
                    firstCol.Width = new GridLength(180);
                    dgDataTable.ColumnDefinitions.Add(firstCol);
                    continue;
                }

                if (i == 1)
                {
                    var secondCol = new ColumnDefinition();
                    secondCol.Width = new GridLength(80);
                    dgDataTable.ColumnDefinitions.Add(secondCol);
                }
            }

            // set Rows
            for (var i = 0; i < totalPayAndRefundData.Count; i++)
            {
                dgDataTable.RowDefinitions.Add(new RowDefinition());
                foreach (var item in totalPayAndRefundData.Values) dgDataTable.RowDefinitions.Add(new RowDefinition());
            }

            // Fill Table data
            var rowIndex = 0;
            foreach (var item in totalPayAndRefundData)
            {
                var txtMeta = new TextBlock
                {
                    Text = item.Key,
                    VerticalAlignment = VerticalAlignment.Top,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    FontWeight = FontWeights.Bold,
                    Margin = new Thickness(0, 10, 0, 0)
                };
                Grid.SetRow(txtMeta, rowIndex);
                Grid.SetColumn(txtMeta, 0);
                dgDataTable.Children.Add(txtMeta);

                foreach (var keypairvalue in item.Value)
                {
                    var stpLeftData = new StackPanel
                    {
                        Orientation = Orientation.Horizontal
                    };
                    var txtTitle = new TextBlock
                    {
                        Text = keypairvalue.Title + ": ",
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    var txtCount = new TextBlock
                    {
                        Text = keypairvalue.Count.ToString(),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Left
                    };
                    stpLeftData.Children.Add(txtTitle);
                    stpLeftData.Children.Add(txtCount);
                    Grid.SetRow(stpLeftData, rowIndex + 1);
                    Grid.SetColumn(stpLeftData, 0);
                    dgDataTable.Children.Add(stpLeftData);

                    var txtAmount = new TextBlock
                    {
                        Text = string.Format("{0:0.000}", keypairvalue.Amount),
                        VerticalAlignment = VerticalAlignment.Top,
                        HorizontalAlignment = HorizontalAlignment.Right
                    };
                    Grid.SetRow(txtAmount, rowIndex + 1);
                    Grid.SetColumn(txtAmount, 1);
                    dgDataTable.Children.Add(txtAmount);

                    rowIndex++;
                }

                rowIndex++;
            }

            stpTablePayAndRefundText.Children.Add(separator1);
            stpTablePayAndRefundText.Children.Add(stpHeader);
            stpTablePayAndRefundText.Children.Add(separator2);
            stpTablePayAndRefundText.Children.Add(dgDataTable);

            blkTablePayAndRefundText.Child = stpTablePayAndRefundText;
        }

        private void Generate_TableReceiptText(BlockUIContainer blkTableReceiptText)
        {
            //// Main stackPanel in Table Text
            var stpTableReceiptText = new StackPanel();

            // Seperate Line
            var separator1 = new Rectangle();
            separator1.Stroke = new SolidColorBrush(Colors.Black);
            separator1.StrokeThickness = 3;
            separator1.Height = 3;
            separator1.Width = double.NaN;

            //Table Header
            var stpHeader = new StackPanel();
            var tbHeader = new TextBlock
            {
                Text = "Tổng hóa đơn",
                HorizontalAlignment = HorizontalAlignment.Center,
                FontSize = 12,
                Margin = new Thickness(0, 5, 0, 5),
                FontWeight = FontWeights.Bold
            };
            stpHeader.Children.Add(tbHeader);

            // Seperate Line
            var separator2 = new Rectangle();
            separator2.Stroke = new SolidColorBrush(Colors.Black);
            separator2.StrokeThickness = 3;
            separator2.Height = 3;
            separator2.Width = double.NaN;


            // Calculate Data
//            var totalReceiptData = CalculateReceipt();


            // Create Table
            var dgDataTable = new Grid();
            dgDataTable.Width = 300;
            // set Columns
            for (var i = 0; i < 2; i++)
            {
                if (i == 0)
                {
                    var firstCol = new ColumnDefinition();
                    firstCol.Width = new GridLength(180);
                    dgDataTable.ColumnDefinitions.Add(firstCol);
                    continue;
                }

                if (i == 1)
                {
                    var secondCol = new ColumnDefinition();
                    secondCol.Width = new GridLength(80);
                    dgDataTable.ColumnDefinitions.Add(secondCol);
                }
            }
//            // set Rows
//            for (int i = 0; i < totalReceiptData.Count; i++)
//            {
//                dgDataTable.RowDefinitions.Add(new RowDefinition());
//                foreach (var item in totalReceiptData.Values)
//                {
//                    dgDataTable.RowDefinitions.Add(new RowDefinition());
//                }
//            }

//            // Fill Table data
//            int rowIndex = 0;
//            foreach (var item in totalReceiptData)
//            {
//                TextBlock txtMeta = new TextBlock()
//                {
//                    Text = item.Key,
//                    VerticalAlignment = VerticalAlignment.Top,
//                    HorizontalAlignment = HorizontalAlignment.Left,
//                    FontWeight = FontWeights.Bold,
//                    Margin = new Thickness(0, 10, 0, 0)
//                };
//                Grid.SetRow(txtMeta, rowIndex);
//                Grid.SetColumn(txtMeta, 0);
//                dgDataTable.Children.Add(txtMeta);
//
//                foreach (var keypairvalue in item.Value)
//                {
//                    StackPanel stpLeftData = new StackPanel()
//                    {
//                        Orientation = Orientation.Horizontal
//                    };
//                    TextBlock txtTitle = new TextBlock()
//                    {
//                        Text = keypairvalue.Title + ": ",
//                        VerticalAlignment = VerticalAlignment.Top,
//                        HorizontalAlignment = HorizontalAlignment.Left,
//                        Margin = new Thickness(10, 0, 0, 0)
//                    };
//                    TextBlock txtCount = new TextBlock()
//                    {
//                        Text = keypairvalue.Count.ToString(),
//                        VerticalAlignment = VerticalAlignment.Top,
//                        HorizontalAlignment = HorizontalAlignment.Left
//                    };
//                    stpLeftData.Children.Add(txtTitle);
//                    stpLeftData.Children.Add(txtCount);
//                    Grid.SetRow(stpLeftData, rowIndex + 1);
//                    Grid.SetColumn(stpLeftData, 0);
//                    dgDataTable.Children.Add(stpLeftData);
//
//                    TextBlock txtAmount = new TextBlock()
//                    {
//                        Text = string.Format("{0:0.000}", keypairvalue.Amount),
//                        VerticalAlignment = VerticalAlignment.Top,
//                        HorizontalAlignment = HorizontalAlignment.Right
//                    };
//                    Grid.SetRow(txtAmount, rowIndex + 1);
//                    Grid.SetColumn(txtAmount, 1);
//                    dgDataTable.Children.Add(txtAmount);
//
//                    rowIndex++;
//                }
//
//                rowIndex++;
//            }


            stpTableReceiptText.Children.Add(separator1);
            stpTableReceiptText.Children.Add(stpHeader);
            stpTableReceiptText.Children.Add(separator2);
            stpTableReceiptText.Children.Add(dgDataTable);


            blkTableReceiptText.Child = stpTableReceiptText;
        }


        private Dictionary<string, List<MyPairValue>> CalculateTotalSales()
        {
            var result = new Dictionary<string, List<MyPairValue>>();
            var orderDetailsQuery =
                _cloudPosUnitofwork.OrderDetailsRepository.Get(x => x.OrderNote.OrderTime.CompareTo(From) >= 0
                                                                    && x.OrderNote.OrderTime.CompareTo(To) <= 0);
            var orderQuery =
                _cloudPosUnitofwork.OrderRepository.Get(x => x.OrderTime.CompareTo(From) >= 0
                                                             && x.OrderTime.CompareTo(To) <= 0);


            // Total Dribnk
            var orderDetailsAlcoholQuery = orderDetailsQuery.Where(x => x.Product.Type == (int) ProductType.Drink);
            decimal alcoholTotalAmount = 0;
            foreach (var orderDetails in orderDetailsAlcoholQuery)
                alcoholTotalAmount +=
                    orderDetails.Quan * orderDetails.Product.Price * (100 - orderDetails.Discount) / 100;
            var alcoholCal = new MyPairValue
            {
                Title = "Số lượng",
                Count = orderDetailsAlcoholQuery.Count(),
                Amount = alcoholTotalAmount
            };
            result.Add("Đồ uống", new List<MyPairValue>
            {
                alcoholCal
            });


            // Total Topping
            var orderDetailsBeverageQuery = orderDetailsQuery.Where(x => x.Product.Type == (int) ProductType.Topping);
            decimal beverageTotalAmount = 0;
            foreach (var orderDetails in orderDetailsBeverageQuery)
                beverageTotalAmount +=
                    orderDetails.Quan * orderDetails.Product.Price * (100 - orderDetails.Discount) / 100;
            var beverageCal = new MyPairValue
            {
                Title = "Số lượng",
                Count = orderDetailsBeverageQuery.Count(),
                Amount = beverageTotalAmount
            };
            result.Add("Topping", new List<MyPairValue>
            {
                beverageCal
            });


            // Total Dessert
            var orderDetailsFoodQuery = orderDetailsQuery.Where(x => x.Product.Type == (int) ProductType.Dessert);
            decimal foodTotalAmount = 0;
            foreach (var orderDetails in orderDetailsFoodQuery)
                foodTotalAmount += orderDetails.Quan * orderDetails.Product.Price * (100 - orderDetails.Discount) / 100;
            var foodCal = new MyPairValue
            {
                Title = "Số lượng",
                Count = orderDetailsFoodQuery.Count(),
                Amount = foodTotalAmount
            };
            result.Add("Đồ ngọt", new List<MyPairValue>
            {
                foodCal
            });


            // Total Other
            var orderDetailsOtherQuery = orderDetailsQuery.Where(x => x.Product.Type == (int) ProductType.Other);
            decimal otherTotalAmount = 0;
            foreach (var orderDetails in orderDetailsOtherQuery)
                otherTotalAmount +=
                    orderDetails.Quan * orderDetails.Product.Price * (100 - orderDetails.Discount) / 100;
            var otherCal = new MyPairValue
            {
                Title = "Số lượng",
                Count = orderDetailsOtherQuery.Count(),
                Amount = otherTotalAmount
            };
            result.Add("Khác", new List<MyPairValue>
            {
                otherCal
            });


            // SubTotal
            // real TotalAmount
            decimal totalAmount = 0;
            foreach (var orderDetails in orderDetailsQuery)
                totalAmount += orderDetails.Quan * orderDetails.Product.Price * (100 - orderDetails.Discount) / 100;
            var orderTotalCal = new MyPairValue
            {
                Title = "Đơn hàng",
                Amount = totalAmount,
                Count = orderQuery.Count()
            };
            // VAT
            decimal totalVAT = 0;
            foreach (var order in orderQuery)
            {
                decimal curTotalAmount = 0;
                foreach (var orderDetails in order.OrderNoteDetails)
                    curTotalAmount += orderDetails.Quan * orderDetails.Product.Price * (100 - orderDetails.Discount) /
                                      100;

                totalVAT += curTotalAmount * 10 / 100;
            }

            var VATTotalCal = new MyPairValue
            {
                Title = "VAT",
                Amount = totalVAT,
                Count = orderQuery.Count()
            };

            // DIscount
            decimal totalDisc = 0;
            var countDisc = 0;
            foreach (var order in orderQuery)
                if (order.Discount != 0)
                {
                    totalDisc += order.TotalPriceNonDisc - order.TotalPrice;
                    countDisc++;
                }

            var DiscTotalCal = new MyPairValue
            {
                Title = "Giảm giá",
                Amount = totalDisc,
                Count = countDisc
            };

            result.Add("Tổng số phụ", new List<MyPairValue>
            {
                orderTotalCal,
                VATTotalCal,
                DiscTotalCal
            });


            // Total
            decimal total = 0;
            foreach (var order in orderQuery) total += order.TotalPrice;
            var totalCal = new MyPairValue
            {
                Title = "Đơn hàng",
                Amount = total,
                Count = orderQuery.Count()
            };
            result.Add("Tổng", new List<MyPairValue>
            {
                totalCal
            });

            return result;
        }

        private Dictionary<string, List<MyPairValue>> CalculatePayAndRefund()
        {
            var result = new Dictionary<string, List<MyPairValue>>();
            var orderQuery =
                _cloudPosUnitofwork.OrderRepository.Get(x => x.OrderTime.CompareTo(From) >= 0
                                                             && x.OrderTime.CompareTo(To) <= 0);

            //Total Cash
            var orderCashQuery = orderQuery.Where(x => x.paymentMethod == (int) PaymentMethod.Cash);
            decimal cashTotalAmount = 0;
            foreach (var order in orderCashQuery) cashTotalAmount += order.TotalPrice;
            var cashCal = new MyPairValue
            {
                Title = "Đơn hàng",
                Count = orderCashQuery.Count(),
                Amount = cashTotalAmount
            };
            result.Add("Tiền mặt", new List<MyPairValue>
            {
                cashCal
            });

            //Total Credit
            var orderCreditQuery = orderQuery.Where(x => x.paymentMethod == (int) PaymentMethod.Credit);
            decimal creditTotalAmount = 0;
            foreach (var order in orderCreditQuery) creditTotalAmount += order.TotalPrice;
            var creditCal = new MyPairValue
            {
                Title = "Đơn hàng",
                Count = orderCreditQuery.Count(),
                Amount = creditTotalAmount
            };
            result.Add("Thẻ", new List<MyPairValue>
            {
                creditCal
            });



            return result;
        }

//        private Dictionary<string, List<MyPairValue>> CalculateReceipt()
//        {
//            var result = new Dictionary<string, List<MyPairValue>>();
//
//            var receiptQuery =
//                _cloudPosUnitofwork.ReceiptNoteRepository.Get(x => x.Inday.CompareTo(From) >= 0
//                                                     && x.Inday.CompareTo(To) <= 0);
//
//            //Total Receipt
//            decimal totalAmount = 0;
//            foreach (var order in receiptQuery)
//            {
//                totalAmount += order.TotalAmount;
//            }
//            MyPairValue receiptCal = new MyPairValue()
//            {
//                Title = "Bills",
//                Count = receiptQuery.Count(),
//                Amount = totalAmount
//            };
//            result.Add("Total", new List<MyPairValue>()
//            {
//                receiptCal
//            });
//
//            return result;
//        }
    }

    public class MyPairValue
    {
        public string Title { get; set; }
        public int Count { get; set; }
        public decimal Amount { get; set; }
    }
}