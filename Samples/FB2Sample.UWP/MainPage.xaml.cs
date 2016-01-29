using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using FB2Library;

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
				try
				{
					using (var reader = new FB2Reader())
					{
						_file = await reader.LoadAsync(s);
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
			if (_file.MainBody != null)
			{
				bookInfo.Text = $"Title: {_file.MainBody.Title.TitleData[0]} {_file.MainBody.Title.TitleData[1]}";
			}
		}

		private void Close_Click(object sender, RoutedEventArgs e)
		{
			bookInfo.Text = string.Empty;
			_file = null;
			textBlock.Text = "Closed";
		}
	}
}
