using FilmRecipeLab.Domain.Models;

namespace FilmRecipeLab.Application.Abstractions;

public interface IXT50RecipeWriter
{
    void Write(XT50Recipe recipe, Stream destination);

    void Write(XT50Recipe recipe, string filePath);
}
