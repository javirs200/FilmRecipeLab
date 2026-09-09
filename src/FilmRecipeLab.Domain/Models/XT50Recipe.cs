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

        FilmSimulation = GetProperty("FilmSimulation") ?? string.Empty;
        DynamicRange = GetProperty("DynamicRange") ?? string.Empty;
        ExposureBias = ParseDecimal("ExposureBias");
        HighlightTone = ParseDecimal("HighlightTone");
        ShadowTone = ParseDecimal("ShadowTone");
        Color = ParseInt("Color");
        Sharpness = ParseInt("Sharpness");
        NoiseReduction = ParseInt("NoisReduction");
        Clarity = ParseInt("Clarity");
        WhiteBalance = GetProperty("WhiteBalance") ?? string.Empty;
        WhiteBalanceShiftRed = ParseInt("WBShiftR");
        WhiteBalanceShiftBlue = ParseInt("WBShiftB");
        WhiteBalanceColorTemperature = GetProperty("WBColorTemp") ?? string.Empty;
        GrainEffect = GetProperty("GrainEffect") ?? string.Empty;
        GrainEffectSize = GetProperty("GrainEffectSize") ?? string.Empty;
        ChromeEffect = GetProperty("ChromeEffect") ?? string.Empty;
        ColorChromeBlue = GetProperty("ColorChromeBlue") ?? string.Empty;
        SmoothSkinEffect = GetProperty("SmoothSkinEffect") ?? string.Empty;
        BlackImageTone = ParseInt("BlackImageTone");
        MonochromaticColorRedGreen = ParseInt("MonochromaticColor_RG");
        LensModulationOptimization = GetProperty("LensModulationOpt") ?? string.Empty;
        ColorSpace = GetProperty("ColorSpace") ?? string.Empty;
        DigitalTeleConverter = GetProperty("DigitalTeleConv") ?? string.Empty;
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

    public string WhiteBalanceColorTemperature { get; set; }

    public string GrainEffect { get; set; }

    public string GrainEffectSize { get; set; }

    public string ChromeEffect { get; set; }

    public string ColorChromeBlue { get; set; }

    public string SmoothSkinEffect { get; set; }

    public int? BlackImageTone { get; set; }

    public int? MonochromaticColorRedGreen { get; set; }

    public string LensModulationOptimization { get; set; }

    public string ColorSpace { get; set; }

    public string DigitalTeleConverter { get; set; }

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

    private int? ParseInt(string name)
    {
        return int.TryParse(GetProperty(name), out var value) ? value : null;
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
}
