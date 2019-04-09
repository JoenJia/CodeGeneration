using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Schema;
using System.IO;
using System.Xml.Linq;

namespace CodeGeneration
{
    public class WebUIGenerator
    {
        const string ModelNameSpace = "FirstNational.Net.API.Delivery.Web.Models";
        const string CommentsHeader = @"
@*Code genereated from CodeGeneration tool according schema*@";
        const string ModelHeader = "@model {0}.{1}";
        const string PanelHeader = @"
  <div class=""panel panel-default form-panel"">
    <div class=""panel-body"">";
        const string PanelFooter = @"
    </div>
  </div>
";
        const string GroupHeader = @"
        <div class=""form-group"">";
        const string GroupFooter = @"
        </div>";
        const string LabelPattern = @"
            <label for=""{0}"" class=""control-label col-md-2"">{1}</label>";
        const string InputPattern = @"
            <input name=""{0}"" type=""{1}"" class=""form-control"" id=""{0}"" value=""@Model.{0}"" {2}/>";
        const string DatePattern = @"
            <input name=""{0}"" type=""date"" class=""form-control"" id=""{0}"" value='@Model.{0}.ToString(""yyyy-MM-dd"")' {1}/>";
        const string SelectInputPattern = @"
            <select id=""{0}"" name=""{0}"" class=""form-control"">";
        const string OptionNullPattern = @"
                <option value = """" @(Model.{0}Specified == false ? ""selected"" : """")></option>";
        const string OptionPattern = @"
                <option value = ""{0}"" @(Model.{2} == {0} ? ""selected"" : """")>{1}</option>";
        const string SelectInputEnd = @"
            </select>";
        const string CheckBoxPattern= @"
            <input name=""{0}"" type=""checkbox"" id=""{0}"" class=""checkbox"" value=""@Model.{0}"" />";
        private string _schemaFile;
        private string _targetFolder;
        private List<string> _processedObj = new List<string>();
        public WebUIGenerator(string schemaFile, string targetFolder)
        {
            _schemaFile = schemaFile;
            _targetFolder = targetFolder;
            if (!Directory.Exists(_targetFolder))
            {
                Directory.CreateDirectory(_targetFolder);
            }
        }
        public void Generate()
        {
            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.Add(null, _schemaFile);
            schemaSet.Compile();
            //scan from root element
            foreach (XmlSchemaElement element in schemaSet
                .Schemas()
                .Cast<XmlSchema>()
                .SelectMany(s => s.Elements.Values.Cast<XmlSchemaElement>()))
            {
                IterateOverElement(element);
            }
            System.Diagnostics.Process.Start(_targetFolder);
        }

        void IterateOverElement(XmlSchemaElement element)
        {
            var schemaType = element.ElementSchemaType;
            if (schemaType == null)
            {
                schemaType = element.SchemaType;
            }

            IterateOverType(element.Name, schemaType);
        }

        void IterateOverType(string objName, XmlSchemaType schemaType)
        {
            var complexType = schemaType as XmlSchemaComplexType;
            if (complexType == null)
            {
                return;
            }
            var sequence = complexType.ContentTypeParticle as XmlSchemaSequence;
            if (sequence == null)
            {
                return;
            }
            //So far FN schema do not need Attributes for input  
            //if (complexType.AttributeUses.Count > 0)
            //{
            //    var enumerator = complexType.AttributeUses.GetEnumerator();
            //    while (enumerator.MoveNext())
            //    {
            //        var attribute = (XmlSchemaAttribute)enumerator.Value;
            //        Debug.WriteLine(root + "." + attribute.Name + " (attribute)");
            //    }
            //}
            if (!string.IsNullOrEmpty(complexType.Name))
            {
                objName = complexType.Name;
            }
            if (_processedObj.Contains(objName))
            {
                return;
            }
            _processedObj.Add(objName);

            StreamWriter sw = new StreamWriter(Path.Combine(_targetFolder, objName + "Form.cshtml"));
            sw.WriteLine(CommentsHeader);
            sw.WriteLine(string.Format(ModelHeader, ModelNameSpace, objName));
            sw.Write(PanelHeader);
            sw.WriteLine();
            sw.WriteLine(@"            <input name=""EntityID"" type=""hidden"" id=""EntityID"" value=""@Model.EntityID"" />");
            sw.WriteLine(@"            <input name=""ParentID"" type=""hidden"" id=""ParentID"" value=""@Model.ParentID"" />");

            foreach (XmlSchemaElement childElement in sequence.Items)
            {
                // recursion
                var propName = childElement.Name;
                var propTypeIsComplex = false;
                var propType = string.Empty;
                var isRequired = false;
                if (childElement.ElementSchemaType != null)
                {
                    if (childElement.ElementSchemaType is XmlSchemaSimpleType)
                    {
                        propType = childElement.ElementSchemaType.QualifiedName.Name;
                        isRequired = childElement.MinOccurs > 0;
                        if (string.IsNullOrEmpty(propType)
                            && childElement.ElementSchemaType.BaseXmlSchemaType != null)
                        {
                            propType = childElement.ElementSchemaType.BaseXmlSchemaType.QualifiedName.Name;
                        }
                    }
                    else
                    {
                        propTypeIsComplex = true;
                        propType = childElement.ElementSchemaType.QualifiedName.Name;
                    }
                }
                if (propTypeIsComplex)
                {
                    IterateOverElement(childElement);
                }
                else
                {
                    //simple type, render input elements
                    var fldTypeBase = childElement.SchemaType?.BaseXmlSchemaType?.QualifiedName;
                    string comments = string.Empty;
                    if (childElement.Annotation != null)
                    {
                        XmlSchemaDocumentation doc = childElement?.Annotation?.Items[0] as XmlSchemaDocumentation;
                        if (doc != null && doc.Markup.Length > 0)
                        {
                            comments = doc.Markup[0].Value;
                        }
                    }
                    var propLabel = FrieldlyName(propName);
                    var required = string.Empty;
                    if (isRequired)
                    {
                        propLabel = propLabel + "*";
                        required = "required=\"required\"";
                    }
                    if (propName != "ID")
                    {
                        sw.Write(GroupHeader);
                        sw.Write(string.Format(LabelPattern, propName, propLabel));
                    }
                    switch (propType)
                    {
                        case "dateTime":
                        case "date":
                            sw.Write(string.Format(DatePattern, propName, required));
                            break;
                        case "boolean":
                            sw.Write(string.Format(CheckBoxPattern, propName));
                            break;
                        case "XSEnumCodeType":
                            sw.Write(string.Format(SelectInputPattern, propName));
                            var comLines = comments.Split('\n');
                            if (!isRequired)
                            {
                                sw.Write(string.Format(OptionNullPattern, propName));
                            }
                            foreach (var line in comLines)
                            {
                                var trimLine = line.Trim();
                                if (trimLine.Length == 0) continue;
                                char firstChar = trimLine.ToArray()[0];
                                if (!Char.IsNumber(firstChar)) continue;
                                var lineArr = trimLine.Split(' ');
                                var val = int.Parse(lineArr[0].Trim(':').Trim('-'));
                                var name = trimLine.Substring(lineArr[0].Length);
                                name = name.Trim();
                                name = name.Trim('-');
                                name = name.Trim(',');
                                name = name.Trim('.');
                                name = name.Trim('\r');
                                name = name.Trim();
                                sw.Write(string.Format(OptionPattern, val, name, propName));
                            }
                            sw.Write(SelectInputEnd);
                            break;
                        case "int":
                        case "short":
                            sw.Write(string.Format(InputPattern, propName, "text", required));
                            break;
                        default:
                            if (propName == "ID")
                            {
                                sw.WriteLine();
                                sw.WriteLine(@"            <input name=""ID"" type=""hidden"" id=""ID"" value=""@Model.ID"" />");
                            }
                            else
                            {
                                sw.Write(string.Format(InputPattern, propName, "text", required));
                            }
                            break;
                    }
                    if (propName != "ID")
                    {
                        sw.Write(GroupFooter);
                    }
                }
            }

            sw.Write(PanelFooter);
            sw.WriteLine();
            sw.Close();

        }

        private object FrieldlyName(string name)
        {
            StringBuilder sb = new StringBuilder();
            var nameArr = name.ToArray();
            int idx = 0;
            int prevUpcaseIdx = 0;
            foreach(var c in nameArr)
            {
                if (idx == 0)
                {
                    sb.Append(c.ToString().ToUpper());
                    prevUpcaseIdx = 0;
                }
                else
                {
                    if (c >= 'A' && c <= 'Z')
                    {
                        if (prevUpcaseIdx < idx - 1)
                        {
                            sb.Append(" ");
                        }
                        prevUpcaseIdx = idx;
                    }
                    sb.Append(c.ToString());
                }
                idx++;
            }
            return sb.ToString();
        }


        private void GenerateForm(XElement schemaDoc, XElement elm)
        {
            var formName = elm.Attribute("name").Value;
            StreamWriter sw = new StreamWriter(Path.Combine(_targetFolder, formName + "Form.cshtml"));
            sw.WriteLine(CommentsHeader);
            string indent = "    ";
            XNamespace ns = "http://www.w3.org/2001/XMLSchema";
            var typeName = elm.Attribute("name").Value;
            bool typeSpecified = false;
            if (elm.Attribute("type") != null)
            {
                typeSpecified = true;
                typeName = elm.Attribute("type").Value;
            }
            //SP script
            sw.WriteLine(string.Format(ModelHeader, ModelNameSpace, typeName));
            sw.WriteLine(PanelHeader);

            XElement fldsSeq;
            if (typeSpecified)
            {
                var type = (from t in schemaDoc.Descendants(ns + "complexType")
                            where t.Attribute("name")?.Value == typeName
                            select t).FirstOrDefault();
                fldsSeq = type.Element(ns + "sequence");
            }
            else
            {
                fldsSeq = elm.Element(ns + "complexType").Element(ns + "sequence");
            }
            var hasRowGuidField = false;
            foreach (var fldElm in fldsSeq.Elements())
            {
                var fldType = fldElm.Element(ns + "simpleType");
                if (fldType == null)
                {
                    continue;
                }
                var fldName = fldElm.Attribute("name").Value;
                var nullable = false;
                var isPk = false;
                if (fldElm.Attribute("nillable")?.Value == "true"
                    || fldElm.Attribute("minOccurs")?.Value == "0")
                {
                    nullable = true;
                }
                var fldTypeBase = fldType.Element(ns + "restriction").Attribute("base").Value;
                if (fldTypeBase == "xs:ID")
                {
                    //if (fldInfo.Name.ToLower() == "rowguid")
                    //{
                    //    hasRowGuidField = true;
                    //}
                    //continue; //primary key
                    isPk = true;
                }
                //fields.Add(fldInfo);
                var comments = fldElm.Element(ns + "annotation")?.Element(ns + "appinfo")?.Value;
                sw.WriteLine(GroupHeader);
                sw.WriteLine(string.Format(LabelPattern, fldName, FrieldlyName(fldName)));

                switch (fldTypeBase)
                {
                    case "xs:dateTime":
                    case "xs:date":
                        sw.WriteLine(string.Format(InputPattern, fldName, "date"));
                        break;
                    case "xs:boolean":
                        sw.WriteLine(string.Format(CheckBoxPattern, fldName));
                        break;
                    case "xs:int":
                    case "xs:short":
                        if (fldType.Name == "XSEnumType")
                        {
                            sw.WriteLine(string.Format(SelectInputPattern, fldName));
                            var comLines = comments.Split('\n');
                            foreach (var line in comLines)
                            {
                                var trimLine = line.Trim();
                                if (trimLine.Length == 0) continue;
                                char firstChar = trimLine.ToArray()[0];
                                if (!Char.IsNumber(firstChar)) continue;
                                var lineArr = trimLine.Split(' ');
                                var val = int.Parse(lineArr[0].Trim(':').Trim('-'));
                                var name = trimLine.Substring(lineArr[0].Length);
                                name = name.Trim();
                                name = name.Trim('-');
                                name = name.Trim(',');
                                name = name.Trim('.');
                                name = name.Trim('\r');
                                name = name.Trim();
                                sw.WriteLine(string.Format(OptionPattern, val, name, fldName));
                            }
                            sw.WriteLine(SelectInputEnd);
                        }
                        else
                        {
                            sw.WriteLine(string.Format(InputPattern, fldName, "text"));
                        }
                        break;
                    default:
                        sw.WriteLine(string.Format(InputPattern, fldName, "text"));
                        break;
                }
                sw.WriteLine(GroupFooter);
            }
            sw.WriteLine(PanelFooter);
            sw.WriteLine();
            sw.Close();
        }

    }
}
