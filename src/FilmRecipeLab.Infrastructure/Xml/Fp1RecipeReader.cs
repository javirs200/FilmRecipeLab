using System.Xml.Linq;
using FilmRecipeLab.Application.Abstractions;
using FilmRecipeLab.Domain.Models;

namespace FilmRecipeLab.Infrastructure.Xml;

public sealed class Fp1RecipeReader : IXT50RecipeReader
{
    public XT50Recipe Read(Stream source)
    {
        ArgumentNullException.ThrowIfNull(source);

        var document = XDocument.Load(source, LoadOptions.PreserveWhitespace);
        var root = document.Root ?? throw new InvalidDataException("The FP1 document has no root element.");
        if (root.Name != "ConversionProfile")
        {
            throw new InvalidDataException("The FP1 document must have a ConversionProfile root element.");
        }

        var group = root.Element("PropertyGroup")
            ?? throw new InvalidDataException("The FP1 document has no PropertyGroup element.");
        var device = group.Attribute("device")?.Value
            ?? throw new InvalidDataException("The FP1 document has no camera device.");
        var profileVersion = group.Attribute("version")?.Value ?? string.Empty;
        var label = group.Attribute("label")?.Value ?? string.Empty;
        var properties = group.Elements()
            .ToDictionary(element => element.Name.LocalName, element => (string?)element.Value, StringComparer.Ordinal);

        return new XT50Recipe(profileVersion, device, label, properties);
    }

    public XT50Recipe Read(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var source = File.OpenRead(filePath);
        return Read(source);
    }

    public IReadOnlyList<XT50Recipe> ReadMany(IEnumerable<string> filePaths)
    {
        ArgumentNullException.ThrowIfNull(filePaths);

        return filePaths.Select(Read).ToArray();
    }
}
