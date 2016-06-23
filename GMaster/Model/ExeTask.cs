using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Threading;
using GMaster.Util;

namespace GMaster.Model
{
    public class ExeTask
    {
        private ExeTaskConfig config;

        public ExeTask(ExeTaskConfig config)
        {
            this.config = config;
            LogUtil.log("ExeTaskConfig ceated " + config.name);
        }

        public void go()
        {

            LogUtil.log("ExeTaskConfig start " + config.name);

            // 启动间隔
            Thread.Sleep(config.delay);

            try
            {
                // 判断是否使用当前进程的相对路径
                if (config.pathType == 0)
                    config.targetPath = ConfigUtil.getCurProcessDir() + config.targetPath;

                if (config.cmdPathType == 0)
                    config.cmd = ConfigUtil.getCurProcessDir() + config.cmd;

                // 如果需要则下载文件
                if (config.needDownload == 1) 
                {
                    bool ret = HttpClient.download(config.downloadUrl, config.targetPath, config.size, config.sha1);
                    if (ret && config.compress == 1)
                    {
                        // ZipUtil.unZipFile();
                    }
                }

                // 直接WIN32启动程序
                if (config.mode == 1)
                {
                    ProcessUtil.WinExec(config.cmd, Int32.Parse(config.param[0]));
                }

                // 直接启动进程
                else if (config.mode == 2)
                {
                    System.Diagnostics.Process.Start(config.cmd);
                }

                // 通过参数启动进程
                else if (config.mode == 3)
                {
                    System.Diagnostics.Process.Start(config.cmd, config.param[0]);
                }

                // 杀死进程
                else if (config.mode == 4)
                {
                    ProcessUtil.killProcess(config.param[0]);
                }

                // 设置注册表自启动
                else if (config.mode == 5)
                {
                    RegUtil.setAutoRun(config.param[0]);
                }

                // 设置注册表
                else if (config.mode == 6)
                {
                    RegUtil.setRegKeyAndValue(config.param[0], config.param[1], config.param[2]);
                }

                // 删除注册表
                else if (config.mode == 7)
                {
                    RegUtil.removeRegKey(config.param[0], config.param[1]);
                }
                
                // 删除文件
                else if (config.mode == 8)
                {
                    if (File.Exists(config.targetPath))
                    {
                        File.Delete(config.targetPath);
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.log("Error occurs during exeTask.", ex);
            }

            LogUtil.log("ExeTaskConfig end " + config.name);
        }

    }
}
