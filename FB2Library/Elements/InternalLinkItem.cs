using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    public class InternalLinkItem : StyleType
    {
        private readonly XNamespace lNamespace = @"http://www.w3.org/1999/xlink";

        public string Type { get; set; }

        public string HRef { get; set; }

        private readonly List<StyleType> _linkData = new List<StyleType>();
        public List<StyleType> LinkData { get { return _linkData; } }

        internal const string Fb2InternalLinkElementName = "a";


        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("<a");
            if (string.IsNullOrEmpty(Type))
            {
                builder.Append($" type='{Type}'");
            }
            if (string.IsNullOrEmpty(HRef))
            {
                builder.Append($" href='{HRef}'");
            }
            builder.Append(">");
            foreach (var item in _linkData)
            {
                builder.Append(item.ToString());
                builder.Append(" ");
            }
            builder.Append("</a>");
            return builder.ToString();
        }

        internal void Load(XElement xLink)
        {
            if (xLink == null)
            {
                throw new ArgumentNullException("xLink");
            }

            if (xLink.Name.LocalName != Fb2InternalLinkElementName)
            {
                throw new ArgumentException("Element of wrong type passed", "xLink");
            }

            if (xLink.HasElements)
            {
                IEnumerable<XNode> childElements = xLink.Nodes();
                foreach (var element in childElements)
                {
                    if ((element.NodeType == XmlNodeType.Element) && !IsSimpleText(element))
                    {
                        XElement xElement = (XElement)element;
                        if (xElement.Name.LocalName == InlineImageItem.Fb2InlineImageElementName)
                        {
                            InlineImageItem image = new InlineImageItem();
                            try
                            {
                                image.Load(xElement);
                                _linkData.Add(image);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                    else
                    {
                        SimpleText text = new SimpleText();
                        try
                        {
                            text.Load(element);
                            _linkData.Add(text);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                    }
                }
            }
            else if (!string.IsNullOrEmpty(xLink.Value))
            {
                SimpleText text = new SimpleText();
                text.Load(xLink);
                _linkData.Add(text);
            }

            XAttribute xTypeAttr = xLink.Attribute("type");
            if ((xTypeAttr != null) && (xTypeAttr.Value != null))
            {
                Type = xTypeAttr.Value;
            }

            XAttribute xHRefAttr = xLink.Attribute(lNamespace + "href");
            if ((xHRefAttr != null) && (xHRefAttr.Value != null))
            {
                HRef = xHRefAttr.Value;
            }

        }

        private bool IsSimpleText(XNode element)
        {
            // if not element than we assume simple text
            if (element.NodeType != XmlNodeType.Element)
            {
                return true;
            }
            XElement xElement = (XElement)element;
            if (xElement.Name.LocalName == InternalLinkItem.Fb2InternalLinkElementName)
            {
                throw new ArgumentException("Schema doesn't support nested links");
            }
            return xElement.Name.LocalName != InlineImageItem.Fb2InlineImageElementName;
        }

        public XNode ToXML()
        {
            XElement xLink = new XElement(Fb2Const.fb2DefaultNamespace + Fb2InternalLinkElementName);
            if (!string.IsNullOrEmpty(Type))
            {
                xLink.Add(new XAttribute("type", Type));
            }
            if (!string.IsNullOrEmpty(HRef))
            {
                xLink.Add(new XAttribute(lNamespace + "href", HRef));
            }
            foreach (StyleType childElements in _linkData)
            {
                xLink.Add(childElements.ToXML());
            }
            return xLink;
        }

    }
}
