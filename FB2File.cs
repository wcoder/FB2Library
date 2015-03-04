using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using FB2Library.Elements;
using FB2Library.HeaderItems;

namespace FB2Library
{
    public class FB2File
    {
        private XNamespace _fileNameSpace = XNamespace.None;

        private const string TitleInfoElementName = "title-info";
        private const string SrcTitleInfoElementName = "src-title-info";
        private const string DocumentInfoElementName = "document-info";
        private const string Fb2TextDescriptionElementName = "description";
        private const string Fb2TextBodyElementName = "body";
        private const string Fb2BinaryElementName = "binary";


        private readonly ItemTitleInfo _titleInfo = new ItemTitleInfo();
        private readonly ItemTitleInfo _srcTitleInfo = new ItemTitleInfo();
        private readonly ItemDocumentInfo _documentInfo = new ItemDocumentInfo();
        private readonly ItemPublishInfo _publishInfo = new ItemPublishInfo();
        private readonly List<ItemCustomInfo> _customInfo = new List<ItemCustomInfo>();
        private BodyItem _mainBody;
        private readonly List<BodyItem> _bodiesList = new List<BodyItem>();
        private readonly Dictionary<string, BinaryItem> _binaryObjects = new Dictionary<string, BinaryItem>();
        private readonly List<StyleElement> _styles = new List<StyleElement>();
        private readonly List<ShareInstructionType> _output = new List<ShareInstructionType>();



        /// <summary>
        /// Get list of images (the image data itself) stored as binary objects
        /// </summary>
        public Dictionary<string, BinaryItem> Images { get { return _binaryObjects;} }


        /// <summary>
        /// Get list of bodies in document (including main)
        /// </summary>
        public List<BodyItem> Bodies { get { return _bodiesList;}}

        /// <summary>
        /// Get Document TitleInfo structure
        /// </summary>
        public ItemTitleInfo TitleInfo { get { return _titleInfo; } }

        /// <summary>
        /// Get Document DocumentInfo structure
        /// </summary>
        public ItemDocumentInfo DocumentInfo { get { return _documentInfo; } }

        /// <summary>
        /// Get Document SourceTitleInfo structure
        /// </summary>
        public ItemTitleInfo SourceTitleInfo { get { return _srcTitleInfo; } }


        /// <summary>
        /// Get custom added information (if available)
        /// </summary>
        public List<ItemCustomInfo> CustomInfo { get { return _customInfo; } }

        /// <summary>
        /// Get Document PublishInfo structure
        /// </summary>
        public ItemPublishInfo PublishInfo { get { return _publishInfo; } }

        /// <summary>
        /// Get main body structure (book itself)
        /// </summary>
        public BodyItem MainBody { get { return _mainBody; } }

        /// <summary>
        /// Get styles applied to the book 
        /// </summary>
        public List<StyleElement> StyleSheets { get { return _styles; } }


        /// <summary>
        /// Loads the file as data from XML data
        /// </summary>
        /// <param name="fileDocument">XML document containing the file</param>
        /// <param name="loadHeaderOnly">if true loads only header information</param>
        public void Load(XDocument fileDocument,bool loadHeaderOnly)
        {
            if (fileDocument == null)
            {
                throw new ArgumentNullException("fileDocument");
            }
            if (fileDocument.Root == null)
            {
                throw new ArgumentException("Document's root is NULL (empty document passed)");
            }


            // theoretically the namespace should be "http://www.gribuser.ru/xml/fictionbook/2.0" but just to be sure with invalid files
            _fileNameSpace = fileDocument.Root.GetDefaultNamespace();


            _styles.Clear();
            IEnumerable<XElement> xStyles = fileDocument.Elements(_fileNameSpace + StyleElement.StyleElementName).ToArray();
            // attempt to load some bad FB2 with wrong namespace
            if (!xStyles.Any())
            {
                xStyles = fileDocument.Elements(StyleElement.StyleElementName);
            }
            foreach (var style in xStyles)
            {
                var element = new StyleElement();
                try
                {
                    element.Load(style);
                    _styles.Add(element);
                }
                catch
                {
                    // ignored
                }
            }

            LoadDescriptionSection(fileDocument);

            if (!loadHeaderOnly)
            {
                XNamespace namespaceUsed = _fileNameSpace;
                // Load body elements (first is main text)
                if (fileDocument.Root != null)
                {
                    IEnumerable<XElement> xBodys = fileDocument.Root.Elements(_fileNameSpace + Fb2TextBodyElementName).ToArray();
                    // try to read some badly formatted FB2 files
                    if (!xBodys.Any())
                    {
                        namespaceUsed = "";
                        xBodys = fileDocument.Root.Elements(namespaceUsed + Fb2TextBodyElementName);
                    }
                    foreach (var body in xBodys)
                    {
                        var bodyItem = new BodyItem() { NameSpace = namespaceUsed };
                        try
                        {
                            bodyItem.Load(body);
                        }
                        catch (Exception)
                        {
                            continue;
                        }
                        _bodiesList.Add(bodyItem);
                    }
                }
                if (_bodiesList.Count > 0)
                {
                    _mainBody = _bodiesList[0];   
                }


                // Load binaries sections (currently images only)
                if (fileDocument.Root != null)
                {
                    IEnumerable<XElement> xBinaryes = fileDocument.Root.Elements(namespaceUsed + Fb2BinaryElementName).ToArray();
                    if (!xBinaryes.Any())
                    {
                        xBinaryes = fileDocument.Root.Elements(Fb2BinaryElementName);
                    }
                    foreach (var binarye in xBinaryes)
                    {
                        var item = new BinaryItem();
                        try
                        {
                            item.Load(binarye);
                        }
                        catch
                        {                       
                            continue;
                        }
                        // add just unique IDs to fix some invalid FB2s 
                        if (!_binaryObjects.ContainsKey(item.Id))
                        {
                            _binaryObjects.Add(item.Id, item);                        
                        }
                    }
                }
            }

        }

