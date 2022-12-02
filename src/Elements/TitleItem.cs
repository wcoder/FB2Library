using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    public class TitleItem : IFb2TextItem
    {
        internal const string Fb2TitleElementName = "title";

        private readonly List<IFb2TextItem> _titleData = new List<IFb2TextItem>();

        public List<IFb2TextItem> TitleData => _titleData;

        /// <summary>
        /// Language attribute
        /// </summary>
        public string LangAttribute { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            foreach (var textItem in TitleData)
            {
                builder.Append(textItem.ToString());
                builder.Append(" ");
            }
            return builder.ToString();
        }

        internal void Load(XElement xTitle)
        {
            _titleData.Clear();
            if (xTitle == null)
            {
                throw new ArgumentNullException(nameof(xTitle));
            }

            if (xTitle.Name.LocalName != Fb2TitleElementName)
            {
                throw new ArgumentException("Element of wrong type passed", nameof(xTitle));
            }

            IEnumerable<XElement> subElements = xTitle.Elements();
            foreach (var element in subElements)
            {
                switch (element.Name.LocalName)
                {
                    case EmptyLineItem.Fb2EmptyLineElementName:
                        _titleData.Add(new EmptyLineItem());
                        break;
                    case ParagraphItem.Fb2ParagraphElementName:
                        ParagraphItem paragraph = new ParagraphItem();
                        try
                        {
                            paragraph.Load(element);
                            _titleData.Add(paragraph);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to load paragraph: {ex.Message}.");
                        }
                        break;
                    default:
                        Debug.WriteLine(
                            $"TitleItem:Load - invalid element <{element.Name.LocalName}> encountered in title .");
                        break;
                }
            }

            LangAttribute = null;
            XAttribute xLang = xTitle.Attribute(XNamespace.Xml + "lang");
            if (xLang != null && !string.IsNullOrEmpty(xLang.Value))
            {
                LangAttribute = xLang.Value;
            }
        }
        public XNode ToXML()
        {
            XElement xTitle = new XElement(Fb2Const.fb2DefaultNamespace + Fb2TitleElementName);
            if (!string.IsNullOrEmpty(LangAttribute))
            {
                xTitle.Add(new XAttribute(XNamespace.Xml + "lang", LangAttribute));
            }

            foreach (IFb2TextItem titleItem in _titleData)
            {
                xTitle.Add(titleItem.ToXML());
            }
            return xTitle;
        }
    }
}
