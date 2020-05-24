using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace FunctionUtil
{
    public class XmlHelper
    {

        #region 公共属性

        /// <summary>
        /// 配置文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 根节点名称
        /// </summary>
        public string RootName { get; set; }
        /// <summary>
        /// 文档对象
        /// </summary>
        public XDocument Document
        {
            get { return _Document; }
            private set { _Document = value; }
        }
        private XDocument _Document;

        #endregion

        #region 构造函数

        public XmlHelper(string fileName, string rootName)
        {
            this.FileName = fileName;
            this.RootName = rootName;

            if(!System.IO.File.Exists(FileName))
            {
                CreateXmlFile(FileName, RootName);
                IsCreateJustNow = true;
            }
            else
            {
                Document = XDocument.Load(FileName);                
            }

        }

        /// <summary>
        /// 配置文件是否为新创建
        /// </summary>
        public bool IsCreateJustNow { get; set; }

        #endregion

        #region 公共方法        

        /// <summary>
        /// 创建配置文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="rootName"></param>
        public void CreateXmlFile(string fileName, string rootName)
        {
            Document = new XDocument();
            if (!System.IO.File.Exists(fileName))
            {
                XElement root = new XElement(RootName);
                Document.Add(root);                
                Document.Save(fileName);
            }
        }

        /// <summary>
        /// 初始化分组，主要检查分组是否存在，不存在则创建
        /// </summary>
        /// <param name="groupName"></param>
        public XElement AddGroup(string groupName)
        {
            XElement element = Document.Root.Element(groupName);
            if(element == null)
            {
                Document.Root.SetElementValue(groupName, "");
                element = GetElementByName(groupName);
                
            }

            return element;
        }


        #region 获取元素

        /// <summary>
        /// 获取元素集合
        /// </summary>
        /// <param name="elementName"></param>
        /// <returns></returns>
        public IEnumerable<XElement> GetElementsByName(string elementName)
        {
            return Document.Descendants(elementName);
        }

        /// <summary>
        /// 获取元素
        /// </summary>
        /// <param name="elementName">元素名称</param>
        /// <returns></returns>
        public XElement GetElementByName(string elementName)
        {
            IEnumerable<XElement> xElements = Document.Descendants(elementName);

            return xElements.ToList().FirstOrDefault();
        }

        /// <summary>
        /// 获取元素
        /// </summary>
        /// <param name="elementName">元素名称</param>
        /// <param name="xKeyAttribute">属性</param>
        /// <returns></returns>
        public XElement GetElementByName(string elementName,XAttribute xKeyAttribute)
        {
            if (xKeyAttribute == null) return null;

            IEnumerable<XElement> xElements = Document.Descendants(elementName);

            return xElements.FirstOrDefault(x => x.Attribute(xKeyAttribute.Name).Value == xKeyAttribute.Value);
        }

        /// <summary>
        /// 获取元素集合
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public IEnumerable<XElement> GetElementsByGroupName(string groupName)
        {
            IEnumerable<XElement> result = new List<XElement>();
            foreach (var groupElement in Document.Root.Elements())
            {
                if (groupElement.Name == groupName)
                {
                    result = groupElement.Elements();
                }
            }

            return result;
        }

        /// <summary>
        /// 获取元素集合
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public IEnumerable<XElement> GetElementsByParentName(string parentName)
        {
            IEnumerable<XElement> result = new List<XElement>();
            XElement parent = GetElementByName(parentName);
            if(parent != null)
            {
                result = parent.Elements();
            }

            return result;
        }

        #endregion

        #region 创建元素

        /// <summary>
        /// 在根节点下创建一个节点
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public XElement AddElementOfRoot(string elementName, string value = "")
        {
            if (!string.IsNullOrEmpty(elementName))
            {
                XElement element = new XElement(elementName, value ?? "");
                Document.Root.Add(element);
                return element;
            }

            return null;

        }

        /// <summary>
        /// 创建一个节点
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="elementName">子节点名称</param>
        /// <returns></returns>
        public XElement AddElement(XElement parent, string elementName, string value = "")
        {
            if (parent != null && !string.IsNullOrEmpty(elementName))
            {
                XElement xElement = new XElement(elementName, value ?? "");
                parent.Add(xElement);

                return xElement;
            }

            return null;
        }

        /// <summary>
        /// 创建一个节点
        /// </summary>
        /// <param name="parent">父节点</param>
        /// <param name="elementName">子节点名称</param>
        /// <returns></returns>
        public void AddElement(XElement parent, XElement subElement)
        {
            if (parent != null && subElement != null)
            {
                parent.Add(subElement);
            }
        }

        public void AddAttribute(XElement xElement, string attributeName,string value)
        {
            if(xElement != null && !string.IsNullOrEmpty(attributeName))
            {
                if(xElement.Attribute(attributeName) == null)
                {
                    XAttribute xAttribute = new XAttribute(attributeName, value);
                    xElement.Add(xAttribute);
                }
                else
                {
                    xElement.SetAttributeValue(attributeName, value);

                }
            }
        }

        #endregion

        #region 删除元素

        /// <summary>
        /// 移除一个元素
        /// </summary>
        /// <param name="elementName"></param>
        public void RemoveElement(string elementName)
        {
            XElement xElement = GetElementByName(elementName);
            if(xElement != null)
            {
                xElement.Remove();                
            }

        }

        /// <summary>
        /// 移除一个元素
        /// </summary>
        /// <param name="elementName"></param>
        public void RemoveElement(string elementName,XAttribute xKeyAttribute)
        {
            XElement xElement = GetElementByName(elementName, xKeyAttribute);
            if (xElement != null)
            {
                xElement.Remove();                
            }
        }

        /// <summary>
        /// 移除一个元素
        /// </summary>
        /// <param name="elementName"></param>
        public void RemoveElement(XElement xElement)
        {
            if(xElement != null)
            {
                var resultCollection = GetElementsByName(xElement.Name.ToString());
                if(resultCollection.Any())
                {
                    foreach (var result in resultCollection)
                    {
                        bool exists = true;
                        if (xElement.Attributes().Count() == result.Attributes().Count())
                        {
                            foreach (var target in xElement.Attributes())
                            {
                                if(result.Attribute(target.Name.ToString()).Value != target.Value)
                                {
                                    exists = false;
                                    return;
                                }
                            }
                        }

                        if(exists)
                        {
                            xElement.Remove();
                            return;
                        }
                    }
                }
            }
            
        }

        #endregion


        /// <summary>
        /// 保存配置
        /// </summary>
        public void Save()
        {
            Document.Save(FileName);
        }

        /// <summary>
        /// 另存为
        /// </summary>
        /// <param name="fileName"></param>
        public void SaveAs(string fileName)
        {
            Document.Save(fileName);
        }

        #endregion

        #region 扩展方法

        /// <summary>
        /// 获取配置组的配置值列表
        /// </summary>
        /// <param name="groupName">根目录下的分组名称</param>
        /// <param name="configItemName">配置项名称：默认为config,空字符串时取分组下所有配置项</param>
        /// <returns></returns>
        public List<string> GetGroupConfigValues(string groupName, string configItemName = "config")
        {
            List<string> config = null;
            XElement xElement = this.Document.Root.Element(groupName);
            if (xElement != null)
            {
                config = new List<string>();
                IEnumerable<XElement> xElements = null;
                if (string.IsNullOrEmpty(configItemName))
                {
                    xElements = xElement.Elements();
                }
                else
                {
                    xElements = xElement.Elements(configItemName);
                }

                foreach (var element in xElements)
                {
                    config.Add(element.Value);
                }
            }

            return config;
        }

        /// <summary>
        /// 获取元素值
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string GetElementValue(string elementName,string defaultValue="")
        {
            var element = GetElementByName(elementName);
            if (element != null)
            {
                return element.Value;
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 设置元素值
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="value"></param>
        public void SetElementValue(string elementName,string value)
        {
            var element = GetElementByName(elementName);
            if(element!=null)
            {
                element.Value = value;
            }
        }

        #endregion

    }
}