        private void LoadDescriptionSection(XDocument fileDocument, bool loadBinaryItems = true)
        {
            if (fileDocument == null)
            {
                throw new ArgumentNullException("fileDocument");
            }
            if (fileDocument.Root == null)
            {
                throw new NullReferenceException("LoadDescriptionSection: Root is null");
            }
            XElement xTextDescription = fileDocument.Root.Element(_fileNameSpace + Fb2TextDescriptionElementName);
            // attempt to load some bad FB2 with wrong namespace
            XNamespace namespaceUsed = _fileNameSpace;
            if (xTextDescription == null)
            {
                namespaceUsed = "";
                xTextDescription = fileDocument.Root.Element(Fb2TextDescriptionElementName);
            }
            if (xTextDescription != null)
            {
                // Load Title info 
                XElement xTitleInfo = xTextDescription.Element(namespaceUsed + TitleInfoElementName);
                if (xTitleInfo != null)
                {
                    _titleInfo.Namespace = namespaceUsed;
                    try
                    {
                        _titleInfo.Load(xTitleInfo);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Error reading title info : {0}", ex.Message);
                    }

                }

                // Load Src Title info 
                XElement xSrcTitleInfo = xTextDescription.Element(namespaceUsed + SrcTitleInfoElementName);
                if (xSrcTitleInfo != null)
                {
                    _srcTitleInfo.Namespace = _fileNameSpace;
                    try
                    {
                        _srcTitleInfo.Load(xSrcTitleInfo);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Error reading source title info : {0}", ex.Message);
                    }
                }

                // Load document info
                XElement xDocumentInfo = xTextDescription.Element(namespaceUsed + DocumentInfoElementName);
                if (xDocumentInfo != null)
                {
                    _documentInfo.Namespace = _fileNameSpace;
                    try
                    {
                        _documentInfo.Load(xDocumentInfo);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Error reading document info : {0}", ex.Message);
                    }
                }

                // Load publish info 
                XElement xPublishInfo = xTextDescription.Element(namespaceUsed + ItemPublishInfo.PublishInfoElementName);
                if (xPublishInfo != null)
                {
                    _publishInfo.Namespace = _fileNameSpace;
                    try
                    {
                        _publishInfo.Load(xPublishInfo);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Error reading publishing info : {0}", ex.Message);
                    }
                }

                XElement xCustomInfo = xTextDescription.Element(namespaceUsed + ItemCustomInfo.CustomInfoElementName);
                if (xCustomInfo != null)
                {
                    var custElement = new ItemCustomInfo {Namespace = _fileNameSpace};
                    try
                    {
                        custElement.Load(xCustomInfo);
                        _customInfo.Add(custElement);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Error reading custom info : {0}", ex.Message);
                    }
                }

                IEnumerable<XElement> xInstructions = xTextDescription.Elements(xTextDescription.Name.Namespace + "output");
                int outputCount = 0;
                _output.Clear();
                foreach (var xInstruction in xInstructions)
                {
                    // only two elements allowed by scheme
                    if (outputCount > 1)
                    {
                        break;
                    }
                    var outp = new ShareInstructionType { Namespace = namespaceUsed };
                    try
                    {
                        outp.Load(xInstruction);
                        _output.Add(outp);
                    }
                    catch (Exception ex)
                    {
                        Debug.Print("Error reading output instructions : {0}", ex);
                    }
                    finally
                    {
                        outputCount++;
                    }
                }

                if (loadBinaryItems && _titleInfo.Cover != null)
                {

                    foreach (InlineImageItem coverImag in _titleInfo.Cover.CoverpageImages)
                    {
                        if (string.IsNullOrEmpty(coverImag.HRef))
                        {
                            continue;
                        }
                        string coverref = coverImag.HRef.Substring(0, 1) == "#" ? coverImag.HRef.Substring(1) : coverImag.HRef;
                        IEnumerable<XElement> xBinaryes =
                            fileDocument.Root.Elements(_fileNameSpace + Fb2BinaryElementName).Where(
                                cov => ((cov.Attribute("id") != null) && (cov.Attribute("id").Value == coverref)));
                        foreach (var binarye in xBinaryes)
                        {
                            var item = new BinaryItem();
                            try
                            {
                                item.Load(binarye);
                            }
                            catch (Exception)
                            {

                                continue;
                            }
                            // add just unique IDs to fix some invalid FB2s 
                            if (!_binaryObjects.ContainsKey(item.Id))
                            {
                                _binaryObjects.Add(item.Id, item);
                            }
                        }
                    }
                }
            }
        }

