namespace FB2Library.Elements
{
    public class TextAuthorItem : ParagraphItem
    {
        internal const string Fb2TextAuthorElementName = "text-author";

        protected override string GetElementName()
        {
            return Fb2TextAuthorElementName;
        }
    }
}