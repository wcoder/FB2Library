using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FB2Sample.UWP.Models
{
	public class BookHeader : BookLine
	{
		private byte _headerLevel;
		private string _text;

		public BookHeader(string text, byte headerLevel = 0)
		{
			_headerLevel = headerLevel;
			_text = text;
		}

		public override UIElement ToView()
		{
			return new TextBlock
			{
				FontSize = 18,
				FontWeight = new FontWeight { Weight = 700 },
				Text = _text
			};
		}
	}
}
