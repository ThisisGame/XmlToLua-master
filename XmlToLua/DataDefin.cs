using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XmlToLua
{
    class ElementData
    {
        public string name;
        public List<AttributeData> attributeList;
        public Dictionary<string, List<ElementData>> childDic;

        public void AddChildELement(string elementName,ElementData child)
        {
            if (!childDic.ContainsKey(elementName))
            {
                childDic.Add(elementName, new List<ElementData>());
            }
            childDic[elementName].Add(child);
        }
    }

    class AttributeData
    {
        public string name;
        public string value;
        public AttributeType dataType = AttributeType.NIL;
    }

    enum AttributeType
    {
        NUMBER = 1,
        STRING = 2,
        NIL = 3,
        VECTOR3 = 4,
        QUATERNION = 5
    }
}
