using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    public class ImageItem : IFb2TextItem
    {
        private readonly XNamespace xLinkNamespace = @"http://www.w3.org/1999/xlink";

        public string ImageType { get; set; }
        public string HRef { get; set; }
        public string AltText { get; set; }
        public string Title { get; set; }
        public string ID { get; set; }


        internal const string Fb2ImageElementName = "image";

        internal void Load(XElement xImage)
        {
            if (xImage == null)
            {
                throw new ArgumentNullException("xImage");
            }

            if (xImage.Name.LocalName != Fb2ImageElementName)
            {
                throw new ArgumentException("Element of wrong type passed", "xImage");
            }

            XAttribute xType = xImage.Attribute(xLinkNamespace + "type");
            if ((xType != null) && (xType.Value != null))
            {
                ImageType = xType.Value;
            }


            XAttribute xHRef = xImage.Attribute(xLinkNamespace + "href");
            if ((xHRef != null) && (xHRef.Value != null))
            {
                HRef = xHRef.Value;
            }

            XAttribute xAlt = xImage.Attribute("alt");
            if ((xAlt != null) && (xAlt.Value != null))
            {
                AltText = xAlt.Value;
            }

            XAttribute xTitle = xImage.Attribute("title");
            if ((xTitle != null) && (xTitle.Value != null))
            {
                Title = xTitle.Value;
            }

            XAttribute xID = xImage.Attribute("id");
            if ((xID != null) && (xID.Value != null))
            {
                ID = xID.Value;
            }

        }

    }
}
