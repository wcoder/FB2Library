using System;
using System.Diagnostics;
using System.Xml.Linq;
using FB2Library.Elements;

namespace FB2Library.HeaderItems
{
    /// <summary>
    /// Information about some paper/outher published document, that was used as a source of this xml document
    /// </summary>
    public class ItemPublishInfo : ItemInfoBase
    {
        private const string BookNameElementName = "book-name";
        private const string PublisherElementName = "publisher";
        private const string CityElementName = "city";
        private const string YearElementName = "year";
        private const string ISBNElementName = "isbn";

        public const string PublishInfoElementName = "publish-info";

        /// <summary>
        /// ISBN of original book
        /// </summary>
        public TextFieldType ISBN { get; set; }

        /// <summary>
        /// Year of the original (paper) publication
        /// </summary>
        public int? Year { get; set; }

        /// <summary>
        /// City where the original (paper) book was published
        /// </summary>
        public TextFieldType City { get; set; }

        /// <summary>
        /// Original (paper) book publisher
        /// </summary>
        public TextFieldType Publisher { get; set; }

        internal void Load(XElement xPublishInfo)
        {
            if (xPublishInfo == null)
            {
                throw new ArgumentNullException(nameof(xPublishInfo));
            }

            // Load book name
            BookTitle = null;
            XElement xBookName = xPublishInfo.Element(FileNameSpace + BookNameElementName);
            if (xBookName != null)  
            {
                BookTitle = new TextFieldType();
                try
                {
                    BookTitle.Load(xBookName);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading publisher book name : {ex.Message}");
                }
            }

            // Load publisher
            Publisher = null;
            XElement xPublisher = xPublishInfo.Element(FileNameSpace + PublisherElementName);
            if (xPublisher != null)
            {
                Publisher = new TextFieldType();
                try
                {
                    Publisher.Load(xPublisher);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading publishers : {ex.Message}");
                }
            }

            // Load city 
            City = null;
            XElement xCity = xPublishInfo.Element(FileNameSpace + CityElementName);
            if (xCity != null)
            {
                City = new TextFieldType();
                try
                {
                    City.Load(xCity);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading publishers' City: {ex.Message}");
                }
            }

            // Load year 
            Year = null;
            XElement xYear = xPublishInfo.Element(FileNameSpace + YearElementName);
            if (xYear != null)
            {
                if (int.TryParse( xYear.Value, out var year))
                {
                    Year = year;
                }
            }

            // Load ISBN
            ISBN = null;
            XElement xISBN = xPublishInfo.Element(FileNameSpace + ISBNElementName);
            if (xISBN != null) 
            {
                ISBN = new TextFieldType();
                try
                {
                    ISBN.Load(xISBN);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading publishers' ISBN: {ex.Message}");
                }
            }

            // Load sequence here
            ItemSequences.Clear();
            foreach (var xSequence in xPublishInfo.Elements(FileNameSpace + SequenceType.SequenceElementName))
            {
                var sec = new SequenceType();
                try
                {
                    sec.Load(xSequence);
                    ItemSequences.Add(sec);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading publisher sequence data: {ex.Message}");
                }
            }
        }

        public XElement ToXML()
        {
            var xPublishInfo = new XElement(Fb2Const.fb2DefaultNamespace + PublishInfoElementName);

            if (BookTitle != null)
            {
                xPublishInfo.Add(BookTitle.ToXML(BookNameElementName));
            }
            if (Publisher != null)
            {
                xPublishInfo.Add(Publisher.ToXML(PublisherElementName));
            }
            if (City != null)
            {
                xPublishInfo.Add(City.ToXML(CityElementName));
            }
            if (Year != null)
            {
                xPublishInfo.Add(new XElement(Fb2Const.fb2DefaultNamespace + YearElementName, Year.ToString()));
            }
            if (ISBN != null)
            {
                xPublishInfo.Add(ISBN.ToXML(ISBNElementName));
            }
            foreach (SequenceType sec in ItemSequences)
            {
                xPublishInfo.Add(sec.ToXML());
            }

            return xPublishInfo;
        }
    }
}
