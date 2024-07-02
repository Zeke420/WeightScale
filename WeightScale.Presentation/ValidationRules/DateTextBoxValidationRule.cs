using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace WeightScale.Presentation.ValidationRules
{
    public class DateTextBoxValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            var pattern = @"[0-9][0-9]\/[0-9][0-9]\/[0-9][0-9][0-9][0-9]";
            var match = Regex.Match((string)value, pattern);

            return new ValidationResult(match.Success, "Not Valid");
        }
    }
}