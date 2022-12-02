using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using FB2Library.Elements.Poem;
using FB2Library.Elements.Table;

namespace FB2Library.Elements
{
    public class CiteItem : IFb2TextItem
    {
        internal const string Fb2CiteElementName = "cite";

        private readonly List<IFb2TextItem> _citeData = new List<IFb2TextItem>();
        private readonly List<ParagraphItem> _textAuthors = new List<ParagraphItem>();
        
        public string ID { get; set; }
        public List<IFb2TextItem> CiteData => _citeData;
        public List<ParagraphItem> TextAuthors => _textAuthors;
        public string Lang { get; set; }

        internal void Load(XElement xCite)
        {
            _citeData.Clear();
            if (xCite == null)
            {
                throw new ArgumentNullException(nameof(xCite));
            }

            if (xCite.Name.LocalName != Fb2CiteElementName)
            {
                throw new ArgumentException("Element of wrong type passed", nameof(xCite));
            }

            _textAuthors.Clear();
            IEnumerable<XElement> xItems = xCite.Elements();
            foreach (var element in xItems)
            {
                switch (element.Name.LocalName)
                {
                    case ParagraphItem.Fb2ParagraphElementName:
                        ParagraphItem paragraph = new ParagraphItem();
                        try
                        {
                            paragraph.Load(element);
                            _citeData.Add(paragraph);
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case PoemItem.Fb2PoemElementName:
                        PoemItem poem = new PoemItem();
                        try
                        {
                            poem.Load(element);
                            _citeData.Add(poem);
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case EmptyLineItem.Fb2EmptyLineElementName:
                        EmptyLineItem emptyLine = new EmptyLineItem();
                        try
                        {
                            _citeData.Add(emptyLine);
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case SubTitleItem.Fb2SubtitleElementName:
                        SubTitleItem subtitle = new SubTitleItem();
                        try
                        {
                            subtitle.Load(element);
                            _citeData.Add(subtitle);
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case TableItem.Fb2TableElementName:
                        TableItem table = new TableItem();
                        try
                        {
                            table.Load(element);
                            _citeData.Add(table);

                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case TextAuthorItem.Fb2TextAuthorElementName:
                        //ParagraphItem author = new ParagraphItem();
                        TextAuthorItem author = new TextAuthorItem();
                        try
                        {
                            author.Load(element);
                            _textAuthors.Add(author);
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    default:
                        Debug.WriteLine(
                            $"CiteItem:Load - invalid element <{element.Name.LocalName}> encountered in title .");
                        break;
                }
            }

            Lang = null;
            XAttribute xLang = xCite.Attribute(XNamespace.Xml + "lang");
            if (xLang != null && xLang.Value != null)
            {
                Lang = xLang.Value;
            }

            ID = null;
            XAttribute xID = xCite.Attribute("id");
            if (xID != null && xID.Value != null)
            {
                ID = xID.Value;
            }
            
        }

        public XNode ToXML()
        {
            XElement xCite = new XElement(Fb2Const.fb2DefaultNamespace + Fb2CiteElementName);
            if (!string.IsNullOrEmpty(ID))
            {
                xCite.Add(new XAttribute("id", ID));
            }
            if (!string.IsNullOrEmpty(Lang))
            {
                xCite.Add(new XAttribute(XNamespace.Xml + "lang", Lang));
            }

            foreach (IFb2TextItem citeItem in _citeData)
            {
                xCite.Add(citeItem.ToXML());
            }
            foreach (ParagraphItem authorItem in _textAuthors)
            {
                xCite.Add(authorItem.ToXML());
            }

            return xCite;
        }
    }
}
