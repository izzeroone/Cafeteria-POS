using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Cafocha.BusinessContext.Helper.PrintHelper.Model;

namespace Cafocha.BusinessContext.Helper.PrintHelper
{
    public class ReceiptPrintHelper : IPrintHelper
    {
        private static readonly string startupProjectPath =
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        private readonly int SETORDER = 1;

        public ReceiptPrintHelper()
        {
        }

        public ReceiptPrintHelper(Owner owner)
        {
            Owner = owner;
        }

        public Owner Owner { get; set; }

        public OrderForPrint Order { get; set; }

        public int OrderMode { get; set; }

        public FlowDocument CreateDocument()
        {
            return CreateReceiptDocument();
        }


        /// <summary>
        ///     Create a complete Receipt
        /// </summary>
        /// <returns></returns>
        public FlowDocument CreateReceiptDocument()
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
            if (Owner != null) Generate_HeadText(blkHeadText, Owner);


            // Info Text
            var blkInfoText = new BlockUIContainer();
            Generate_InfoText(blkInfoText, Order.getMetaReceiptInfo());

            // Table Text
            var blkTableText = new BlockUIContainer
            {
                Margin = new Thickness(0, 10, 0, 0)
            };
            Generate_TableText(blkTableText, Order.getMetaReceiptTable(), Order.GetOrderDetailsForReceipt());

            // Summary Text
            var blkSummaryText = new BlockUIContainer
            {
                Margin = new Thickness(0, 10, 0, 0)
            };
            Generate_SummaryText(blkSummaryText, Order, "vnd");


            // Food Text
            var blkFootText = new BlockUIContainer
            {
                Margin = new Thickness(0, 20, 0, 0)
            };
            Generate_FootText(blkFootText);

            //// Add Paragraph to Section
            //sec.Blocks.Add(p1);
            sec.Blocks.Add(blkHeadText);
            sec.Blocks.Add(blkInfoText);
            sec.Blocks.Add(blkTableText);
            sec.Blocks.Add(blkSummaryText);
            sec.Blocks.Add(blkFootText);

            // Add Section to FlowDocument
            doc.Blocks.Add(sec);


            return doc;
        }


        /// <summary>
        ///     Create the Foot Section of Receipt
        /// </summary>
        /// <param name="blkFootText"></param>
        private void Generate_FootText(BlockUIContainer blkFootText)
        {
            // Main stackPanel of Foot Text
            var stpFootText = new StackPanel();


            var stpExp = new StackPanel();
            var txtExp = new TextBlock
            {
                Text = "receipt is available only in one month.",
                FontSize = 11,
                FontFamily = new FontFamily("Century Gothic"),
                FontWeight = FontWeights.UltraBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0)
            };
            stpExp.Children.Add(txtExp);


