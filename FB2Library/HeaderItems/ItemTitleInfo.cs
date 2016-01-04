using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using FB2Library.Elements;

// TODO: make genre list instead of just string 
// TODO: add coverpage format and reading
// TODO: add sequence format and reading


namespace FB2Library.HeaderItems
{
    /// <summary>
    /// Book (as a book opposite a document) description
    /// </summary>
    public class ItemTitleInfo : ItemInfoBase
    {
        private const string GenreElementName = "genre";
        private const string BookTitleElementName = "book-title";
        private const string AnnotationElementName = "annotation";
        private const string KeywordsElementName = "keywords";
        private const string CoverPageElementName = "coverpage";
        private const string LanguageElementName = "lang";
        private const string SourceLanguageElementName = "src-lang";

        private readonly List<TitleGenreType> _genres = new List<TitleGenreType>();

        private readonly List<AuthorType> _translators = new List<AuthorType>();

        private readonly List<AuthorType> _bookAuthors = new List<AuthorType>();





        /// <summary>
        /// Translators if this is a translation
        /// </summary>
        public IEnumerable<AuthorType> Translators
        {
            get { return _translators; }
        }

        /// <summary>
        /// Book's source language if this is a translation
        /// </summary>
        public string SrcLanguage { get; set; }

        /// <summary>
        /// Keywords used by search engine
        /// </summary>
        public TextFieldType Keywords { get; set; }

        /// <summary>
        /// Genres of the item
        /// </summary>
        public IEnumerable<TitleGenreType> Genres
        {
            get { return _genres; }
        }


        /// <summary>
        /// Authors of this book
        /// </summary>
        public IEnumerable<AuthorType> BookAuthors
        {
            get { return _bookAuthors; }
        }


        /// <summary>
        /// Book's annotation
        /// </summary>
        public AnnotationItem Annotation { set; get; }

        /// <summary>
        /// Book date , can be a range like 1990-1991
        /// </summary>
        public DateItem BookDate { get; set; }

        /// <summary>
        /// Book's language
        /// </summary>
        public string Language { set; get; }

        public CoverPage Cover { get; set; }

