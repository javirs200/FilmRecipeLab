using System.Windows;
using FilmRecipeLab.Domain.Models;
using FilmRecipeLab.Presentation.ViewModels;
using Microsoft.Win32;

namespace FilmRecipeLab.Presentation;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static IReadOnlyList<string> FilmSimulationCodes => XT50Recipe.FilmSimulationCodes.ToArray();

    public static IReadOnlyList<string> WhiteBalanceCodes => XT50Recipe.WhiteBalanceCodes.ToArray();

    public static IReadOnlyList<string> DynamicRanges { get; } = new[] { "100", "200", "400" };

    public static IReadOnlyList<string> ColorSpaces { get; } = new[] { "sRGB", "AdobeRGB" };

    public static IReadOnlyList<string> GrainEffects { get; } = new[] { "OFF", "STRONG", "WEAK" };

    public static IReadOnlyList<string> GrainSizes { get; } = new[] { "SMALL", "LARGE" };

    private readonly MainWindowViewModel viewModel = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = viewModel;
        viewModel.ScanConfiguredFolderIfAvailable();
    }

    private void ChooseFolder_Click(object sender, RoutedEventArgs e)
    {
        var dialog = new OpenFolderDialog
        {
            Title = "Seleccionar carpeta de perfiles FP1",
            Multiselect = false
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

    private void SaveProfile_Click(object sender, RoutedEventArgs e)
    {
        if (viewModel.SelectedRecipe is null)
        {
            MessageBox.Show(this, "Selecciona primero un perfil.", "Guardar perfil", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            viewModel.SaveSelectedProfile();
        }
        catch (Exception exception)
        {
            MessageBox.Show(this, exception.Message, "No se pudo guardar el perfil", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}