            var stpThank = new StackPanel();
            var txtThank = new TextBlock
            {
                Text = "Thank you!",
                FontSize = 11,
                FontFamily = new FontFamily("Century Gothic"),
                FontWeight = FontWeights.UltraBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0)
            };
            stpThank.Children.Add(txtThank);

            var stpSeeAgain = new StackPanel();
            var txtSeeAgain = new TextBlock
            {
                Text = "See You Again!",
                FontSize = 11,
                FontFamily = new FontFamily("Century Gothic"),
                FontWeight = FontWeights.UltraBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 0)
            };
            stpSeeAgain.Children.Add(txtSeeAgain);

            var rand = new Random();
            var stpEmo = new StackPanel();
            var imgEmo = new Image();
            var bimg = new BitmapImage();
            bimg.BeginInit();
            bimg.UriSource = new Uri(startupProjectPath + "\\Images\\rand" + rand.Next(5) + ".png", UriKind.Absolute);
            bimg.EndInit();
            imgEmo.Source = bimg;
            imgEmo.HorizontalAlignment = HorizontalAlignment.Center;
            imgEmo.Margin = new Thickness(125, 0, 0, 0);
            stpEmo.Children.Add(imgEmo);


            stpFootText.Children.Add(stpExp);
            stpFootText.Children.Add(stpThank);
            stpFootText.Children.Add(stpSeeAgain);
            stpFootText.Children.Add(stpEmo);

            blkFootText.Child = stpFootText;
        }

        /// <summary>
        ///     Create the summary section of Receipt
        /// </summary>
        /// <param name="blkTableText"></param>
        /// <param name="order">the order data for filling the receip</param>
        /// <param name="moneyUnit"></param>
        private void Generate_SummaryText(BlockUIContainer blkSummaryText, OrderForPrint order, string moneyUnit)
        {
            var stpSummary = new StackPanel
            {
                Orientation = Orientation.Vertical
            };


            // Sale Value
            var stpSaleValue = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            var tbSaleValueLable = new TextBlock
            {
                Text = "Sale Value:",
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 12,
                FontWeight = FontWeights.UltraBold,
                Margin = new Thickness(90, 0, 0, 0),
                Width = 120
            };
            var tbSaleValueValue = new TextBlock
            {
                Text = string.Format("{0:0.000}", order.SaleValue),
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 11,
                Width = 60,
                TextAlignment = TextAlignment.Right
            };
            stpSaleValue.Children.Add(tbSaleValueLable);
            stpSaleValue.Children.Add(tbSaleValueValue);


            // Service Charge
            var stpSVC = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            var tbSVCLable = new TextBlock
            {
                Text = "SVC (5%):",
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 12,
                FontWeight = FontWeights.UltraBold,
                Margin = new Thickness(90, 0, 0, 0),
                Width = 120
            };

            // VAT
            var stpVAT = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            var tbVATLable = new TextBlock
            {
                Text = "VAT (10%):",
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 12,
                FontWeight = FontWeights.UltraBold,
                Margin = new Thickness(90, 0, 0, 0),
                Width = 120
            };
            var tbVATValue = new TextBlock
            {
                Text = string.Format("{0:0.000}", order.Vat),
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 11,
                Width = 60,
                TextAlignment = TextAlignment.Right
            };
            stpVAT.Children.Add(tbVATLable);
            stpVAT.Children.Add(tbVATValue);


            // Total Price
            var stpTotalPrice = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            var tbTotalPriceLable = new TextBlock
            {
                Text = "Total Amount " + "(" + moneyUnit + "):",
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 12,
                FontWeight = FontWeights.UltraBold,
                Margin = new Thickness(90, 0, 0, 0),
                Width = 120
            };
            var tbTotalPriceValue = new TextBlock
            {
                Text = string.Format("{0:0.000}", order.TotalPrice),
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 11,
                Width = 60,
                TextAlignment = TextAlignment.Right
            };
            stpTotalPrice.Children.Add(tbTotalPriceLable);
            stpTotalPrice.Children.Add(tbTotalPriceValue);

            // Customer Pay
            var stpCustomerPay = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            var tbCustomerPayLable = new TextBlock
            {
                Text = "Customer Pay:",
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 12,
                FontWeight = FontWeights.UltraBold,
                Margin = new Thickness(90, 0, 0, 0),
                Width = 120
            };
            var tbCustomerPayValue = new TextBlock
            {
                Text = string.Format("{0:0.000}", order.CustomerPay),
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 11,
                Width = 60,
                TextAlignment = TextAlignment.Right
            };
            stpCustomerPay.Children.Add(tbCustomerPayLable);
            stpCustomerPay.Children.Add(tbCustomerPayValue);

            // Pay Back
            var stpPayBack = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            var tbPayBackLable = new TextBlock
            {
                Text = "Change:",
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 12,
                FontWeight = FontWeights.UltraBold,
                Margin = new Thickness(90, 0, 0, 0),
                Width = 120
            };
            var tbPayBackValue = new TextBlock
            {
                Text = string.Format("{0:0.000}", order.PayBack),
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 11,
                Width = 60,
                TextAlignment = TextAlignment.Right
            };
            stpPayBack.Children.Add(tbPayBackLable);
            stpPayBack.Children.Add(tbPayBackValue);


            stpSummary.Children.Add(stpSaleValue);
            stpSummary.Children.Add(stpSVC);
            stpSummary.Children.Add(stpVAT);
            stpSummary.Children.Add(stpTotalPrice);
            stpSummary.Children.Add(stpCustomerPay);
            stpSummary.Children.Add(stpPayBack);


            blkSummaryText.Child = stpSummary;
        }

        /// <summary>
        ///     Create the table section of Receipt
        /// </summary>
        /// <param name="blkTableText"></param>
        /// <param name="gridMeta">The meta header of the table</param>
        /// <param name="listData">The data source for table</param>
        private void Generate_TableText(BlockUIContainer blkTableText, string[] gridMeta,
            List<OrderDetailsForPrint> listData)
        {
            var dgDataTable = new Grid();
            dgDataTable.Width = 300;
            // set Columns
            for (var i = 0; i < 4; i++)
            {
                if (i == 0)
                {
                    var firstCol = new ColumnDefinition();
                    firstCol.Width = new GridLength(120);
                    dgDataTable.ColumnDefinitions.Add(firstCol);
                    continue;
                }

                if (i == 1)
                {
                    var secondCol = new ColumnDefinition();
                    secondCol.Width = new GridLength(30);
                    dgDataTable.ColumnDefinitions.Add(secondCol);
                    continue;
                }

                if (i == 2)
                {
                    var otherCol = new ColumnDefinition();
                    otherCol.Width = new GridLength(55);
                    dgDataTable.ColumnDefinitions.Add(otherCol);
                }
                else
                {
                    var otherCol = new ColumnDefinition();
                    otherCol.Width = new GridLength(65);
                    dgDataTable.ColumnDefinitions.Add(otherCol);
                }
            }

            // set Rows
            for (var i = 0; i < listData.Count + 2; i++) dgDataTable.RowDefinitions.Add(new RowDefinition());


            // add Meta
            for (var i = 0; i < 4; i++)
            {
                var txtMeta = new TextBlock();
                if (i == 2 || i == 3)
                {
                    txtMeta.Text = gridMeta[i];
                    txtMeta.FontSize = 11;
                    txtMeta.FontWeight = FontWeights.Bold;
                    txtMeta.VerticalAlignment = VerticalAlignment.Stretch;
                    txtMeta.TextAlignment = TextAlignment.Right;
                    Grid.SetRow(txtMeta, 0);
                    Grid.SetColumn(txtMeta, i);
                }
                else
                {
                    txtMeta.Text = gridMeta[i];
                    txtMeta.FontSize = 11;
                    txtMeta.FontWeight = FontWeights.Bold;
                    txtMeta.VerticalAlignment = VerticalAlignment.Stretch;
                    Grid.SetRow(txtMeta, 0);
                    Grid.SetColumn(txtMeta, i);
                }

                dgDataTable.Children.Add(txtMeta);
            }

            var rowIndex = 1;
           

            foreach (var orderItem in listData)
            {
                var txtProductName = new TextBlock();
                txtProductName.Width = 115;
                txtProductName.Text = seperateLongProductName(orderItem.ProductName);
                txtProductName.FontSize = 11;
                txtProductName.VerticalAlignment = VerticalAlignment.Top;
                txtProductName.HorizontalAlignment = HorizontalAlignment.Left;
                txtProductName.Margin = new Thickness(0, 0, 0, 5);
                Grid.SetRow(txtProductName, rowIndex);
                Grid.SetColumn(txtProductName, 0);
                dgDataTable.Children.Add(txtProductName);

                var txtQuan = new TextBlock();
                txtQuan.Text = orderItem.Quan.ToString();
                txtQuan.FontSize = 11;
                txtQuan.VerticalAlignment = VerticalAlignment.Top;
                txtQuan.Margin = new Thickness(0, 0, 0, 5);
                Grid.SetRow(txtQuan, rowIndex);
                Grid.SetColumn(txtQuan, 1);
                dgDataTable.Children.Add(txtQuan);

                var txtPrice = new TextBlock();
                if (OrderMode == SETORDER)
                    txtPrice.Text = "";
                else
                    txtPrice.Text = string.Format("{0:0.000}", orderItem.ProductPrice);

                txtPrice.FontSize = 11;
                txtPrice.VerticalAlignment = VerticalAlignment.Stretch;
                txtPrice.HorizontalAlignment = HorizontalAlignment.Right;
                txtPrice.Margin = new Thickness(0, 0, 0, 5);
                Grid.SetRow(txtPrice, rowIndex);
                Grid.SetColumn(txtPrice, 2);
                dgDataTable.Children.Add(txtPrice);

                var txtAmt = new TextBlock();
                if (OrderMode == SETORDER)
                    txtAmt.Text = "";
                else
                    txtAmt.Text = string.Format("{0:0.000}", orderItem.Amt);
                txtAmt.FontSize = 11;
                txtAmt.VerticalAlignment = VerticalAlignment.Stretch;
                txtAmt.TextAlignment = TextAlignment.Right;
                txtAmt.Margin = new Thickness(0, 0, 0, 5);
                Grid.SetRow(txtAmt, rowIndex);
                Grid.SetColumn(txtAmt, 3);
                dgDataTable.Children.Add(txtAmt);

                rowIndex++;
            }


            blkTableText.Child = dgDataTable;
        }

        /// <summary>
        ///     Create the Info section of Receipt
        /// </summary>
        /// <param name="blkInfoText"></param>
        /// <param name="infos">The information of Receipt</param>
        private void Generate_InfoText(BlockUIContainer blkInfoText, Dictionary<string, string> infos)
        {
            var stpMain = new StackPanel();

            foreach (var info in infos)
            {
                var stpInfo = new StackPanel
                {
                    Orientation = Orientation.Horizontal
                };
                var tbInfoKey = new TextBlock
                {
                    Text = info.Key + ":",
                    FontFamily = new FontFamily("Century Gothic"),
                    FontSize = 10,
                    FontWeight = FontWeights.UltraBold,
                    Margin = new Thickness(0, 0, 10, 0),
                    Width = 70
                };
                var tbInfoValue = new TextBlock
                {
                    Text = info.Value,
                    FontFamily = new FontFamily("Century Gothic"),
                    FontSize = 10,
                    Width = 150
                };
                stpInfo.Children.Add(tbInfoKey);
                stpInfo.Children.Add(tbInfoValue);

                stpMain.Children.Add(stpInfo);
            }

            blkInfoText.Child = stpMain;
        }


        /// <summary>
        ///     Create the Head section of Receipt
        /// </summary>
        /// <param name="blkHeadText"></param>
        private void Generate_HeadText(BlockUIContainer blkHeadText, Owner owner)
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
            bimg.UriSource = new Uri(startupProjectPath + "\\Images\\" + owner.ImgName, UriKind.Absolute);
            bimg.EndInit();
            imgOwner.Source = bimg;
            imgOwner.HorizontalAlignment = HorizontalAlignment.Center;
            imgOwner.Margin = new Thickness(85, 0, 0, 0);
            stpLogo.Children.Add(imgOwner);


            var address = "";
            // modify the long address
            if (owner.Address.Length > 54)
            {
                var address1st = owner.Address.Substring(0, 53);
                var address2nd = owner.Address.Substring(53);
                address = address1st + "\n\t" + address2nd;
            }
            else
            {
                address = owner.Address;
            }


            var txtAddress = new TextBlock
            {
                Text = "ADDRESS:  " + address,
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 10,
                FontWeight = FontWeights.UltraBold
            };
            var txtPhone = new TextBlock
            {
                Text = "PHONE:  " + owner.Phone,
                FontFamily = new FontFamily("Century Gothic"),
                FontSize = 10,
                Margin = new Thickness(0, 0, 0, 5),
                FontWeight = FontWeights.UltraBold
            };
            var stpPageName = new StackPanel();
            var txtPageName = new TextBlock
            {
                Text = owner.PageName,
                FontSize = 13,
                FontFamily = new FontFamily("Century Gothic"),
                FontWeight = FontWeights.UltraBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            stpPageName.Children.Add(txtPageName);


            stpHeadText.Children.Add(stpLogo);
            //stpHeadText.Children.Add(txtOwnerName);
            stpHeadText.Children.Add(txtAddress);
            stpHeadText.Children.Add(txtPhone);
            stpHeadText.Children.Add(stpPageName);

            blkHeadText.Child = stpHeadText;
        }


        private string seperateLongProductName(string productName)
        {
            var result = "";

            var splProductName = productName.Split(' ');
            result = splProductName[0];

            var line = result;
            for (var i = 1; i < splProductName.Length; i++)
            {
                line += " " + splProductName[i];
                if (line.Length > 16)
                {
                    result += "\n" + splProductName[i];
                    line = splProductName[i];
                }
                else
                {
                    result += " " + splProductName[i];
                }
            }

            return result;
        }
    }

    public class Owner
    {
        public string ImgName { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string PageName { get; set; }
    }
}