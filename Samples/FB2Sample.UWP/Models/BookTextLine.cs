using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FB2Sample.UWP.Models
{
	public class BookTextLine : BookLine
	{
		private string _text;

		public BookTextLine(string text)
		{
			_text = text;
		}

		public override UIElement ToView()
		{
			return new TextBlock
			{
				Text = _text
			};
		}
	}
}
