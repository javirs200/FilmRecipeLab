using System.Globalization;
using System.Xml.Linq;
using FilmRecipeLab.Application.Abstractions;
using FilmRecipeLab.Domain.Models;

namespace FilmRecipeLab.Infrastructure.Xml;

public sealed class Fp1RecipeWriter : IXT50RecipeWriter
{
    public void Write(XT50Recipe recipe, Stream destination)
    {
        ArgumentNullException.ThrowIfNull(recipe);
        ArgumentNullException.ThrowIfNull(destination);

        var document = CreateDocument(recipe);
        document.Save(destination, SaveOptions.DisableFormatting);
    }

    public void Write(XT50Recipe recipe, string filePath)
    {
        ArgumentNullException.ThrowIfNull(recipe);
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        using var destination = File.Create(filePath);
        Write(recipe, destination);
    }

    private static XDocument CreateDocument(XT50Recipe recipe)
    {
        var properties = new Dictionary<string, string?>(recipe.Properties, StringComparer.Ordinal)
        {
            ["FilmSimulation"] = recipe.FilmSimulation,
            ["DynamicRange"] = recipe.DynamicRange,
            ["ExposureBias"] = FormatExposureBias(recipe.ExposureBias),
            ["HighlightTone"] = FormatDecimal(recipe.HighlightTone),
            ["ShadowTone"] = FormatDecimal(recipe.ShadowTone),
            ["Color"] = FormatInt(recipe.Color),
            ["Sharpness"] = FormatInt(recipe.Sharpness),
            ["NoisReduction"] = FormatInt(recipe.NoiseReduction),
            ["Clarity"] = FormatInt(recipe.Clarity),
            ["WhiteBalance"] = recipe.WhiteBalance,
            ["WBShiftR"] = FormatInt(recipe.WhiteBalanceShiftRed),
            ["WBShiftB"] = FormatInt(recipe.WhiteBalanceShiftBlue),
            ["WBColorTemp"] = FormatColorTemperature(recipe.WhiteBalanceColorTemperature),
            ["GrainEffect"] = recipe.GrainEffect,
            ["GrainEffectSize"] = recipe.GrainEffectSize,
            ["ChromeEffect"] = recipe.ChromeEffect,
            ["ColorChromeBlue"] = recipe.ColorChromeBlue,
            ["SmoothSkinEffect"] = recipe.SmoothSkinEffect,
            ["BlackImageTone"] = FormatInt(recipe.BlackImageTone),
            ["MonochromaticColor_RG"] = FormatInt(recipe.MonochromaticColorRedGreen),
            ["LensModulationOpt"] = recipe.LensModulationOptimization,
            ["ColorSpace"] = recipe.ColorSpace,
            ["DigitalTeleConv"] = "OFF"
        };

        var propertyGroup = new XElement(
            "PropertyGroup",
            new XAttribute("device", recipe.Device),
            new XAttribute("version", recipe.ProfileVersion),
            new XAttribute("label", recipe.Label),
            properties.Select(property => new XElement(property.Key, property.Value ?? string.Empty)));

        return new XDocument(
            new XDeclaration("1.0", "utf-8", null),
            new XElement(
                "ConversionProfile",
                new XAttribute("application", "XRFC"),
                new XAttribute("version", "1.12.0.0"),
                propertyGroup));
    }

    private static string? FormatInt(int? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    private static string? FormatDecimal(decimal? value)
    {
        return value?.ToString(CultureInfo.InvariantCulture);
    }

    private static string? FormatColorTemperature(int? value)
    {
        return value.HasValue ? $"{value.Value.ToString(CultureInfo.InvariantCulture)}K" : null;
    }

    private static string? FormatExposureBias(decimal? value)
    {
        if (!value.HasValue)
        {
            return null;
        }

        var sign = value.Value < 0 ? "M" : "P";
        var absoluteValue = Math.Abs(value.Value);
        var major = decimal.Truncate(absoluteValue);
        var minor = decimal.Round((absoluteValue - major) * 100, 0, MidpointRounding.AwayFromZero);

        var majorText = major.ToString("0", CultureInfo.InvariantCulture);
        var minorText = minor.ToString("00", CultureInfo.InvariantCulture);
        return $"{sign}{majorText}P{minorText}";
    }
}
