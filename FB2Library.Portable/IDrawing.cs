using FB2Library.Elements;
using System.IO;

namespace FB2Library.Portable
{
	public interface IDrawing
	{
		ContentTypeEnum GetImageType(MemoryStream stream);
	}
}
