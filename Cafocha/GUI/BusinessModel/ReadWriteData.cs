using System.IO;
using System.Text;
using System.Windows;

namespace Cafocha.GUI.BusinessModel
{
    public class ReadWriteData
    {
        //string a = System.IO.Directory.GetCurrentDirectory();
        private static readonly string startupProjectPath =
            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;

        private static string ENCRYPT_PHASE = "OMA_ZIO_12312";

        public static string[] ReadPrinterSetting()
        {
            using (var fs = new FileStream(startupProjectPath + "\\SerializedData\\printerSetting.txt", FileMode.Open))
            {
                using (var rd = new StreamReader(fs, Encoding.UTF8))
                {
                    var printer = rd.ReadLine();
                    var result = printer?.Split(',');

                    if (result?.Length >= 4) return result;
                }

                MessageBox.Show("There has no previous setting, so the configuration will set to default!");
                return null;
            }
        }

        public static void WritePrinterSetting(string printers)
        {
            using (var fs = new FileStream(startupProjectPath + "\\SerializedData\\printerSetting.txt",
                FileMode.Create))
            {
                using (var sWriter = new StreamWriter(fs, Encoding.UTF8))
                {
                    sWriter.WriteLine(printers);
                }
            }
        }

        public static string ReadDBConfig()
        {
            using (var fs = new FileStream(startupProjectPath + "\\SerializedData\\dbconfig.txt", FileMode.Open))
            {
                using (var rd = new StreamReader(fs, Encoding.UTF8))
                {
                    var dbConfig = rd.ReadLine();
                    var result = dbConfig?.Split(',');

                    return dbConfig;
                }


                return null;
            }
        }

        //ToDo: Need to encrypt config before save to file
        public static void WriteDBConfig(string dbconfig)
        {
            using (var fs = new FileStream(startupProjectPath + "\\SerializedData\\dbconfig.txt", FileMode.Create))
            {
                using (var sWriter = new StreamWriter(fs, Encoding.UTF8))
                {
                    sWriter.WriteLine(dbconfig);
                }
            }
        }
    }
}