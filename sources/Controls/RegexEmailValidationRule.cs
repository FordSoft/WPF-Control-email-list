using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace EmailClientApplication.Controls
{
    public class RegexEmailValidationRule : ValidationRule
    {
        private const string Pattern = @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$";
        //@"^[a-zA-Z][\w\.-]{2,28}[a-zA-Z0-9]@[a-zA-Z0-9][\w\.-]*[a-zA-Z0-9]\.[a-zA-Z][a-zA-Z\.]*[a-zA-Z]$";
            
        private readonly Regex _regex;

        public RegexEmailValidationRule()
        {
            _regex = new Regex(Pattern, RegexOptions.IgnoreCase);
        }

        public override ValidationResult Validate(object value, CultureInfo ultureInfo)
        {
            if (string.IsNullOrWhiteSpace(value as string)) 
                return new ValidationResult(true, null);
            
            var splitValue = ((string) value).Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries).Select(s=> s.Trim());
            foreach (var v in splitValue)
            {
                if (v == string.Empty)
                    continue;

                if (!_regex.Match(v).Success)
                    return new ValidationResult(false, string.Format("The value '{0}' is not a valid e-mail address", v));
                    
            }
            return new ValidationResult(true, null);
            
        }
    }
}