using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ASoft;

namespace ASoft.Model
{
    public class ConfigModel
    {
        public const String DefaultSeqName = "default";
        public static String ConnectionString = Config.GetAppSettings("ConnectionString");
    }
}
