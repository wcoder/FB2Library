using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Storage;
using FB2Library;
using System.Collections.Generic;
using FB2Library.Elements;
using FB2Library.Elements.Poem;
using FB2Sample.UWP.Models;

namespace FB2Sample.UWP
{
	public sealed partial class MainPage : Page
	{
		private FB2File _file;
		private List<BookLine> _lines;

		public MainPage()
		{
			InitializeComponent();

			_file = new FB2File();
			_lines = new List<BookLine>();
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

				try
				{
					Task.Factory.StartNew(async () =>
					{
						PrepareBodies();

						await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
						{
							foreach (var line in _lines)
							{
								BookContent.Children.Add(line.ToView());
							}
						});
					});

					
				}
				catch (Exception e)
				{
					textBlock.Text = $"Exception: {e.Message}";
				}
			}
		}

		private void PrepareBodies()
		{
			foreach (var bodyItem in _file.Bodies)
			{
				AddTitle(bodyItem.Title);

				foreach (SectionItem sectionItem in bodyItem.Sections)
				{
					PrepareTextItem(sectionItem);
				}
			}
		}

		private void PrepareTextItems(IEnumerable<IFb2TextItem> items)
		{
			foreach (var item in items)
			{
				if (item is IFb2TextItem)
				{
					PrepareTextItem(item);
				}
				else
				{
					_lines.Add(new BookTextLine(item.ToString()));
				}
			}
		}

		private void PrepareTextItem(IFb2TextItem textItem)
		{
			if (textItem is CiteItem)
			{
				PrepareTextItems(((CiteItem)textItem).CiteData);
				return;
			}

			if (textItem is PoemItem)
			{
				var item = (PoemItem)textItem;
				AddTitle(item.Title);
				PrepareTextItems(item.Content);
				return;
			}

			if (textItem is SectionItem)
			{
				var item = (SectionItem)textItem;
				AddTitle(item.Title);
				PrepareTextItems(item.Content);
				return;
			}

			if (textItem is StanzaItem)
			{
				var item = (StanzaItem)textItem;
				AddTitle(item.Title);
				PrepareTextItems(item.Lines);
				return;
			}

			if (textItem is ParagraphItem
				|| textItem is EmptyLineItem)
			{
				_lines.Add(new BookTextLine(textItem.ToString()));
				return;
			}

			if (textItem is ImageItem)
			{
				var item = (ImageItem)textItem;
				var key = item.HRef.Replace("#", string.Empty);

				if (_file.Images.ContainsKey(key))
				{
					var data = _file.Images[key].BinaryData;
					_lines.Add(new BookImage(data));
				}
				return;
			}

			throw new Exception(textItem.GetType().ToString());
		}

		

	

		private void AddTitle(TitleItem titleItem)
		{
			if (titleItem != null)
			{
				foreach (var title in titleItem.TitleData)
				{
					_lines.Add(new BookHeader(title.ToString()));
				}
			}
		}


		private void Close_Click(object sender, RoutedEventArgs e)
		{
			bookInfo.Text = string.Empty;
			_file = null;
			textBlock.Text = "Closed";
			_lines.Clear();
			BookContent.Children.Clear();
		}
	}
}