        public XDocument ToXML(bool bExportHeaderOnly)
        {
            var fileDocument = new XDocument(new XDeclaration("1.0","UTF-8","no"));

            var xFictionBook = new XElement(Fb2Const.fb2DefaultNamespace+"FictionBook",
                        new XAttribute(XNamespace.Xmlns + "l", @"http://www.w3.org/1999/xlink"),
                        new XAttribute("xmlns", Fb2Const.fb2DefaultNamespace));

            fileDocument.Add(xFictionBook);

            foreach (StyleElement style in _styles)
            {
                try
                {
                    xFictionBook.Add(style.ToXML());
                }
                catch (Exception ex)
                {
                    Debug.Print("Error converting style data to XML: {0}", ex);
                }
            }

            var xDescription = new XElement(Fb2Const.fb2DefaultNamespace+ Fb2TextDescriptionElementName);
            xFictionBook.Add(xDescription);

            try
            {
                xDescription.Add(_titleInfo.ToXML(TitleInfoElementName));
            }
            catch (Exception ex)
            {
                Debug.Print("Error converting TitleInfo data to XML: {0}", ex.Message);
            }

            if (_srcTitleInfo.BookTitle!=null)
            {
                try
                {
                    xDescription.Add(_srcTitleInfo.ToXML(SrcTitleInfoElementName));
                }
                catch (Exception ex)
                {
                    Debug.Print("Error converting SrcTitleInfo data to XML: {0}", ex.Message);
                }
            }

            try
            {
                xDescription.Add(_documentInfo.ToXML());
            }
            catch (Exception ex)
            {
                Debug.Print("Error converting DocumentInfo data to XML: {0}", ex.Message);
            }

            xDescription.Add(_publishInfo.ToXML());
            foreach (ItemCustomInfo custinfo in _customInfo)
            {
                xDescription.Add(custinfo.ToXML());
            }
            foreach (ShareInstructionType outp in _output)
            {
                xDescription.Add(outp.ToXML());
            }



            if (!bExportHeaderOnly)
            {
                foreach (BodyItem bodyItem in _bodiesList)
                {
                    xFictionBook.Add(bodyItem.ToXML());
                }
                foreach (KeyValuePair<string, BinaryItem> bItem in _binaryObjects)
                {
                    xFictionBook.Add(bItem.Value.ToXML());
                }
            }
            else
            {
                foreach (InlineImageItem coverImag in _titleInfo.Cover.CoverpageImages)
                {
                    if (string.IsNullOrEmpty(coverImag.HRef))
                    {
                        continue;
                    }
                    string coverref = coverImag.HRef.Substring(0, 1) == "#" ? coverImag.HRef.Substring(1) : coverImag.HRef;
                    xFictionBook.Add(_binaryObjects[coverref].ToXML());
                }
            }

            return fileDocument;
        }

    }
}
