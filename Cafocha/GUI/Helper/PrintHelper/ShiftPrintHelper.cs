using System.Windows;
using System.Windows.Documents;

namespace Cafocha.GUI.Helper.PrintHelper
{
    public class ShiftPrintHelper : IPrintHelper
    {
        public FlowDocument CreateDocument()
        {
            return CreateShiftDocument();
        }

        public FlowDocument CreateShiftDocument()
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


            // Info Text
            var blkInfoText = new BlockUIContainer();
            Generate_InfoText(blkInfoText);

            // Table Text
            var blkTableText = new BlockUIContainer
            {
                Margin = new Thickness(0, 10, 0, 0)
            };
            Generate_TableText(blkTableText);

            // Summary Text
            var blkSummaryText = new BlockUIContainer
            {
                Margin = new Thickness(0, 10, 0, 0)
            };
            Generate_SummaryText(blkSummaryText);


            //// Add Paragraph to Section
            //sec.Blocks.Add(p1);
            sec.Blocks.Add(blkHeadText);
            sec.Blocks.Add(blkInfoText);
            sec.Blocks.Add(blkTableText);
            sec.Blocks.Add(blkSummaryText);

            // Add Section to FlowDocument
            doc.Blocks.Add(sec);


            return doc;
        }

        private void Generate_SummaryText(BlockUIContainer blkSummaryText)
        {
        }

        private void Generate_TableText(BlockUIContainer blkTableText)
        {
        }

        private void Generate_InfoText(BlockUIContainer blkInfoText)
        {
        }

        private void Generate_HeadText(BlockUIContainer blkHeadText)
        {
        }
    }
}