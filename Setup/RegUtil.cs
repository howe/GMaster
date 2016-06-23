using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text;

namespace GMLib
{
    public class RegUtil
    {
        public static string REG_HKEY_SMWCR = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        public static void setAutoRun(string fileName)
        {
            RegistryKey reg = null;
            try
            {
                if (System.IO.File.Exists(fileName)) {
                    String name = fileName.Substring(fileName.LastIndexOf(@"\") + 1);
                    reg = Registry.LocalMachine.OpenSubKey(REG_HKEY_SMWCR, true);
                    if (reg == null)
                    {
                        reg = Registry.LocalMachine.CreateSubKey(REG_HKEY_SMWCR);
                    }
                    reg.SetValue(name, fileName);
                }
            } 
            catch (Exception ex)
            {
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }
        }

        public static void setRegKeyAndValue(string key, string name, string value)
        {
            RegistryKey reg = null;
            try
            {
                reg = Registry.LocalMachine.OpenSubKey(key, true);
                if (reg == null)
                {
                    reg = Registry.LocalMachine.CreateSubKey(key);
                }
                reg.SetValue(name, value);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (reg != null)
                    reg.Close();
            }
        }

        public static void removeRegKey(string key, string name)
        {
            RegistryKey delKey = null;
            try
            {
                delKey = Registry.LocalMachine.OpenSubKey(key, true);
                delKey.DeleteValue(name);
            }
            catch (Exception ex)
            {
            }
            finally
            {
                if (delKey != null)
                    delKey.Close();
            }
        }
    }
}
