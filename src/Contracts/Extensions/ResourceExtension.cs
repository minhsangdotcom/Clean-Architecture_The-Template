using System.Xml.Linq;

namespace Contracts.Extensions;

public static class ResourceExtension
{
    public static Dictionary<string, ResourceResult> ReadResxFile(string filePath)
    {
        try
        {
            // Load the .resx file as an XDocument
            XDocument resxDoc = XDocument.Load(filePath);

            // Query the XML for each <data> element, which holds key-value pairs and comments
            Dictionary<string, ResourceResult> dataElements = resxDoc
                .Root!.Elements("data")
                .Select(elem => new KeyValuePair<string, ResourceResult>(
                    elem.Attribute("name")?.Value!,
                    new ResourceResult(
                        elem.Attribute("name")?.Value!,
                        elem.Element("value")?.Value!,
                        elem.Element("comment")?.Value
                    )
                ))
                .ToDictionary();

            return dataElements;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error reading the resx file: {ex.Message}");
        }
    }
}

public record ResourceResult(string Key, string Value, string? Comment);
