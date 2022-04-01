namespace headlessCMS.Models.DTOs
{
    public class CreateCollectionDTO
    {
        public string Name { get; set; }
        public Dictionary<string,string> Fields { get; set; }
    }
}
