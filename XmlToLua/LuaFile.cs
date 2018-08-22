using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace XmlToLua
{
    class LuaFile
    {
        private const string META_TABLE = "local defaultTable={__metatable=false, __newindex=function()end}";
        private const string END_OF_FILE = "setmetatable(dataTable, defaultTable)\r\n";
	    private const string REAL_END_FILE = "return dataTable";
	    private const string BEHAVIOR_NAMESPACE = "Game.Behavior.Exported.";


		private string fileName;
	    private string single_file_name;
		private string real_file_name;
		private ElementData rootElement;
        StringBuilder builder;

        public void FileGeneration(string filePath, ElementData rootElement)
        {
            if (rootElement == null)
            {
                return;
            }
	        real_file_name = Path.GetFileNameWithoutExtension(filePath);
			single_file_name = real_file_name.ToLower();
			fileName = single_file_name + ".lua";

			this.rootElement = rootElement;

            builder = new StringBuilder();
            Build();
            WriteFile();
        }


        #region build element string
        /// <summary>
        /// 从root开始处理
        /// </summary>
        private void Build()
        {
            builder.Append("local dataTable ={");
            BuildAttribute(rootElement.attributeList);
            BuildChild(rootElement.childDic);
            builder.AppendLine("}").AppendLine(META_TABLE).AppendLine(END_OF_FILE);
	        builder.Append(BEHAVIOR_NAMESPACE).Append(real_file_name).Append(" = dataTable;\r\n");
	        builder.AppendLine(REAL_END_FILE);
        }

        /// <summary>
        /// 处理子节点
        /// </summary>
        /// <param name="elementDic"></param>
        private void BuildChild(Dictionary<string, List<ElementData>> elementDic)
        {
            int childCount = 1;
            foreach (KeyValuePair<string, List<ElementData>> item in elementDic)
            {
                bool needToArray = NeedToArray(item.Key, item.Value);
                if (needToArray)
                {
                    string key = item.Key.StartsWith("TA_", StringComparison.Ordinal) ? item.Key.Substring(3) : item.Key;
                    builder.Append($"[\"{key}\"]=").Append("{");
                }
                else
                {
                    builder.Append($"[\"{item.Key}\"]=").Append("{");
                }

                int count = 1;
                foreach (ElementData child in item.Value)
                {
                    if (needToArray)
                    {
                        builder.Append($"[{count}]=").Append("{");
                    }

                    BuildAttribute(child.attributeList);
                    if (child.childDic.Count > 0)
                    {
                        BuildChild(child.childDic);
                    }

                    if (needToArray)
                    {
                        builder.Append("}");
                        if (count < item.Value.Count)
                        {
                            builder.Append(",\r\n");
                        }
                        count++;
                    }
                }

                builder.Append("}");

                if (childCount < elementDic.Count)
                {
                    string str = builder.ToString();
                    builder.Append(",");
                    str = builder.ToString();
                }

                childCount++;
            }
        }


        /// <summary>
        /// 属性构建
        /// </summary>
        /// <param name="attributes"></param>
        private void BuildAttribute(List<AttributeData> attributes, bool withKey = true)
        {
            int count = 1;
            foreach (AttributeData item in attributes)
            {
                builder.Append($"[\"{item.name}\"]=");
                switch (item.dataType)
                {
                    case AttributeType.NUMBER:
                        builder.Append(item.value);
                        break;
                    case AttributeType.STRING:
                        builder.Append($"\"{item.value}\"");
                        break;
                    case AttributeType.NIL:
                        builder.Append("nil");
                        break;
                    default:
                        break;
                }
                if (count < attributes.Count)
                {
                    builder.Append(",\r\n");
                }
            }
        }

        /// <summary>
        /// 是否需要改为数组形式
        /// </summary>
        /// <param name="name"></param>
        /// <param name="datas"></param>
        /// <returns></returns>
        private bool NeedToArray(string name, List<ElementData> datas) => name.StartsWith("TA_", StringComparison.Ordinal) || datas.Count > 1;
        #endregion

        #region Write File
        /// <summary>
        /// 文件写入
        /// </summary>
        private void WriteFile()
        {
			var utf8WithoutBom = new UTF8Encoding(false,true);//Encoding.UTF8
/*			if (!System.IO.File.Exists(fileName))
			{
				using (System.IO.FileStream fs = System.IO.File.Create(fileName))
				{
				}
			}*/
			//using (StreamWriter writer = new StreamWriter(File.Open(fileName, FileMode.Create, FileAccess.Write, FileShare.None), Encoding.UTF8))
			using (StreamWriter writer = new StreamWriter(fileName,false, utf8WithoutBom))
			{
				writer.Write(builder.ToString());
				writer.Flush();
				writer.Close();
			}
        }

        #endregion
    }
}
