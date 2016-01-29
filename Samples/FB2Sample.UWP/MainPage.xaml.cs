using FB2Library.Portable;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage.Pickers;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;

namespace FB2Sample.UWP
{
	public sealed partial class MainPage : Page
	{
		private FB2File _file;

		public MainPage()
		{
			InitializeComponent();

			_file = new FB2File();
		}

		private async void Choose_Click(object sender, RoutedEventArgs e)
		{
			var file = await CreatePicker().PickSingleFileAsync();
			if (file != null)
			{
				textBlock.Text = "Picked file: " + file.Name;

				await OpenFileAsync(file);
			}
			else
			{
				textBlock.Text = "Operation cancelled.";
			}
		}

		private FileOpenPicker CreatePicker()
		{
			var picker = new FileOpenPicker();
			picker.ViewMode = PickerViewMode.Thumbnail;
			picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
			picker.FileTypeFilter.Add(".fb2");
			return picker;
		}

		private async Task OpenFileAsync(StorageFile file)
		{
			using (var s = await file.OpenStreamForReadAsync())
			{
				ReadFB2FileStream(s);
			}

			PrepareFile();
		}

		private void ReadFB2FileStream(Stream s)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				DtdProcessing = DtdProcessing.Ignore
			};
			XDocument fb2Document = null;
			using (XmlReader reader = XmlReader.Create(s, settings))
			{
				fb2Document = XDocument.Load(reader, LoadOptions.PreserveWhitespace);
			}
			
			try
			{
				if (_file != null) _file = new FB2File();
				_file.Load(fb2Document, false);
			}
			catch (Exception ex)
			{
				bookInfo.Text = string.Format("Error loading file : {0}", ex.Message);
			}
		}

		private void PrepareFile()
		{
			if (_file.MainBody != null)
			{
				bookInfo.Text = $"Title: {_file.MainBody.Title.TitleData[0]} {_file.MainBody.Title.TitleData[1]}";
			}
		}
	}
}
