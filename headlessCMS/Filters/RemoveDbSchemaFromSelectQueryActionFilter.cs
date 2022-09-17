using headlessCMS.Mappers;
using headlessCMS.Models.Services.SelectQuery;
using Microsoft.AspNetCore.Mvc.Filters;

namespace headlessCMS.Filters
{
    public class RemoveDbSchemaFromSelectQueryActionFilter : ActionFilterAttribute
    {
        private readonly string NameOfParameter;

        public RemoveDbSchemaFromSelectQueryActionFilter(string nameOfParameter)
        {
            NameOfParameter = nameOfParameter;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
          var parameterValue = (SelectQueryParametersDataCollection)filterContext.ActionArguments[NameOfParameter];
          ApiRequestMapper.RemoveSchemaFromSelectQuery(parameterValue);
        }
    }
}
