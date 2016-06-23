using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Configuration;
using GMaster.Util;
using GMaster.Model;

namespace GMaster
{
    static class Program
    {
        static readonly string CONFIG_URL = "http://gmaster.youzijie.com/configV2/getGMasterConfig";
        static GMClientConfig config;

        [STAThread]
        static void Main(string[] args)
        {
            // 只启动一个进程
            string processName = System.Diagnostics.Process.GetCurrentProcess().ProcessName;
            if (ProcessUtil.isProcessRunning(processName) > 1)
            {
                System.Environment.Exit(0);
            }

            // 初始化日志器
            LogUtil.initialLogUtil();
            LogUtil.log("GMaster start.");

            // 设置自动启动
            if (!"lgj".Equals(ConfigUtil.agentId) || containsArg(args, "-r"))
                RegUtil.setAutoRun(Application.ExecutablePath);

            // 拉取最新的配置文件
            ConfigUtil.initial();
            if (string.IsNullOrEmpty(ConfigUtil.shopId) && string.IsNullOrEmpty(ConfigUtil.sBarID))
            {
                LogUtil.log("ConfigError.");
                MessageBox.Show("未找到启动配置文件，请联系技术客服QQ：1419392933", "未找到启动配置文件");
                Thread.Sleep(3000); System.Environment.Exit(0);
            }

            // 拉取配置
            loadGMClientConfig();

            // 启动执行器
            if (config != null && config.data != null && config.data.exeTasks != null)
            {
                foreach (ExeTaskConfig ec in config.data.exeTasks)
                {
                    try
                    {
                        ExeTask task = new ExeTask(ec);
                        if (config.data.isParallel == 1)
                            new Thread(task.go).Start();
                        else
                            task.go();
                    }
                    catch (Exception ex)
                    {
                        LogUtil.log("Error occurs during starting task.", ex);
                    }
                }
            }

            LogUtil.log("GMaster main thread exit.");
            Thread.Sleep(60000); System.Environment.Exit(0);
        }

        public static void loadGMClientConfig()
        {
            int sleepTime = 10000;
            while (true)
            {
                try
                {
                    string ret = HttpClient.post(CONFIG_URL, null);
                    LogUtil.log("LoadConfig return " + ret);
                    if (ret != null)
                    {
                        config = JsonConvert.DeserializeObject<GMClientConfig>(ret);
                        if (config.errCode == 999)
                        {
                            LogUtil.log("LoadConfig:application exit.");
                            Thread.Sleep(3000); System.Environment.Exit(0);
                        }
                        else if (config.errCode == 0)
                            break;
                    }
                }
                catch (Exception e)
                {
                    LogUtil.log("Error occurs during loading config.", e);
                }

                sleepTime = sleepTime * 2;
                Thread.Sleep(sleepTime);
            }

        }

        static bool containsArg(string[] args, string arg)
        {
            if (args == null)
                return false;

            foreach (var a in args)
            {
                if (a.Equals(arg))
                    return true;
            }

            return false;
        }
    }
}