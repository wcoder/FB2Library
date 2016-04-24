using FB2Library.Reader.Interfaces;

namespace FB2Library.Reader.Lines
{
	public class HeaderLine : ILine
	{
		public byte HeaderLevel { get; set; }
		public string Text { get; set; }
	}
}
