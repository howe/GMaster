using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace GMaster.Util
{
    public class ProcessUtil
    {
        [DllImport("kernel32.dll")]
        public static extern int WinExec(string exeName, int operType);  

        public static int isProcessRunning(string process)
        {
            Process[] proc = Process.GetProcessesByName(process);
            return proc.Length;
        }

        public static int startProcess(string process, int operType)
        {
            return WinExec(process, operType);
        }

        public static int killProcess(string process)
        {
            Process[] p = Process.GetProcessesByName(process);
            p[0].Kill();
            return isProcessRunning(process);
        }
    }
}
