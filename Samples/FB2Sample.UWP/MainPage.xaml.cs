using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using FB2Library;
using FB2Library.Reader;
using Windows.UI.Text;
using FB2Library.Reader.Interfaces;
using FB2Library.Reader.Lines;

namespace FB2Sample.UWP
{
	public sealed partial class MainPage : Page
	{
		private FB2File _file;
		private IEnumerable<ILine> _lines;

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
				var reader = new FB2Reader();
				try
				{
					var xml = await GetStringFromStreamAsync(s);
					_file = await reader.LoadAsync(xml);
					_lines = await reader.ReadAsync(_file);

					DisplayLines();
				}
				catch (Exception ex)
				{
					bookInfo.Text = string.Format("Error loading file : {0}", ex.Message);
				}
				finally
				{
					loading.Visibility = Visibility.Collapsed;
					reader.Dispose();
				}
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



		private void DisplayLines()
		{
			foreach (var line in _lines)
			{
				if (line is HeaderLine)
				{
					bookContent.Children.Add(new TextBlock
					{
						FontWeight = new FontWeight { Weight = 700 },
						Text = ((HeaderLine)line).Text
					});
				}
				else if (line is TextLine)
				{
					bookContent.Children.Add(new TextBlock { Text = ((TextLine)line).Text });
				}
				else if (line is ImageLine)
				{
					var image = ((ImageLine)line);
					bookContent.Children.Add(new Image { Width = 100, Height = 100 });
				}
			}
		}

	


		private void Close_Click(object sender, RoutedEventArgs e)
		{
			bookInfo.Text = string.Empty;
			textBlock.Text = "Closed";
			_file = null;
			_lines = null;
			bookContent.Children.Clear();
		}
	}
}
