using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FB2Library.Elements;

namespace FB2Library.HeaderItems
{
    public class CoverPage
    {
        internal const string Fb2CoverPageImageElementName = "coverpage";

        private readonly List<InlineImageItem> _coverImages = new List<InlineImageItem>();

        private XNamespace _fileNameSpace = XNamespace.None;

        protected string GetElementName()
        {
            return Fb2CoverPageImageElementName;
        }

        public List<InlineImageItem> CoverpageImages => _coverImages;

        public bool HasImages() => _coverImages.Count > 0;

        /// <summary>
        /// XML namespace used to read the document
        /// </summary>
        public XNamespace Namespace
        {
            set { _fileNameSpace = value; }
            get { return _fileNameSpace; }
        }


        internal void Load(XElement xCoverPage)
        {
            if (xCoverPage == null)
            {
                throw new ArgumentNullException(nameof(xCoverPage));
            }

            if (xCoverPage.Name.LocalName != GetElementName())
            {
                throw new ArgumentException("Element of wrong type passed", nameof(xCoverPage));
            }

            _coverImages.Clear();
            IEnumerable<XElement> xImages = xCoverPage.Elements(_fileNameSpace +InlineImageItem.Fb2InlineImageElementName);
            foreach (var xImage in xImages)
            {
                InlineImageItem image = new InlineImageItem();
                try
                {
                    image.Load(xImage);
                    _coverImages.Add(image);
                }
                catch (Exception)
                {
                    // ignore
                }
            }
        }
        
        public XElement ToXML()
        {
            XElement xCover = new XElement(Fb2Const.fb2DefaultNamespace + Fb2CoverPageImageElementName);
            foreach (InlineImageItem imageItem in _coverImages)
            {
                xCover.Add(imageItem.ToXML());
            }

            return xCover;
        }
    }
}