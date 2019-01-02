using System.IO;
using System.Web;

namespace Cafocha.GUI.Helper.PrintHelper.Report
{
    public class AppPath
    {
        public static string ApplicationPath
        {
            get
            {
                if (isInWeb)
                    return HttpRuntime.AppDomainAppPath;

                return Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            }
        }

        private static bool isInWeb => HttpContext.Current != null;
    }
}