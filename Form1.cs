﻿using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml;
using System.Windows.Forms;

namespace CodeGeneration
{
    public partial class Form1 : Form
    {
        const string CSharpFileName = "FNEnumValues.cs";
        const string SqlFileName = "FNEnumValues.sql";
        const string XmlDocFileName = "FN_EnumFields.xml";
        const string EnumCodeType = "XSEnumCodeType";
        const string CodeHeader =
@"// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by CodeGeneration utility on {0}
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace FirstNational.Net.API.Schema.XML.Enums";


        public Form1()
        {
            InitializeComponent();
            txtSchemaFile.Text = Properties.Settings.Default.SchemaFile;
            txtTargetFolder.Text = Properties.Settings.Default.TargetFolder;
            txtDbSubmission.Text = Properties.Settings.Default.DbSubmissionFile;
            txtDbUtility.Text = Properties.Settings.Default.DbUtilityFile;
            txtDbTargetFolder.Text = Properties.Settings.Default.DbTargetFolder;
        }

        private void btnSchemaFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.FileName = txtSchemaFile.Text;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtSchemaFile.Text = fd.FileName;
                Properties.Settings.Default.SchemaFile = fd.FileName;
                Properties.Settings.Default.Save();
            }

        }

        private void btnEnumValues_Click(object sender, EventArgs e)
        {
            List<string> enumFieldNames = new List<string>();
            //Dictionary<string, StringBuilder> enumFieldComments = new Dictionary<string, StringBuilder>();
            Dictionary<string, List<string>> enumFieldsPaths = new Dictionary<string, List<string>>();
            Dictionary<string, List<EnumNameValuePair>> enumFieldsValues = new Dictionary<string, List<EnumNameValuePair>>();
            XNamespace ns = "http://www.w3.org/2001/XMLSchema";
            XElement fnSchema = XElement.Load(txtSchemaFile.Text);
            var enumCodeFields = from fld in fnSchema.Descendants(ns+"element")
                                 where fld.Attribute("type")?.Value == EnumCodeType
                                 select fld;
            foreach (var fld in enumCodeFields)
            {
                var fldName = fld.Attribute("name").Value;
                var path = string.Join("/", fld.AncestorsAndSelf().Where(x => x.Attribute("name") != null).Select(x => x.Attribute("name")?.Value).Reverse().ToArray());
                //StringBuilder sbComments;
                if (!enumFieldNames.Contains(fldName))
                {
                    var fldComments = fld.Element(ns + "annotation").Element(ns + "documentation").Value;
                    enumFieldNames.Add(fldName);
                    //sbComments = new StringBuilder();
                    //sbComments.Append("    /// Field Name: ");
                    //sbComments.AppendLine(fldName);
                    //sbComments.Append("    /// Path://");
                    //sbComments.AppendLine( string.Join("/", fld.AncestorsAndSelf().Where(x => x.Attribute("name") != null).Select(x => x.Attribute("name")?.Value).Reverse().ToArray()));
                    //enumFieldComments.Add(fldName, sbComments);
                    enumFieldsPaths.Add(fldName, new List<string>());
                    enumFieldsValues.Add(fldName, new List<EnumNameValuePair>());
                    var comLines = fldComments.Split('\n');
                    foreach(var line in comLines)
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
                        var pair = new EnumNameValuePair() { Name = name, Value = val };
                        enumFieldsValues[fldName].Add(pair);
                    }
                }
                else
                {
                    //sbComments = enumFieldComments[fldName];
                    //sbComments.Append("    /// Path://");
                    //sbComments.AppendLine(string.Join("/", fld.AncestorsAndSelf().Where(x => x.Attribute("name") != null).Select(x => x.Attribute("name")?.Value).Reverse().ToArray()));
                }
                enumFieldsPaths[fldName].Add(path);
            }
            StreamWriter fileCs = new StreamWriter(Path.Combine(txtTargetFolder.Text, CSharpFileName));
            fileCs.WriteLine(string.Format(CodeHeader, DateTime.Now));
            fileCs.WriteLine("{");
            XmlDocument doc = new XmlDocument();
            var root = doc.CreateElement("enumFields");
            doc.AppendChild(root);
            var xmlComments = doc.CreateElement("comments");
            xmlComments.InnerText = string.Format("This document is generated from FN_Schema.xsd by CodeGeneration utility on {0}", DateTime.Now);
            root.AppendChild(xmlComments);
            //StreamWriter fileSql = new StreamWriter(Path.Combine(txtTargetFolder.Text, SqlFileName));
            foreach (var fldName in enumFieldNames)
            {
                var xmlFld = doc.CreateElement("enumField");
                root.AppendChild(xmlFld);
                var xmlAttrFldName = doc.CreateAttribute("fieldName");
                xmlAttrFldName.Value = fldName;
                xmlFld.Attributes.Append(xmlAttrFldName);

                fileCs.WriteLine("    /// <summary>");
                fileCs.Write("    /// <fieldName>");
                fileCs.Write(fldName);
                fileCs.WriteLine("</fieldName>");
                var xmlPaths = doc.CreateElement("paths");
                xmlFld.AppendChild(xmlPaths);
                var xmlOptions = doc.CreateElement("options");
                xmlFld.AppendChild(xmlOptions);
                foreach (var path in enumFieldsPaths[fldName])
                {
                    var xmlPath = doc.CreateElement("path");
                    xmlPath.InnerText = path;
                    xmlPaths.AppendChild(xmlPath);
                    fileCs.Write("    /// <path>");
                    fileCs.Write(path);
                    fileCs.WriteLine("</path>");
                }
                //fileCs.Write(enumFieldComments[fldName].ToString());
                fileCs.WriteLine("    /// </summary>");
                fileCs.Write("    public enum ");
                fileCs.WriteLine(fldName);
                fileCs.WriteLine("    {");
                foreach(var pair in enumFieldsValues[fldName])
                {
                    var xmlOption = doc.CreateElement("option");
                    xmlOptions.AppendChild(xmlOption);
                    var xmlAttrName = doc.CreateAttribute("name");
                    var xmlAttrValue = doc.CreateAttribute("value");
                    xmlOption.Attributes.Append(xmlAttrValue);
                    xmlOption.Attributes.Append(xmlAttrName);
                    xmlAttrName.Value = pair.Name;
                    xmlAttrValue.Value = pair.Value.ToString();
                    fileCs.Write("        ");
                    var enumName = pair.Name.Replace("+", "and");
                    enumName = enumName.Replace("N/A", "NA");
                    enumName = enumName.Replace("%", "");
                    enumName = enumName.Replace("'", "");
                    enumName = enumName.Replace(".", "");
                    enumName = enumName.Replace("(", "");
                    enumName = enumName.Replace(")", "");
                    enumName = enumName.Replace('-', '_');
                    enumName = enumName.Replace(' ', '_');
                    enumName = enumName.Replace("/", "_or_");
                    enumName = enumName.Replace("__", "_");
                    enumName = enumName.Replace("__", "_");
                    enumName = enumName.Replace("__", "_");
                    if (char.IsDigit(enumName.ToArray()[0]))
                    {
                        enumName = "_" + enumName;
                    }
                    fileCs.Write(enumName);
                    fileCs.Write(" = ");
                    fileCs.Write(pair.Value);
                    fileCs.WriteLine(",");
                }
                fileCs.WriteLine("    }");
                fileCs.WriteLine();
                fileCs.WriteLine();
            }
            fileCs.WriteLine("}");
            fileCs.Close();
            fileCs.Dispose();
            doc.Save(Path.Combine(txtTargetFolder.Text, XmlDocFileName));
            if (MessageBox.Show("Code and Documents generated sucessfully, quit?", "Generated", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnTargetFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.SelectedPath = txtTargetFolder.Text;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtTargetFolder.Text = fd.SelectedPath;
                Properties.Settings.Default.TargetFolder = fd.SelectedPath;
                Properties.Settings.Default.Save();
            }

        }

        private class EnumNameValuePair
        {
            public string Name { get; set; }
            public int Value { get; set; }
        }

        private void btnDbSubmission_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.FileName = this.txtDbSubmission.Text;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtDbSubmission.Text = fd.FileName;
                Properties.Settings.Default.DbSubmissionFile = fd.FileName;
                Properties.Settings.Default.Save();
            }

        }

        private void btnDbUtility_Click(object sender, EventArgs e)
        {
            OpenFileDialog fd = new OpenFileDialog();
            fd.FileName = this.txtDbUtility.Text;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtDbUtility.Text = fd.FileName;
                Properties.Settings.Default.DbUtilityFile = fd.FileName;
                Properties.Settings.Default.Save();
            }

        }

        private void btnSPTargetFolder_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fd = new FolderBrowserDialog();
            fd.SelectedPath = txtDbTargetFolder.Text;
            if (fd.ShowDialog() == DialogResult.OK)
            {
                txtDbTargetFolder.Text = fd.SelectedPath;
                Properties.Settings.Default.DbTargetFolder = fd.SelectedPath;
                Properties.Settings.Default.Save();
            }

        }

        private void btnDBSP_Click(object sender, EventArgs e)
        {
            SPGenerator gen = new CodeGeneration.SPGenerator(txtDbSubmission.Text, txtDbUtility.Text, txtDbTargetFolder.Text);
            gen.Generate();
            if (MessageBox.Show("Code and Documents generated sucessfully, quit?", "Generated", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void btnRaml_Click(object sender, EventArgs e)
        {
            RamlGenerator gen = new RamlGenerator( txtSchemaFile.Text, txtTargetFolder.Text);
            gen.Generate();
            if (MessageBox.Show("Yamal types files generated sucessfully, quit?", "Generated", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                this.Close();
            }
        }
    }
}