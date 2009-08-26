using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace FB2Library.Elements
{
    public class SimpleText : IFb2TextItem
    {
        private TextStyles style = TextStyles.Normal;
        private readonly List<SimpleText> subtext = new List<SimpleText>();

        public string Text { get; set; }


        public List<SimpleText> Children { get { return subtext; } }

        public TextStyles Style
        {
            get { return style; }

            set { style = value; }
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(Text))
            {
                StringBuilder builder = new StringBuilder();
                foreach (var textItem in subtext)
                {
                    builder.Append(textItem.ToString());
                    builder.Append(" ");
                }
                return builder.ToString();
            }
            else
            {
                return Text;
            }
        }


        public bool HasChildren { get { return (subtext.Count>0); }}

        public void Load(XNode xText)
        {
            subtext.Clear();
            if (xText == null)
            {
                throw new ArgumentNullException("xText");
            }
            switch (xText.NodeType)
            {
                case XmlNodeType.Text:
                    XText textNode = (XText) xText;
                    if (!string.IsNullOrEmpty(textNode.Value))
                    {
                        Text = textNode.Value;
                        style = TextStyles.Normal;
                    }
                    break;
                case XmlNodeType.Element:
                    XElement xTextElement = (XElement)xText;
                    if (xTextElement.HasElements)
                    {
                        Text = string.Empty;
                        IEnumerable<XElement> childElements = xTextElement.Elements();
                        foreach (var element in childElements)
                        {
                            SimpleText text = new SimpleText();
                            try
                            {
                                text.Load(element);
                                subtext.Add(text);
                            }
                            catch (Exception)
                            {
                                continue;
                            }
                        }
                    }
                    else
                    {
                        switch (xTextElement.Name.LocalName)
                        {
                            case "strong":
                                style = TextStyles.Strong;
                                Text = xTextElement.Value;
                                break;
                            case "emphasis":
                                style = TextStyles.Emphasis;
                                Text = xTextElement.Value;
                                break;
                            case "code":
                                style = TextStyles.Code;
                                Text = xTextElement.Value;
                                break;
                            case "sub":
                                style = TextStyles.Sub;
                                Text = xTextElement.Value;
                                break;
                            case "sup":
                                style = TextStyles.Sup;
                                Text = xTextElement.Value;
                                break;
                            default:
                                style = TextStyles.Normal;
                                Text = xTextElement.Value;                                
                                break;
                        }
                    }
                    break;
            }
        }
    }
}
