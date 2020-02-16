using System;
using System.Reflection;

namespace BOSU
{
    class SystemHardwareInfo
    {
        public static String GetCurrentTime()
        {
            return DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
        }

        public static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }
    }
}
