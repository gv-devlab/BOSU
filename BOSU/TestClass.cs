using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BOSU
{
    class TestClass
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        public void activateCMD()
        {
            AllocConsole();
            Console.WriteLine("activateCMD" + Environment.NewLine);
        }
        public void cmdInteraction()
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            //System.Diagnostics.ProcessWindowStyle.Hidden to run as worker
            startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Normal;
            startInfo.FileName = "cmd.exe";
            startInfo.Arguments = "/C start Notepad.exe";
            process.StartInfo = startInfo;
            process.Start();
        }
    }
}
