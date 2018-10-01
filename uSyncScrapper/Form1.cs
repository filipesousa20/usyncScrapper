using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using uSyncScrapper.Models;

namespace uSyncScrapper
{
    public partial class Form1 : Form
    {
        private string[] compositionAliasToIgnore = new string[] { "seo", "visibility", "redirect" };

        private string[] docTypesToIgnore = new string[] { "errorPage" };

        public Form1()
        {
            InitializeComponent();
        }

        private void buttonBrowseFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void buttonScrap_Click(object sender, EventArgs e)
        {
            ParseUSyncfilesToHtml(textBox1.Text);
        }

        private void ParseUSyncfilesToHtml(string folder)
        {
            var docTypes = ParseUSyncFiles(folder);
            var html = GenerateHtml(docTypes);
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Html Files (*.html)|*.html";
            dlg.DefaultExt = "html";
            dlg.AddExtension = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(dlg.FileName, html);
            }
        }

        private IEnumerable<DocumentType> ParseUSyncFiles(string folder)
        {


            //pages
            var docTypes = new List<DocumentType>();
            string documentTypeFolder = Directory
                .GetDirectories(folder, "DocumentType", SearchOption.AllDirectories)
                .First();
            var pagesDirectory = Directory
                .GetDirectories(documentTypeFolder, "master", SearchOption.AllDirectories)
                .First();
            var websiteSettingsDirectory = Directory
                .GetDirectories(documentTypeFolder, "website-settings", SearchOption.AllDirectories)
                .First();
            var pages = Directory.GetFiles(pagesDirectory, "*.config", SearchOption.AllDirectories);
            var websiteSettings = (Directory.GetFiles(websiteSettingsDirectory, "*.config", SearchOption.AllDirectories));
            var files = websiteSettings.Union(pages);

            //datatypes
            string datatypeFolder = Directory
                        .GetDirectories(folder, "DataType", SearchOption.AllDirectories)
                        .First();
            var datatypeFiles = Directory.GetFiles(datatypeFolder, "*.config", SearchOption.AllDirectories);

            var dataTypeDocuments = new List<XDocument>();
            foreach (var datatypeFile in datatypeFiles)
            {
                dataTypeDocuments.Add(XDocument.Load(datatypeFile));
            }

            //compositions
            var compositionsFolder = Directory
                .GetDirectories(documentTypeFolder, "compositions", SearchOption.AllDirectories)
                .First();
            var compositionsFiles = Directory.GetFiles(compositionsFolder, "*.config", SearchOption.AllDirectories);
            var allCompositionsDocuments = new List<XDocument>();
            foreach (var compositionsFile in compositionsFiles)
            {
                allCompositionsDocuments.Add(XDocument.Load(compositionsFile));
            }

            int index = 1;
            foreach (var file in files)
            {
                try
                {
                    var docType = new DocumentType();
                    XDocument doc = XDocument.Load(file);

                    var name = doc
                        .Root
                        .Element("Info")
                        .Element("Name")
                        .Value;

                    var alias = doc
                       .Root
                       .Element("Info")
                       .Element("Alias")
                       .Value;
                    if (docTypesToIgnore.Any(i => i.Contains(alias))) {continue;}

                    docType.Name = name.Replace("Website Settings", "Global Items");

                    var description = doc
                        .Root
                        .Element("Info")
                        .Element("Description")
                        .Value;
                    docType.Description = description;

                    var compositionsDocuments = GetCompositions(allCompositionsDocuments, doc);

                    var allTabs = new List<Tab>();
                    allTabs.AddRange(GetTabs(doc));
                    allTabs.AddRange(GetCompositionsTabs(compositionsDocuments));
                    allTabs = allTabs.OrderBy(i => i.Order).ToList();

                    var allProperties = new List<DocumentTypeProperty>();
                    allProperties.AddRange(GetCompositionsProperties(compositionsDocuments, doc));
                    allProperties.AddRange(GetDocumentProperties(doc));

                    allProperties = allProperties
                        .OrderBy(p => allTabs.IndexOf(allTabs.First(t => t.Caption == p.Tab)))
                        .ThenBy(i => i.Order)
                        .ToList();
                    docType.Properties = allProperties;

                    ComputeNestedContentMaxItems(dataTypeDocuments, allProperties);
                    ComputeTreePickerMaxItems(dataTypeDocuments, allProperties);

                    if (!docType.Properties.Any()) { continue; }
                    docTypes.Add(docType);
                    docType.Index = index;
                    index++;
                }
                catch (Exception ex)
                {
                }
            }
            return docTypes;
        }

        private IEnumerable<Tab> GetCompositionsTabs(IEnumerable<XDocument> compositions)
        {
            var tabs = new List<Tab>();
            foreach (var comp in compositions)
            {
                tabs.AddRange(GetTabs(comp));
            }
            return tabs;
        }

