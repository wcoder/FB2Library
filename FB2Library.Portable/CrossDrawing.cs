using FB2Library.Portable;
using System;

namespace FB2Library
{
	public class CrossDrawing
	{
		static Lazy<IDrawing> Implementation = new Lazy<IDrawing>(() => CreateDrawing(), System.Threading.LazyThreadSafetyMode.PublicationOnly);

		/// <summary>
		/// Current settings to use
		/// </summary>
		public static IDrawing Current
		{
			get
			{
				var ret = Implementation.Value;
				if (ret == null)
				{
					throw NotImplementedInReferenceAssembly();
				}
				return ret;
			}
		}

		static IDrawing CreateDrawing()
		{
#if PORTABLE
        return null;
#else
			return new DrawingImplementation();
#endif
		}

		internal static Exception NotImplementedInReferenceAssembly()
		{
			return new NotImplementedException("This functionality is not implemented in the portable version of this assembly.  You should reference the NuGet package from your main application project in order to reference the platform-specific implementation.");
		}
	}
}
