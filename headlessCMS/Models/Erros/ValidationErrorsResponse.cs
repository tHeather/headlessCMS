using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace headlessCMS.Models.Erros
{
    public class ValidationErrorsResponse
    {
        public int StatusCode { get; } = StatusCodes.Status400BadRequest;
        public IEnumerable<ValidationError> Errors { get; }

        public ValidationErrorsResponse(ModelStateDictionary modelsState)
        {
            Errors = modelsState.Select(model => new ValidationError
            {
                Field = model.Key,
                Messages = model.Value.Errors.Select(e => e.ErrorMessage)
            });
        }
    }
}