        public void Load(XElement xTitleInfo)
        {
            if ( xTitleInfo == null )
            {
                throw new ArgumentNullException("xTitleInfo");
            }

            // Load genres
            _genres.Clear();
            IEnumerable<XElement> xGenres = xTitleInfo.Elements(FileNameSpace + GenreElementName);
            foreach ( XElement xGenre in xGenres )
            {
                if ( (xGenre != null) )
                {
                    var genre = new TitleGenreType {Genre = xGenre.Value};
                    XAttribute xMatch = xGenre.Attribute("match");
                    if (xMatch != null && !string.IsNullOrEmpty(xMatch.Value))
                    {
                        int percentage;
                        if (int.TryParse(xMatch.Value,out percentage))
                        {
                            genre.Match = percentage;
                        }
                    }
                    _genres.Add(genre);
                }
            }

            // Load authors
            _bookAuthors.Clear();
            IEnumerable<XElement> xAuthors = xTitleInfo.Elements(FileNameSpace + AuthorType.AuthorElementName);
            foreach (XElement xAuthor in xAuthors )
            {
                var author = new AuthorItem { Namespace = FileNameSpace };
                try
                {
                    author.Load(xAuthor);
                    _bookAuthors.Add(author);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading author: {0}",ex.Message));
                    continue;
                }
            }


            // Load Title
            BookTitle = null;
            XElement xBookTitle = xTitleInfo.Element(FileNameSpace + BookTitleElementName);
            if (xBookTitle != null) 
            {
                BookTitle = new TextFieldType();
                try
                {
                    BookTitle.Load(xBookTitle);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading book title: {0}", ex.Message));
                }
            }

            // Load Annotation
            Annotation = null;
            XElement xAnnotation = xTitleInfo.Element(FileNameSpace + AnnotationElementName);
            if (xAnnotation != null) 
            {
                Annotation = new AnnotationItem();
                try
                {
                    Annotation.Load(xAnnotation);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading annotation: {0}", ex.Message));
                }
            }


            // Load keywords
            Keywords = null;
            XElement xKeywords = xTitleInfo.Element(FileNameSpace + KeywordsElementName);
            if (xKeywords != null) 
            {
                Keywords    =   new TextFieldType();
                try
                {
                    Keywords.Load(xKeywords);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading keywords: {0}", ex.Message));
                }
            }

            // Load Book date
            BookDate = null;
            XElement xBookDate = xTitleInfo.Element(FileNameSpace + DateItem.Fb2DateElementName);
            if (xBookDate != null) 
            {
                BookDate = new DateItem();
                try
                {
                    BookDate.Load(xBookDate);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading book date: {0}", ex.Message));
                }
            }

            Cover = null;
            // we should load coverpage images here but no use for them as for now
            XElement xCoverPage = xTitleInfo.Element(FileNameSpace + CoverPageElementName);
            if ( xCoverPage != null)
            {
                Cover = new CoverPage{Namespace = FileNameSpace};
                try
                {
                    Cover.Load(xCoverPage);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading cover: {0}", ex.Message));
                }
            }

            // Load Language
            Language = null;
            XElement xLanguage = xTitleInfo.Element(FileNameSpace + LanguageElementName);
            if ( (xLanguage != null))
            {
                Language = xLanguage.Value;
            }
            else
            {
                Debug.Write("Language not specified in title section");                
            }

            // Load source language
            SrcLanguage = null;
            XElement xSrcLanguage = xTitleInfo.Element(FileNameSpace + SourceLanguageElementName);
            if ( (xSrcLanguage != null) )
            {
                SrcLanguage = xSrcLanguage.Value;
            }

            // Load translators 
            _translators.Clear();
            IEnumerable<XElement> xTranslators = xTitleInfo.Elements(FileNameSpace + AuthorType.TranslatorElementName);
            foreach ( XElement xTranslator in xTranslators )
            {
                var translator = new TranslatorItem { Namespace = FileNameSpace };
                try
                {
                    translator.Load(xTranslator);
                    _translators.Add(translator);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading translator: {0}", ex.Message));
                    continue;
                }
            }

            // Load sequences
            ItemSequences.Clear();
            IEnumerable<XElement> xSequences = xTitleInfo.Elements(FileNameSpace + SequenceType.SequenceElementName);
            foreach (var xSequence in xSequences)
            {
               var sec = new SequenceType{ Namespace = FileNameSpace };
                try
                {
                    sec.Load(xSequence);
                    ItemSequences.Add(sec);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading sequence data: {0}",ex.Message));
                    continue;
                }
            }
        }

        public XElement ToXML(string nameElement)
        {
            var xTitleInfo = new XElement(Fb2Const.fb2DefaultNamespace + nameElement);
            foreach (TitleGenreType genre in _genres)
            {
                try
                {
                    xTitleInfo.Add(genre.ToXML());
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error converting genre data to XML: {0}", ex.Message));
                    continue;                    
                }
            }
            foreach (AuthorType author in _bookAuthors)
            {
                try
                {
                    xTitleInfo.Add(author.ToXML());
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error converting genre data to XML: {0}", ex.Message));
                    continue;
                }
            }

            if (BookTitle != null)
            {
                xTitleInfo.Add(BookTitle.ToXML(BookTitleElementName));
            }

            if (Annotation != null)
            {
                xTitleInfo.Add(Annotation.ToXML());
            }
            if (Keywords != null)
            {
                xTitleInfo.Add(Keywords.ToXML(KeywordsElementName));
            }
            if (BookDate != null)
            {
                xTitleInfo.Add(BookDate.ToXML());
            }
            if (Cover != null)
            {
                xTitleInfo.Add(Cover.ToXML());
            }
            if (!string.IsNullOrEmpty(Language))
            {
                xTitleInfo.Add(new XElement(Fb2Const.fb2DefaultNamespace + LanguageElementName, Language));
            }
            if (!string.IsNullOrEmpty(SrcLanguage))
            {
                xTitleInfo.Add(new XElement(Fb2Const.fb2DefaultNamespace + SourceLanguageElementName, SrcLanguage));
            }
            foreach (AuthorType translat in _translators)
            {
                xTitleInfo.Add(translat.ToXML());
            }
            foreach (SequenceType sec in ItemSequences)
            {
                xTitleInfo.Add(sec.ToXML());
            }

            return xTitleInfo;
        }

    } // class
}
