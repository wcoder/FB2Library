using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace FB2Library
{
	/// <summary>
	/// Simple class for reading Fb2 file.
	/// </summary>
	public class FB2Reader : IFB2Reader
	{
		/// <summary>
		/// Read Fb2 file from Stream.
		/// </summary>
		/// <param name="stream">Fb2 file as a stream.</param>
		/// <param name="settings">Settings for reading fb2 file.</param>
		/// <returns></returns>
		public Task<FB2File> ReadAsync(Stream stream, XmlLoadSettings settings)
		{
			if (settings == null)
				throw new ArgumentNullException(nameof(settings));

			return Task.Factory.StartNew(() =>
			{
				var file = new FB2File();
				using (var reader = XmlReader.Create(stream, settings.ReaderSettings))
				{
					var fb2Document = XDocument.Load(reader, settings.Options);
					file.Load(fb2Document, false);
				}
				return file;
			});
		}

		/// <summary>
		/// Read Fb2 file from string.
		/// </summary>
		/// <param name="xml">Fb2 file content as a string.</param>
		/// <returns></returns>
		public Task<FB2File> ReadAsync(string xml)
		{
			return Task.Factory.StartNew(() =>
			{
				var file = new FB2File();
				var fb2Document = XDocument.Parse(xml);
				file.Load(fb2Document, false);
				return file;
			});
		}
	}
}
