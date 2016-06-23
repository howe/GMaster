using GMMaster.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Net.NetworkInformation;
using System.Text;
using System.Xml;

namespace GMaster.Util
{
    public class ConfigUtil
    {
        // 龙管家模块配置文件
        private static readonly string RWY_MODULE_FILE_32 = @"C:\Windows\System32\Rwymoudle.dat";
        private static readonly string RWY_MODULE_FILE_64 = @"C:\Windows\SysWOW64\Rwymoudle.dat";
        private static readonly string RWY_FEEMODULE_FILE_32 = @"C:\Windows\System32\RwyFeemoudle.dat";
        private static readonly string RWY_FEEMODULE_FILE_64 = @"C:\Windows\SysWOW64\RwyFeemoudle.dat";

        // 配置文件信息
        private static readonly string CONFIG_FILE = "config.properties";

        // 机器信息
        public static readonly string hostName = System.Net.Dns.GetHostName();
        public static readonly string mac = getMacByNetworkInterface();
        public static readonly string cpu = getCPUId();
        public static readonly string osv = getOSVersion();

        // 版本和配置信息
        // cv 单数表示龙管家版本 agentId 为 lgj 双数表示 非龙管家版本  agentId 为 other
        public static readonly string agentId = "lgj";
        public static readonly string cv = "209";

        // 门店信息
        public static string shopId = "";
        public static string token = "";
        public static string sBarID = "";


        public static void initial()
        {
            // 读取竞技大师配置
            String configfile = getCurProcessDir() + CONFIG_FILE;
            LogUtil.log("ConfigUtil.initial read configFile:" + configfile);
            if (File.Exists(configfile))
            {
                string[] lines = File.ReadAllLines(configfile);
                foreach (string line in lines)
                {
                    LogUtil.log("ConfigUtil.initial readline:" + line);
                    if (line.StartsWith("shopId=", true, null))
                        shopId = line.Substring(7);
                    if (line.StartsWith("token=", true, null))
                        token = line.Substring(6);
                }
            }

            // 读取龙管家sBarID
            if (string.IsNullOrEmpty(sBarID) && File.Exists(RWY_MODULE_FILE_64))
                sBarID = readRWYConfig(RWY_MODULE_FILE_64);
            if (string.IsNullOrEmpty(sBarID) && File.Exists(RWY_MODULE_FILE_32))
                sBarID = readRWYConfig(RWY_MODULE_FILE_32);
            if (string.IsNullOrEmpty(sBarID) && File.Exists(RWY_FEEMODULE_FILE_64))
                sBarID = readRWYConfig(RWY_FEEMODULE_FILE_64);
            if (string.IsNullOrEmpty(sBarID) && File.Exists(RWY_FEEMODULE_FILE_32))
                sBarID = readRWYConfig(RWY_FEEMODULE_FILE_32);

            // 生成第三方DLL
            prepareDll("Newtonsoft.Json.dll", Resources.Newtonsoft_Json);
            prepareDll("ThirdGame.dll", Resources.ThirdGame);
        }

        public static string readRWYConfig(string file)
        {
            foreach (string line in File.ReadAllLines(file))
            {
                if (line.StartsWith("sBarID=", true, null))
                    return line.Substring(7);
            }
            return "";
        }

        public static string getCurProcessDir()
        {
            string path = System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName;
            int index = path.LastIndexOf("\\");
            path = path.Substring(0, index);
            return path + "\\";
        }

        private static string getMacByNetworkInterface()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface ni in interfaces)
            {
                string mac = ni.GetPhysicalAddress().ToString();
                mac.Replace(":", "");
                return mac;
            }
            return "unknownmac";
        }

        private static string getCPUId()
        {
            string cpuInfo = "";
            ManagementClass mc = new ManagementClass("Win32_Processor");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                cpuInfo = mo.Properties["ProcessorId"].Value.ToString();
                return cpuInfo.ToString();
            }

            return "unknowncpu";
        }

        public static string getOSVersion()
        {
            Version ver = System.Environment.OSVersion.Version;
            string strClient = "";
            if (ver.Major == 5 && ver.Minor == 1)
            {
                strClient = "WinXP";
            }
            else if (ver.Major == 6 && ver.Minor == 0)
            {
                strClient = "WinVista";
            }
            else if (ver.Major == 6 && ver.Minor == 1)
            {
                strClient = "Win7";
            }
            else if (ver.Major == 5 && ver.Minor == 0)
            {
                strClient = "Win2000";
            }
            else
            {
                strClient = "unknown";
            }
            return strClient;
        }


        /// <summary>
        /// 生成DLL文件
        /// </summary>
        public static void prepareDll(string name, byte[] bits)
        {
            name = getCurProcessDir() + name;
            if (!System.IO.File.Exists(name))
            {
                // 生成 DLL
                FileStream fs = null;
                try
                {
                    byte[] bit = bits;
                    fs = new FileStream(name, FileMode.OpenOrCreate);
                    fs.Write(bit, 0, bit.Length);
                    fs.Flush();
                    LogUtil.log("Prepared dll " + name + " length " + bit.Length);
                }
                catch (Exception e)
                {
                    LogUtil.log("Error occurs during prepareing dll. " + e.Message);
                }
                finally
                {
                    if (fs != null)
                        fs.Close();
                }
            }

        }

    }
}
