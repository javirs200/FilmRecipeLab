using System.IO;
using System.Windows;
using System.Windows.Input;
using FilmRecipeLab.Domain.Models;
using FilmRecipeLab.Presentation.ViewModels;
using Microsoft.Win32;

namespace FilmRecipeLab.Presentation;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static string RawStudioRootFolder => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "com.fujifilm.denji",
        "X_RAW_STUDIO");

    public static IReadOnlyList<FilmSimulationOption> FilmSimulations { get; } =
    [
        new("Provia", "Provia (Standard)"),
        new("Velvia", "Velvia (Vivid)"),
        new("Astia", "Astia (Soft)"),
        new("Classic", "Classic Chrome"),
        new("Reala", "Reala Ace"),
        new("ClassicNEGA", "Classic Negative"),
        new("NostalgicNEGA", "Nostalgic Neg."),
        new("Eterna", "Eterna (Cinema)"),
        new("BleachBypass", "Eterna Bleach Bypass"),
        new("NEGAhi", "PRO Neg. Hi"),
        new("NEGAStd", "PRO Neg. Std"),
        new("Acros", "Acros (Standard)"),
        new("AcrosR", "Acros Red"),
        new("AcrosYe", "Acros Yellow"),
        new("AcrosG", "Acros Green"),
        new("BW", "Monochrome (Standard)"),
        new("BR", "Monochrome Red"),
        new("BYe", "Monochrome Yellow"),
        new("BG", "Monochrome Green"),
        new("Sepia", "Sepia")
    ];

    public static IReadOnlyList<string> WhiteBalanceCodes => XT50Recipe.WhiteBalanceCodes.ToArray();

    public static IReadOnlyList<string> DynamicRanges { get; } = new[] { "100", "200", "400" };

    public static IReadOnlyList<string> ColorSpaces { get; } = new[] { "sRGB", "AdobeRGB" };

    public static IReadOnlyList<string> GrainEffects { get; } = new[] { "OFF", "STRONG", "WEAK" };

    public static IReadOnlyList<string> GrainSizes { get; } = new[] { "SMALL", "LARGE" };

    public static string ApplicationVersion =>
        $"v{typeof(MainWindow).Assembly.GetName().Version?.ToString(2) ?? throw new InvalidOperationException("La aplicación no tiene una versión configurada.")}";

    private readonly MainWindowViewModel viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        Title = $"FilmRecipe Lab {ApplicationVersion}";
        DataContext = viewModel;
        viewModel.ScanConfiguredFolderIfAvailable();
    }

    private void ChooseFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Seleccionar carpeta de perfiles FP1",
            Multiselect = false,
            InitialDirectory = Directory.Exists(RawStudioRootFolder) ? RawStudioRootFolder : null
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        try
        {
            viewModel.SetConfiguredFolder(dialog.FolderName);
            viewModel.ScanConfiguredFolder();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "No se pudo configurar la carpeta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SaveCopy_Click(object sender, RoutedEventArgs e)
    {
        if (viewModel.SelectedRecipe is null)
        {
            MessageBox.Show(this, "Selecciona primero un perfil.", "Guardar copia", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            viewModel.SaveSelectedProfileAsCopy();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "No se pudo guardar la copia", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void NewRecipe_Click(object sender, RoutedEventArgs e)
    {
        viewModel.CreateNewRecipe();
    }

    private void ReloadFolder_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            viewModel.ScanConfiguredFolder();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "No se pudo recargar la carpeta", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void UpdateProfile_Click(object sender, RoutedEventArgs e)
    {
        ConfirmAndUpdateSelectedProfile();
    }

    private void RemoveRecipe_Click(object sender, RoutedEventArgs e)
    {
        ConfirmAndDeleteSelectedRecipe();
    }

    private void RecipeList_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Delete)
        {
            return;
        }

        ConfirmAndDeleteSelectedRecipe();
        e.Handled = true;
    }

    private void ConfirmAndDeleteSelectedRecipe()
    {
        var recipe = viewModel.SelectedRecipe;
        var filePath = viewModel.GetSelectedRecipeFilePath();
        if (recipe is null)
        {
            return;
        }

        var message = filePath is null
            ? $"¿Quieres descartar el borrador '{recipe.Label}'?"
            : $"¿Quieres borrar la receta '{recipe.Label}' y su archivo?\n\n{filePath}\n\nEsta acción no se puede deshacer.";
        var confirmation = MessageBox.Show(
            this,
            message,
            "Confirmar borrado",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (confirmation != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            viewModel.DeleteSelectedRecipeFileAndRemove();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "No se pudo borrar el perfil", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void ConfirmAndUpdateSelectedProfile()
    {
        var recipe = viewModel.SelectedRecipe;
        var filePath = viewModel.GetSelectedRecipeFilePath();
        if (recipe is null)
        {
            return;
        }

        if (filePath is null)
        {
            MessageBox.Show(this, "Esta receta todavía no tiene un archivo. Usa 'Guardar copia nueva' para crear su primer FP1.", "Receta sin guardar", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var confirmation = MessageBox.Show(
            this,
            $"¿Quieres actualizar '{recipe.Label}' sobrescribiendo este archivo?\n\n{filePath}",
            "Confirmar actualización",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning,
            MessageBoxResult.No);

        if (confirmation != MessageBoxResult.Yes)
        {
            return;
        }

        try
        {
            viewModel.UpdateSelectedProfile();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "No se pudo actualizar el perfil", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}