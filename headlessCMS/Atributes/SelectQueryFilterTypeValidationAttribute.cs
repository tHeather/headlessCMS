using headlessCMS.Constants;
using System.ComponentModel.DataAnnotations;

namespace headlessCMS.Atributes
{
    public class SelectQueryFilterTypeValidationAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            ErrorMessage = "Incorrect filter name.";

            return SelectQueryFilters.FilterTypesList.Contains(value);
        }
    }
}
