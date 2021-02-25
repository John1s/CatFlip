using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Catflip.Api.Dtos
{
    public class PasswordComplexityAttribute: ValidationAttribute
    {

        public override bool IsValid(object value)
        {
            if(value == null)
            {
                //Required behavious is provided by other validation attributes.
                return true;
            }
            var text = value.ToString();
            var numberRegEx = new Regex("[0-9]{1}");
            var specialRegEx = new Regex("[#?!@$%^&*-]{1}");

            return numberRegEx.IsMatch(text) && specialRegEx.IsMatch(text);
        }

        public override string FormatErrorMessage(string name)
        {
            return $"The {name} must contain at least 1 number and 1 special character form '#, ?, !, @, $, %, ^, &, *, -'";
        }
    }
}
