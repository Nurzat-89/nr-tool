using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace GUI.Converters
{
    internal class DoubleValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            if (value == null)
                return new ValidationResult(false, "Пустое значение");
            else if (!IsNumber(value.ToString()))
            {
                return new ValidationResult(false, "Неправильное число");
            }           
            return ValidationResult.ValidResult;
        }

        private static bool IsNumber(string aTextNumber)
        {
            return double.TryParse(aTextNumber, out double val) || val < 0;
        }
    }
}
