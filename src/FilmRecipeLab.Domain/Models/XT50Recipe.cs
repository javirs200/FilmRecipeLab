using System.Globalization;

namespace FilmRecipeLab.Domain.Models;

public sealed class XT50Recipe
{
    private readonly Dictionary<string, string?> properties;

    public const string SupportedDevice = "X-T50";

    public XT50Recipe(
        string profileVersion,
        string device,
        string label,
        IReadOnlyDictionary<string, string?> properties)
    {
        if (!string.Equals(device, SupportedDevice, StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException($"Only {SupportedDevice} recipes are supported.", nameof(device));
        }

        ProfileVersion = profileVersion;
        Device = device;
        Label = label;
        this.properties = new Dictionary<string, string?>(properties, StringComparer.Ordinal);

        FilmSimulation = GetRequiredProperty("FilmSimulation");
        DynamicRange = GetRequiredProperty("DynamicRange");
        ExposureBias = ParseExposureBias(GetProperty("ExposureBias"));
        HighlightTone = ParseDecimal("HighlightTone");
        ShadowTone = ParseDecimal("ShadowTone");
        Color = ParseInt("Color");
        Sharpness = ParseInt("Sharpness");
        NoiseReduction = ParseInt("NoisReduction");
        Clarity = ParseInt("Clarity");
        WhiteBalance = GetRequiredProperty("WhiteBalance");
        WhiteBalanceShiftRed = ParseInt("WBShiftR");
        WhiteBalanceShiftBlue = ParseInt("WBShiftB");
        WhiteBalanceColorTemperature = ParseColorTemperature(GetProperty("WBColorTemp"));
        GrainEffect = GetRequiredProperty("GrainEffect");
        GrainEffectSize = GetRequiredProperty("GrainEffectSize");
        ChromeEffect = GetRequiredProperty("ChromeEffect");
        ColorChromeBlue = GetRequiredProperty("ColorChromeBlue");
        SmoothSkinEffect = GetRequiredProperty("SmoothSkinEffect");
        BlackImageTone = ParseInt("BlackImageTone");
        MonochromaticColorRedGreen = ParseInt("MonochromaticColor_RG");
        LensModulationOptimization = GetRequiredProperty("LensModulationOpt");
        ColorSpace = GetRequiredProperty("ColorSpace");
        DigitalTeleConverter = "OFF";
    }

    public string ProfileVersion { get; }

    public string Device { get; }

    public string Label { get; set; }

    public string FilmSimulation { get; set; }

    public string DynamicRange { get; set; }

    public decimal? ExposureBias { get; set; }

    public decimal? HighlightTone { get; set; }

    public decimal? ShadowTone { get; set; }

    public int? Color { get; set; }

    public int? Sharpness { get; set; }

    public int? NoiseReduction { get; set; }

    public int? Clarity { get; set; }

    public string WhiteBalance { get; set; }

    public int? WhiteBalanceShiftRed { get; set; }

    public int? WhiteBalanceShiftBlue { get; set; }

    public int? WhiteBalanceColorTemperature { get; set; }

    public string GrainEffect { get; set; }

    public string GrainEffectSize { get; set; }

    public string ChromeEffect { get; set; }

    public string ColorChromeBlue { get; set; }

    public string SmoothSkinEffect { get; set; }

    public int? BlackImageTone { get; set; }

    public int? MonochromaticColorRedGreen { get; set; }

    public string LensModulationOptimization { get; set; }

    public string ColorSpace { get; set; }

    public string DigitalTeleConverter { get; }

    public IReadOnlyDictionary<string, string?> Properties => properties;

    public string? GetProperty(string name)
    {
        return properties.TryGetValue(name, out var value) ? value : null;
    }

    public void SetProperty(string name, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        properties[name] = value;
    }

    public static readonly IReadOnlySet<string> FilmSimulationCodes = new HashSet<string>(StringComparer.Ordinal)
    {
        "Provia", "Velvia", "Astia", "Classic", "Reala", "NEGAhi", "NEGAStd",
        "ClassicNEGA", "NostalgicNEGA", "Eterna", "BleachBypass", "Acros", "AcrosYe",
        "AcrosG", "AcrosR", "BW", "BYe", "BG", "Sepia"
    };

    public static readonly IReadOnlySet<string> WhiteBalanceCodes = new HashSet<string>(StringComparer.Ordinal)
    {
        "INVALID", "Auto", "Auto_Ambience", "Auto_White", "Custom1", "Custom2", "Custom3",
        "Daylight", "FLight1", "FLight2", "FLight3", "Incand", "Shade", "Temperature", "UWater"
    };

    private string GetRequiredProperty(string name)
    {
        return GetProperty(name) ?? string.Empty;
    }

    private int? ParseInt(string name)
    {
        return int.TryParse(GetProperty(name), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value)
            ? value
            : null;
    }

    private decimal? ParseDecimal(string name)
    {
        return decimal.TryParse(
            GetProperty(name),
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out var value)
            ? value
            : null;
    }

    private static decimal? ParseExposureBias(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var numericValue))
        {
            return numericValue;
        }

        var sign = value[0] switch
        {
            'P' => 1m,
            'M' => -1m,
            _ => 0m
        };

        if (sign == 0m || value.Length < 4 || value[2] != 'P')
        {
            return null;
        }

        return decimal.TryParse(value[1..2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var major)
            && decimal.TryParse(value[3..], NumberStyles.Integer, CultureInfo.InvariantCulture, out var minor)
            ? sign * (major + minor / 100m)
            : null;
    }

    private static int? ParseColorTemperature(string? value)
    {
        if (string.IsNullOrWhiteSpace(value) || !value.EndsWith('K'))
        {
            return null;
        }

        return int.TryParse(value[..^1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var kelvin)
            ? kelvin
            : null;
    }
}
