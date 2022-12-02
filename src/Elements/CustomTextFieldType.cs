using System;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    public class CustomTextFieldType : TextFieldType 
    {
        /// <summary>
        /// Get/Set info-type attribute
        /// </summary>
        public string InfoType { get; private set; }

        public override void Load(XElement xElement)
        {
            base.Load(xElement);

            InfoType = null;
            XAttribute xInfoType = xElement.Attribute("info-type");
            if (xInfoType == null)
            {
                throw new Exception("info-type attribute required for custom info element");
            }

            InfoType = xInfoType.Value;
        }
    }
}
