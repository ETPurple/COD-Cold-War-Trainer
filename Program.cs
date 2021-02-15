using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using ___.util;

namespace ___
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //check for debugging applications on start.
            OPSEC.ScanAndKill();

            if (OPSEC.IsDebuggerActive() == 1)
            {
                Environment.FailFast("");
            }

            //Registered Application with Auth.GG
            util.Auth.OnProgramStart.Initialize("N}89WA*azkbP9kA8", "53740", "WWeQcDQXk4pfJXXWJF1sY0QKX5oZQRi6fuy", "1.0");

            //Display Login Form
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Login());
        }
    }
}
