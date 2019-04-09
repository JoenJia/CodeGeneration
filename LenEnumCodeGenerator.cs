using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;

namespace CodeGeneration
{
    public class LenEnumCodeGenerator
    {

        string _schemaFolder;
        string _targetFolder;
        List<string> _enumFields = new List<string>();
        const string TargetFile = "Lendesk_enumCode.xml";
        public LenEnumCodeGenerator(string schemaFolder, string targetFolder)
        {
            _schemaFolder = schemaFolder;
            _targetFolder = targetFolder;
        }
        public void Generate()
        {
            var targetPath = System.IO.Path.Combine(_targetFolder, TargetFile);
            XmlDocument targetDoc = new XmlDocument();
            var root = targetDoc.CreateElement("menuCode");
            targetDoc.AppendChild(root);
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(_schemaFolder);
            foreach(var f in di.GetFiles("*.xsd"))
            {
                GenerateSchemaFile(targetDoc, f.FullName);
            }
            targetDoc.Save(targetPath);
        }
        private void GenerateSchemaFile(XmlDocument targetDoc, string filePath)
        {
            XNamespace ns = "http://www.w3.org/2001/XMLSchema";
            XElement schemaDoc = XElement.Load(filePath);
            var root = targetDoc.ChildNodes[0];
            foreach (var elm in schemaDoc.Descendants(ns + "element").Where(x => x.Element(ns + "simpleType")?
            .Element(ns+ "restriction")?
            .Attribute("base").Value== "len:DataDictionaryType"))
            {
                var elmName = elm.Attribute("name").Value;
                var elmType = elm.Element(ns + "simpleType");
                if (elmType == null)
                {
                    continue;
                }
                var restr = elmType.Element(ns + "restriction");
                if (restr == null)
                {
                    continue;
                }
                if (_enumFields.Contains(elmName))
                {
                    continue;
                }
                _enumFields.Add(elmName);
                var enumNode = targetDoc.CreateElement("all");
                root.AppendChild(enumNode);
                var codeNameNode = targetDoc.CreateElement("menuCodeName");
                codeNameNode.InnerText = elmName.Replace("Choice", "");
                enumNode.AppendChild(codeNameNode);
                var colNameNode = targetDoc.CreateElement("columnName");
                enumNode.AppendChild(colNameNode);
                var nameNode = targetDoc.CreateElement("name");
                nameNode.InnerText = elmName;
                colNameNode.AppendChild(nameNode);
                var enums = restr.Elements(ns + "enumeration");
                foreach(var em in enums)
                {
                    var emVal = em.Attribute("value").Value;
                    var emAnn = em.Element(ns + "annotation");
                    var emText = emAnn.Value;
                    var itemTextNode = targetDoc.CreateElement("text");
                    enumNode.AppendChild(itemTextNode);
                    var itemCodeNode = targetDoc.CreateElement("code");
                    itemCodeNode.InnerText = emVal;
                    itemTextNode.AppendChild(itemCodeNode);
                    var itemEnNode = targetDoc.CreateElement("textEnglish");
                    itemEnNode.InnerText = emText;
                    itemTextNode.AppendChild(itemEnNode);
                }
            }
            //dedicated type definition
            foreach (var elm in schemaDoc.Descendants(ns + "simpleType").Where(x => x.Attribute("name") != null &&  x.Element(ns + "restriction")?.Attribute("base")?.Value == "len:DataDictionaryType"))
            {
                var elmName = elm.Attribute("name").Value;
                var restr = elm.Element(ns + "restriction");
                if (restr == null)
                {
                    continue;
                }
                if (_enumFields.Contains(elmName))
                {
                    continue;
                }
                _enumFields.Add(elmName);
                var enumNode = targetDoc.CreateElement("all");
                root.AppendChild(enumNode);
                var codeNameNode = targetDoc.CreateElement("menuCodeName");
                codeNameNode.InnerText = elmName.Replace("Type", "");
                enumNode.AppendChild(codeNameNode);
                var colNameNode = targetDoc.CreateElement("columnName");
                enumNode.AppendChild(colNameNode);
                var nameNode = targetDoc.CreateElement("name");
                nameNode.InnerText = elmName;
                colNameNode.AppendChild(nameNode);
                var enums = restr.Elements(ns + "enumeration");
                foreach (var em in enums)
                {
                    var emVal = em.Attribute("value").Value;
                    var emAnn = em.Element(ns + "annotation");
                    var emText = emAnn.Value;
                    var itemTextNode = targetDoc.CreateElement("text");
                    enumNode.AppendChild(itemTextNode);
                    var itemCodeNode = targetDoc.CreateElement("code");
                    itemCodeNode.InnerText = emVal;
                    itemTextNode.AppendChild(itemCodeNode);
                    var itemEnNode = targetDoc.CreateElement("textEnglish");
                    itemEnNode.InnerText = emText;
                    itemTextNode.AppendChild(itemEnNode);
                }
            }

        }
    }
}
