using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace ResourcesSrcComparer
{
    internal class ResourceFileComparer
    {
        private static void Log(RichTextBox logConsole, string message)
        {
            if (logConsole == null) return;

            logConsole.AppendText($"{Environment.NewLine}{message}");
        }

        internal Tuple<Dictionary<string, string>, List<string>> Compare(string sourceFilePath, string targetFilePath, bool apply = false, RichTextBox logConsole = null)
        {
            XmlDocument srcXmlDocument = new XmlDocument(), tgtXmlDocument = new XmlDocument();
            srcXmlDocument.Load(sourceFilePath);
            tgtXmlDocument.Load(targetFilePath);

            var srcDataNodes = srcXmlDocument.SelectNodes("//data");

            List<string> srcNodesNotInTarget = new List<string>();
            var nodesWithDifferentValues = new Dictionary<string, string>();

            string dataXPath = "//data[@name=\"{0}\"]";

            for (int i = 0; i < srcDataNodes.Count; i++)
            {
                string name = srcDataNodes[i].Attributes.GetNamedItem("name").Value,
                    value = srcDataNodes[i].SelectSingleNode("value").InnerText;

                var matchedTargetNode = tgtXmlDocument.SelectSingleNode(string.Format(dataXPath, name));

                if (apply && matchedTargetNode == null)
                {
                    Log(logConsole, $"appending new element to name: {name} & value: {value}");
                    var srcNode = srcDataNodes[i];
                    var element = tgtXmlDocument.CreateNode(srcNode.NodeType, srcNode.Name, srcNode.NamespaceURI);
                    foreach (XmlAttribute attr in srcNode.Attributes)
                    {
                        var newAttribute = tgtXmlDocument.CreateAttribute(attr.Prefix, attr.LocalName, attr.NamespaceURI);
                        newAttribute.Value = attr.Value;
                        element.Attributes.Append(newAttribute);
                    }

                    element.InnerXml = srcNode.InnerXml;

                    tgtXmlDocument.DocumentElement.AppendChild(element);
                    tgtXmlDocument.Save(targetFilePath);
                    srcNodesNotInTarget.Add(name);
                    continue;
                }

                if (!apply && matchedTargetNode == null) { srcNodesNotInTarget.Add(name); continue; }

                var valueNode = matchedTargetNode.SelectSingleNode("value");

                if (apply && !valueNode.InnerText.Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    Log(logConsole, $"Updating value from: {valueNode.InnerText} & to: {value}");
                    valueNode.InnerText = value; tgtXmlDocument.Save(targetFilePath); continue;
                }

                if (!valueNode.InnerText.Equals(value, StringComparison.OrdinalIgnoreCase)) nodesWithDifferentValues.Add(name, value);
            }

            return new Tuple<Dictionary<string, string>, List<string>>(nodesWithDifferentValues, srcNodesNotInTarget);
        }
    }
}
