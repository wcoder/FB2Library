﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    public class SequenceType
    {
        private const string NameAttributeName = "name";
        private const string NumberAttributeName = "number";

        public const string SequenceElementName = "sequence";

        private readonly List<SequenceType> _content = new List<SequenceType>();

        /// <summary>
        /// XML namespace used to read the document
        /// </summary>
        public XNamespace Namespace { get; set; } = XNamespace.None;

        /// <summary>
        /// Get/Set name of the sequence
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get/Set sequence number
        /// </summary>
        public int? Number { get; set; }

        /// <summary>
        /// Get/Set language
        /// </summary>
        public string Language { get; set; }

        public void Load(XElement xElement)
        {
            if (xElement == null)
            {
                throw new ArgumentNullException(nameof(xElement));
            }
            if (xElement.Name.LocalName != SequenceElementName)
            {
                throw new ArgumentException("Element of wrong type passed", nameof(xElement));
            }

            // read all subsequences
            _content.Clear();
            foreach (var element in xElement.Elements(Namespace + SequenceElementName))
            {
                var subElement = new SequenceType();
                try
                {
                    subElement.Load(element);
                    _content.Add(subElement);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error reading sequence element: {ex.Message}");
                }
            }

            // read "name" attribute
            Name = null;
            XAttribute xName = xElement.Attribute(NameAttributeName);
            if (xName != null && !string.IsNullOrEmpty(xName.Value))
            {
                Name = xName.Value;
            }
            else
            {
                throw new Exception("Name attribute in sequence is required!");
            }

            // read number attribute
            Number = null;
            XAttribute xNumber = xElement.Attribute(NumberAttributeName);
            if (xNumber != null && !string.IsNullOrEmpty(xNumber.Value))
            {
                if (int.TryParse(xNumber.Value, out var value))
                {
                    Number = value;
                }
            }

            // read lang attribute
            Language = null;
            XAttribute xLang = xElement.Attribute(XNamespace.Xml + "lang");
            if (xLang != null && string.IsNullOrEmpty(xLang.Value))
            {
                Language = xLang.Value;
            }
        }

        public XElement ToXML()
        {
            XElement xSequence = new XElement(Fb2Const.fb2DefaultNamespace + SequenceElementName);
            if (!string.IsNullOrEmpty(Name))
            {
                xSequence.Add(new XAttribute(NameAttributeName, Name));
            }
            if (Number != null)
            { 
                xSequence.Add(new XAttribute(NumberAttributeName, Number));
            }
            if (!string.IsNullOrEmpty(Language))
            {
                xSequence.Add(new XAttribute(XNamespace.Xml + "lang", Language));
            }
            foreach (SequenceType subSequence in _content)
            {
                xSequence.Add(subSequence.ToXML());
            }

            return xSequence;
        }
    }
}
