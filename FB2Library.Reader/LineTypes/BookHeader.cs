namespace FB2Library.Reader.LineTypes
{
	public class BookHeader : IBaseLine
	{
		public byte HeaderLevel { get; set; }
		public string Text { get; set; }


		//public override UIElement ToView()
		//{
		//	return new TextBlock
		//	{
		//		FontSize = 18,
		//		FontWeight = new FontWeight { Weight = 700 },
		//		Text = _text
		//	};
		//}
	}
}
