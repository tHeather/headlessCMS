using headlessCMS.Mappers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace headlessCMS.Filters
{
    public class AddDraftDbSchemaActionFilter : ActionFilterAttribute
    {
        private readonly string NameOfParameter;

        public AddDraftDbSchemaActionFilter(string nameOfParameter)
        {
            NameOfParameter = nameOfParameter;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var isParameterExists = filterContext.ActionArguments.TryGetValue(NameOfParameter, out var parameterValue);
            if (isParameterExists)
            {
                filterContext.ActionArguments[NameOfParameter] = ApiRequestMapper.AddDraftDbSchemaPrefix((string)parameterValue);
            }
        }
    }
}
