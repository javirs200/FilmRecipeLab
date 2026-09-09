using System.Text;
using FilmRecipeLab.Domain.Models;
using FilmRecipeLab.Infrastructure.Xml;

namespace FilmRecipeLab.Tests.Domain;

public sealed class Fp1RecipeReaderTests
{
    [Fact]
    public void Read_Loads_XT50_Profile_And_Properties()
    {
        const string xml = """
            <?xml version="1.0" encoding="utf-8"?>
            <ConversionProfile application="XRFC" version="1.12.0.0">
              <PropertyGroup device="X-T50" version="X-T50_0100" label="TestRecipe">
                <FilmSimulation>Provia</FilmSimulation>
                <DynamicRange>400</DynamicRange>
                <ExposureBias>0.33</ExposureBias>
                <HDR />
              </PropertyGroup>
            </ConversionProfile>
            """;

        using var source = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var recipe = new Fp1RecipeReader().Read(source);

        Assert.Equal(XT50Recipe.SupportedDevice, recipe.Device);
        Assert.Equal("X-T50_0100", recipe.ProfileVersion);
        Assert.Equal("TestRecipe", recipe.Label);
        Assert.Equal("Provia", recipe.GetProperty("FilmSimulation"));
        Assert.Equal("400", recipe.GetProperty("DynamicRange"));
        Assert.Equal(string.Empty, recipe.GetProperty("HDR"));
        Assert.Equal("Provia", recipe.FilmSimulation);
        Assert.Equal("400", recipe.DynamicRange);
        Assert.Equal(0.33m, recipe.ExposureBias);
    }

    [Fact]
    public void Read_Rejects_Unsupported_Camera()
    {
        const string xml = """
            <ConversionProfile>
              <PropertyGroup device="X-T5" version="X-T5_0100" label="OtherCamera" />
            </ConversionProfile>
            """;

        using var source = new MemoryStream(Encoding.UTF8.GetBytes(xml));

        Assert.Throws<ArgumentException>(() => new Fp1RecipeReader().Read(source));
    }

      [Fact]
      public void Read_Loads_Reference_Profile()
      {
        using var source = File.OpenRead(Path.Combine(AppContext.BaseDirectory, "DemoFile.FP1"));
        var recipe = new Fp1RecipeReader().Read(source);

        Assert.Equal("X-T50", recipe.Device);
        Assert.Equal("DisparoOriginalDr400%", recipe.Label);
        Assert.Equal("Provia", recipe.FilmSimulation);
        Assert.Equal("400", recipe.DynamicRange);
        Assert.Equal(0, recipe.ExposureBias);
        Assert.Equal(0, recipe.HighlightTone);
        Assert.Equal(0, recipe.ShadowTone);
        Assert.Equal(0, recipe.Color);
        Assert.Equal(0, recipe.Sharpness);
        Assert.Equal(0, recipe.NoiseReduction);
        Assert.Equal(0, recipe.Clarity);
        Assert.Equal("INVALID", recipe.WhiteBalance);
        Assert.Equal("OFF", recipe.GrainEffect);
        Assert.Equal("SMALL", recipe.GrainEffectSize);
        Assert.Equal("sRGB", recipe.ColorSpace);
        Assert.Equal("OFF", recipe.DigitalTeleConverter);
      }

      [Fact]
      public void ReadMany_Loads_Each_Profile_Independently()
      {
        var reader = new Fp1RecipeReader();
        var referenceFile = Path.Combine(AppContext.BaseDirectory, "DemoFile.FP1");
        var files = new[] { referenceFile, referenceFile };

        var recipes = reader.ReadMany(files);

        Assert.Equal(2, recipes.Count);
        Assert.All(recipes, recipe => Assert.Equal("X-T50", recipe.Device));
        Assert.NotSame(recipes[0], recipes[1]);
      }
}
