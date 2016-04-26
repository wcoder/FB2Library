using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using FB2Library;
using Windows.UI.Text;

namespace FB2Sample.UWP
{
	public sealed partial class MainPage : Page
	{
		private FB2File _file;

		public MainPage()
		{
			InitializeComponent();

			_file = new FB2File();

			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
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
			loading.Visibility = Visibility.Visible;

			using (var s = await file.OpenStreamForReadAsync())
			{
				try
				{
					var xml = await GetStringFromStreamAsync(s);
					_file = await new FB2Reader().ReadAsync(xml);

					//DisplayLines();
				}
				catch (Exception ex)
				{
					bookInfo.Text = string.Format("Error loading file : {0}", ex.Message);
				}
				finally
				{
					loading.Visibility = Visibility.Collapsed;
				}
			}
		}

		private async Task ReadFB2FileStreamAsync(Stream stream)
		{
			// setup
			var readerSettings = new XmlReaderSettings
			{
				DtdProcessing = DtdProcessing.Ignore
			};
			var loadSettings = new XmlLoadSettings(readerSettings);

			try
			{
				// reading
				FB2File file = await new FB2Reader().ReadAsync(stream, loadSettings);
			}
			catch (Exception ex)
			{
				Debug.WriteLine(string.Format("Error loading file : {0}", ex.Message));
			}
		}


		private async Task<string> GetStringFromStreamAsync(Stream stream)
		{
			// get first line
			var firstLineBuffer = new byte[60];
			stream.Read(firstLineBuffer, 0, firstLineBuffer.Length);
			stream.Position = 0;
			var firstLine = Encoding.UTF8.GetString(firstLineBuffer);

			// create reader
			StreamReader streamReader;
			if (firstLine.Contains("windows-1251"))
			{
				streamReader = new StreamReader(stream, Encoding.GetEncoding(1251));
			}
			else
			{
				streamReader = new StreamReader(stream, true);
			}

			var xml = await streamReader.ReadToEndAsync();

			streamReader.Dispose();

			return xml;
		}



		//private void DisplayLines()
		//{
		//	foreach (var line in _lines)
		//	{
		//		if (line is HeaderLine)
		//		{
		//			bookContent.Children.Add(new TextBlock
		//			{
		//				FontWeight = new FontWeight { Weight = 700 },
		//				Text = ((HeaderLine)line).Text
		//			});
		//		}
		//		else if (line is TextLine)
		//		{
		//			bookContent.Children.Add(new TextBlock { Text = ((TextLine)line).Text });
		//		}
		//		else if (line is ImageLine)
		//		{
		//			var image = ((ImageLine)line);
		//			bookContent.Children.Add(new Image { Width = 100, Height = 100 });
		//		}
		//	}
		//}

	


		private void Close_Click(object sender, RoutedEventArgs e)
		{
			bookInfo.Text = string.Empty;
			textBlock.Text = "Closed";
			_file = null;
			//_lines = null;
			bookContent.Children.Clear();
		}
	}
}
