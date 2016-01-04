using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using FB2Library.Elements;
using System.Globalization;


namespace FB2Library.HeaderItems
{
    /// <summary>
    /// Information about this particular (xml) document
    /// </summary>
    public class ItemDocumentInfo
    {
        private const string ProgramUsedElementName = "program-used";
        private const string SourceURLElementName = "src-url";
        private const string SourceOCRElementName = "src-ocr";
        private const string IdElementName = "id";
        private const string VersionElementName = "version";
        private const string HistoryElementName = "history";


        private readonly List<AuthorType> _documentAuthors = new List<AuthorType>();
        private readonly List<AuthorType> _documentPublishers = new List<AuthorType>();
        private readonly List<string> _sourceUrLs = new List<string>();

        private XNamespace _fileNameSpace = XNamespace.None;

        private string _id = string.Empty;

        /// <summary>
        /// XML namespace used to read the document
        /// </summary>
        public XNamespace Namespace
        {
            set { _fileNameSpace = value; }
            get { return _fileNameSpace; }
        }

        /// <summary>
        /// Author of the original (online) document, if this is a conversion
        /// </summary>
        public TextFieldType SourceOCR { get; set; }

        /// <summary>
        /// Source URL if this document is a conversion of some other (online) document
        /// </summary>
        public IEnumerable<string> SourceURLs
        {
            get { return this._sourceUrLs;  }
        }

        /// <summary>
        ///  List of document authors,  
        /// note this is NOT book authors but document creators 
        /// </summary>
        public List<AuthorType> DocumentAuthors
        {
            get { return this._documentAuthors; }
        }

        /// <summary>
        /// Owners of the fb2 document copyrights
        /// </summary>
        public List<AuthorType> DocumentPublishers
        {
            get { return this._documentPublishers; }
        }

        /// <summary>
        /// Text specifiying program used to create the document
        /// </summary>
        public TextFieldType ProgramUsed2Create { set; get; }

        /// <summary>
        /// Date of the document creation
        /// </summary>
        public DateItem DocumentDate { set; get; }

        /// <summary>
        /// Document's UID
        /// </summary>
        public string ID
        {
            get
            {
                if (!string.IsNullOrEmpty(_id))
                {
                    return _id.ToLower();
                }
                return _id;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _id = value.ToLower();
                }
                else
                {
                    _id = value;
                }
            }
        }

        /// <summary>
        /// Document's version
        /// </summary>
        public float? DocumentVersion { get; set; }

        /// <summary>
        /// Document changes history
        /// </summary>
        public AnnotationType History { get; set; }


