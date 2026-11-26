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
    private RecipeListEntry? selectedEntry;
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

    public ObservableCollection<RecipeListEntry> Recipes { get; } = new();

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

    public RecipeListEntry? SelectedEntry
    {
        get => selectedEntry;
        set
        {
            if (ReferenceEquals(selectedEntry, value))
            {
                if (value is not null)
                {
                    value.RefreshDirtyState();
                }
                return;
            }

            if (selectedEntry is not null)
            {
                selectedEntry.RefreshDirtyState();
            }

            selectedEntry = value;
            if (selectedEntry is not null)
            {
                selectedEntry.RefreshDirtyState();
            }

            OnPropertyChanged();
            OnPropertyChanged(nameof(SelectedRecipe));
        }
    }

    public XT50Recipe? SelectedRecipe => SelectedEntry?.Recipe;

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
        var previouslySelectedPath = SelectedEntry?.FilePath;
        var paths = filePaths.ToArray();
        var recipes = reader.ReadMany(paths);

        Recipes.Clear();
        for (var index = 0; index < recipes.Count; index++)
        {
            var recipe = recipes[index];
            Recipes.Add(new RecipeListEntry(recipe, paths[index]));
        }

        SelectedEntry = Recipes.FirstOrDefault(entry =>
            string.Equals(entry.FilePath, previouslySelectedPath, StringComparison.OrdinalIgnoreCase))
            ?? Recipes.FirstOrDefault();
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
            SelectedEntry = null;
            StatusMessage = "No se encontraron perfiles FP1 en la carpeta.";
            return;
        }

        LoadProfiles(paths);
    }

    public void CreateNewRecipe()
    {
        var entry = new RecipeListEntry(XT50Recipe.CreateDefault(), null);
        Recipes.Add(entry);
        SelectedEntry = entry;
        StatusMessage = "Receta nueva: edítala y guárdala como copia para crear el archivo FP1.";
    }

    public void SaveSelectedProfileAsCopy()
    {
        if (SelectedRecipe is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ConfiguredFolder) || !Directory.Exists(ConfiguredFolder))
        {
            throw new DirectoryNotFoundException("Configura primero una carpeta válida.");
        }

        var selectedEntry = SelectedEntry!;
        selectedEntry.RefreshLabel();

        if (selectedEntry.FilePath is null)
        {
            var newRecipePath = CreateNewProfilePath(selectedEntry.Recipe.Label);
            writer.Write(selectedEntry.Recipe, newRecipePath);
            selectedEntry.SetSavedPath(newRecipePath);
            selectedEntry.MarkClean();
            StatusMessage = $"Perfil guardado: {Path.GetFileName(newRecipePath)}";
            return;
        }

        var copy = selectedEntry.Recipe.Clone();
        var filePath = CreateNewProfilePath(copy.Label);
        writer.Write(copy, filePath);
        var copyEntry = new RecipeListEntry(copy, filePath);
        copyEntry.MarkClean();
        Recipes.Add(copyEntry);
        SelectedEntry = copyEntry;
        StatusMessage = $"Perfil guardado: {Path.GetFileName(filePath)}";
    }

    public void UpdateSelectedProfile()
    {
        if (SelectedEntry is null)
        {
            return;
        }

        if (SelectedEntry.FilePath is null)
        {
            throw new InvalidOperationException("La receta todavía no tiene un archivo. Usa Guardar copia nueva para crear el FP1.");
        }

        writer.Write(SelectedEntry.Recipe, SelectedEntry.FilePath);
        SelectedEntry.MarkClean();
        StatusMessage = $"Perfil actualizado: {SelectedEntry.FileName}";
    }

    public string? GetSelectedRecipeFilePath()
    {
        return SelectedEntry?.FilePath;
    }

    public void DeleteSelectedRecipeFileAndRemove()
    {
        if (SelectedEntry is null)
        {
            return;
        }

        var removedEntry = SelectedEntry;
        if (removedEntry.FilePath is not null)
        {
            File.Delete(removedEntry.FilePath);
        }
        var removedIndex = Recipes.IndexOf(removedEntry);
        Recipes.RemoveAt(removedIndex);
        SelectedEntry = Recipes.Count == 0
            ? null
            : Recipes[Math.Min(removedIndex, Recipes.Count - 1)];
        StatusMessage = removedEntry.FilePath is null
            ? $"Borrador '{removedEntry.Label}' descartado."
            : $"{removedEntry.Label} y {removedEntry.FileName} se borraron.";
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
