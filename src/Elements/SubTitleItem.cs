namespace FB2Library.Elements
{
    public class SubTitleItem : ParagraphItem
    {
        internal const string Fb2SubtitleElementName = "subtitle";

        protected override string GetElementName()
        {
            return Fb2SubtitleElementName;
        }
    }
}
