using System.Windows.Documents;

namespace Cafocha.BusinessContext.Helper.PrintHelper
{
    public interface IPrintHelper
    {
        FlowDocument CreateDocument();
    }
}