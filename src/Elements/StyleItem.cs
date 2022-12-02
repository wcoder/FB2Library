using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    /// <summary>
    /// Represents FB2 style tag
    /// </summary>
    public class StyleItem : IFb2TextItem, StyleType
    {
        internal const string StyleItemName = "style";

        /// <summary>
        /// Get/Set name of the sequence
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Get/Set language
        /// </summary>
        public string Lang { get; set; }
        public List<StyleType> StyleData { get; set; } = new List<StyleType>();
        protected virtual string GetElementName()
        {
            return StyleItemName;
        }
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder($"<style name='{Name}' xml:lang='{Lang}'>");
            foreach (var textItem in StyleData)
            {
                builder.Append(textItem.ToString());
                builder.Append(" ");
            }
            builder.Append("</style>");
            return builder.ToString();
        }
        /// <summary>
        /// Load element data from the node
        /// </summary>
        /// <param name="xStyle"></param>
        public void Load(XElement xStyle)
        {
            if (xStyle == null)
            {
                throw new ArgumentNullException("style");
            }
            if (xStyle.Name.LocalName != StyleItemName)
            {
                throw new ArgumentException(string.Format("The element is of type {0} while StyleItem accepts only {1} types", xStyle.Name.LocalName, StyleItemName));
            }

            Lang = null;
            XAttribute xLang = xStyle.Attribute(XNamespace.Xml + "lang");
            if (xLang != null)
            {
                Lang = xLang.Value;
            }

            Name = string.Empty;
            XAttribute xName = xStyle.Attribute("name");
            if (xName != null && xName.Value != null)
            {
                Name = xName.Value;
            }

            if (xStyle.HasElements)
            {
                IEnumerable<XNode> childElements = xStyle.Nodes();
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
                                StyleData.Add(image);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (xElement.Name.LocalName == InternalLinkItem.Fb2InternalLinkElementName)
                        {
                            InternalLinkItem linkItem = new InternalLinkItem();
                            try
                            {
                                linkItem.Load(xElement);
                                StyleData.Add(linkItem);
                            }
                            catch (Exception)
                            {
                            }
                        }
                        else if (xElement.Name.LocalName == StyleItemName)
                        {
                            StyleItem styleItem = new StyleItem();
                            try
                            {
                                styleItem.Load(xElement);
                                StyleData.Add(styleItem);
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
                            StyleData.Add(text);
                        }
                        catch (Exception)
                        {
                        }
                    }
                }

            }
            else if (!string.IsNullOrEmpty(xStyle.Value))
            {
                SimpleText text = new SimpleText();
                text.Load(xStyle);
                StyleData.Add(text);
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
            switch (xElement.Name.LocalName)
            {
                case InternalLinkItem.Fb2InternalLinkElementName:
                case InlineImageItem.Fb2InlineImageElementName:
                case StyleItem.StyleItemName:
                    return false;
            }
            return true;
        }
        public XNode ToXML()
        {
            if (Name == null || string.IsNullOrEmpty(Name))
            {
                throw new Exception("Name attribute is required by standard");
            }
            XElement xStyle = new XElement(Fb2Const.fb2DefaultNamespace + StyleItemName, new XAttribute("name", Name));
            if (!string.IsNullOrEmpty(Lang))
            {
                xStyle.Add(new XAttribute(XNamespace.Xml + "lang", Lang));
            }
            foreach (StyleType childElements in StyleData)
            {
                xStyle.Add(childElements.ToXML());
            }
            return xStyle;
        }
    }
}
