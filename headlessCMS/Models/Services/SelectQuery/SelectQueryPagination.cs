using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace headlessCMS.Models.Services.SelectQuery
{
    public class SelectQueryPagination
    {
        [OnError]
        internal void OnError(StreamingContext context, ErrorContext errorContext)
        {
            errorContext.Handled = true;
        }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Page size must be greater or equal to 1.")]
        public int PageSize { get; set; } = 1;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Page number must be greater or equal to 0.")]
        public int PageNumber{ get; set; }
    }
}
