using System;
using System.Threading;
using System.Windows.Forms;

namespace BOSU
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static readonly Mutex mutex = new Mutex(true, "{852ebeb0-118b-436a-8aae-681df14ba46b}");
        [STAThread]
        static void Main()
        {
            if (mutex.WaitOne(TimeSpan.Zero, true))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
                mutex.ReleaseMutex();
            }
            else
            {
                MessageBox.Show("Already running! =)");
            }
        }
    }
}
