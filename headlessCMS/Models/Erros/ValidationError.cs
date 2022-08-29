namespace headlessCMS.Models.Erros
{
    public class ValidationError
    {
        public string Field { get; set; }

        public IEnumerable<string> Messages { get; set; }
    }
}
