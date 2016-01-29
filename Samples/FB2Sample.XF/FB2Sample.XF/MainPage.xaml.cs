using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using FB2Library;
using System.Net.Http;

namespace FB2Sample.XF
{
	public partial class MainPage : ContentPage
	{
		private FB2File _file;

		public MainPage()
		{
			InitializeComponent();
		}

		private async void DownloadClicked(object sender, EventArgs args)
		{
			textBlock.Text = "Start download...";

			using (var httpClient = new HttpClient())
			{
				var stream = await httpClient.GetStreamAsync("https://dl.dropboxusercontent.com/u/30506652/data/test.fb2");

				textBlock.Text = "Reading...";
				try
				{
					using (var reader = new FB2Reader())
					{
						_file = await reader.LoadAsync(stream);
					}
				}
				catch (Exception ex)
				{
					bookInfo.Text = string.Format("Error loading file : {0}", ex.Message);
				}
			}

			PrepareFile();
		}

		private void PrepareFile()
		{
			textBlock.Text = "Done";
			if (_file.MainBody != null)
			{
				bookInfo.Text = $"Title: {_file.MainBody.Title.TitleData[0]} {_file.MainBody.Title.TitleData[1]}";
			}
		}

		private void CloseClicked(object sender, EventArgs args)
		{
			bookInfo.Text = string.Empty;
			_file = null;
			textBlock.Text = "Closed";
		}
	}
}
