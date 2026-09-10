using System.Globalization;
using System.Windows.Data;

namespace FilmRecipeLab.Presentation;

public sealed class NumericValueConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value switch
        {
            null => 0d,
            decimal decimalValue => (double)decimalValue,
            int intValue => intValue,
            _ => System.Convert.ToDouble(value, CultureInfo.InvariantCulture)
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var numericValue = System.Convert.ToDouble(value, CultureInfo.InvariantCulture);
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(int))
        {
            return (int)Math.Round(numericValue, MidpointRounding.AwayFromZero);
        }

        if (underlyingType == typeof(decimal))
        {
            return (decimal)numericValue;
        }

        return numericValue;
    }
}
