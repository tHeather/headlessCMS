using headlessCMS.Constants;
using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Atributes
{
    public class JoinTypeValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            ErrorMessage = "Incorrect join type name.";

            return JoinTypes.JoinTypesList.Contains(value);
        }
    }
}
