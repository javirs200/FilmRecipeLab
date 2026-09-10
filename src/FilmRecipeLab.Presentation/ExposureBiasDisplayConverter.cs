using System.Globalization;
using System.Windows.Data;

namespace FilmRecipeLab.Presentation;

public sealed class ExposureBiasDisplayConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not decimal decimalValue)
        {
            return "-";
        }

        var thirds = (int)Math.Round(Math.Abs(decimalValue) * 3m, MidpointRounding.AwayFromZero);
        var whole = thirds / 3;
        var remainder = thirds % 3;
        var sign = decimalValue < 0 ? "-" : string.Empty;
        var fraction = remainder switch
        {
            1 => "1/3",
            2 => "2/3",
            _ => string.Empty
        };

        if (whole == 0 && fraction.Length > 0)
        {
            return sign + fraction;
        }

        return fraction.Length == 0
            ? sign + whole.ToString(CultureInfo.InvariantCulture)
            : $"{sign}{whole} {fraction}";
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}
