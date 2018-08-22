using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.IO;

namespace XmlToLua
{
    class MyXmlReader
    {
        private ElementData rootElement = null;

        /// <summary>
        /// 读取Xml文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public ElementData ReadXmlFile(string filePath)
        {
            XmlDocument xmlDoc = new XmlDocument();
            try
            {
                Console.WriteLine($"{Path.GetFileName(filePath)} -----------> {Path.GetFileNameWithoutExtension(filePath)}.lua");
                xmlDoc.Load(filePath);
                ReadElement(xmlDoc.ChildNodes);
            }
            catch (XmlException e)
            {
                Console.WriteLine($"File: {Path.GetFileName(filePath)}; ErrorMessage: {e.Message}");
            }
            return rootElement;
        }

        /// <summary>
        /// 读取元素
        /// </summary>
        /// <param name="nodeList"></param>
        /// <param name="parent"></param>
        private void ReadElement(XmlNodeList nodeList, ElementData parent = null)
        {
            foreach (XmlNode item_node in nodeList)
            {
	            if (!(item_node is XmlElement))
	            {
					continue;
	            }
				var item = item_node as XmlElement;

				ElementData element = new ElementData()
                {
                    name = item.Name,
                    attributeList = new List<AttributeData>(),
                    childDic = new Dictionary<string, List<ElementData>>(),
                };
                element.attributeList = ReadElementAttribute(item);
                if (item.HasChildNodes)
                {
                    ReadElement(item.ChildNodes, element);
                }
                if (parent == null)
                {
                    rootElement = element;
                }
                else
                {
                    parent.AddChildELement(item.Name, element);
                }
            }
        }

        /// <summary>
        /// 取得当前元素的所有属性
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private List<AttributeData> ReadElementAttribute(XmlElement element)
        {
            List<AttributeData> attributeList = new List<AttributeData>();
            foreach (XmlAttribute item in element.Attributes)
            {
                AttributeData data = new AttributeData()
                {
                    name = item.Name,
                    value = item.Value,
			};
				//去掉多余的引号
	            data.value = data.value.Replace("\"", "");
				data.dataType = GetAttributeType(data.value);
                attributeList.Add(data);
            }
            return attributeList;
        }

        /// <summary>
        /// 取得属性值的数据类型
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private AttributeType GetAttributeType(string value)
        {
            AttributeType result = AttributeType.NIL;
	        double d;

			if (double.TryParse(value, out d))
            {
                result = AttributeType.NUMBER;
            }
            else if (!string.IsNullOrEmpty(value))
            {
                result = AttributeType.STRING;
            }

            return result;
        }
    }
}
