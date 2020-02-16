using System;
using System.IO;
using System.Reflection;

namespace BOSU
{
    public class LogWriter
    {
        private string logPath = string.Empty;

        public LogWriter()
        {
        }
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }
        public void LogWrite(string logMessage)
        {
            logPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string filePath = logPath + "\\" + "log.txt";
            try
            {
                using (StreamWriter streamWriter = File.AppendText(filePath))
                {
                    Log(logMessage, streamWriter);
                }
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }

        public void Log(string logMessage, TextWriter textWriter)
        {
            try
            {
                textWriter.Write(Environment.NewLine + "Log Entry : ");
                textWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                textWriter.WriteLine("  :");
                textWriter.WriteLine("  :{0}", logMessage);
                textWriter.WriteLine("-------------------------------");
            }
            catch (Exception exception)
            {
                string message = exception.Message;
            }
        }
    }
}
