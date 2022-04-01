namespace headlessCMS.Models.Models
{
    public class CollectionField
    {
        public Guid Id { get; set; }
        public Guid CollectionId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
