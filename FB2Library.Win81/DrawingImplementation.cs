using FB2Library.Portable;
using System;
using System.Linq;
using FB2Library.Elements;
using System.IO;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;

namespace FB2Library
{
	public class DrawingImplementation : IDrawing
	{
		public ContentTypeEnum GetImageType(MemoryStream stream)
		{
			// TODO: fix it
			var t = Task.Factory.StartNew(async () => await BitmapDecoder.CreateAsync(stream.AsRandomAccessStream()));
			t.ConfigureAwait(false);
			t.Wait();

			var ct = t.Result.Result.DecoderInformation.MimeTypes.First();

			switch (ct)
			{
				case "image/jpg":
					return ContentTypeEnum.ContentTypeJpeg;
				case "image/png":
					return ContentTypeEnum.ContentTypePng;
				case "image/gif":
					return ContentTypeEnum.ContentTypeGif;
				default:
					return ContentTypeEnum.ContentTypeUnknown;
			}
		}
	}
}
