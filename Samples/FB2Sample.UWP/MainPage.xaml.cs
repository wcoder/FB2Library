using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using FB2Library;
using FB2Library.Reader;
using FB2Library.Reader.LineTypes;
using Windows.UI.Text;

namespace FB2Sample.UWP
{
	public sealed partial class MainPage : Page
	{
		private FB2File _file;
		private IEnumerable<IBaseLine> _lines;

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
			loading.Visibility = Visibility.Visible;

			using (var s = await file.OpenStreamForReadAsync())
			{
				var reader = new FB2Reader();
				try
				{
					_file = await reader.LoadAsync(s);
					_lines = await reader.ReadAsync(_file);

					//DisplayLines();
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

		private void DisplayLines()
		{
			foreach (var line in _lines)
			{
				if (line is BookHeader)
				{
					bookContent.Children.Add(new TextBlock
					{
						FontWeight = new FontWeight { Weight = 700 },
						Text = ((BookHeader)line).Text
					});
				}
				else if (line is BookTextLine)
				{
					bookContent.Children.Add(new TextBlock { Text = ((BookTextLine)line).Text });
				}
				else if (line is BookImage)
				{
					var image = ((BookImage)line);
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
