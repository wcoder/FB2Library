namespace FB2Library.Elements.Poem
{
    public class VPoemParagraph : ParagraphItem
    {
        internal const string Fb2VParagraphItemName = "v";

        protected override string GetElementName()
        {
            return Fb2VParagraphItemName;
        }
    }
}