        internal void Load(XElement xDocumentInfo)
        {
            if (xDocumentInfo == null)
            {
                throw new ArgumentNullException("xDocumentInfo");
            }


            // Load document authors 
            _documentAuthors.Clear();
            IEnumerable<XElement> xAuthors = xDocumentInfo.Elements(_fileNameSpace + AuthorType.AuthorElementName);
            if ( xAuthors != null )
            {
                foreach ( XElement xAuthor in xAuthors)
                {
                    AuthorItem author = new AuthorItem{ Namespace = _fileNameSpace };
                    try
                    {
                        author.Load(xAuthor);
                        _documentAuthors.Add(author);
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail(string.Format("Error reading document authors : {0}",ex.Message));
                        continue;
                    }
                }
            }

            // load Program used to create
            ProgramUsed2Create = null;
            XElement xProgramUsed = xDocumentInfo.Element(_fileNameSpace + ProgramUsedElementName);
            if (xProgramUsed != null) 
            {
                ProgramUsed2Create = new TextFieldType();
                try
                {
                    ProgramUsed2Create.Load(xProgramUsed);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading program used to create : {0}", ex.Message));
                }
            }

            // Load creation date 
            DocumentDate = null;
            XElement xDate = xDocumentInfo.Element(_fileNameSpace + DateItem.Fb2DateElementName);
            if (xDate != null)
            {
                DocumentDate = new DateItem();
                try
                {
                    DocumentDate.Load(xDate);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading document date : {0}", ex.Message));
                }
            }

            // Load Source URLs
            _sourceUrLs.Clear();
            IEnumerable<XElement> xSrcURLs = xDocumentInfo.Elements(_fileNameSpace + SourceURLElementName);
            if ( xSrcURLs != null )
            {
                foreach ( XElement xSrcURL in xSrcURLs )
                {
                    if ( (xSrcURL != null) && (xSrcURL.Value != null) )
                    {
                        string srcURL = xSrcURL.Value;
                        _sourceUrLs.Add(srcURL);
                    }
                }
            }

            // Load SourceOCR
            SourceOCR = null;
            XElement xSrcOcr = xDocumentInfo.Element(_fileNameSpace + SourceOCRElementName);
            if  (xSrcOcr != null)
            {
                SourceOCR = new TextFieldType();
                try
                {
                    SourceOCR.Load(xSrcOcr);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading source OCR : {0}", ex.Message));
                }
            }

            // load document's ID
            ID = null;
            XElement xID = xDocumentInfo.Element(_fileNameSpace + IdElementName);
            if ( (xID != null) && (xID.Value != null) )
            {
                ID = xID.Value;
            }

            // load document's version
            DocumentVersion = null;
            XElement xVersion = xDocumentInfo.Element(_fileNameSpace + VersionElementName);
            if ( (xVersion != null) && (xVersion.Value != null))
            {
                string version = xVersion.Value;
                try
                {
                    var cult = new CultureInfo("", false);

                    DocumentVersion = float.Parse(version, cult.NumberFormat);
                }
                catch(FormatException ex)
                {
                    Debug.Fail(string.Format("Error reading document version : {0}", ex.Message));
                }
            }

            // Load change history 
            History = null;
            XElement xHistory = xDocumentInfo.Element(_fileNameSpace + HistoryElementName);
            if (xHistory != null) 
            {
                History = new AnnotationType() { ElementName = HistoryElementName };
                try
                {
                    History.Load(xHistory);
                }
                catch (Exception ex)
                {
                    Debug.Fail(string.Format("Error reading document history : {0}", ex.Message));
                }
            }

            // Load copyright owners
            _documentPublishers.Clear();
            IEnumerable<XElement> xPublishers = xDocumentInfo.Elements(_fileNameSpace + AuthorType.PublisherElementName);
            if ( xPublishers != null )
            {
                foreach (XElement xPublisher in xPublishers )
                {
                    PublisherItem publisher = new PublisherItem { Namespace = _fileNameSpace };
                    try
                    {
                        publisher.Load(xPublisher);
                        _documentPublishers.Add(publisher);
                    }
                    catch (Exception ex)
                    {
                        Debug.Fail(string.Format("Error reading document publishers : {0}", ex.Message));
                        continue;
                    }
                }
            }
        }

        public XElement ToXML()
        {
            var xDocumentInfo = new XElement(Fb2Const.fb2DefaultNamespace + "document-info");
            foreach(AuthorType author in _documentAuthors)
            {
                xDocumentInfo.Add(author.ToXML());
            }
            if(ProgramUsed2Create!=null)
            {
                xDocumentInfo.Add(ProgramUsed2Create.ToXML(ProgramUsedElementName));
            }
            if(DocumentDate!=null)
            {
                xDocumentInfo.Add(DocumentDate.ToXML());
            }
            foreach(string srcUrl in _sourceUrLs)
            {
                xDocumentInfo.Add(new XElement(Fb2Const.fb2DefaultNamespace + SourceURLElementName, srcUrl));
            }
            if(SourceOCR!=null)
            {
                xDocumentInfo.Add(SourceOCR.ToXML(SourceOCRElementName));
            }
            if(!string.IsNullOrEmpty(ID))
            {
                xDocumentInfo.Add(new XElement(Fb2Const.fb2DefaultNamespace + IdElementName, ID));
            }
            if(DocumentVersion!=null)
            {
                var cult = new CultureInfo("", false);
                xDocumentInfo.Add(new XElement(Fb2Const.fb2DefaultNamespace + VersionElementName,string.Format(cult.NumberFormat,"{0:F}",DocumentVersion)));
            }
            if(History!=null)
            {
                xDocumentInfo.Add(History.ToXML());
            }
            foreach(AuthorType publish in _documentPublishers)
            {
                xDocumentInfo.Add(publish.ToXML());
            }

            return xDocumentInfo;
        
        }
    } //class 
}
