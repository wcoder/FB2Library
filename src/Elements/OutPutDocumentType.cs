using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    public class OutPutDocumentType : IShareInstructionElement
    {
        public const string OutputDocumentElementName = "output-document-class";

        private readonly List<ShareInstructionType> _parts = new List<ShareInstructionType>();

        private XNamespace _fileNameSpace = XNamespace.None;

        /// <summary>
        /// XML namespace used to read the document
        /// </summary>
        public XNamespace Namespace
        {
            set { _fileNameSpace = value; }
            get { return _fileNameSpace; }
        }

        /// <summary>
        /// Get list of the instruction parts
        /// </summary>
        public List<ShareInstructionType> Parts => _parts;

        /// <summary>
        /// Get/Set Name attribute
        /// </summary>
        public string Name { get; set;}

        /// <summary>
        /// Get/Set document part creation type
        /// </summary>
        public GenerationInstructionEnum Create { get; set; }

        /// <summary>
        /// Get/Set price
        /// </summary>
        public float? Price { get; set; }

        public void Load(XElement xElement)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }

            _parts.Clear();
            IEnumerable<XElement> xParts = xElement.Elements(_fileNameSpace + "part");
            foreach (var xPart in xParts)
            {
                ShareInstructionType part = new ShareInstructionType {Namespace = _fileNameSpace};
                try
                {
                    part.Load(xPart);
                    _parts.Add(part);
                }
                catch (Exception)
                {
                    // ignored
                }
            }

            Name = null;
            XAttribute xName = xElement.Attribute("name");
            if (xName == null)
            {
                throw new Exception("name is required attribute - absent");
            }
            Name = xName.Value;


            Create = GenerationInstructionEnum.Unknown;
            XAttribute xCreate = xElement.Attribute("create");
            if (xCreate != null)
            {
                switch (xCreate.Value)
                {
                    case "require":
                        Create = GenerationInstructionEnum.Require;
                        break;
                    case "allow":
                        Create = GenerationInstructionEnum.Allow;
                        break;
                    case "deny":
                        Create = GenerationInstructionEnum.Deny;
                        break;
                    default:
                        Debug.WriteLine($"Invalid instruction type : {xCreate.Value}");
                        break;
                }                
            }

            Price = null;
            XAttribute xPrice = xElement.Attribute("price");
            if (xPrice != null && !string.IsNullOrEmpty(xPrice.Value))
            {
                if (float.TryParse(xPrice.Value, out var val))
                {
                    Price = val;
                }
            }

        }
        private string GetXCreate()
        {
            switch (Create)
            {
                case GenerationInstructionEnum.Require:
                    return "require";
                case GenerationInstructionEnum.Allow:
                    return "allow";
                case GenerationInstructionEnum.Deny:
                    return "deny";
                default:
                    return "";
                //Debug.WriteLine(string.Format("Invalid instruction type : {0}", xIncludeAll.Value));
                //break;
            }
        }

        public XElement ToXML()
        {
            XElement xOutputDocumentClass = new XElement(Fb2Const.fb2DefaultNamespace + OutputDocumentElementName);
            xOutputDocumentClass.Add(new XAttribute("name", Name));
            if (Create != GenerationInstructionEnum.Unknown)
            {
                xOutputDocumentClass.Add(new XAttribute("create", GetXCreate()));
            }
            if (Price != null)
            {
                xOutputDocumentClass.Add(new XAttribute("price", Price.ToString()));
            }
            foreach (ShareInstructionType partElement in _parts)
            {
                xOutputDocumentClass.Add(partElement.ToXML());
            }
            return xOutputDocumentClass;
        }
    }
}
