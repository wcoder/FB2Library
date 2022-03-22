using System;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    public class TextFieldType
    {
        public TextFieldType()
        {
            Text = string.Empty;
        }

        /// <summary>
        /// Get/Set text value
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Get/Set language
        /// </summary>
        public string Language { get; set; }

        public override string ToString()
        {
            return Text;
        }

        public virtual void Load(XElement xElement)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }
            Text = string.Empty;
            if (!string.IsNullOrEmpty(xElement.Value))
            {
                Text = xElement.Value;
            }

            Language = null;
            // TODO: глянуть 
            XAttribute xLang = xElement.Attribute(XNamespace.Xml + "lang");
            if (xLang != null && string.IsNullOrEmpty(xLang.Value))
            {
                Language = xLang.Value;
            }
        }

        public virtual XElement ToXML(string nameElement)
        {
            XElement xText = new XElement(Fb2Const.fb2DefaultNamespace + nameElement, Text);
            if (Language != null)
            { 
               xText.Add(new XAttribute(XNamespace.Xml + "lang",Language));
            }
            return xText;
        }
    }
}
