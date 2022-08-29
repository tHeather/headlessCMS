using headlessCMS.Constants;
using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Atributes
{
    public class OrderTypeValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            ErrorMessage = "Incorrect order name.";

            return OrderTypes.OrderTypesList.Contains(value);
        }
    }
}
