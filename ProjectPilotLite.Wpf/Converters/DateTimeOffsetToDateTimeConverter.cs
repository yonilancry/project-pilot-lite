using System;
using System.Globalization;
using System.Windows.Data;

namespace ProjectPilotLite.Wpf.Converters;

public class DateTimeOffsetToDateTimeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is DateTimeOffset offset ? offset.DateTime : null;

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => value is DateTime date ? new DateTimeOffset(date) : null;
}
