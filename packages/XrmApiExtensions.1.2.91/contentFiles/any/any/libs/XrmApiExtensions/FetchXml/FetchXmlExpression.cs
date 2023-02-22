using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Connectiv.XrmCommon.Extensions.FetchXml
{
    public class FetchXmlExpression
    {
        XmlDocument _fetchXml = new XmlDocument();

        public String RawXml
        {
            get
            {
                return _fetchXml.InnerXml;
            }
        }

        public String EntityName
        {
            get
            {
                return _fetchXml.DocumentElement.SelectSingleNode("//entity").Attributes[0].Value;
            }
        }

        public ColumnSet Columns
        {
            get
            {
                if (_fetchXml.DocumentElement.SelectNodes("//entity/all-attributes").Count > 0)
                    return new ColumnSet(true);
                else if(_fetchXml.DocumentElement.SelectNodes("//entity/no-attrs").Count > 0)
                {
                    return new ColumnSet(false);
                }
                else
                {
                    String[] attributes = _fetchXml.DocumentElement.SelectNodes("//entity/attribute").OfType<XmlNode>().Select(v=>v.Attributes[0].Value).ToArray();
                    return new ColumnSet(attributes);
                }
            }
            set
            {
                RemoveAllAttributes();
                AddAttributes(value);
            }
        }

        protected void AddAttributes(ColumnSet cols)
        {
            XmlNode entityNode = _fetchXml.DocumentElement.SelectSingleNode("//entity");
            if (cols.AllColumns)
            {
                entityNode.AppendChild(_fetchXml.CreateElement("all-attributes"));
            }
            else
            {
                foreach (String attributeName in cols.Columns)
                {
                    entityNode.AddAttribute("attribute", new Dictionary<string, string>() { { "name", attributeName } });
                }
            }
        }

        protected void RemoveAllAttributes(string root = null)
        {
            string rootNode = root ?? "//entity";
            XmlNode entityNode = _fetchXml.DocumentElement.SelectSingleNode(rootNode);

            foreach (string subNode in new List<String> { "/all-attributes", "/no-attrs", "/attribute" })
            {
                XmlNodeList attributeNodes = entityNode.SelectNodes(rootNode + subNode);
                foreach (XmlNode attributeToLoad in attributeNodes)
                {
                    attributeToLoad.ParentNode.RemoveChild(attributeToLoad);
                }
            }

            rootNode += "/link-entity";
            XmlNode linkentityNode = _fetchXml.DocumentElement.SelectSingleNode(rootNode);
            if(linkentityNode != null)
            {
                this.RemoveAllAttributes(rootNode);
            }
        }

        private void RemoveOrder()
        {
            XmlNodeList entityAttributes = _fetchXml.DocumentElement.SelectNodes("//entity/order");
            foreach (XmlNode order in entityAttributes)
            {
                order.ParentNode.RemoveChild(order);
            }
        }

        public FetchXmlExpression(String fetch)
        {
            _fetchXml.LoadXml(fetch);
        }

        public FetchXmlExpression ConvertToCountAggregation()
        {
            var retVal = new FetchXmlExpression(_fetchXml.InnerXml);
            retVal.RemoveAllAttributes();
            retVal.RemoveOrder();
            retVal._fetchXml.DocumentElement.SelectSingleNode("//entity").AddAttribute("attribute", new Dictionary<string, string>() { {"name" , retVal.EntityName+"id"}, { "aggregate", "count" } , { "alias" , "totalcount" } });
            retVal._fetchXml.DocumentElement.SelectSingleNode("//fetch").AddProperty(new KeyValuePair<string, string>("aggregate", "true"));
            return retVal;
        }
    }

    public static class XmlExtensions
    {
        public static void AddAttribute(this XmlNode instance, String attributeName, Dictionary<String, String> attributes)
        {
            var crmattr = instance.OwnerDocument.CreateElement(attributeName);
            foreach (KeyValuePair<String, String> attribute in attributes)
            {
                crmattr.AddProperty(attribute);

            }
            instance.AppendChild(crmattr);
        }

        public static void AddProperty(this XmlNode instance, KeyValuePair<String, String> property)
        {
            var attrib = instance.OwnerDocument.CreateAttribute(property.Key);
            attrib.Value = property.Value;
            instance.Attributes.Append(attrib); // Attributes[0] = name
        }
    }
}
