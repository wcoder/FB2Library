using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FB2Library.Elements
{
    public class EmptyLineItem : IFb2TextItem
    {
        internal const string Fb2EmptyLineElementName = "empty-line";

        public override string ToString()
        {
            return "\n";
        }

    }
}
