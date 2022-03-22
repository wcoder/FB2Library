﻿using System;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    public class ImageItem : IFb2TextItem
    {
        internal const string Fb2ImageElementName = "image";

        private readonly XNamespace _xLinkNamespace = @"http://www.w3.org/1999/xlink";

        public string ImageType { get; set; }
        public string HRef { get; set; }
        public string AltText { get; set; }
        public string Title { get; set; }
        public string ID { get; set; }

        internal void Load(XElement xImage)
        {
            if (xImage == null)
            {
                throw new ArgumentNullException(nameof(xImage));
            }

            if (xImage.Name.LocalName != Fb2ImageElementName)
            {
                throw new ArgumentException("Element of wrong type passed", nameof(xImage));
            }

            XAttribute xType = xImage.Attribute(_xLinkNamespace + "type");
            if (xType != null && xType.Value != null)
            {
                ImageType = xType.Value;
            }


            XAttribute xHRef = xImage.Attribute(_xLinkNamespace + "href");
            if (xHRef != null && xHRef.Value != null)
            {
                HRef = xHRef.Value;
            }

            XAttribute xAlt = xImage.Attribute("alt");
            if (xAlt != null && xAlt.Value != null)
            {
                AltText = xAlt.Value;
            }

            XAttribute xTitle = xImage.Attribute("title");
            if (xTitle != null && xTitle.Value != null)
            {
                Title = xTitle.Value;
            }

            XAttribute xID = xImage.Attribute("id");
            if (xID != null && xID.Value != null)
            {
                ID = xID.Value;
            }
        }

        public XNode ToXML()
        {
            XElement xImage = new XElement(Fb2Const.fb2DefaultNamespace + Fb2ImageElementName);
            if (!string.IsNullOrEmpty(ImageType))
            {
                xImage.Add(new XAttribute(_xLinkNamespace + "type", ImageType));
            }
            if (!string.IsNullOrEmpty(HRef))
            {
                xImage.Add(new XAttribute(_xLinkNamespace + "href", HRef));
            }
            if (!string.IsNullOrEmpty(AltText))
            {
                xImage.Add(new XAttribute("alt", AltText));
            }
            if (!string.IsNullOrEmpty(Title))
            {
                xImage.Add(new XAttribute("title", Title));
            }
            if (!string.IsNullOrEmpty(ID))
            {
                xImage.Add(new XAttribute("id", ID));
            }
            return xImage;
        }
    }
}
