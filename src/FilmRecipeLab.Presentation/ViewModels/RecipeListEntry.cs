using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using FilmRecipeLab.Domain.Models;

namespace FilmRecipeLab.Presentation.ViewModels;

public sealed class RecipeListEntry : INotifyPropertyChanged
{
    private XT50Recipe cleanRecipe;
    private bool hasChanges;

    public RecipeListEntry(XT50Recipe recipe, string filePath)
    {
        Recipe = recipe;
        FilePath = filePath;
        cleanRecipe = recipe.Clone();
        Recipe.PropertyChanged += (_, _) => RefreshDirtyState();
    }

    public XT50Recipe Recipe { get; }

    public string FilePath { get; }

    public string FileName => Path.GetFileName(FilePath);

    public string Label => Recipe.Label;

    public bool HasChanges
    {
        get => hasChanges;
        private set
        {
            if (hasChanges == value)
            {
                return;
            }

            hasChanges = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public void RefreshLabel()
    {
        OnPropertyChanged(nameof(Label));
        RefreshDirtyState();
    }

    public void RefreshDirtyState()
    {
        HasChanges = !HasSameEditableState(cleanRecipe, Recipe);
    }

    public void MarkClean()
    {
        cleanRecipe = Recipe.Clone();
        RefreshLabel();
        RefreshDirtyState();
    }

    private static bool HasSameEditableState(XT50Recipe first, XT50Recipe second)
    {
        var firstState = $"{first.Label}|{first.FilmSimulation}|{first.DynamicRange}|{first.ExposureBias}|{first.HighlightTone}|{first.ShadowTone}|{first.Color}|{first.Sharpness}|{first.NoiseReduction}|{first.Clarity}|{first.WhiteBalance}|{first.WhiteBalanceShiftRed}|{first.WhiteBalanceShiftBlue}|{first.WhiteBalanceColorTemperature}|{first.GrainEffect}|{first.GrainEffectSize}|{first.ChromeEffect}|{first.ColorChromeBlue}|{first.SmoothSkinEffect}|{first.BlackImageTone}|{first.MonochromaticColorRedGreen}|{first.LensModulationOptimization}|{first.ColorSpace}|{SerializeProperties(first)}";
        var secondState = $"{second.Label}|{second.FilmSimulation}|{second.DynamicRange}|{second.ExposureBias}|{second.HighlightTone}|{second.ShadowTone}|{second.Color}|{second.Sharpness}|{second.NoiseReduction}|{second.Clarity}|{second.WhiteBalance}|{second.WhiteBalanceShiftRed}|{second.WhiteBalanceShiftBlue}|{second.WhiteBalanceColorTemperature}|{second.GrainEffect}|{second.GrainEffectSize}|{second.ChromeEffect}|{second.ColorChromeBlue}|{second.SmoothSkinEffect}|{second.BlackImageTone}|{second.MonochromaticColorRedGreen}|{second.LensModulationOptimization}|{second.ColorSpace}|{SerializeProperties(second)}";
        return string.Equals(firstState, secondState, StringComparison.Ordinal);
    }

    private static string SerializeProperties(XT50Recipe recipe)
    {
        return string.Join("\u001f", recipe.Properties
            .OrderBy(property => property.Key, StringComparer.Ordinal)
            .Select(property => $"{property.Key}={property.Value}"));
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
