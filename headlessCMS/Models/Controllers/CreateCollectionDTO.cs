using headlessCMS.Models.Shared;

namespace headlessCMS.Models.DTOs
{
    public class CreateCollectionDTO
    {
        public string Name { get; set; }
        public IEnumerable<Field> Fields { get; set; }
    }
}
