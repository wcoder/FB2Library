using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace FB2Sample.UWP.Models
{
	public abstract class BookLine
	{
		public abstract UIElement ToView();
	}
}
