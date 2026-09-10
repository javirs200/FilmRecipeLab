using FilmRecipeLab.Infrastructure.Xml;

const string inputPath = "Resources/DemoFile.FP1";
const string outputPath = "Resources/ManualExportTest.FP1";

var reader = new Fp1RecipeReader();
var writer = new Fp1RecipeWriter();
var recipe = reader.Read(inputPath);

recipe.Label = "ManualExportTest";
recipe.ExposureBias = 0.67m;
recipe.WhiteBalance = "Temperature";
recipe.WhiteBalanceColorTemperature = 9000;

writer.Write(recipe, outputPath);

var exported = reader.Read(outputPath);
Console.WriteLine($"Generated: {outputPath}");
Console.WriteLine($"Label: {exported.Label}");
Console.WriteLine($"ExposureBias: {exported.GetProperty("ExposureBias")} ({exported.ExposureBias})");
Console.WriteLine($"WBColorTemp: {exported.GetProperty("WBColorTemp")} ({exported.WhiteBalanceColorTemperature} K)");
Console.WriteLine($"DigitalTeleConv: {exported.GetProperty("DigitalTeleConv")}");
