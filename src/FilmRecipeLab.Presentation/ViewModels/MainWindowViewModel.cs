using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using FilmRecipeLab.Domain.Models;
using FilmRecipeLab.Infrastructure.Xml;

namespace FilmRecipeLab.Presentation.ViewModels;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private readonly Fp1RecipeReader reader = new();
    private readonly Fp1RecipeWriter writer = new();
    private readonly ConfiguredFolderStore folderStore = new();
    private readonly Dictionary<XT50Recipe, XT50Recipe> originalRecipes = new();
    private readonly Dictionary<XT50Recipe, string> recipePaths = new();
    private XT50Recipe? selectedRecipe;
    private string configuredFolder = string.Empty;
    private string statusMessage = "Abre uno o varios perfiles FP1 para comenzar.";

    public MainWindowViewModel()
    {
        ConfiguredFolder = folderStore.Load() ?? string.Empty;
    }

    public void ScanConfiguredFolderIfAvailable()
    {
        if (string.IsNullOrWhiteSpace(ConfiguredFolder) || !Directory.Exists(ConfiguredFolder))
        {
            StatusMessage = "Selecciona una carpeta FP1 para comenzar.";
            return;
        }

        try
        {
            ScanConfiguredFolder();
        }
        catch (Exception exception)
        {
            StatusMessage = $"No se pudo escanear la carpeta: {exception.Message}";
        }
    }

    public ObservableCollection<XT50Recipe> Recipes { get; } = new();

    public string ConfiguredFolder
    {
        get => configuredFolder;
        private set
        {
            if (configuredFolder == value)
            {
                return;
            }

            configuredFolder = value;
            OnPropertyChanged();
        }
    }

    public XT50Recipe? SelectedRecipe
    {
        get => selectedRecipe;
        set
        {
            if (ReferenceEquals(selectedRecipe, value))
            {
                return;
            }

            selectedRecipe = value;
            OnPropertyChanged();
        }
    }

    public string StatusMessage
    {
        get => statusMessage;
        private set
        {
            if (statusMessage == value)
            {
                return;
            }

            statusMessage = value;
            OnPropertyChanged();
        }
    }

    public void LoadProfiles(IEnumerable<string> filePaths)
    {
        var paths = filePaths.ToArray();
        var recipes = reader.ReadMany(paths);

        Recipes.Clear();
        originalRecipes.Clear();
        recipePaths.Clear();
        for (var index = 0; index < recipes.Count; index++)
        {
            var recipe = recipes[index];
            Recipes.Add(recipe);
            originalRecipes[recipe] = recipe.Clone();
            recipePaths[recipe] = paths[index];
        }

        SelectedRecipe = Recipes.FirstOrDefault();
        StatusMessage = $"{Recipes.Count} perfil(es) cargado(s).";
    }

    public void SetConfiguredFolder(string folderPath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(folderPath);

        var fullPath = Path.GetFullPath(folderPath);
        if (!Directory.Exists(fullPath))
        {
            throw new DirectoryNotFoundException($"La carpeta no existe: {fullPath}");
        }

        folderStore.Save(fullPath);
        ConfiguredFolder = fullPath;
        StatusMessage = $"Carpeta configurada: {fullPath}";
    }

    public void ScanConfiguredFolder()
    {
        if (string.IsNullOrWhiteSpace(ConfiguredFolder) || !Directory.Exists(ConfiguredFolder))
        {
            throw new DirectoryNotFoundException("Configura primero una carpeta válida.");
        }

        var paths = Directory.EnumerateFiles(ConfiguredFolder, "*.FP1", SearchOption.TopDirectoryOnly)
            .OrderBy(path => path, StringComparer.CurrentCultureIgnoreCase)
            .ToArray();

        if (paths.Length == 0)
        {
            Recipes.Clear();
            SelectedRecipe = null;
            StatusMessage = "No se encontraron perfiles FP1 en la carpeta.";
            return;
        }

        LoadProfiles(paths);
    }

    public void SaveSelectedProfile()
    {
        if (SelectedRecipe is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ConfiguredFolder) || !Directory.Exists(ConfiguredFolder))
        {
            throw new DirectoryNotFoundException("Configura primero una carpeta válida.");
        }

        var editedRecipe = SelectedRecipe;
        var filePath = originalRecipes.ContainsKey(editedRecipe)
            ? CreateNewProfilePath(editedRecipe.Label)
            : recipePaths[editedRecipe];
        writer.Write(editedRecipe, filePath);

        var recipeIndex = Recipes.IndexOf(editedRecipe);
        if (recipeIndex >= 0 && originalRecipes.TryGetValue(editedRecipe, out var originalRecipe))
        {
            Recipes[recipeIndex] = originalRecipe;
            Recipes.Insert(recipeIndex + 1, editedRecipe);
            originalRecipes.Remove(editedRecipe);
            recipePaths[originalRecipe] = recipePaths[editedRecipe];
            recipePaths[editedRecipe] = filePath;
            SelectedRecipe = null;
            SelectedRecipe = editedRecipe;
        }

        StatusMessage = $"Perfil guardado: {Path.GetFileName(filePath)}";
    }

    private string CreateNewProfilePath(string label)
    {
        var safeLabel = string.Join("_", label.Split(Path.GetInvalidFileNameChars(), StringSplitOptions.RemoveEmptyEntries)).Trim();
        if (string.IsNullOrWhiteSpace(safeLabel))
        {
            safeLabel = "Recipe";
        }

        var basePath = Path.Combine(ConfiguredFolder, safeLabel + ".FP1");
        if (!File.Exists(basePath))
        {
            return basePath;
        }

        for (var suffix = 2; ; suffix++)
        {
            var candidate = Path.Combine(ConfiguredFolder, $"{safeLabel} ({suffix}).FP1");
            if (!File.Exists(candidate))
            {
                return candidate;
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
