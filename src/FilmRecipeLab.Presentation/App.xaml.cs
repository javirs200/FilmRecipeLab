using System.IO;
using System.Windows;
using System.Windows.Threading;

namespace FilmRecipeLab.Presentation;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : System.Windows.Application
{
	public App()
	{
		DispatcherUnhandledException += HandleUnhandledException;
	}

	private static void HandleUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
	{
		var logPath = Path.Combine(Path.GetTempPath(), "FilmRecipeLab-startup-error.txt");
		File.WriteAllText(logPath, e.Exception.ToString());
		MessageBox.Show($"FilmRecipe Lab no pudo iniciarse.\n\n{e.Exception.Message}\n\nDetalles: {logPath}", "Error de inicio", MessageBoxButton.OK, MessageBoxImage.Error);
		e.Handled = true;
		Current.Shutdown(1);
	}
}

