using System.Collections.Generic;
using System.Xml.Linq;
using FB2Library.Elements;

namespace FB2Library.HeaderItems
{
    public class ItemInfoBase
    {
        protected readonly List<SequenceType> ItemSequences = new List<SequenceType>();

        protected XNamespace FileNameSpace = XNamespace.None;


        /// <summary>
        /// Get list of sequences
        /// </summary>
        public List<SequenceType> Sequences { get { return ItemSequences; } }

        /// <summary>
        /// XML namespace used to read the document
        /// </summary>
        public XNamespace Namespace
        {
            set { FileNameSpace = value; }
            get { return FileNameSpace; }
        }

        /// <summary>
        /// Book Title
        /// </summary>
        public TextFieldType BookTitle { set; get; }

    }
}
