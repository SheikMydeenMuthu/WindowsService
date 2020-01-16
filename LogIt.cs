using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PushNotificationService
{
    public static class LogIt
    {
        public static void WriteErrorLog(Exception ex)
        {         
            try
            {
                string fileName = GetFolderAndFilePath();

                using (StreamWriter sw = (File.Exists(fileName)) ? File.AppendText(fileName) : File.CreateText(fileName))
                {                    
                    sw.WriteLine("\n==================**************START EXCEPTION****************==================");

                    sw.WriteLine("\n" + DateTime.Now.ToString() + ": " + ex.Source.ToString().Trim() + ";" + ex.Message.ToString().Trim());

                    sw.WriteLine("\n* STACK TRACE * \n" + ex);

                    sw.WriteLine("\n==================**************END EXCEPTION****************==================");

                    sw.Flush();
                    sw.Close();
                }
            }
            catch(Exception e)
            {
                WriteErrorLog(e.Message);
            }
        }

        public static void WriteErrorLog(string Message)
        {

            try
            {
                string fileName = GetFolderAndFilePath();

                using (StreamWriter sw = (File.Exists(fileName)) ? File.AppendText(fileName) : File.CreateText(fileName))
                {
                    sw.WriteLine("\n==================**************START EXCEPTION****************==================");

                    sw.WriteLine("\n" + DateTime.Now.ToString() + ": " + Message);


                    sw.WriteLine("\n==================**************END EXCEPTION****************==================");

                    sw.Flush();
                    sw.Close();
                }
            }
            catch (Exception e)
            {
                WriteErrorLog(e);
            }
        }
        internal static string GetFolderAndFilePath()
        {
            DateTime dateTime = DateTime.Now;
            string filepathwithFolder = string.Empty;

            string filePath = AppDomain.CurrentDomain.BaseDirectory;
            if (!Directory.Exists(filePath + "\\Log"))
            {
                filepathwithFolder = System.IO.Path.Combine(filePath, "Log");
                System.IO.Directory.CreateDirectory(filepathwithFolder);
            }
            else
                filepathwithFolder = filePath + "\\Log";


            string fileName = filepathwithFolder + "\\App.LOG." + dateTime.ToString("yyyyMMdd") + ".txt";

            return fileName;
        }
    }
}
