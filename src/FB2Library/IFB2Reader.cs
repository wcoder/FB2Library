using System.IO;
using System.Threading.Tasks;

namespace FB2Library
{
	public interface IFB2Reader
	{
		/// <summary>
		/// Read Fb2 file from Stream.
		/// </summary>
		/// <param name="stream">Fb2 file as a stream.</param>
		/// <param name="settings">Settings for reading fb2 file.</param>
		/// <returns></returns>
		Task<FB2File> ReadAsync(Stream stream, XmlLoadSettings settings);

		/// <summary>
		/// Read Fb2 file from string.
		/// </summary>
		/// <param name="xml">Fb2 file content as a string.</param>
		/// <returns></returns>
		Task<FB2File> ReadAsync(string xml);
	}
}
