using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlToLua
{
    class Settings
    {
        public static string xmlReadPath = Environment.CurrentDirectory;
        public static string luaGeneratePath = Environment.CurrentDirectory;
        public static bool includeChildDir = false;
        public static bool useMultiThread = false;

        public static void ReadRuntimeSetting(string[] args)
        {
            foreach (string item in args)
            {
                if (item.IndexOf("=", StringComparison.Ordinal) != -1)
                {
                    string[] setting = item.Split('=');
                    if (setting.Length != 2)
                    {
                        continue;
                    }
                    switch (setting[0])
                    {
                        case "-GenratePath":
                            luaGeneratePath = setting[1];
                            break;
                        case "-ReadPath":
                            xmlReadPath = setting[1];
                            break;
                        case "-ChildDir":
		                    bool needChild;
		                    bool multiThread;

							includeChildDir = Boolean.TryParse(setting[1], out needChild) ? needChild : false;
                            break;
                        case "-MultiThread":
                            useMultiThread = Boolean.TryParse(setting[1], out multiThread) ? multiThread: false;
							break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}
