using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace SampleWinFormClient
{
    class Program
    {
        /// <summary>
        /// The MainForm entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.Run(new MainForm());
        }
    }
}
