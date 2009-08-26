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
        private readonly List<EpigraphItem> epigraphs = new List<EpigraphItem>();
        private readonly List<IFb2TextItem> content = new List<IFb2TextItem>();
        private readonly List<TextAuthorItem> authors = new List<TextAuthorItem>();

        public TitleItem Title { get; set; }
        public List<IFb2TextItem> Content { get { return content; } }
        public List<TextAuthorItem> Authors { get { return authors; } }
        public List<EpigraphItem> Epigraphs { get { return epigraphs; } }
        public string Lang { get; set; }
        public string ID { set; get; }
        public DateItem Date { set; get; }



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
            XElement xTitle = xPoem.Element(xPoem.Name.Namespace + TitleItem.Fb2TitleElementName);
            if ((xTitle != null) && (xTitle.Value != null))
            {
                Title = new TitleItem();
                Title.Load(xTitle);
            }

            epigraphs.Clear();
            IEnumerable<XElement> xEpigraphs = xPoem.Elements(xPoem.Name.Namespace + EpigraphItem.Fb2EpigraphElementName);
            foreach (var epigraph in xEpigraphs)
            {
                EpigraphItem epigraphItem = new EpigraphItem();
                try
                {
                    epigraphItem.Load(epigraph);
                    epigraphs.Add(epigraphItem);
                }
                catch (Exception)
                {

                }
            }

            content.Clear();
            IEnumerable<XElement> xElements = xPoem.Elements(xPoem.Name.Namespace + StanzaItem.Fb2StanzaElementName);
            foreach (var xElement in xElements)
            {
                StanzaItem stanza = new StanzaItem();
                try
                {
                    stanza.Load(xElement);
                    content.Add(stanza);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            authors.Clear();
            IEnumerable<XElement> xAuthors = xPoem.Elements(xPoem.Name.Namespace + TextAuthorItem.Fb2TextAuthorElementName);
            foreach (var xAuthor in xAuthors)
            {
                TextAuthorItem author = new TextAuthorItem();
                try
                {
                    author.Load(xAuthor);
                    authors.Add(author);
                }
                catch (Exception)
                {
                    continue;
                }
            }

            Date = null;
            XElement xDate = xPoem.Element(xPoem.Name.Namespace + DateItem.Fb2DateElementName);
            if (xDate != null)
            {
                Date = new DateItem();
                try
                {
                    Date.Load(xDate);
                }
                catch (Exception)
                {
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
    }
}