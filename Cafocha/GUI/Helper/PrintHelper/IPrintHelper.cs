using System.Windows.Documents;

namespace Cafocha.GUI.Helper.PrintHelper
{
    public interface IPrintHelper
    {
        FlowDocument CreateDocument();
    }
}