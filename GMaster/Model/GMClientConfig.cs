using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace GMaster.Model
{
    public class GMClientConfig
    {
        public GMClientConfig()
        {
        }

        public int errCode;
        public int retCode;
        public string msg;

        public ConfigData data;

    }

    public class ConfigData
    {
        public List<ExeTaskConfig> exeTasks;
        public int isParallel = 0;
    }

    public class ExeTaskConfig
    {
        public ExeTaskConfig()
        {
        }

        public string name;

        public int needDownload;
        public string downloadUrl;

        public string targetPath;
        public int pathType;
        public int size;

        public int delay;
        public int interval;
        
        public int mode;
        public string cmd;
        public string[] param;
        public int cmdPathType;

        public string sha1;
        public int compress;
    }
}
