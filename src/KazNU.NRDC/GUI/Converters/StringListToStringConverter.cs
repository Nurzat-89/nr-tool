using NuclearData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using static NuclearData.Constants;

namespace GUI.Converters
{
    internal class StringListToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IDictionary<RTYPE, IDecayData> tmpDecays)
            {
                return string.Join(", ", tmpDecays.OrderBy(x=>x.Key).Select(x=>x.Key.ToString()));
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
