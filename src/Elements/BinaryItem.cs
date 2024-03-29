﻿using System;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    public enum ContentTypeEnum
    {
        ContentTypeUnknown,
        ContentTypeJpeg,
        ContentTypePng,
        ContentTypeGif,
    }

    public class BinaryItem
    {
        private const string ContentTypeAttributeName = "content-type";
        private const string IdAttributeName = "id";

        internal const string Fb2BinaryItemName = "binary";

        public static Func<byte[], ContentTypeEnum> DetectContentType { get; set; }

        public ContentTypeEnum ContentType { get; set; }
        public byte[] BinaryData { get; set; }
        public string Id { get; set; }

        internal void Load(XElement binarye)
        {
            if (binarye == null)
            {
                throw new ArgumentNullException(nameof(binarye));
            }

            if (binarye.Name.LocalName != Fb2BinaryItemName)
            {
                throw new ArgumentException("Element of wrong type passed", nameof(binarye));
            }

            XAttribute xContentType = binarye.Attribute(ContentTypeAttributeName);
            if (xContentType?.Value == null)
            {
                throw new NullReferenceException("content type not defined/present");
            }
            switch (xContentType.Value.ToLower())
            {
                case "image/jpeg":
                case "image/jpg":
                    ContentType = ContentTypeEnum.ContentTypeJpeg;
                    break;
                case "image/png":
                    ContentType = ContentTypeEnum.ContentTypePng;
                    break;
                case "image/gif":
                    ContentType = ContentTypeEnum.ContentTypeGif;
                    break;
                case "application/octet-stream": // something not detected , generated by various converters
                    break;
            }

            XAttribute idAttribute = binarye.Attribute(IdAttributeName);
            
            Id = idAttribute?.Value ?? throw new NullReferenceException("ID not defined/present");

            if (BinaryData != null)
            {
                BinaryData = null;
            }
            BinaryData = Convert.FromBase64String(binarye.Value);

            // try to detect type, this will detect for unknown and fix for wrongly set
            if (DetectContentType == null)
            {
                ContentType = ContentTypeEnum.ContentTypeUnknown;
            }
            else
            {
                var contentType = DetectContentType(BinaryData);

                // if we were not able to detect type and type was not set
                if (contentType == ContentTypeEnum.ContentTypeUnknown
                    && ContentType == ContentTypeEnum.ContentTypeUnknown)
                {
                    // then we throw exception
                    throw new Exception("Unknown image content type passed");
                }

                ContentType = contentType;
            }
        }

        protected string GetXContentType()
        {
            switch (ContentType)
            {
                case ContentTypeEnum.ContentTypeJpeg:
                    return "image/jpg";
                case ContentTypeEnum.ContentTypePng:
                    return "image/png";
                case ContentTypeEnum.ContentTypeGif:
                    return "image/gif";
                case ContentTypeEnum.ContentTypeUnknown:
                default:
                    return "";
            }
        }

        public XElement ToXML()
        {
            XElement xBinary = new XElement(Fb2Const.fb2DefaultNamespace + Fb2BinaryItemName);
            xBinary.Add(new XAttribute(ContentTypeAttributeName,GetXContentType()));
            xBinary.Add(new XAttribute(IdAttributeName,Id));
            xBinary.Value=Convert.ToBase64String(BinaryData);

            return xBinary;
        }
    }
}
