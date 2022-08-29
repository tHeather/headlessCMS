using headlessCMS.Constants;
using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Atributes
{
    public class LogicalOperationValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            ErrorMessage = "Incorrect logical operation name.";

            return LogicalOperations.LogicalOperationsList.Contains(value);
        }
    }
}
