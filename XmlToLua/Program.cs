using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;

namespace XmlToLua
{
    class Program
    {
        static void Main(string[] args)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            Settings.ReadRuntimeSetting(args);
            FileInfo[] files = GetAllFile();
            if (Settings.useMultiThread)
            {
                MultiThreadSupport(files);
            }
            else
            {
                SingleThreadSupport(files);
            }
            watch.Stop();

            Console.WriteLine($"\r\n\r\n{watch.Elapsed.TotalSeconds}s");
            Console.WriteLine("Done");
            Console.WriteLine("Press any key to exit");
            Console.Read();
        }

        #region Thread
        static void MultiThreadSupport(FileInfo[] files)
        {
            List<Task> taskList = new List<Task>();
            foreach (FileInfo item in files)
            {
                Task newTask = Task.Run(() =>
                {
                    MyXmlReader reader = new MyXmlReader();
                    LuaFile lua = new LuaFile();
                    ElementData root = reader.ReadXmlFile(item.FullName);
                    lua.FileGeneration(item.FullName, root);
                });
                taskList.Add(newTask);
            }
            if (taskList.Count > 0)
            {
                Task.WaitAll(taskList.ToArray());
            }
        }

        static void SingleThreadSupport(FileInfo[] files)
        {
            MyXmlReader reader = new MyXmlReader();
            MyXmlReader.modifyVector3 = (str) => {
                return "Vector3.new" + str;
            };
            LuaFile lua = new LuaFile();
            foreach (FileInfo item in files)
            {
                ElementData root = reader.ReadXmlFile(item.FullName);
                lua.FileGeneration(item.FullName, root);
            }
        }

        static FileInfo[] GetAllFile()
        {
            DirectoryInfo directory = new DirectoryInfo(Settings.xmlReadPath);
            if (directory.Exists)
            {
                return directory.GetFiles("*.xml", Settings.includeChildDir ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
            }
            else
            {
                Console.WriteLine("Error: directory not exists");
                return null;
            }
        }
        #endregion
    }
}
