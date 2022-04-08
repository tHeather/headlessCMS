using headlessCMS.Models.Shared;

namespace headlessCMS.Models.ValueObjects
{
    public class CreateCollection
    {
        public string Name { get; set; }
        public IEnumerable<Field> Fields { get; set; }
    }
}
