using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using FB2Library.Elements.Poem;

namespace FB2Library.Elements
{
    public class EpigraphItem : IFb2TextItem
    {
        internal const string Fb2EpigraphElementName = "epigraph";

        private readonly List<IFb2TextItem> _epigraphData = new List<IFb2TextItem>();
        private readonly List<IFb2TextItem> _textAuthors = new List<IFb2TextItem>();

        public string ID { get; set; }
        public List<IFb2TextItem> EpigraphData => _epigraphData;
        public List<IFb2TextItem> TextAuthors => _textAuthors;

        internal void Load(XElement xEpigraph)
        {
            _epigraphData.Clear();
            if (xEpigraph == null)
            {
                throw new ArgumentNullException(nameof(xEpigraph));
            }
            if (xEpigraph.Name.LocalName != Fb2EpigraphElementName)
            {
                throw new ArgumentException("Element of wrong type passed", nameof(xEpigraph));
            }

            IEnumerable<XElement> xItems = xEpigraph.Elements();
            _textAuthors.Clear();
            foreach (var element in xItems)
            {
                switch (element.Name.LocalName)
                {
                    case ParagraphItem.Fb2ParagraphElementName:
                        ParagraphItem paragraph = new ParagraphItem();
                        try
                        {
                            paragraph.Load(element);
                            _epigraphData.Add(paragraph);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to load paragraph: {ex.Message}.");
                        }
                        break;
                    case PoemItem.Fb2PoemElementName:
                        PoemItem poem = new PoemItem();
                        try
                        {
                            poem.Load(element);
                            _epigraphData.Add(poem);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to load poem: {ex.Message}.");
                        }
                        break;
                    case CiteItem.Fb2CiteElementName:
                        CiteItem cite = new CiteItem();
                        try
                        {
                            cite.Load(element);
                            _epigraphData.Add(cite);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to load citation: {ex.Message}.");
                        }
                        break;
                    case EmptyLineItem.Fb2EmptyLineElementName:
                        EmptyLineItem emptyLine = new EmptyLineItem();
                        try
                        {
                            _epigraphData.Add(emptyLine);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to load empty line: {ex.Message}.");
                        }
                        break;
                    case TextAuthorItem.Fb2TextAuthorElementName:
                        TextAuthorItem author = new TextAuthorItem();
                        //SimpleText author = new SimpleText();
                        try
                        {
                            author.Load(element);
                            _textAuthors.Add(author);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"Failed to load text author: {ex.Message}.");
                        }
                        break;
                    default:
                        Debug.WriteLine(
                            $"EpigraphItem:Load - invalid element <{element.Name.LocalName}> encountered in title.");
                        break;
                }
            }

            XAttribute xID = xEpigraph.Attribute("id");
            if (xID != null && xID.Value != null)
            {
                ID = xID.Value;
            }
        }

        public XNode ToXML()
        {
            XElement xEpigraph = new XElement(Fb2Const.fb2DefaultNamespace + Fb2EpigraphElementName);
            if (!string.IsNullOrEmpty(ID))
            {
                xEpigraph.Add(new XAttribute("id", ID));
            }

            foreach (IFb2TextItem epigrafItem in _epigraphData)
            {
                xEpigraph.Add(epigrafItem.ToXML());
            }
            foreach (IFb2TextItem epigrafAuthor in _textAuthors)
            {
                xEpigraph.Add(epigrafAuthor.ToXML());
            }

            return xEpigraph;
        }
    }
}
