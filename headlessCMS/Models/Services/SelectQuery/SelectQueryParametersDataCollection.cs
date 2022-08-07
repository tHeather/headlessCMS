namespace headlessCMS.Models.Services.Select
{
    public class SelectQueryParametersDataCollection
    {
        public List<SelectQuerySelectedField> SelctedFields { get; set; }

        public string From { get; set; }

        public List<SelectQueryJoin> Joins { get; set; }

        public List<SelectFiltersField> FieldsFilters { get; set; }
    }
}