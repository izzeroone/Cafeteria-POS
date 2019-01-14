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
    public class StockInPrinter : IPrintHelper
    {
        private static readonly string startupProjectPath =
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        private readonly int SETORDER = 1;

        public StockInPrinter()
        {
        }

        public StockInPrinter(Owner owner)
        {
            Owner = owner;
        }

        public Owner Owner { get; set; }

        public StockInForPrint StockIn { get; set; }


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
            Generate_HeadText(blkHeadText, "Phiếu nhập kho");

            // Info Text
            var blkInfoText = new BlockUIContainer();
            Generate_InfoText(blkInfoText, StockIn.getMetaReceiptInfo());

            // Table Text
            var blkTableText = new BlockUIContainer
            {
                Margin = new Thickness(0, 10, 0, 0)
            };
            Generate_TableText(blkTableText, StockIn.getMetaReceiptTable(), StockIn.StockInDetails);

            // Summary Text
            var blkSummaryText = new BlockUIContainer
            {
                Margin = new Thickness(0, 10, 0, 0)
            };
            Generate_SummaryText(blkSummaryText, StockIn, "vnd");

            sec.Blocks.Add(blkHeadText);
            sec.Blocks.Add(blkInfoText);
            sec.Blocks.Add(blkTableText);
            sec.Blocks.Add(blkSummaryText);

            // Add Section to FlowDocument
            doc.Blocks.Add(sec);


            return doc;
        }

        private void Generate_HeadText(BlockUIContainer blkHeadText, string pageName)
        {
            var stpHeadText = new StackPanel
            {
                Orientation = Orientation.Vertical
            };
            var stpPageName = new StackPanel();
            var txtPageName = new TextBlock
            {
                Text = pageName,
                FontSize = 13,
                FontFamily = new FontFamily("Century Gothic"),
                FontWeight = FontWeights.UltraBold,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 0, 0, 10)
            };
            stpPageName.Children.Add(txtPageName);
            stpHeadText.Children.Add(stpPageName);
            blkHeadText.Child = stpHeadText;
        }


        /// <summary>
        ///     Create the summary section of Receipt
        /// </summary>
        /// <param name="blkTableText"></param>
        /// <param name="order">the order data for filling the receip</param>
        /// <param name="moneyUnit"></param>
        private void Generate_SummaryText(BlockUIContainer blkSummaryText, StockInForPrint order, string moneyUnit)
        {
            var stpSummary = new StackPanel
            {
                Orientation = Orientation.Vertical
            };


     



            // Total Price
            var stpTotalPrice = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            var tbTotalPriceLable = new TextBlock
            {
                Text = "Thành tiền " + "(" + moneyUnit + "):",
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

           
            stpSummary.Children.Add(stpTotalPrice);


            blkSummaryText.Child = stpSummary;
        }

        /// <summary>
        ///     Create the table section of Receipt
        /// </summary>
        /// <param name="blkTableText"></param>
        /// <param name="gridMeta">The meta header of the table</param>
        /// <param name="listData">The data source for table</param>
        private void Generate_TableText(BlockUIContainer blkTableText, string[] gridMeta,
            List<StockInDetailForPrint> listData)
        {
            var dgDataTable = new Grid();
            dgDataTable.Width = 300;
            // set Columns
            for (var i = 0; i < 5; i++)
            {
                if (i == 0)
                {
                    var firstCol = new ColumnDefinition();
                    firstCol.Width = new GridLength(90);
                    dgDataTable.ColumnDefinitions.Add(firstCol);
                    continue;
                }

                if (i == 1)
                {
                    var secondCol = new ColumnDefinition();
                    secondCol.Width = new GridLength(60);
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


            foreach (var stockInDetailForPrint in listData)
            {
                var txtProductName = new TextBlock();
                txtProductName.Width = 115;
                txtProductName.Text = seperateLongProductName(stockInDetailForPrint.Name);
                txtProductName.FontSize = 11;
                txtProductName.VerticalAlignment = VerticalAlignment.Top;
                txtProductName.HorizontalAlignment = HorizontalAlignment.Left;
                txtProductName.Margin = new Thickness(0, 0, 0, 5);
                Grid.SetRow(txtProductName, rowIndex);
                Grid.SetColumn(txtProductName, 0);
                dgDataTable.Children.Add(txtProductName);

                var txtQuan = new TextBlock();
                txtQuan.Text = stockInDetailForPrint.Quan.ToString();
                txtQuan.FontSize = 11;
                txtQuan.VerticalAlignment = VerticalAlignment.Top;
                txtQuan.Margin = new Thickness(0, 0, 0, 5);
                Grid.SetRow(txtQuan, rowIndex);
                Grid.SetColumn(txtQuan, 1);
                dgDataTable.Children.Add(txtQuan);

                var txtPrice = new TextBlock();
                txtPrice.Text = stockInDetailForPrint.Price.ToString();
                txtPrice.FontSize = 11;
                txtPrice.VerticalAlignment = VerticalAlignment.Stretch;
                txtPrice.HorizontalAlignment = HorizontalAlignment.Right;
                txtPrice.Margin = new Thickness(0, 0, 0, 5);
                Grid.SetRow(txtPrice, rowIndex);
                Grid.SetColumn(txtPrice, 2);
                dgDataTable.Children.Add(txtPrice);

                var txtAmt = new TextBlock();
                txtAmt.Text = string.Format("{0:0.000}", stockInDetailForPrint.TotalPrice);
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

}