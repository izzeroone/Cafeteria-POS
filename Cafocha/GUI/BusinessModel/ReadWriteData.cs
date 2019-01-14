//using System;
//using System.IO;
//using System.Text;
//using System.Windows;
//
//namespace Cafocha.GUI.BusinessModel
//{
//    public class ReadWriteData
//    {
//        //string a = System.IO.Directory.GetCurrentDirectory();
//        private static readonly string startupProjectPath =
//            Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
//
//        private static string ENCRYPT_PHASE = "OMA_ZIO_12312";
//
//        public static string ReadDBConfig()
//        {
//            try
//            {
//                var fs = new FileStream(startupProjectPath + "\\dbconfig.txt", FileMode.Open);
//                using (var rd = new StreamReader(fs, Encoding.UTF8))
//                {
//                    var dbConfig = rd.ReadLine();
//                    var result = dbConfig?.Split(',');
//
//                    return dbConfig;
//                }
//            }
//            catch (Exception e)
//            {
//                return null;
//            }
//        }
//
//        //ToDo: Need to encrypt config before save to file
//        public static void WriteDBConfig(string dbconfig)
//        {
//            try
//            {
//                var fs = new FileStream(startupProjectPath + "\\dbconfig.txt", FileMode.Create);
//                {
//                    using (var sWriter = new StreamWriter(fs, Encoding.UTF8))
//                    {
//                        sWriter.WriteLine(dbconfig);
//                    }
//                }
//            }
//            catch (Exception e)
//            {
//                return;
//            }
//        }
//    }
//}