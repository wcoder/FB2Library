using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;
using System.Xml;
using FB2Library;
using FB2Library.Elements;


Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var filePath = Path.Combine("..", "..", "..", "..", "..", "files", "test.fb2");

await using var fileStream = new FileStream(filePath, FileMode.Open);

var readerSettings = new XmlReaderSettings
{
    DtdProcessing = DtdProcessing.Ignore
};
var loadSettings = new XmlLoadSettings(readerSettings);

BinaryItem.DetectContentType = binaryData =>
{
    try
    {
        using var imgStream = new MemoryStream(binaryData);

        // For MacOS need native libgdiplus, more:
        // https://docs.microsoft.com/en-us/dotnet/core/compatibility/core-libraries/6.0/system-drawing-common-windows-only#recommended-action
#pragma warning disable CA1416
        using var bitmap = new Bitmap(imgStream);

        var rawFormat = bitmap.RawFormat;

        if (rawFormat.Equals(ImageFormat.Jpeg))
            return ContentTypeEnum.ContentTypeJpeg;

        if (rawFormat.Equals(ImageFormat.Png))
            return ContentTypeEnum.ContentTypePng;

        if (rawFormat.Equals(ImageFormat.Gif))
            return ContentTypeEnum.ContentTypeGif;

        return ContentTypeEnum.ContentTypeUnknown;
        
#pragma warning restore CA1416
    }
    catch (Exception ex)
    {
        throw new Exception($"Error during image type detection: {ex}", ex);
    }
};


var reader = new FB2Reader();
var file = await reader.ReadAsync(fileStream, loadSettings);


Console.WriteLine("Done");