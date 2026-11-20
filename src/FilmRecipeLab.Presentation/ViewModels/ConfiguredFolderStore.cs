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
        var defaultRoot = GetDefaultRootFolderPath();
        var defaultPath = GetDefaultFolderPath();

        if (File.Exists(settingsFilePath))
        {
            var configuredPath = File.ReadAllText(settingsFilePath).Trim();
            if (Directory.Exists(configuredPath))
            {
                if (PathsEqual(configuredPath, defaultRoot) && Directory.Exists(defaultPath))
                {
                    Save(defaultPath);
                    return defaultPath;
                }

                return configuredPath;
            }
        }

        if (!Directory.Exists(defaultPath))
        {
            return null;
        }

        Save(defaultPath);
        return defaultPath;
    }

    public void Save(string folderPath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(settingsFilePath)!);
        File.WriteAllText(settingsFilePath, folderPath);
    }

    private static string GetDefaultFolderPath()
    {
        return Path.Combine(
            GetDefaultRootFolderPath(),
            "X-T50",
            "X-T50_0100");
    }

    private static string GetDefaultRootFolderPath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "com.fujifilm.denji",
            "X_RAW_STUDIO");
    }

    private static bool PathsEqual(string first, string second)
    {
        return string.Equals(
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(first)),
            Path.TrimEndingDirectorySeparator(Path.GetFullPath(second)),
            StringComparison.OrdinalIgnoreCase);
    }
}
