using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace FB2Library.Elements.Poem
{
    public class PoemItem : IFb2TextItem
    {
        private readonly List<EpigraphItem> _epigraphs = new List<EpigraphItem>();
        private readonly List<IFb2TextItem> _content = new List<IFb2TextItem>();
        private readonly List<TextAuthorItem> _authors = new List<TextAuthorItem>();

        public TitleItem Title { get; set; }
        public List<IFb2TextItem> Content => _content;
        public List<TextAuthorItem> Authors => _authors;
        public List<EpigraphItem> Epigraphs => _epigraphs;
        public string Lang { get; set; }
        public string ID { get; set; }
        public DateItem Date { get; set; }

        internal const string Fb2PoemElementName = "poem";

        internal void Load(XElement xPoem)
        {
            if (xPoem == null)
            {
                throw new ArgumentNullException("xPoem");
            }

            if (xPoem.Name.LocalName != Fb2PoemElementName)
            {
                throw new ArgumentException("Element of wrong type passed", "xPoem");
            }

            Title = null;
            Date = null;
            _epigraphs.Clear();
            _content.Clear();
            _authors.Clear();

            IEnumerable<XElement> xElements = xPoem.Elements();
            foreach (var xElement in xElements)
            {
                if (xElement.Name == (XName)(xPoem.Name.Namespace + StanzaItem.Fb2StanzaElementName))
                {
                    StanzaItem stanza = new StanzaItem();
                    try
                    {
                        stanza.Load(xElement);
                        _content.Add(stanza);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                else if (xElement.Name == (XName)(xPoem.Name.Namespace + SubTitleItem.Fb2SubtitleElementName))
                {
                    SubTitleItem subtitle = new SubTitleItem();
                    try
                    {
                        subtitle.Load(xElement);
                        _content.Add(subtitle);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                else if (xElement.Name == (XName)(xPoem.Name.Namespace + TitleItem.Fb2TitleElementName) && Title == null) // only one title
                {
                        Title = new TitleItem();
                        Title.Load(xElement);
                }
                else if (xElement.Name == (XName)(xPoem.Name.Namespace + EpigraphItem.Fb2EpigraphElementName))
                {
                    EpigraphItem epigraphItem = new EpigraphItem();
                    try
                    {
                        epigraphItem.Load(xElement);
                        _epigraphs.Add(epigraphItem);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                else if (xElement.Name == (XName)(xPoem.Name.Namespace + TextAuthorItem.Fb2TextAuthorElementName))
                {
                    TextAuthorItem author = new TextAuthorItem();
                    try
                    {
                        author.Load(xElement);
                        _authors.Add(author);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
                else if (xElement.Name == (XName)(xPoem.Name.Namespace + DateItem.Fb2DateElementName) && Date == null) // only one date
                {
                    Date = new DateItem();
                    try
                    {
                        Date.Load(xElement);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }

            XAttribute xID = xPoem.Attribute("id");
            if ((xID != null) && (xID.Value != null))
            {
                ID = xID.Value;
            }

            Lang = null;
            XAttribute xLang = xPoem.Attribute(XNamespace.Xml + "lang");
            if ((xLang != null) && (xLang.Value != null))
            {
                Lang = xLang.Value;
            }
        }

        public XNode ToXML()
        {
            XElement xPoem = new XElement(Fb2Const.fb2DefaultNamespace + Fb2PoemElementName);
            if (!string.IsNullOrEmpty(ID))
            {
                xPoem.Add(new XAttribute("id", ID));
            }
            if (!string.IsNullOrEmpty(Lang))
            {
                xPoem.Add(new XAttribute(XNamespace.Xml + "lang", Lang));
            }
            if (Title != null)
            {
                xPoem.Add(Title.ToXML());
            }
            foreach (EpigraphItem PoemEpigraph in Epigraphs)
            {
                xPoem.Add(PoemEpigraph.ToXML());
            }
            foreach (IFb2TextItem PoemStanza in _content)
            {
                xPoem.Add(PoemStanza.ToXML());
            }
            foreach (TextAuthorItem TextAuthor in _authors)
            { 
                xPoem.Add(TextAuthor.ToXML());
            }
            if (Date != null)
            {
                xPoem.Add(Date.ToXML());
            }

            return xPoem;
        }
    }
}