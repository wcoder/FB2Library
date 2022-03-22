using System.Xml;
using FB2Library;

var filePath = Path.Combine("..", "..", "..", "..", "files", "test.fb2");
await using var fileStream = new FileStream(filePath, FileMode.Open);

var readerSettings = new XmlReaderSettings
{
    DtdProcessing = DtdProcessing.Ignore
};
var loadSettings = new XmlLoadSettings(readerSettings);
var reader = new FB2Reader();
var file = await reader.ReadAsync(fileStream, loadSettings);

Console.WriteLine("Done");