namespace headlessCMS.Models.ValueObjects
{
    public class CreateCollection
    {
        public string Name { get; set; }
        public Dictionary<string, string> Fields { get; set; }
    }
}
