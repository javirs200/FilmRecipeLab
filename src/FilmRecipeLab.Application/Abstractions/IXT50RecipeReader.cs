using FilmRecipeLab.Domain.Models;

namespace FilmRecipeLab.Application.Abstractions;

public interface IXT50RecipeReader
{
    XT50Recipe Read(Stream source);

    XT50Recipe Read(string filePath);

    IReadOnlyList<XT50Recipe> ReadMany(IEnumerable<string> filePaths);
}
