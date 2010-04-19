﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Linq;
//Добавить для атрибута style и ID засунуть вместо текста SimplText; 
namespace FB2Library.Elements.Table
{
    public enum TableAlignmentsEnum
    {
        Left,
        Right,
        Center,
    }

    public enum TableVAlignmentsEnum
    {
        Top,
        Middle,
        Bottom,
    }

    public interface ICellElement
    {
        string Text { set; get; }
        TableAlignmentsEnum Align { get; set; }
        TableVAlignmentsEnum VAlign { get; set; }
        int? ColSpan { get; set; }
        int? RowSpan { get; set; }
        XElement ToXML();
    }

    public class BaseCellElement : ICellElement
    {
        #region ICellElement Members

        private TableAlignmentsEnum align = TableAlignmentsEnum.Left;
        private TableVAlignmentsEnum vAlign = TableVAlignmentsEnum.Top;

        public string Text{set; get;}

        protected virtual string GetElementName()
        {
            return "";
        }

        public TableAlignmentsEnum Align
        {
            get { return align; }
            set { align = value; }
        }

        public TableVAlignmentsEnum VAlign
        {
            get { return vAlign; }
            set { vAlign = value; }
        }

        public int? ColSpan {get; set; }

        public int? RowSpan { get; set; }

        #endregion

        protected void LoadAlign(XElement xCell)
        {
            XAttribute xAlign = xCell.Attribute("align");
            if ((xAlign != null) && (xAlign.Value != null))
            {
                switch (xAlign.Value)
                {
                    case "left":
                        align = TableAlignmentsEnum.Left;
                        break;
                    case "right":
                        align = TableAlignmentsEnum.Right;
                        break;
                    case "center":
                        align = TableAlignmentsEnum.Center;
                        break;
                    default:
                        Debug.Fail(string.Format("TableRowItem.Load - Unknown alignment {0}"), xAlign.Value);
                        break;
                }
            }
         }

        protected void LoadVAlign(XElement xCell)
        {
            XAttribute xVAlign = xCell.Attribute("valign");
            if ((xVAlign != null) && (xVAlign.Value != null))
            {
                switch (xVAlign.Value)
                {
                    case "top":
                        vAlign = TableVAlignmentsEnum.Top;
                        break;
                    case "middle":
                        vAlign = TableVAlignmentsEnum.Middle;
                        break;
                    case "bottom":
                        vAlign = TableVAlignmentsEnum.Bottom;
                        break;
                    default:
                        Debug.Fail(string.Format("TableRowItem.Load - Unknown alignment {0}"), xVAlign.Value);
                        break;
                }
            }
        }

        protected string GetAlign()
        {
            switch (align)
            {
                case TableAlignmentsEnum.Right:
                    return "right";
                case TableAlignmentsEnum.Center:
                    return "center";
                default:
                    return "left";
            }
        }

        protected string GetVAlign()
        {
            switch (vAlign)
            {
                case TableVAlignmentsEnum.Middle:
                    return "middle";
                case TableVAlignmentsEnum.Bottom:
                    return "bottom";
                default:
                    return "top";
            }
        }

        public XElement ToXML()
        {
            XElement xCell = new XElement(Fb2Const.fb2DefaultNamespace + GetElementName(), Text);
            xCell.Add(new XAttribute("align", GetAlign()));
            xCell.Add(new XAttribute("valign", GetVAlign()));
            if (ColSpan != null)
            {
                xCell.Add(new XAttribute("colspan", ColSpan.ToString()));
            }
            if (RowSpan != null)
            {
                xCell.Add(new XAttribute("rowspan", RowSpan.ToString()));
            }

            return xCell;
        }
    }
}
