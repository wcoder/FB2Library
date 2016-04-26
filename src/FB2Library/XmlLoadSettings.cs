using System.Xml;
using System.Xml.Linq;

namespace FB2Library
{
	public class XmlLoadSettings
	{
		public XmlReaderSettings ReaderSettings { get; }
		public LoadOptions Options { get; }

		public static XmlLoadSettings Default => new XmlLoadSettings(null);

		public XmlLoadSettings(XmlReaderSettings readerSettings, LoadOptions loadOptions = LoadOptions.PreserveWhitespace)
		{
			ReaderSettings = readerSettings;
			Options = loadOptions;
		}
	}
}
