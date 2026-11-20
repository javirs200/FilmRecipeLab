using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace FilmRecipeLab.Domain.Models;

public sealed class XT50Recipe : INotifyPropertyChanged
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

    private string label = string.Empty;
    private string filmSimulation = string.Empty;
    private string dynamicRange = string.Empty;
    private decimal? exposureBias;
    private decimal? highlightTone;
    private decimal? shadowTone;
    private int? color;
    private int? sharpness;
    private int? noiseReduction;
    private int? clarity;
    private string whiteBalance = string.Empty;
    private int? whiteBalanceShiftRed;
    private int? whiteBalanceShiftBlue;
    private int? whiteBalanceColorTemperature;
    private string grainEffect = string.Empty;
    private string grainEffectSize = string.Empty;
    private string chromeEffect = string.Empty;
    private string colorChromeBlue = string.Empty;
    private string smoothSkinEffect = string.Empty;
    private int? blackImageTone;
    private int? monochromaticColorRedGreen;
    private string lensModulationOptimization = string.Empty;
    private string colorSpace = string.Empty;

    public string Label { get => label; set => SetProperty(ref label, value); }

    public string FilmSimulation { get => filmSimulation; set => SetProperty(ref filmSimulation, value); }

    public string DynamicRange { get => dynamicRange; set => SetProperty(ref dynamicRange, value); }

    public decimal? ExposureBias { get => exposureBias; set => SetProperty(ref exposureBias, value); }

    public decimal? HighlightTone { get => highlightTone; set => SetProperty(ref highlightTone, value); }

    public decimal? ShadowTone { get => shadowTone; set => SetProperty(ref shadowTone, value); }

    public int? Color { get => color; set => SetProperty(ref color, value); }

    public int? Sharpness { get => sharpness; set => SetProperty(ref sharpness, value); }

    public int? NoiseReduction { get => noiseReduction; set => SetProperty(ref noiseReduction, value); }

    public int? Clarity { get => clarity; set => SetProperty(ref clarity, value); }

    public string WhiteBalance { get => whiteBalance; set => SetProperty(ref whiteBalance, value); }

    public int? WhiteBalanceShiftRed { get => whiteBalanceShiftRed; set => SetProperty(ref whiteBalanceShiftRed, value); }

    public int? WhiteBalanceShiftBlue { get => whiteBalanceShiftBlue; set => SetProperty(ref whiteBalanceShiftBlue, value); }

    public int? WhiteBalanceColorTemperature { get => whiteBalanceColorTemperature; set => SetProperty(ref whiteBalanceColorTemperature, value); }

    public string GrainEffect { get => grainEffect; set => SetProperty(ref grainEffect, value); }

    public string GrainEffectSize { get => grainEffectSize; set => SetProperty(ref grainEffectSize, value); }

    public string ChromeEffect { get => chromeEffect; set => SetProperty(ref chromeEffect, value); }

    public string ColorChromeBlue { get => colorChromeBlue; set => SetProperty(ref colorChromeBlue, value); }

    public string SmoothSkinEffect { get => smoothSkinEffect; set => SetProperty(ref smoothSkinEffect, value); }

    public int? BlackImageTone { get => blackImageTone; set => SetProperty(ref blackImageTone, value); }

    public int? MonochromaticColorRedGreen { get => monochromaticColorRedGreen; set => SetProperty(ref monochromaticColorRedGreen, value); }

    public string LensModulationOptimization { get => lensModulationOptimization; set => SetProperty(ref lensModulationOptimization, value); }

    public string ColorSpace { get => colorSpace; set => SetProperty(ref colorSpace, value); }

    public string DigitalTeleConverter { get; }

    public IReadOnlyDictionary<string, string?> Properties => properties;

    public XT50Recipe Clone()
    {
        var clone = new XT50Recipe(ProfileVersion, Device, Label, Properties)
        {
            FilmSimulation = FilmSimulation,
            DynamicRange = DynamicRange,
            ExposureBias = ExposureBias,
            HighlightTone = HighlightTone,
            ShadowTone = ShadowTone,
            Color = Color,
            Sharpness = Sharpness,
            NoiseReduction = NoiseReduction,
            Clarity = Clarity,
            WhiteBalance = WhiteBalance,
            WhiteBalanceShiftRed = WhiteBalanceShiftRed,
            WhiteBalanceShiftBlue = WhiteBalanceShiftBlue,
            WhiteBalanceColorTemperature = WhiteBalanceColorTemperature,
            GrainEffect = GrainEffect,
            GrainEffectSize = GrainEffectSize,
            ChromeEffect = ChromeEffect,
            ColorChromeBlue = ColorChromeBlue,
            SmoothSkinEffect = SmoothSkinEffect,
            BlackImageTone = BlackImageTone,
            MonochromaticColorRedGreen = MonochromaticColorRedGreen,
            LensModulationOptimization = LensModulationOptimization,
            ColorSpace = ColorSpace
        };

        return clone;
    }

    public string? GetProperty(string name)
    {
        return properties.TryGetValue(name, out var value) ? value : null;
    }

    public void SetProperty(string name, string? value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        properties[name] = value;
        OnPropertyChanged(name);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return;
        }

        field = value;
        OnPropertyChanged(propertyName);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