        private IEnumerable<Tab> GetTabs(XDocument doc)
        {
            return doc
                .Root
                .Element("Tabs")
                .Elements("Tab")
                .Select(i => new Tab { Caption = i.Element("Caption").Value, Order = int.Parse(i.Element("SortOrder").Value) });
        }

        private IEnumerable<XDocument> GetCompositions(List<XDocument> compositionsDocuments, XDocument doc)
        {
            var compositions = doc
                    .Root
                    .Element("Info")
                    .Element("Compositions")
                    .Elements("Composition")
                    .Select(i => (string)i.Attribute("Key"))
                    .Select(i => compositionsDocuments.Where(j => j
                        .Root
                        .Element("Info")
                        .Element("Key")
                        .Value == i).FirstOrDefault())
                    .Where(c => c != null)
                    .Where(c => !compositionAliasToIgnore.Contains(c
                        .Root
                        .Element("Info")
                        .Element("Alias")
                        .Value));
            return compositions;
        }

        private IEnumerable<DocumentTypeProperty> GetCompositionsProperties(IEnumerable<XDocument> compositionsDocuments, XDocument doc)
        {
            var allProperties = new List<DocumentTypeProperty>();
            foreach (var comp in compositionsDocuments)
            {
                if (comp != null)
                {
                    allProperties.AddRange(GetDocumentProperties(comp));
                }
            }
            return allProperties;
        }

        private IEnumerable<DocumentTypeProperty> GetDocumentProperties(XDocument doc)
        {
            var properties = doc
                    .Root
                    .Element("GenericProperties")
                    .Elements("GenericProperty")
                    .Where(i => checkBoxIncludePropertiesWithoutDescription.Checked ? true : !string.IsNullOrEmpty(i.Element("Description").Value))
                    .Select(i => new DocumentTypeProperty { Name = i.Element("Name").Value, Text = i.Element("Description").Value, Tab = i.Element("Tab").Value, Order = int.Parse(i.Element("SortOrder").Value), Type = i.Element("Type").Value, Definition = i.Element("Definition").Value });
            return properties;
        }

        private void ComputeNestedContentMaxItems(List<XDocument> dataTypeDocuments, List<DocumentTypeProperty> properties)
        {
            var nestedContentProperties = properties
                                    .Where(i => i.Type == "Umbraco.NestedContent");

            foreach (var prop in nestedContentProperties)
            {
                var datatype = dataTypeDocuments.Where(i => i
                    .Root
                    .Attribute("Key")
                    .Value == prop.Definition).FirstOrDefault();
                if (datatype != null)
                {
                    var maxItems = datatype
                        .Root
                        .Element("PreValues")
                        .Elements("PreValue")
                        .FirstOrDefault(i => (string)i.Attribute("Alias") == "maxItems")
                        .Value;
                    var maxItemsDefault = 0;
                    int.TryParse(maxItems, out maxItemsDefault);
                    prop.MaxItems = maxItemsDefault;
                }
            }
        }

        private void ComputeTreePickerMaxItems(List<XDocument> dataTypeDocuments, List<DocumentTypeProperty> properties)
        {
            var treePickerProperties = properties
                                    .Where(i => i.Type.StartsWith("Umbraco.MultiNodeTreePicker"));

            foreach (var prop in treePickerProperties)
            {
                var datatype = dataTypeDocuments.Where(i => i
                    .Root
                    .Attribute("Key")
                    .Value == prop.Definition).FirstOrDefault();
                if (datatype != null)
                {
                    var maxItems = datatype
                        .Root
                        .Element("PreValues")
                        .Elements("PreValue")
                        .FirstOrDefault(i => (string)i.Attribute("Alias") == "maxNumber")
                        .Value;
                    var maxItemsDefault = 0;
                    int.TryParse(maxItems, out maxItemsDefault);
                    prop.MaxItems = maxItemsDefault;
                }
            }
        }

        private string GenerateHtml(IEnumerable<DocumentType> docTypes)
        {
            string documentTypeFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "DocumentType.cshtml");
            string finalDocumentFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views", "FinalDocument.cshtml");

            var templateService = new TemplateService();
            templateService.AddNamespace("uSyncScrapper.Models");
            var body = new StringBuilder();

            foreach (var docType in docTypes)
            {
                body.Append(templateService.Parse(File.ReadAllText(documentTypeFilePath), docType, null, "DocumentType"));
            }

            var finalDocType = new FinalDocument { Body = body.ToString(), DocTypes = docTypes };
            var finalDocument = templateService.Parse(File.ReadAllText(finalDocumentFilePath), finalDocType, null, "FinalDocument");

            return WebUtility.HtmlDecode(finalDocument);
        }
    }
}
