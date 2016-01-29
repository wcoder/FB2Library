using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FB2Library
{
	public class FB2Reader : IDisposable
	{
		private XmlReaderSettings _settings;

		public FB2Reader(XmlReaderSettings settings = null)
		{
			if (settings == null)
			{
				_settings = new XmlReaderSettings
				{
					DtdProcessing = DtdProcessing.Ignore
				};
			}
			else
			{
				_settings = settings;
			}
		}

		public Task<FB2File> LoadAsync(Stream stream, LoadOptions options = LoadOptions.PreserveWhitespace)
		{
			return Task.Factory.StartNew(() =>
			{
				var file = new FB2File();
				using (var reader = XmlReader.Create(stream, _settings))
				{
					var fb2Document = XDocument.Load(reader, options);
					file.Load(fb2Document, false);
				}
				return file;
			});
		}

		public void Dispose()
		{
			_settings = null;
		}
	}
}
