using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Schema;
using System.Xml.Linq;

namespace CodeGeneration
{
    public class RamlGenerator
    {
        const string RamlTypesHeader = @"
#%RAML 1.0 Library

types:";
        const string RamlVer = "#%RAML 1.0 ";

        const string RamlHeader = @"
#%RAML 1.0
title: FN Delivery API
version: 1
protocols: [ HTTPS ] 
baseUri: https://api.firstnational.ca/delivery/{version}/
baseUriParameters: {}
mediaType: application/json
types: !include Deal.lib.raml
";
        const string ModelHeader = @"
namespace FirstNational.Net.API.Delivery.Models
{";
        const string ControllerHeader = @"
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TodoApi.Models;

namespace FirstNational.Net.API.Delivery.Controllers
{";
        const string Indent = "    ";
        private string _schemaFile;
        private string _targetFolder;
        private string _targetTypesFolder;
        private string _targetModelsFolder;
        private string _targetControllersFolder;
        private List<string> _processedObj = new List<string>();
        private StreamWriter _swService;
        private int _indentsService = 0;
        public RamlGenerator(string schemaFile, string targetFolder)
        {
            _schemaFile = schemaFile;
            _targetFolder = targetFolder;
            if (!Directory.Exists(_targetFolder))
            {
                Directory.CreateDirectory(_targetFolder);
            }
            _targetTypesFolder = System.IO.Path.Combine(_targetFolder, "types");
            if (!Directory.Exists(_targetTypesFolder))
            {
                Directory.CreateDirectory(_targetTypesFolder);
            }
            _targetModelsFolder = System.IO.Path.Combine(_targetFolder, "models");
            if (!Directory.Exists(_targetModelsFolder))
            {
                Directory.CreateDirectory(_targetModelsFolder);
            }
            _targetControllersFolder = System.IO.Path.Combine(_targetFolder, "controllers");
            if (!Directory.Exists(_targetControllersFolder))
            {
                Directory.CreateDirectory(_targetControllersFolder);
            }
        }
        public void Generate()
        {
            _swService = new StreamWriter(Path.Combine(_targetFolder, "fn_delivery.raml"));
            _swService.WriteLine(RamlHeader);
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
            _swService.Close();
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
            int indents = 0;
            int svcIndents = 0;
            int ctrIndents = 1;
            int mdlIndents = 1;
            bool isRoot = (objName == "deal");
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

            var objTypeName = GetObjTypeName(objName);
            StringBuilder sb = new StringBuilder();
            StringBuilder sbSvc = new StringBuilder();
            StringBuilder sbMdl = new StringBuilder();
            StringBuilder sbCtrlr = new StringBuilder();

            WriteLine(sbMdl, $"public class {objTypeName}", mdlIndents);
            WriteLine(sbMdl, "{", mdlIndents++);

            WriteLine(sbCtrlr, @"[Route(""api/[controller]"")]", ctrIndents );
            WriteLine(sbCtrlr, @"[ApiController]", ctrIndents);
            WriteLine(sbCtrlr, $"public class {objTypeName}Controller : ControllerBase", ctrIndents);
            WriteLine(sbCtrlr, "{", ctrIndents++);
            WriteLine(sbCtrlr, $"private I{objTypeName}Repository _repo", ctrIndents);
            WriteLine(sbCtrlr, $"public {objTypeName}Controller(I{objTypeName}Repository repo)", ctrIndents);
            WriteLine(sbCtrlr, "{", ctrIndents++);
            WriteLine(sbCtrlr, "_repo = repo", ctrIndents);
            WriteLine(sbCtrlr, "}", --ctrIndents);

            WriteLine(sbCtrlr);
            WriteLine(sbCtrlr, "[HttpGet(\"{id}\")]", ctrIndents);
            WriteLine(sbCtrlr, "public async Task<ActionResult<IEnumerable<TodoItem>>> GetById(long id)", ctrIndents);
            WriteLine(sbCtrlr, "{", ctrIndents++);
            WriteLine(sbCtrlr, "var item = await _repo.GetByIDAsync(id);", ctrIndents);
            WriteLine(sbCtrlr, "if (item == null)", ctrIndents);
            WriteLine(sbCtrlr, "{", ctrIndents++);
            WriteLine(sbCtrlr, "return NotFound();", ctrIndents);
            WriteLine(sbCtrlr, "}", --ctrIndents);
            WriteLine(sbCtrlr, "return item;", ctrIndents);
            WriteLine(sbCtrlr, "}", --ctrIndents);

            WriteLine(sbCtrlr);
            WriteLine(sbCtrlr, "[HttpPost]", ctrIndents);
            WriteLine(sbCtrlr, $"public async Task<ActionResult<{objTypeName}>> Post({objTypeName} item)", ctrIndents);
            WriteLine(sbCtrlr, "{", ctrIndents++);
            WriteLine(sbCtrlr, "_repo.Create(item);", ctrIndents);
            WriteLine(sbCtrlr, "await _repo.SaveChangesAsync();", ctrIndents);
            WriteLine(sbCtrlr, "return CreatedAtAction(nameof(GetById), new { id = item.Id }, item);", ctrIndents);
            WriteLine(sbCtrlr, "}", --ctrIndents);

            WriteLine(sbCtrlr);
            WriteLine(sbCtrlr, "[HttpPut(\"{id}\")]", ctrIndents);
            WriteLine(sbCtrlr, $"public async Task<ActionResult<{objTypeName}>> Put(long id, {objTypeName} item)", ctrIndents);
            WriteLine(sbCtrlr, "{", ctrIndents++);

            WriteLine(sbCtrlr, "if (id != item.Id)", ctrIndents);
            WriteLine(sbCtrlr, "{", ctrIndents++);
            WriteLine(sbCtrlr, "return BadRequest();", ctrIndents);
            WriteLine(sbCtrlr, "}", --ctrIndents);

            WriteLine(sbCtrlr, "_repo.Update(item);", ctrIndents);
            WriteLine(sbCtrlr, "await _repo.SaveChangesAsync();", ctrIndents);
            WriteLine(sbCtrlr, "return  NoContent();", ctrIndents);
            WriteLine(sbCtrlr, "}", --ctrIndents);

            WriteLine(sbCtrlr, "}", --ctrIndents);
            ctrIndents -= 2;
            WriteLine(sbCtrlr, "}", ctrIndents);

            ResourceForType(sbSvc, objTypeName, svcIndents);
            _swService.Write(sbSvc);

            indents = 0;
            if (isRoot)
            {

                WriteLine(sb, RamlTypesHeader);
                indents++;
                Write(sb, GetObjTypeName(objName), indents);
                WriteLine(sb, ":");
            }
            else
            {
                Write(sb, RamlVer);
                WriteLine(sb, "DataType"); // GetObjTypeName(objName));
            }
            indents++;
            WriteLine(sb, "type: object", indents);
            WriteLine(sb, "properties:", indents);
            foreach (XmlSchemaElement childElement in sequence.Items)
            {
                // recursion
                indents++;
                var propName = childElement.Name;
                var propTypeIsComplex = false;
                var propType = string.Empty;
                var isRequired = false;
                var isCollection = false;
                if (childElement.ElementSchemaType != null)
                {
                    isRequired = childElement.MinOccurs > 0;
                    isCollection = childElement.MaxOccurs > 1;
                    if (childElement.ElementSchemaType is XmlSchemaSimpleType)
                    {
                        propType = childElement.ElementSchemaType.QualifiedName.Name;
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
                        if (string.IsNullOrEmpty(propType))
                        {
                            propType = childElement.Name;
                        }
                    }
                }
                if (propTypeIsComplex)
                {
                    WriteLine(sb, $"{propName}:", indents);
                    indents++;
                    WriteLine(sb, $"required: {isRequired.ToString().ToLower()}", indents);
                    if (isCollection)
                    {
                        WriteLine(sb, $"type: array", indents);
                        WriteLine(sb, $"items", indents++);
                        WriteLine(sb, $"type: !includes types/{GetObjTypeName(propType)}.type.raml", indents--);
                    }
                    else
                    {
                        WriteLine(sb, $"type: !includes types/{GetObjTypeName(propType)}.type.raml", indents);
                    }
                    IterateOverElement(childElement);
                }
                else
                {
                    //simple type
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
                    WriteLine(sb, $"{propName}:", indents);
                    indents++;
                    WriteLine(sb, $"required: {isRequired.ToString().ToLower()}", indents);
                    WriteLine(sb, $"description: {comments}", indents);
                    Write(sb, "type: ", indents);
                    //Raml
                    switch (propType)
                    {
                        case "XSEnumCodeType":
                            break;
                        case "int":
                        case "short":
                            Write(sb, "number");
                            if (isCollection)
                            {
                                WriteLine(sb, "[]");
                            }
                            else
                            {
                                WriteLine(sb);
                            }
                            break;
                        case "decimal":
                            Write(sb, "number");
                            if (isCollection)
                            {
                                WriteLine(sb, "[]");
                            }
                            else
                            {
                                WriteLine(sb);
                            }
                            WriteLine(sb, "multipleOf: 0.01", indents);
                            break;
                        case "DataDictionaryType":
                            WriteLine(sb, "string");
                            Write(sb, "enum: [", indents);
                            var enumList = ((XmlSchemaSimpleTypeRestriction)((System.Xml.Schema.XmlSchemaSimpleType)childElement.SchemaType).Content).Facets;
                            for(int i = 0; i < enumList.Count; i ++)
                            {
                                var enm = enumList[i] as XmlSchemaEnumerationFacet ;
                                var anot = enm.Annotation as XmlSchemaAnnotation;
                                var desc = anot.Items[0] as XmlSchemaDocumentation;
                                var label = $"{enm.Value} - {desc.Markup[0].InnerText}";
                                Write(sb, label);
                                if (i < enumList.Count - 1)
                                {
                                    Write(sb, ", ");
                                }
                            }
                            //((System.Xml.Schema.XmlSchemaDocumentation)(new System.Collections.ArrayList.ArrayListDebugView(((System.Xml.Schema.XmlSchemaAnnotated)(new System.Collections.ArrayList.ArrayListDebugView(((System.Collections.CollectionBase)((System.Xml.Schema.XmlSchemaSimpleTypeRestriction)((System.Xml.Schema.XmlSchemaSimpleType)childElement.SchemaType).Content).Facets.List).InnerList).Items[0])).Annotation.Items.InnerList).Items[0])).Markup[0]
                            WriteLine(sb, "]"); 
                            break;
                        default:
                                Write(sb, propType.ToLower());
                            if (isCollection)
                            {
                                WriteLine(sb, "[]");
                            }
                            else
                            {
                                WriteLine(sb);
                            }
                            break;
                    }

                    //Model
                    switch (propType)
                    {
                        case "XSEnumCodeType":
                            break;
                        case "DataDictionaryType":
                            Write(sbMdl, $"{propName}Enum", mdlIndents);
                            //WriteLine(sbMdl, "/* enum:", mdlIndents);
                            //var enumList = ((XmlSchemaSimpleTypeRestriction)((System.Xml.Schema.XmlSchemaSimpleType)childElement.SchemaType).Content).Facets;
                            //for (int i = 0; i < enumList.Count; i++)
                            //{
                            //    var enm = enumList[i] as XmlSchemaEnumerationFacet;
                            //    var anot = enm.Annotation as XmlSchemaAnnotation;
                            //    var desc = anot.Items[0] as XmlSchemaDocumentation;
                            //    var label = $"{enm.Value} - {desc.Markup[0].InnerText}";
                            //    WriteLine(sbMdl, label, mdlIndents);
                            //}
                            ////((System.Xml.Schema.XmlSchemaDocumentation)(new System.Collections.ArrayList.ArrayListDebugView(((System.Xml.Schema.XmlSchemaAnnotated)(new System.Collections.ArrayList.ArrayListDebugView(((System.Collections.CollectionBase)((System.Xml.Schema.XmlSchemaSimpleTypeRestriction)((System.Xml.Schema.XmlSchemaSimpleType)childElement.SchemaType).Content).Facets.List).InnerList).Items[0])).Annotation.Items.InnerList).Items[0])).Markup[0]
                            //WriteLine(sbMdl, "*/");
                            break;
                        default:
                            if (isCollection)
                            {
                                Write(sbMdl, $"Enumerable<{propType}>", mdlIndents);
                            }
                            else
                            {
                                Write(sbMdl, propType, mdlIndents);

                            }
                            break;
                    }
                    WriteLine(sbMdl, $" {propName} {{get; set;}}");

                }
                indents--;
                indents--;
            }
            WriteLine(sbMdl, "}", --mdlIndents);
            mdlIndents -= 2;
            WriteLine(sbMdl, "}", mdlIndents);

            //sw.Write(PanelFooter);

            StreamWriter sw;
            if (isRoot)
            {
                sw = new StreamWriter(Path.Combine(_targetFolder, objName + ".lib.raml"));
            }
            else
            {
                sw = new StreamWriter(Path.Combine(_targetTypesFolder, objName + ".type.raml"));
            }
            //sw.Write(RamlTypesHeader);
            //sw.Write(sbIncludes);
            sw.Write(sb);
            sw.WriteLine();
            sw.Close();

            sw = new StreamWriter(Path.Combine(_targetControllersFolder, objName + "Controler.cs"));
            sw.WriteLine(ControllerHeader);
            sw.Write(sbCtrlr);
            sw.Close();

            sw = new StreamWriter(Path.Combine(_targetModelsFolder, objName + ".cs"));
            sw.WriteLine(ModelHeader);
            sw.Write(sbMdl);
            sw.Close();

        }
        private int ResourceForType(StringBuilder sbSvc, string typeName, int indents)
        {
            WriteLine(sbSvc, $"/{typeName}:");
            indents++;
            PostSecition(sbSvc, typeName, indents);
            WriteLine(sbSvc, "/{id}", indents);
            GetByIdSecition(sbSvc, typeName, indents);
            PutSecition(sbSvc, typeName, indents);
            WriteLine(sbSvc, "put:", indents);
            return indents;
        }
        private int GetByIdSecition(StringBuilder sbSvc, string typeName, int indents)
        {
            indents++;
            WriteLine(sbSvc, "get:", indents);
            indents++;
            WriteLine(sbSvc, $"description: Get {typeName} by ID", indents);
            WriteLine(sbSvc, "responses: ", indents);
            indents++;
            WriteLine(sbSvc, "200: ", indents);
            indents++;
            WriteLine(sbSvc, "body: ", indents);
            indents++;
            WriteLine(sbSvc, "application/json: ", indents);
            indents++;
            WriteLine(sbSvc, $"type: {typeName}", indents);
            WriteLine(sbSvc, $"example: !include examples/{typeName}.json", indents);
            indents -= 3;
            WriteLine(sbSvc, "404: ", indents);
            indents++;
            WriteLine(sbSvc, "body: ", indents);
            indents++;
            WriteLine(sbSvc, "application/json: ", indents);
            indents++;
            WriteLine(sbSvc, $"type: Error", indents);
            WriteLine(sbSvc, $"example: !include examples/error.json", indents);
            return indents;
        }
        private int PutSecition(StringBuilder sbSvc, string typeName, int indents)
        {
            indents++;
            WriteLine(sbSvc, "put:", indents);
            indents++;
            WriteLine(sbSvc, $"description: Update a {typeName} by ID", indents);
            indents++;
            WriteLine(sbSvc, "body: ", indents);
            indents++;
            WriteLine(sbSvc, "application/json: ", indents);
            indents++;
            WriteLine(sbSvc, $"type: {typeName}", indents);
            WriteLine(sbSvc, $"example: !include examples/{typeName}.json", indents);
            indents -= 2;
            WriteLine(sbSvc, "responses: ", indents);
            indents++;
            WriteLine(sbSvc, "200: ", indents);
            indents++;
            WriteLine(sbSvc, "body: ", indents);
            indents++;
            WriteLine(sbSvc, "application/json: ", indents);
            indents++;
            WriteLine(sbSvc, $"type: {typeName}", indents);
            WriteLine(sbSvc, $"example: !include examples/{typeName}.json", indents);
            indents -= 3;
            WriteLine(sbSvc, "404: ", indents);
            indents++;
            WriteLine(sbSvc, "body: ", indents);
            indents++;
            WriteLine(sbSvc, "application/json: ", indents);
            indents++;
            WriteLine(sbSvc, $"type: Error", indents);
            WriteLine(sbSvc, $"example: !include examples/error.json", indents);
            return indents;
        }
        private int PostSecition(StringBuilder sbSvc, string typeName, int indents)
        {
            indents++;
            WriteLine(sbSvc, "post:", indents);
            indents++;
            WriteLine(sbSvc, $"description: create a {typeName}", indents);
            indents++;
            WriteLine(sbSvc, "body: ", indents);
            indents++;
            WriteLine(sbSvc, "application/json: ", indents);
            indents++;
            WriteLine(sbSvc, $"type: {typeName}", indents);
            WriteLine(sbSvc, $"example: !include examples/{typeName}.json", indents);
            indents -= 2;
            WriteLine(sbSvc, "responses: ", indents);
            indents++;
            WriteLine(sbSvc, "201: ", indents);
            indents++;
            WriteLine(sbSvc, "body: ", indents);
            indents++;
            WriteLine(sbSvc, "application/json: ", indents);
            indents++;
            WriteLine(sbSvc, $"type: {typeName}", indents);
            WriteLine(sbSvc, $"example: !include examples/{typeName}.json", indents);
            return indents;
        }

        private void Write(StringBuilder sb, string content="", int indents = 0, bool newLine = false)
        {
            for(int i = 0; i < indents; i ++)
            {
                sb.Append(Indent);
            }
            if (newLine)
            {
                sb.AppendLine(content);
            }
            else
            {
                sb.Append(content);
            }
        }
        private void WriteLine(StringBuilder sb, string content = "", int indents = 0)
        {
            Write(sb, content, indents, true);
        }
        private string GetObjTypeName(string objName)
        {
            if (string.IsNullOrEmpty(objName))
            {
                objName = "UNKNOWN";
            }
            return objName.Substring(0, 1).ToUpper() + objName.Substring(1);
        }
        private object FriendlyName(string name)
        {
            StringBuilder sb = new StringBuilder();
            var nameArr = name.ToArray();
            int idx = 0;
            int prevUpcaseIdx = 0;
            foreach (var c in nameArr)
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

    }
}
