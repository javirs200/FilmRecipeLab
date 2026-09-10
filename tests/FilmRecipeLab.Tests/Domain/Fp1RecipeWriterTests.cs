using System.Text;
using FilmRecipeLab.Infrastructure.Xml;

namespace FilmRecipeLab.Tests.Domain;

public sealed class Fp1RecipeWriterTests
{
    [Fact]
    public void Write_Exports_Typed_Values_And_Preserves_Unknown_Properties()
    {
        const string xml = """
            <ConversionProfile application="XRFC" version="1.12.0.0">
              <PropertyGroup device="X-T50" version="X-T50_0100" label="Original">
                <FilmSimulation>Provia</FilmSimulation>
                <DynamicRange>400</DynamicRange>
                <ExposureBias>P0P00</ExposureBias>
                <WBColorTemp>0K</WBColorTemp>
                <DigitalTeleConv>2</DigitalTeleConv>
                <HDR />
              </PropertyGroup>
            </ConversionProfile>
            """;

        var reader = new Fp1RecipeReader();
        using var input = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var recipe = reader.Read(input);
        recipe.Label = "Updated";
        recipe.ExposureBias = 0.67m;
        recipe.WhiteBalanceColorTemperature = 9000;
        recipe.FilmSimulation = "Sepia";

        using var output = new MemoryStream();
        new Fp1RecipeWriter().Write(recipe, output);
        output.Position = 0;
        var exported = reader.Read(output);

        Assert.Equal("Updated", exported.Label);
        Assert.Equal("Sepia", exported.FilmSimulation);
        Assert.Equal(0.67m, exported.ExposureBias);
        Assert.Equal(9000, exported.WhiteBalanceColorTemperature);
        Assert.Equal(string.Empty, exported.GetProperty("HDR"));
        Assert.Equal("OFF", exported.GetProperty("DigitalTeleConv"));
    }

    [Fact]
    public void Write_Formats_Negative_ExposureBias()
    {
        const string xml = """
            <ConversionProfile>
              <PropertyGroup device="X-T50" version="X-T50_0100" label="Test">
                <ExposureBias>M2P00</ExposureBias>
              </PropertyGroup>
            </ConversionProfile>
            """;

        using var input = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        var recipe = new Fp1RecipeReader().Read(input);
        recipe.ExposureBias = -2m;

    using var output = new MemoryStream();
    new Fp1RecipeWriter().Write(recipe, output);
    output.Position = 0;

    var exported = new Fp1RecipeReader().Read(output);

    Assert.Equal(-2m, exported.ExposureBias);
    Assert.Equal("M2P00", exported.GetProperty("ExposureBias"));
  }
}
