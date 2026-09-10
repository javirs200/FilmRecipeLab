using System.IO;

namespace FilmRecipeLab.Presentation.ViewModels;

public sealed class ConfiguredFolderStore
{
    private readonly string settingsFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        "FilmRecipeLab",
        "profiles-folder.txt");

    public string? Load()
    {
        if (!File.Exists(settingsFilePath))
        {
            return null;
        }

        var path = File.ReadAllText(settingsFilePath).Trim();
        return Directory.Exists(path) ? path : null;
    }

    public void Save(string folderPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath)!);
        File.WriteAllText(settingsFilePath, folderPath);
    }
}
