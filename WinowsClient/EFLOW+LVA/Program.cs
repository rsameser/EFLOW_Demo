using System;
using System.Threading;
using System.Windows.Forms;

namespace EFLOW_LVA
{
    static class Program
    {
        public static Sample sample;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            sample = new Sample();
            Application.Run(sample);
        }


    }
}
