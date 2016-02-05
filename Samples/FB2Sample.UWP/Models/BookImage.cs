using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FB2Sample.UWP.Models
{
	public class BookImage : BookLine
	{
		private byte[] _data;

		public BookImage(byte[] data)
		{
			_data = data;
		}

		public override UIElement ToView()
		{
			return new Image();
		}
	}
}
