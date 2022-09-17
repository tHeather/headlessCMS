using headlessCMS.Mappers;
using headlessCMS.Models.Services.SelectQuery;
using Microsoft.AspNetCore.Mvc.Filters;

namespace headlessCMS.Filters
{
    public class AddDraftDbSchemaToSelectQueryActionFilter : ActionFilterAttribute
    {
        private readonly string NameOfParameter;

        public AddDraftDbSchemaToSelectQueryActionFilter(string nameOfParameter)
        {
            NameOfParameter = nameOfParameter;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          var parameterValue = (SelectQueryParametersDataCollection)filterContext.ActionArguments[NameOfParameter];
          ApiRequestMapper.AddDraftDbSchemaToSelectQuery(parameterValue);
        }
    }
}
