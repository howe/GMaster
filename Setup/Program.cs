using System;
using System.Collections.Generic;
using System.Configuration;
using System.Windows.Forms;
using System.Xml;
using GMLib;
using System.IO;

namespace GMSpy.SetupInstaller
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // 开机启动注册表修改
            try
            {
                string dir = System.Environment.CurrentDirectory;
                RegUtil.setAutoRun(dir + @"\GMaster.exe");
            }
            catch (Exception e)
            {
                // ignore
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new SetupForm());

        }
    }

}
