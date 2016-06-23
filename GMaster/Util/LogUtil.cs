using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Xml;
using System.Diagnostics;
using System.Reflection;

namespace GMaster.Util
{
    public class LogUtil
    {
        private static string LOG_FILE = ConfigUtil.getCurProcessDir() + @"gmaster.log";
        private static StreamWriter writer = null;
        private static Queue queue = Queue.Synchronized(new Queue());

        public static void initialLogUtil()
        {
            if (writer == null)
            {
                Process pro = Process.GetCurrentProcess();
                pro.EnableRaisingEvents = true;
                pro.Exited += new EventHandler(LogUtil.dispose);

                writer = File.AppendText(LOG_FILE);
                new Thread(dequeue).Start();
            }
        }

        public static void dispose(object sender, EventArgs e)
        {
            Thread.Sleep(1500);
            if (writer != null)
            {
                writer.Close();
                writer = null;
            }
        }

        public static void log(string log)
        {
            if (writer == null)
                return;
            enqueue(new StackTrace(), log);
        }

        public static void log(string log, Exception e)
        {
            if (writer == null)
                return;
            enqueue(new StackTrace(), log);
            enqueue(new StackTrace(), e.Message);
        }

        private static void enqueue(StackTrace trace, string log)
        {
            MethodBase methodBase = trace.GetFrame(1).GetMethod();
            string className = methodBase.ReflectedType.Name;
            string methodName = methodBase.Name;

            int id = Thread.CurrentThread.ManagedThreadId;


            string time = DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss]");
            string head = "[" + className + "." + methodName + " " + id + "]";

            queue.Enqueue(time + head + " " + log);
        }

        private static void dequeue()
        {
            while (writer != null)
            {
                try
                {
                    if (queue.Count == 0)
                        Thread.Sleep(1000);
                    else
                    {
                        writer.WriteLine((string) queue.Dequeue());
                        writer.Flush();
                    }
                }
                catch (Exception e)
                {
                    // ignore
                }
            }
        }

    }
}
