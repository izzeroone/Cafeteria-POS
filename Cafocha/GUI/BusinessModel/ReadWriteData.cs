using Cafocha.Repository.DAL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;

namespace Cafocha.GUI.BusinessModel
{
    public class ReadWriteData
    {
        //string a = System.IO.Directory.GetCurrentDirectory();
        static string startupProjectPath = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
        public static string[] ReadPrinterSetting()
        {
            using (FileStream fs = new FileStream(startupProjectPath + "\\SerializedData\\printerSetting.txt", FileMode.Open))
            {
                using (StreamReader rd = new StreamReader(fs, Encoding.UTF8))
                {
                    string printer = rd.ReadLine();
                    string[] result = printer?.Split(',');

                    if (result?.Length >= 4)
                    {
                        return result;
                    }
                }

                MessageBox.Show("There has no previous setting, so the configuration will set to default!");
                return null;
            }
        }

        public static void WritePrinterSetting(string printers)
        {
            using (FileStream fs = new FileStream(startupProjectPath + "\\SerializedData\\printerSetting.txt", FileMode.Create))
            {
                using (StreamWriter sWriter = new StreamWriter(fs, Encoding.UTF8))
                {
                    sWriter.WriteLine(printers);
                }
            }
        }

        public static string[] ReadDBConfig()
        {
            using (FileStream fs = new FileStream(startupProjectPath + "\\SerializedData\\dbconfig.txt", FileMode.Open))
            {
                using (StreamReader rd = new StreamReader(fs, Encoding.UTF8))
                {
                    string dbConfig = rd.ReadLine();
                    string[] result = dbConfig?.Split(',');

                    if (result?.Length >= 5)
                    {
                        return result;
                    }
                }

                
                return null;
            }
        }

        //ToDo: Need to encrypt config before save to file
        public static void WriteDBConfig(string dbconfig)
        {
            using (FileStream fs = new FileStream(startupProjectPath + "\\SerializedData\\dbconfig.txt", FileMode.Create))
            {
                using (StreamWriter sWriter = new StreamWriter(fs, Encoding.UTF8))
                {
                    sWriter.WriteLine(dbconfig);
                }
            }
        }
    }
}
