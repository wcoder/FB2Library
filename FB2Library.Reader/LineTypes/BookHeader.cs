namespace FB2Library.Reader.LineTypes
{
	public struct BookHeader : IBaseLine
	{
		public byte HeaderLevel { get; set; }
		public string Text { get; set; }
	}
}
